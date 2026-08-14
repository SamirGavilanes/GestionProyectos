
using GestionProyectos.Data.Entities.Security;
using GestionProyectos.Data.Entities.TaskManagement;
using Microsoft.EntityFrameworkCore;

namespace GestionProyectos.Data
{
    public class DataDbContext : DbContext
    {
        public DataDbContext(DbContextOptions<DataDbContext> options) : base(options) { }

        public DbSet<Menu> Menu { get; set; } = null!;
        public DbSet<Role> Role { get; set; } = null!;
        public DbSet<RoleMenu> RoleMenu { get; set; } = null!;
        public DbSet<User> User { get; set; } = null!;
        public DbSet<UserRole> UserRole { get; set; } = null!;
        public DbSet<UserTimeOff> UserTimeOff { get; set; } = null!;
        public DbSet<Enterprise> Enterprise { get; set; } = null!;
        public DbSet<Customer> Customer { get; set; } = null!;
        public DbSet<Project> Project { get; set; } = null!;
        public DbSet<Attachment> Attachment { get; set; } = null!;
        public DbSet<Entities.TaskManagement.Task> Task { get; set; } = null!;
        public DbSet<TimeLog> TimeLog { get; set; } = null!;
        public DbSet<Requirement> Requirement { get; set; } = null!;
        public DbSet<RequirementStatus> RequirementStatus { get; set; } = null!;
        public DbSet<ProjectStatus> ProjectStatus { get; set; } = null!;
        public DbSet<Priority> Priority { get; set; } = null!;
        public DbSet<Entities.TaskManagement.TaskStatus> TaskStatus { get; set; } = null!;
        public DbSet<Entities.TaskManagement.TaskDevelopmentPhase> TaskDevelopmentPhase { get; set; } = null!;
        public DbSet<HourType> HourType { get; set; } = null!;
        public DbSet<Entities.TaskManagement.TaskStatusHistory> TaskStatusHistory { get; set; } = null!;
        public DbSet<Entities.TaskManagement.TaskBugStatus> TaskBugStatus { get; set; } = null!;
        public DbSet<Entities.TaskManagement.TaskBug> TaskBug { get; set; } = null!;
        public DbSet<Entities.TaskManagement.TaskBugAttachment> TaskBugAttachment { get; set; } = null!;
        public DbSet<Entities.TaskManagement.TaskNote> TaskNote { get; set; } = null!;
        public DbSet<Entities.TaskManagement.BacklogStatus> BacklogStatus { get; set; } = null!;
        public DbSet<Entities.TaskManagement.BacklogItem> BacklogItem { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<TaskStatusHistory>()
                .HasOne(h => h.TaskStatus)
                .WithMany(s => s.StatusHistoryEntries)
                .HasForeignKey(h => h.TaskStatusId);

            modelBuilder.Entity<TaskStatusHistory>()
                .HasOne(h => h.PreviousTaskStatus)
                .WithMany(s => s.PreviousStatusHistoryEntries)
                .HasForeignKey(h => h.PreviousTaskStatusId)
                .IsRequired(false);

            if (Database.IsInMemory())
            {
                // Los motores no siempre asignan todas las cadenas no anulables al crear
                // entidades. Con nullable habilitado EF las trata como requeridas; se relajan
                // para no romper el flujo de desarrollo con la base de datos en memoria.
                foreach (var entityType in modelBuilder.Model.GetEntityTypes())
                {
                    foreach (var property in entityType.GetProperties())
                    {
                        if (property.ClrType == typeof(string) && !property.IsKey())
                            property.IsNullable = true;
                    }
                }
            }
        }
    }
}
