using Microsoft.EntityFrameworkCore;

namespace Time_Table_Generator.Models
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Availability> Availabilities { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<Holiday> Holidays { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<ExtraActivity> ExtraActivities { get; set; }
        public DbSet<Teacher> Teachers { get; set; }
        public DbSet<Batch> Batches { get; set; }
        public DbSet<SubjectHour> SubjectHours { get; set; }
        public DbSet<Subject> Subjects { get; set; }
        public DbSet<ClassTeacher> ClassTeachers { get; set; }
        public DbSet<TeacherSubject> TeacherSubjects { get; set; }
        public DbSet<EventClassBatch> EventClassBatches { get; set; }
        public DbSet<Class> Classes { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<TimeTable> TimeTables { get; set; }
    }
}
