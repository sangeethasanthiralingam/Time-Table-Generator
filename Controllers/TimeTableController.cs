using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Time_Table_Generator.Models;
using OfficeOpenXml;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Time_Table_Generator.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TimeTableController : ControllerBase
    {
        private readonly AppDbContext _context;

        public TimeTableController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("Generate")]
        public IActionResult GenerateTimeTable([FromBody] TimeTableRequest? request = null, bool isGenerate = true)
        {
            if (!isGenerate)
            {
                var query = _context.TimeTables
                      .Where(t => t.IsActive);
                return Ok(FormatTimetable(query));
            }
              
            MarkOldTimeTablesAsInactive();

            if (request == null)
                request = GetDefaultTimeTableRequest();

            if (!ValidateRequest(request, out var validationError))
                return BadRequest(validationError);

            var timetable = GenerateTimetable(request);
            SaveTimetableToDatabase(timetable);

            return Ok(timetable);
        }

        [HttpGet("ByStudent/{studentId}")]
        public IActionResult GetTimeTableByStudent(int studentId, bool export = false)
        {
            var student = _context.Students
                .Include(s => s.Batch)
                .FirstOrDefault(s => s.Id == studentId);

            if (student == null || student.Batch == null)
                return NotFound("Student or Batch not found.");

            var query = _context.TimeTables
                .Where(t => t.BatchId == student.Batch.Id && t.IsActive);

            return export ? ExportTimeTables(query, $"Student_{studentId}_TimeTables.xlsx") : Ok(FormatTimetable(query));
        }

        [HttpGet("ByTeacher/{teacherId}")]
        public IActionResult GetTimeTableByTeacher(int teacherId, bool export = false)
        {
            var query = _context.TimeTables
                .Where(t => t.TeacherId == teacherId && t.IsActive);

            return export ? ExportTimeTables(query, $"Teacher_{teacherId}_TimeTables.xlsx") : Ok(FormatTimetable(query));
        }

        [HttpGet("ByBatch/{batchId}")]
        public IActionResult GetTimeTableByBatch(int batchId, bool export = false)
        {
            var query = _context.TimeTables
                .Where(t => t.BatchId == batchId && t.IsActive);

            return export ? ExportTimeTables(query, $"Batch_{batchId}_TimeTables.xlsx") : Ok(FormatTimetable(query));
        }

        [HttpGet("All")]
        public IActionResult GetAllTimeTables()
        {
/*            var timetables = GetActiveTimeTables();*/
            var query = _context.TimeTables
                        .Where(t => t.IsActive);

            return Ok(FormatTimetable(query));
        }

        private void MarkOldTimeTablesAsInactive()
        {
            var existingTimetables = _context.TimeTables.Where(t => t.IsActive).ToList();
            foreach (var timetable in existingTimetables)
            {
                timetable.IsActive = false;
                timetable.UpdatedAt = DateTime.Now;
            }
            _context.TimeTables.UpdateRange(existingTimetables);
            _context.SaveChanges();
        }

        private TimeTableRequest GetDefaultTimeTableRequest()
        {
            return new TimeTableRequest
            {
                ClassIds = _context.Classes.Select(c => c.Id).ToList(),
                TeachersIds = _context.Teachers.Select(t => t.Id).ToList(),
                BatchIds = _context.Batches.Select(b => b.Id).ToList(),
                DefaultDays = new List<string> { "Monday", "Tuesday", "Wednesday", "Thursday", "Friday" },
                SubjectsPerDay = 4,
                ClassStartTime = "08:00",
                ClassEndTime = "14:00",
                FreePeriodAllocation = new FreePeriodAllocation
                {
                    WeeklyLimit = 5,
                    Locations = _context.Availabilities.Select(l => l.Type).ToList()
                }
            };
        }

        private bool ValidateRequest(TimeTableRequest request, out string? error)
        {
            if (request.ClassIds == null || !request.ClassIds.Any())
            {
                error = "Class IDs are required.";
                return false;
            }
            if (request.SubjectsPerDay <= 0)
            {
                error = "Subjects per day must be greater than zero.";
                return false;
            }
            if (string.IsNullOrEmpty(request.ClassStartTime) || string.IsNullOrEmpty(request.ClassEndTime))
            {
                error = "Class start and end times are required.";
                return false;
            }
            if (!TimeSpan.TryParse(request.ClassStartTime, out _) || !TimeSpan.TryParse(request.ClassEndTime, out _))
            {
                error = "Invalid time format for start or end time.";
                return false;
            }
            error = null;
            return true;
        }

        private Dictionary<string, List<object>> GenerateTimetable(TimeTableRequest request)
        {
            var timetable = new Dictionary<string, List<object>>();
            var timeTableEntities = new List<TimeTable>();

            foreach (var day in request.DefaultDays.Concat(request.CustomDays))
            {
                if (request.Holidays.Contains(day) || request.Celebrations.Contains(day))
                {
                    timetable[day] = new List<object> { "Holiday or Celebration" };
                    continue;
                }

                timetable[day] = new List<object>();

                foreach (var classId in request.ClassIds)
                {
                    var classEntity = _context.Classes
                        .Include(c => c.Subjects)
                        .Include(c => c.Batches)
                        .FirstOrDefault(c => c.Id == classId);

                    if (classEntity == null) continue;

                    var subjects = classEntity.Subjects.ToList();
                    var teachers = _context.Teachers.Where(t => request.TeachersIds.Contains(t.Id)).ToList();

                    var currentTime = TimeSpan.Parse(request.ClassStartTime);
                    var endTime = TimeSpan.Parse(request.ClassEndTime);
                    var slotDuration = (endTime - currentTime).TotalMinutes / request.SubjectsPerDay;

                    for (int i = 0; i < request.SubjectsPerDay; i++)
                    {
                        if (currentTime >= endTime) break;

                        var subject = subjects.ElementAtOrDefault(i % subjects.Count);
                        var teacher = teachers.ElementAtOrDefault(i % teachers.Count);

                        var startTimeSlot = currentTime;
                        var endTimeSlot = currentTime + TimeSpan.FromMinutes(slotDuration);

                        if (subject == null)
                        {
                            timetable[day].Add(new
                            {
                                ClassId = classId,
                                Subject = "Free Period",
                                Teacher = "N/A",
                                Location = request.FreePeriodAllocation?.Locations.ElementAtOrDefault(i % request.FreePeriodAllocation.Locations.Count) ?? "N/A",
                                StartTime = startTimeSlot.ToString(@"hh\:mm"),
                                EndTime = endTimeSlot.ToString(@"hh\:mm"),
                                Day = day
                            });
                        }
                        else
                        {
                            timetable[day].Add(new
                            {
                                ClassId = classId,
                                Subject = subject.Name,
                                Teacher = _context.Users.Where(t => t.Id == teacher.Id).Select(x => x.Displayname).FirstOrDefault() ?? "Unassigned",
                                StartTime = startTimeSlot.ToString(@"hh\:mm"),
                                EndTime = endTimeSlot.ToString(@"hh\:mm"),
                                Day = day
                            });

                            timeTableEntities.Add(new TimeTable
                            {
                                ClassId = classId,
                                BatchId = classEntity.Batches.FirstOrDefault()?.Id ?? 0,
                                TeacherId = teacher?.Id ?? 0,
                                SubjectId = subject.Id,
                                StartTime = DateTime.Today.Add(startTimeSlot),
                                EndTime = DateTime.Today.Add(endTimeSlot),
                                Day = day,
                                CreatedAt = DateTime.Now,
                                IsActive = true
                            });
                        }

                        currentTime = endTimeSlot;
                    }
                }
            }

            _context.TimeTables.AddRange(timeTableEntities);
            return timetable;
        }

        private void SaveTimetableToDatabase(Dictionary<string, List<object>> timetable)
        {
            _context.SaveChanges();
        }

        private Dictionary<string, List<object>> FormatTimetable(IQueryable<TimeTable> query)
        {
            // Generate the raw list
            var rawTimetable = query
                .Include(t => t.Class)
                .Include(t => t.Batch)
                .Include(t => t.Teacher)
                .ThenInclude(t => t.User)
                .Include(t => t.Subject)
                .Select(t => new
                {
                    ClassName = t.Class != null && t.Class.Name != null ? t.Class.Name : "N/A",
                    BatchName = t.Batch != null && t.Batch.Name != null ? t.Batch.Name : "N/A",
                    TeacherName = t.Teacher != null && t.Teacher.User != null ? t.Teacher.User.Displayname : "N/A",
                    SubjectName = t.Subject != null ? t.Subject.Name : "N/A",
                    StartTime = t.StartTime.ToString("HH:mm"),
                    EndTime = t.EndTime.ToString("HH:mm"),
                    Day = t.Day
                })
                .ToList();

            // Assign unique class IDs based on ClassName & BatchName combination
            var classIds = rawTimetable
                .Select(t => new { t.ClassName, t.BatchName })
                .Distinct()
                .Select((x, index) => new { Key = $"{x.ClassName}-{x.BatchName}", ClassId = index + 1 })
                .ToDictionary(x => x.Key, x => x.ClassId);

            // Group by Day
            var groupedTimetable = rawTimetable
                .GroupBy(t => t.Day)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(t => new
                    {
                        ClassName = t.ClassName,
                        BatchName = t.BatchName,
                        Subject = t.SubjectName,
                        Teacher = t.TeacherName,
                        StartTime = t.StartTime,
                        EndTime = t.EndTime,
                        Day = t.Day
                    }).ToList<object>()
                );

            return groupedTimetable;
        }


        [HttpGet("Export")]
        private IActionResult ExportTimeTables(IQueryable<TimeTable> query, string fileName)
        {
            var timetables = query
                .Include(t => t.Class)
                .Include(t => t.Batch)
                .Include(t => t.Teacher)
                .Include(t => t.Subject)
                .ToList();

            if (!timetables.Any())
                return NotFound("No timetables available to export.");

            using var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add("TimeTables");

            // Add headers
            worksheet.Cells[1, 1].Value = "Class";
            worksheet.Cells[1, 2].Value = "Batch";
            worksheet.Cells[1, 3].Value = "Teacher";
            worksheet.Cells[1, 4].Value = "Subject";
            worksheet.Cells[1, 5].Value = "Start Time";
            worksheet.Cells[1, 6].Value = "End Time";
            worksheet.Cells[1, 7].Value = "Date";

            // Add data
            for (int i = 0; i < timetables.Count; i++)
            {
                var row = i + 2;
                var timetable = timetables[i];

                worksheet.Cells[row, 1].Value = timetable.Class != null ? timetable.Class.Name : "N/A"; // Replace ?. with conditional expression
                worksheet.Cells[row, 2].Value = timetable.Batch != null ? timetable.Batch.Name : "N/A"; // Replace ?. with conditional expression
                worksheet.Cells[row, 3].Value = timetable.Teacher != null && timetable.Teacher.User != null ? timetable.Teacher.User.Displayname : "N/A"; // Replace ?. with conditional expression
                worksheet.Cells[row, 4].Value = timetable.Subject != null ? timetable.Subject.Name : "N/A"; // Replace ?. with conditional expression
                worksheet.Cells[row, 5].Value = timetable.StartTime.ToString("hh:mm tt");
                worksheet.Cells[row, 6].Value = timetable.EndTime.ToString("hh:mm tt");
                worksheet.Cells[row, 7].Value = timetable.Day;
            }

            var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            var fileContent = package.GetAsByteArray();

            return File(fileContent, contentType, fileName);
        }

        [HttpGet("StudentClassBatch/{studentId}")]
        public IActionResult GetStudentClassBatchTimeTable(int studentId)
        {
            var student = _context.Students
                .Include(s => s.Batch)
                .Include(s => s.User) // Ensure User is included
                .FirstOrDefault(s => s.Id == studentId);

            if (student == null || student.Batch == null || student.User == null)
                return NotFound("Student, Batch, or User not found.");

            var timetable = _context.TimeTables
                .Where(t => t.BatchId == student.Batch.Id && t.IsActive) // Retrieve only active timetables
                .Include(t => t.Class)
                .Include(t => t.Batch)
                .Include(t => t.Teacher)
                .Include(t => t.Subject)
                .ToList();

            return JsonWithPreservedReferences(new
            {
                Student = new
                {
                    student.Id,
                    DisplayName = student.User.Displayname,
                    Batch = student.Batch.Name
                },
                TimeTable = timetable
            }); // Use custom JSON result
        }

        private JsonResult JsonWithPreservedReferences(object data)
        {
            var options = new JsonSerializerOptions
            {
                ReferenceHandler = ReferenceHandler.Preserve,
                WriteIndented = true
            };
            return new JsonResult(data, options);
        }
    }

    public class TimeTableRequest
    {
        public int MinimumTeachersPerWeek { get; set; }
        public int SubjectsPerDay { get; set; }
        public required string ClassStartTime { get; set; } // Add 'required' modifier
        public required string ClassEndTime { get; set; } // Add 'required' modifier
        public List<string> DefaultDays { get; set; } = new List<string>();
        public List<string> CustomDays { get; set; } = new List<string>();
        public required FreePeriodAllocation FreePeriodAllocation { get; set; } // Add 'required' modifier
        public List<int> TeachersIds { get; set; } = new List<int>();
        public List<int> BatchIds { get; set; } = new List<int>();
        public List<int> ClassIds { get; set; } = new List<int>();
        public List<int> SubHoursIds { get; set; } = new List<int>();
        public List<int> SubClassIds { get; set; } = new List<int>();
        public List<int> EventsIds { get; set; } = new List<int>();
        public List<string> Holidays { get; set; } = new List<string>();
        public List<string> Celebrations { get; set; } = new List<string>();
        public List<string> Days { get; set; } = new List<string>();
        public List<int> ActivityIds { get; set; } = new List<int>();
        public List<int> ExtraCurricularActivityIds { get; set; } = new List<int>();
    }

    public class FreePeriodAllocation
    {
        public int WeeklyLimit { get; set; }
        public List<string> Locations { get; set; } = new List<string>();
    }
}