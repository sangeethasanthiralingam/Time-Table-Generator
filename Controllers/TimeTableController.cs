using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Time_Table_Generator.Models;
using OfficeOpenXml;
using System.Text.Json; 
using System.Text.Json.Serialization; 
using Microsoft.AspNetCore.Authorization;

namespace Time_Table_Generator.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] 
    public class TimeTableController : ControllerBase
    {
        private readonly AppDbContext _context;

        public TimeTableController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("Generate")]
        public IActionResult GenerateTimeTable([FromBody] TimeTableRequest request, bool isGenerate = true)
        {
            if (!isGenerate)
            {
                var timetables = _context.TimeTables
               .Include(t => t.Class)
               .Include(t => t.Batch)
               .Include(t => t.Teacher)
               .Include(t => t.Subject)
               .ToList();
                return Ok(timetables);
            }

            if (request == null) return BadRequest("Invalid request body.");

            // Validate input
            if (request.ClassIds == null || !request.ClassIds.Any())
                return BadRequest("Class IDs are required.");
            if (request.SubjectsPerDay <= 0)
                return BadRequest("Subjects per day must be greater than zero.");
            if (string.IsNullOrEmpty(request.ClassStartTime) || string.IsNullOrEmpty(request.ClassEndTime))
                return BadRequest("Class start and end times are required.");

            // Parse start and end times
            if (!TimeSpan.TryParse(request.ClassStartTime, out var startTime) ||
                !TimeSpan.TryParse(request.ClassEndTime, out var endTime))
                return BadRequest("Invalid time format for start or end time.");

            // Calculate total available slots per day
            var totalMinutes = (endTime - startTime).TotalMinutes;
            var slotDuration = totalMinutes / request.SubjectsPerDay;
            if (slotDuration <= 0) return BadRequest("Invalid time range or subjects per day.");

            // Initialize timetable structure
            var timetable = new Dictionary<string, List<object>>();
            var timeTableEntities = new List<TimeTable>();

            foreach (var day in request.DefaultDays.Concat(request.CustomDays))
            {
                // Skip holidays and celebrations
                if (request.Holidays.Contains(day) || request.Celebrations.Contains(day))
                {
                    timetable[day] = new List<object> { "Holiday or Celebration" };
                    continue;
                }

                timetable[day] = new List<object>();

                foreach (var classId in request.ClassIds)
                {
                    // Fetch class, subjects, and teachers
                    var classEntity = _context.Classes
                        .Include(c => c.Subjects)
                        .Include(c => c.Batches)
                        .FirstOrDefault(c => c.Id == classId);

                    if (classEntity == null) continue;

                    var subjects = classEntity.Subjects.ToList();
                    var teachers = _context.Teachers.Where(t => request.TeachersIds.Contains(t.Id)).ToList();

                    // Generate periods for the day
                    var currentTime = startTime;
                    for (int i = 0; i < request.SubjectsPerDay; i++)
                    {
                        if (currentTime >= endTime) break;

                        var subject = subjects.ElementAtOrDefault(i % subjects.Count);
                        var teacher = teachers.ElementAtOrDefault(i % teachers.Count);

                        var startTimeSlot = currentTime;
                        var endTimeSlot = currentTime + TimeSpan.FromMinutes(slotDuration);

                        // Handle free periods
                        if (subject == null)
                        {
                            timetable[day].Add(new
                            {
                                ClassId = classId,
                                Subject = "Free Period",
                                Teacher = "N/A",
                                Location = request.FreePeriodAllocation?.Locations.ElementAtOrDefault(i % request.FreePeriodAllocation.Locations.Count) ?? "N/A",
                                StartTime = startTimeSlot.ToString(@"hh\:mm"),
                                EndTime = endTimeSlot.ToString(@"hh\:mm")
                            });
                        }
                        else
                        {
                            // Add to timetable response
                            timetable[day].Add(new
                            {
                                ClassId = classId,
                                Subject = subject.Name,
                                Teacher = _context.Users.Where(t => t.Id == teacher.Id).Select(x => x.Displayname).FirstOrDefault() ?? "Unassigned",
                                EndTime = endTimeSlot.ToString(@"hh\:mm")
                            });

                            // Store in TimeTable table
                            timeTableEntities.Add(new TimeTable
                            {
                                ClassId = classId,
                                BatchId = classEntity.Batches.FirstOrDefault()?.Id ?? 0,
                                TeacherId = teacher?.Id ?? 0,
                                SubjectId = subject.Id,
                                StartTime = DateTime.Today.Add(startTimeSlot),
                                EndTime = DateTime.Today.Add(endTimeSlot),
                                Date = DateTime.Today
                            });
                        }

                        currentTime = endTimeSlot;
                    }
                }
            }

            // Save timetable to database
            _context.TimeTables.AddRange(timeTableEntities);
            _context.SaveChanges();

            // Update availability for free period locations
            if (request.FreePeriodAllocation != null && request.FreePeriodAllocation.Locations.Any())
            {
                foreach (var location in request.FreePeriodAllocation.Locations)
                {
                    var availability = _context.Availabilities.FirstOrDefault(a => a.Type == location);
                    if (availability != null)
                    {
                        availability.IsAvailable = false;
                        _context.Availabilities.Update(availability);
                    }
                }
                _context.SaveChanges();
            }

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

            var query = _context.TimeTables.Where(t => t.BatchId == student.Batch.Id);

            if (export)
                return ExportTimeTables(query, $"Student_{studentId}_TimeTables.xlsx");

            var timetable = query
                .Include(t => t.Class)
                .Include(t => t.Teacher)
                .ThenInclude(t => t.User)
                .Include(t => t.Subject)
                  .Select(t => new
                  {
                      ClassName = t.Class != null ? t.Class.Name : "N/A", // Replace ?. with conditional expression
                      BatchName = t.Batch != null ? t.Batch.Name : "N/A", // Replace ?. with conditional expression
                      TeacherName = t.Teacher != null && t.Teacher.User != null ? t.Teacher.User.Displayname : "N/A", // Replace ?. with conditional expression
                      SubjectName = t.Subject != null ? t.Subject.Name : "N/A", // Replace ?. with conditional expression
                      StartTime = t.StartTime.ToString("hh:mm tt"),
                      EndTime = t.EndTime.ToString("hh:mm tt"),
                      Date = t.Date.ToString("yyyy-MM-dd")
                  })
                .ToList();

            return Ok(timetable);
        }

        [HttpGet("ByTeacher/{teacherId}")]
        public IActionResult GetTimeTableByTeacher(int teacherId, bool export = false)
        {
            var query = _context.TimeTables.Where(t => t.TeacherId == teacherId);

            if (export)
                return ExportTimeTables(query, $"Teacher_{teacherId}_TimeTables.xlsx");

            var timetable = query
                .Include(t => t.Class)
                .Include(t => t.Batch)
                .Include(t => t.Subject)
                  .Select(t => new
                  {
                      ClassName = t.Class != null ? t.Class.Name : "N/A", // Replace ?. with conditional expression
                      BatchName = t.Batch != null ? t.Batch.Name : "N/A", // Replace ?. with conditional expression
                      SubjectName = t.Subject != null ? t.Subject.Name : "N/A", // Replace ?. with conditional expression
                      StartTime = t.StartTime.ToString("hh:mm tt"),
                      EndTime = t.EndTime.ToString("hh:mm tt"),
                      Date = t.Date.ToString("yyyy-MM-dd")
                  })
                .ToList();

            if (!timetable.Any())
                return NotFound("Teacher not found or no timetable available.");

            return Ok(timetable);
        }

        [HttpGet("ByBatch/{batchId}")]
        public IActionResult GetTimeTableByBatch(int batchId, bool export = false)
        {
            var query = _context.TimeTables.Where(t => t.BatchId == batchId);

            if (export)
                return ExportTimeTables(query, $"Batch_{batchId}_TimeTables.xlsx");

            var timetable = query
                .Include(t => t.Class)
                .Include(t => t.Teacher)
                .ThenInclude(t => t.User)
                .Include(t => t.Subject)
                  .Select(t => new
                  {
                      ClassName = t.Class != null ? t.Class.Name : "N/A", // Replace ?. with conditional expression
                      BatchName = t.Batch != null ? t.Batch.Name : "N/A", // Replace ?. with conditional expression
                      TeacherName = t.Teacher != null && t.Teacher.User != null ? t.Teacher.User.Displayname : "N/A", // Replace ?. with conditional expression
                      SubjectName = t.Subject != null ? t.Subject.Name : "N/A", // Replace ?. with conditional expression
                      StartTime = t.StartTime.ToString("hh:mm tt"),
                      EndTime = t.EndTime.ToString("hh:mm tt"),
                      Date = t.Date.ToString("yyyy-MM-dd")
                  })
                .ToList();

            if (!timetable.Any())
                return NotFound("Batch not found or no timetable available.");

            return Ok(timetable);
        }

        [HttpGet("All")]
        public IActionResult GetAllTimeTables()
        {
            var timetables = _context.TimeTables
                .Include(t => t.Class)
                .Include(t => t.Batch)
                .Include(t => t.Teacher)
                .ThenInclude(t => t.User) // Ensure Teacher.User is included
                .Include(t => t.Subject)
                .Select(t => new
                {
                    ClassName = t.Class != null ? t.Class.Name : "N/A", // Replace ?. with conditional expression
                    BatchName = t.Batch != null ? t.Batch.Name : "N/A", // Replace ?. with conditional expression
                    TeacherName = t.Teacher != null && t.Teacher.User != null ? t.Teacher.User.Displayname : "N/A", // Replace ?. with conditional expression
                    SubjectName = t.Subject != null ? t.Subject.Name : "N/A", // Replace ?. with conditional expression
                    StartTime = t.StartTime.ToString("hh:mm tt"),
                    EndTime = t.EndTime.ToString("hh:mm tt"),
                    Date = t.Date.ToString("yyyy-MM-dd")
                })
                .ToList();

            return Ok(timetables); // Return simplified structure
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
                worksheet.Cells[row, 7].Value = timetable.Date.ToString("yyyy-MM-dd");
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
                .Include(t => t.Class)
                .Include(t => t.Batch)
                .Include(t => t.Teacher)
                .Include(t => t.Subject)
                .Where(t => t.BatchId == student.Batch.Id)
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