using Microsoft.EntityFrameworkCore;

namespace Time_Table_Generator.Models
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Availability> Availabilities { get; set; }
        public DbSet<EventModel> Events { get; set; }
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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure MySQL-compatible types
            modelBuilder.Entity<Class>(entity =>
            {
                entity.Property(e => e.Name).HasColumnType("varchar(255)"); // Use varchar for strings
                entity.HasMany(c => c.Students) // Configure one-to-many relationship
                      .WithOne()
                      .HasForeignKey(s => s.ClassId)
                      .OnDelete(DeleteBehavior.NoAction);
                entity.HasMany(c => c.Subjects) // Configure Class-Subjects relationship
                      .WithMany(s => s.Classes);
                entity.HasMany(c => c.Teachers) // Configure Class-Teachers relationship
                      .WithMany(t => t.Classes);
            });

            modelBuilder.Entity<Batch>(entity =>
            {
                entity.Property(e => e.Name).HasColumnType("varchar(255)"); // Use varchar for strings
                entity.Property(e => e.ClassId).HasColumnType("int"); // Ensure ClassId is an integer
                entity.HasMany(b => b.Students) // Configure Batch-Students relationship
                      .WithOne(s => s.Batch)
                      .HasForeignKey(s => s.BatchId)
                      .OnDelete(DeleteBehavior.NoAction);
            });

            modelBuilder.Entity<Batch>()
                .HasOne(b => b.Class)
                .WithMany(c => c.Batches)
                .HasForeignKey(b => b.ClassId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<TeacherSubject>(entity =>
            {
                entity.HasOne(ts => ts.Subject)
                    .WithMany()
                    .HasForeignKey(ts => ts.SubjectId)
                    .OnDelete(DeleteBehavior.NoAction);
            });

            modelBuilder.Entity<ClassTeacher>(entity =>
            {
                entity.HasOne(ct => ct.Class)
                    .WithMany()
                    .HasForeignKey(ct => ct.ClassId)
                    .OnDelete(DeleteBehavior.NoAction);
            });

            modelBuilder.Entity<SubjectHour>(entity =>
            {
                entity.HasOne(sh => sh.Subject)
                    .WithMany()
                    .HasForeignKey(sh => sh.SubjectId)
                    .OnDelete(DeleteBehavior.NoAction);
            });

            modelBuilder.Entity<EventClassBatch>(entity =>
            {
                entity.HasOne(ecb => ecb.Batch)
                    .WithMany()
                    .HasForeignKey(ecb => ecb.BatchId)
                    .OnDelete(DeleteBehavior.NoAction);

                entity.HasOne(ecb => ecb.Class)
                    .WithMany()
                    .HasForeignKey(ecb => ecb.ClassId)
                    .OnDelete(DeleteBehavior.NoAction);
            });

            modelBuilder.Entity<Availability>(entity =>
            {
                entity.Property(e => e.Type).HasColumnType("varchar(255)"); // Use varchar for strings
                entity.Property(e => e.IsAvailable).HasColumnType("tinyint(1)"); // Use tinyint for boolean
                entity.Property(e => e.StartTime).HasColumnType("datetime"); // Use datetime for DateTime
                entity.Property(e => e.EndTime).HasColumnType("datetime"); // Use datetime for DateTime
                entity.Property(e => e.Date).HasColumnType("date"); // Use date for DateTime
            });

            modelBuilder.Entity<Teacher>(entity =>
            {
                entity.HasMany(t => t.Classes) // Configure Teacher-Classes relationship
                      .WithMany(c => c.Teachers);

                entity.HasMany(t => t.Subjects) // Configure Teacher-Subjects relationship
                      .WithMany(s => s.Teachers);
            });

            modelBuilder.Entity<Subject>(entity =>
            {
                entity.HasMany(s => s.Classes) // Configure Subject-Classes relationship
                      .WithMany(c => c.Subjects);

                entity.HasMany(s => s.Teachers) // Configure Subject-Teachers relationship
                      .WithMany(t => t.Subjects);
            });
        }
    }
}
