using GestionProyectos.Data.Entities.Security;
using GestionProyectos.Data.Entities.TaskManagement;
using Microsoft.EntityFrameworkCore;

namespace GestionProyectos.Data
{
    /// <summary>
    /// Puebla la base de datos en memoria con datos de prueba para desarrollo.
    /// Usuario de acceso: admin@grupomas.com / Password: 123456
    /// </summary>
    public static class DevDataSeeder
    {
        private const short Active = 1;
        private const short Inactive = 0;

        public static void Seed(DataDbContext context)
        {
            if (context.User.Any())
                return;

            if (context.Database.IsInMemory())
                context.Database.EnsureCreated();

            var now = DateTime.Now;

            // SEGURIDAD -------------------------------------------------------
            var adminRole = new Role { Id = 1, Description = "Administrador" };
            var developerRole = new Role { Id = 2, Description = "Desarrollador" };
            SetAudit(adminRole, now);
            SetAudit(developerRole, now);

            var menus = new List<Menu>
            {
                // GRUPOS
                Menu(100, "Gestión", 1, "", null, 10),
                Menu(101, "Organización", 7, "", null, 20),
                Menu(102, "Catálogos", 10, "", null, 30),
                Menu(103, "Seguridad", 13, "", null, 40),

                // GESTIÓN
                Menu(1, "Proyectos", 1, "/project-list", 100, 11),
                Menu(2, "Requerimientos", 2, "/requirement-list", 100, 12),
                Menu(3, "Tareas", 3, "/task-management", 100, 13),
                Menu(15, "Bugs", 19, "/task-bug-list", 100, 14),
                Menu(21, "Backlog", 23, "/backlog", 100, 15),
                Menu(16, "Desempeño", 20, "/performance", 100, 16),
                Menu(19, "Reporte de horas", 21, "/reporte-horas", 100, 17),
                Menu(20, "Control registro horas", 22, "/control-registro-horas", 100, 18),
                Menu(23, "Bloqueos externos", 25, "/bloqueos-externos", 100, 19),

                // ORGANIZACIÓN
                Menu(5, "Empresas", 7, "/enterprise-list", 101, 21),
                Menu(6, "Clientes", 8, "/customer-list", 101, 22),
                Menu(11, "Miembros del equipo", 15, "/user-list", 101, 23),
                Menu(17, "Ausentismos", 18, "/absence-list", 101, 24),

                // CATÁLOGOS
                Menu(7, "Prioridades", 9, "/priority-list", 102, 31),
                Menu(8, "Estado de Proyecto", 10, "/project-status-list", 102, 32),
                Menu(9, "Estado de Requerimiento", 11, "/requirement-status-list", 102, 33),
                Menu(10, "Estado de Tarea", 12, "/task-status-list", 102, 34),
                Menu(12, "Estado de Bug", 16, "/task-bug-status-list", 102, 35),
                Menu(13, "Fase de Desarrollo", 17, "/task-development-phase-list", 102, 36),
                Menu(18, "Tipos de Hora", 19, "/hour-type-list", 102, 37),
                Menu(22, "Estado de Backlog", 24, "/backlog-status-list", 102, 38),

                // SEGURIDAD
                Menu(111, "Roles", 14, "/security-roles", 103, 41),
                Menu(112, "Funcionalidad", 14, "/security-menus", 103, 42),
                Menu(114, "Permisos", 14, "/security-role-menus", 103, 44),
            };
            foreach (var m in menus) SetAudit(m, now);

            var adminRoleMenus = menus.Select((m, i) => SetAudit(new RoleMenu
            {
                Id = i + 1,
                RoleId = adminRole.Id,
                MenuId = m.Id,
                CanView = true,
                CanCreate = true,
                CanEdit = true,
                CanDelete = true,
                CanRegisterHours = true,
                CanFinalize = true
            }, now)).ToList();

            var developerRoleMenus = new List<RoleMenu>
            {
                SetAudit(new RoleMenu { Id = adminRoleMenus.Count + 1, RoleId = developerRole.Id, MenuId = 100, CanView = true, CanCreate = false, CanEdit = false, CanDelete = false, CanRegisterHours = false, CanFinalize = false }, now),
                SetAudit(new RoleMenu { Id = adminRoleMenus.Count + 2, RoleId = developerRole.Id, MenuId = 3, CanView = true, CanCreate = true, CanEdit = false, CanDelete = false, CanRegisterHours = true, CanFinalize = false }, now),
                SetAudit(new RoleMenu { Id = adminRoleMenus.Count + 3, RoleId = developerRole.Id, MenuId = 15, CanView = true, CanCreate = true, CanEdit = true, CanDelete = false, CanRegisterHours = false, CanFinalize = false }, now),
                SetAudit(new RoleMenu { Id = adminRoleMenus.Count + 4, RoleId = developerRole.Id, MenuId = 21, CanView = true, CanCreate = true, CanEdit = true, CanDelete = false, CanRegisterHours = false, CanFinalize = false }, now),
            };

            var users = new List<User>
            {
                User(1, "Admin", "General", "admin@grupomas.com", "123456", "Gerente de Proyectos"),
                User(2, "Paolo", "Desarrollador", "paolo@grupomas.com", "123456", "Desarrollador Senior"),
                User(3, "Maria", "Analista", "maria@grupomas.com", "123456", "Analista Funcional"),
            };
            foreach (var u in users) SetAudit(u, now);

            var userRoles = new List<UserRole>
            {
                SetAudit(new UserRole { Id = 1, UserId = 1, RoleId = adminRole.Id }, now),
                SetAudit(new UserRole { Id = 2, UserId = 2, RoleId = developerRole.Id }, now),
                SetAudit(new UserRole { Id = 3, UserId = 3, RoleId = adminRole.Id }, now),
            };

            context.Role.AddRange(adminRole, developerRole);
            context.Menu.AddRange(menus);
            context.RoleMenu.AddRange(adminRoleMenus);
            context.RoleMenu.AddRange(developerRoleMenus);
            context.User.AddRange(users);
            context.UserRole.AddRange(userRoles);

            var timeOffs = new List<UserTimeOff>
            {
                new() { Id = 1, UserId = 2, Type = 1, StartDate = now.AddDays(4).Date, EndDate = now.AddDays(8).Date, Hours = 40, Description = "Vacaciones programadas" },
                new() { Id = 2, UserId = 3, Type = 2, StartDate = now.Date, EndDate = now.Date, Hours = 4, Description = "Permiso personal" },
                new() { Id = 3, UserId = 2, Type = 2, StartDate = now.AddDays(-1).Date, EndDate = now.AddDays(2).Date, Hours = 24, Description = "Permiso médico" },
            };
            foreach (var t in timeOffs) SetAudit(t, now);
            context.UserTimeOff.AddRange(timeOffs);

            // CATALOGOS -------------------------------------------------------
            var enterprises = new List<Enterprise>
            {
                Enterprise(1, "Grupo MAS"),
                Enterprise(2, "TuEnlace"),
            };
            foreach (var e in enterprises) SetAudit(e, now);

            var customers = new List<Customer>
            {
                Customer(1, "Banco Pichincha", 1),
                Customer(2, "Claro", 1),
                Customer(3, "Corporacion Favorita", 2),
            };
            foreach (var c in customers) SetAudit(c, now);

            var projectStatuses = new List<ProjectStatus>
            {
                ProjectStatus(1, "Planificado", 1, "gray"),
                ProjectStatus(2, "Activo", 2, "blue"),
                ProjectStatus(3, "En pausa", 3, "amber"),
                ProjectStatus(4, "Finalizado", 4, "green", isClosed: true),
            };
            foreach (var s in projectStatuses) SetAudit(s, now);

            var requirementStatuses = new List<RequirementStatus>
            {
                RequirementStatus(1, "Pendiente", 1, "gray", false),
                RequirementStatus(2, "En progreso", 2, "amber", false),
                RequirementStatus(3, "Completado", 3, "green", true),
            };
            foreach (var s in requirementStatuses) SetAudit(s, now);

            var priorities = new List<Priority>
            {
                Priority(1, "Alta", "red"),
                Priority(2, "Media", "amber"),
                Priority(3, "Baja", "green"),
            };
            foreach (var p in priorities) SetAudit(p, now);

            var taskStatuses = new List<Entities.TaskManagement.TaskStatus>
            {
                TaskStatus(1, "Pendiente", 1, "gray"),
                TaskStatus(2, "En progreso", 2, "blue"),
                TaskStatus(3, "Finalizada", 3, "green"),
                TaskStatus(4, "Bloqueo interno", 4, "red"),
                TaskStatus(5, "Bloqueo externo", 5, "orange"),
            };
            foreach (var s in taskStatuses) SetAudit(s, now);

            var taskBugStatuses = new List<TaskBugStatus>
            {
                TaskBugStatus(1, "Reportado", 1, "red"),
                TaskBugStatus(2, "En revisión", 2, "amber"),
                TaskBugStatus(3, "Corregido", 3, "blue"),
                TaskBugStatus(4, "Cerrado", 4, "green"),
            };
            foreach (var s in taskBugStatuses) SetAudit(s, now);

            var backlogStatuses = new List<BacklogStatus>
            {
                BacklogStatus(1, "Pendiente", 1, "amber"),
                BacklogStatus(2, "En análisis", 2, "sky"),
                BacklogStatus(3, "En requerimiento", 3, "green", isClosed: true),
                BacklogStatus(4, "Descartado", 4, "gray", isClosed: true),
            };
            foreach (var s in backlogStatuses) SetAudit(s, now);

            var developmentPhases = new List<TaskDevelopmentPhase>
            {
                DevelopmentPhase(5, "Planificada", 0, "sky"),
                DevelopmentPhase(1, "Desarrollo", 1, "blue"),
                DevelopmentPhase(2, "QA", 2, "amber"),
                DevelopmentPhase(3, "Lista para instalar", 3, "purple"),
                DevelopmentPhase(4, "En producción", 4, "green"),
            };
            foreach (var p in developmentPhases) SetAudit(p, now);

            var hourTypes = new List<HourType>
            {
                HourType(1, "Horas de Negocio", "Negocio, planificación y análisis para entender y estructurar el problema.", 1, "blue"),
                HourType(2, "Horas Técnicas", "Desarrollo, base de datos, arquitectura e infraestructura.", 2, "amber"),
                HourType(3, "Horas de Calidad", "Pruebas y validación antes de cerrar la tarea.", 3, "green"),
            };
            foreach (var h in hourTypes) SetAudit(h, now);

            context.Enterprise.AddRange(enterprises);
            context.Customer.AddRange(customers);
            context.ProjectStatus.AddRange(projectStatuses);
            context.RequirementStatus.AddRange(requirementStatuses);
            context.Priority.AddRange(priorities);
            context.TaskStatus.AddRange(taskStatuses);
            context.TaskBugStatus.AddRange(taskBugStatuses);
            context.BacklogStatus.AddRange(backlogStatuses);
            context.TaskDevelopmentPhase.AddRange(developmentPhases);
            context.HourType.AddRange(hourTypes);

            // PROYECTOS -------------------------------------------------------
            var projects = new List<Project>
            {
                Project(1, "Portal de Clientes", customerId: 1, projectStatusId: 2, start: now.AddMonths(-2), end: now.AddMonths(1)),
                Project(2, "App Movil de Cobros", customerId: 2, projectStatusId: 2, start: now.AddMonths(-1), end: now.AddMonths(2)),
                Project(3, "Integracion ERP", customerId: 3, projectStatusId: 4, start: now.AddMonths(-4), end: now.AddMonths(-1)),
            };
            foreach (var p in projects) SetAudit(p, now);
            context.Project.AddRange(projects);

            // REQUERIMIENTOS --------------------------------------------------
            var requirements = new List<Requirement>
            {
                Requirement(1, "Login con OTP", projectId: 1, statusId: 2, priorityId: 1, start: now.AddMonths(-2)),
                Requirement(2, "Dashboard de saldos", projectId: 1, statusId: 1, priorityId: 2, start: now.AddMonths(-1)),
                Requirement(3, "Pago con tarjeta", projectId: 2, statusId: 2, priorityId: 1, start: now.AddDays(-20)),
                Requirement(4, "Sincronizacion de facturas", projectId: 3, statusId: 3, priorityId: 3, start: now.AddMonths(-4)),
            };
            foreach (var r in requirements) SetAudit(r, now);
            context.Requirement.AddRange(requirements);

            // TAREAS ----------------------------------------------------------
            var tasks = new List<Entities.TaskManagement.Task>
            {
                Task(1, requirementId: 1, userId: 2, hours: 8, "Disenar pantalla de login", priorityId: 1, statusId: 2, start: now.AddDays(-10), developmentPhaseId: 1),
                Task(2, requirementId: 1, userId: 2, hours: 5, "Implementar envio de OTP", priorityId: 1, statusId: 5, start: now.AddDays(-8), withinScope: false, scopeReason: 1, developmentPhaseId: 2),
                Task(3, requirementId: 2, userId: 3, hours: 12, "Consulta de saldos", priorityId: 2, statusId: 1, start: now.AddDays(-6), developmentPhaseId: 1),
                Task(4, requirementId: 3, userId: 2, hours: 16, "Pasarela de pagos", priorityId: 1, statusId: 2, start: now.AddDays(-5), developmentPhaseId: 3),
                Task(5, requirementId: 1, userId: 1, hours: 6, "Revision de avance semanal", priorityId: 2, statusId: 2, start: now.AddDays(-4), developmentPhaseId: 2),
            };
            foreach (var t in tasks) SetAudit(t, now);
            context.Task.AddRange(tasks);

            // REGISTROS DE TIEMPO ---------------------------------------------
            var timeLogs = new List<TimeLog>
            {
                TimeLog(1, userId: 2, taskId: 1, hours: 4, date: now.AddDays(-9), progress: 10, hourTypeId: 2),
                TimeLog(2, userId: 2, taskId: 1, hours: 4, date: now.AddDays(-8), progress: 15, hourTypeId: 2),
                TimeLog(3, userId: 2, taskId: 2, hours: 3, date: now.AddDays(-7), progress: 10, hourTypeId: 1),
                TimeLog(4, userId: 3, taskId: 3, hours: 6, date: now.AddDays(-5), progress: 25, hourTypeId: 2),
                TimeLog(5, userId: 2, taskId: 4, hours: 8, date: now.AddDays(-4), progress: 20, hourTypeId: 2),
                TimeLog(6, userId: 2, taskId: 1, hours: 2, date: now.Date, progress: 5, hourTypeId: 3),
                TimeLog(7, userId: 2, taskId: 4, hours: 3, date: now.Date, progress: 30, hourTypeId: 2),
                TimeLog(8, userId: 3, taskId: 3, hours: 2, date: now.Date, progress: 15, hourTypeId: 1),
                TimeLog(9, userId: 1, taskId: 5, hours: 2, date: now.Date, progress: 0, hourTypeId: 1),
            };
            foreach (var tl in timeLogs) SetAudit(tl, now);
            context.AddRange(timeLogs);

            // ADJUNTOS --------------------------------------------------------
            var attachments = new List<Attachment>
            {
                SetAudit(new Attachment { Id = 1, RequirementId = 1, FileName = "mockup-login.png", FilePath = "GestionProyectosQA/mockup-login.png" }, now),
                SetAudit(new Attachment { Id = 2, RequirementId = 4, FileName = "spec-erp.pdf", FilePath = "GestionProyectosQA/spec-erp.pdf" }, now),
            };
            context.Attachment.AddRange(attachments);

            var taskStatusHistory = new List<TaskStatusHistory>
            {
                new()
                {
                    Id = 1, TaskId = 2, TaskStatusId = 5, PreviousTaskStatusId = 1,
                    Reason = "Falta definir plantilla de correo OTP con el cliente.",
                    ChangedByUserId = 2, Created = now.AddDays(-6), CreatedBy = 2, RowStatus = Active
                },
                new()
                {
                    Id = 2, TaskId = 2, TaskStatusId = 1, PreviousTaskStatusId = null,
                    Reason = "Tarea creada en estado Pendiente.",
                    ChangedByUserId = 2, Created = now.AddDays(-8), CreatedBy = 2, RowStatus = Active
                }
            };
            context.TaskStatusHistory.AddRange(taskStatusHistory);

            var taskBugs = new List<TaskBug>
            {
                new()
                {
                    Id = 1, RequirementId = 1, TaskId = 2, Description = "El código OTP expira antes de lo indicado en pantalla (5 min vs 2 min reales).",
                    TaskBugStatusId = 1, ReportedByUserId = 2,
                    StartDate = now.AddDays(-5), EndDate = now.AddDays(-5),
                    Created = now.AddDays(-5), CreatedBy = 2, RowStatus = Active
                },
                new()
                {
                    Id = 2, RequirementId = 1, TaskId = 1, Description = "Ajuste de contraste en botón de login.",
                    TaskBugStatusId = 3, ReportedByUserId = 2,
                    StartDate = now.AddDays(-6), EndDate = now.AddDays(-4),
                    Created = now.AddDays(-6), CreatedBy = 2, RowStatus = Active
                },
                new()
                {
                    Id = 3, RequirementId = 3, TaskId = 4, Description = "Timeout en pasarela con tarjetas corporativas.",
                    TaskBugStatusId = 1, ReportedByUserId = 2,
                    StartDate = now.AddDays(-3), EndDate = now.AddDays(-3),
                    Created = now.AddDays(-3), CreatedBy = 2, RowStatus = Active
                },
                new()
                {
                    Id = 4, RequirementId = 3, TaskId = 4, Description = "Validación incorrecta de CVV.",
                    TaskBugStatusId = 2, ReportedByUserId = 2,
                    StartDate = now.AddDays(-2), EndDate = now.AddDays(-1),
                    Created = now.AddDays(-2), CreatedBy = 2, RowStatus = Active
                },
                new()
                {
                    Id = 5, RequirementId = 2, TaskId = null, Description = "Gráfico de saldos no actualiza al cambiar de moneda.",
                    TaskBugStatusId = 1, ReportedByUserId = 1,
                    StartDate = now.AddDays(-1), EndDate = now.AddDays(-1),
                    Created = now.AddDays(-1), CreatedBy = 1, RowStatus = Active
                }
            };
            context.TaskBug.AddRange(taskBugs);

            var taskBugAttachments = new List<TaskBugAttachment>
            {
                SetAudit(new TaskBugAttachment
                {
                    Id = 1, TaskBugId = 1, FileName = "otp-expiracion.png",
                    FilePath = "GestionProyectosQA/tasks/2/bugs/1/"
                }, now)
            };
            context.TaskBugAttachment.AddRange(taskBugAttachments);

            var taskNotes = new List<TaskNote>
            {
                new()
                {
                    Id = 1, TaskId = 2,
                    Content = "Revisar con el cliente el tiempo de expiración del OTP antes de continuar con las pruebas.",
                    AuthorUserId = 2,
                    Created = now.AddDays(-4), CreatedBy = 2, RowStatus = Active
                },
                new()
                {
                    Id = 2, TaskId = 2,
                    Content = "Plantilla de correo pendiente de aprobación por marketing.",
                    AuthorUserId = 1,
                    Created = now.AddDays(-3), CreatedBy = 1, RowStatus = Active
                }
            };
            context.TaskNote.AddRange(taskNotes);

            var backlogItems = new List<BacklogItem>
            {
                new()
                {
                    Id = 1, Name = "Notificaciones push",
                    Description = "Enviar alertas al móvil cuando una tarea cambie de estado.",
                    BacklogStatusId = 1,
                    Created = now.AddDays(-10), CreatedBy = 1, RowStatus = Active
                },
                new()
                {
                    Id = 2, Name = "Exportar backlog a Excel",
                    Description = "Permitir descargar el backlog filtrado en formato Excel.",
                    BacklogStatusId = 2,
                    Created = now.AddDays(-7), CreatedBy = 1, RowStatus = Active
                },
                new()
                {
                    Id = 3, Name = "Plantillas de requerimiento",
                    Description = "Crear requerimientos a partir de plantillas predefinidas.",
                    BacklogStatusId = 3,
                    Created = now.AddDays(-5), CreatedBy = 2, RowStatus = Active
                },
                new()
                {
                    Id = 4, Name = "Integración con Slack",
                    Description = "Publicar cambios de estado en un canal de Slack.",
                    BacklogStatusId = 4,
                    Created = now.AddDays(-2), CreatedBy = 1, RowStatus = Active
                }
            };
            context.BacklogItem.AddRange(backlogItems);

            context.SaveChanges();
        }

        /// <summary>
        /// Asegura los ítems de Seguridad en el menú lateral (bases ya pobladas).
        /// </summary>
        public static void EnsureSecurityMenus(DataDbContext context)
        {
            var now = DateTime.UtcNow;

            foreach (var legacy in context.Menu.Where(m =>
                         m.RowStatus == Active &&
                         ((m.Page == "/security-admin" && m.Id != 103) ||
                          m.Id == 113 ||
                          m.Page == "/security-user-roles")).ToList())
            {
                legacy.RowStatus = Inactive;
                legacy.Updated = now;
                legacy.UpdatedBy = 1;
            }

            EnsureMenu(context, 103, "Seguridad", 13, "", null, 40, now);

            var securityItems = new (long Id, string Description, int Icon, string Page, int Order)[]
            {
                (111, "Roles", 14, "/security-roles", 41),
                (112, "Funcionalidad", 14, "/security-menus", 42),
                (114, "Permisos", 14, "/security-role-menus", 44),
            };

            foreach (var item in securityItems)
                EnsureMenu(context, item.Id, item.Description, item.Icon, item.Page, 103, item.Order, now);

            EnsureRoleMenu(context, 1, 103, now);
            foreach (var item in securityItems)
                EnsureRoleMenu(context, 1, item.Id, now);

            context.SaveChanges();
        }

        /// <summary>
        /// Actualiza catálogo de estados de proyecto en bases ya pobladas.
        /// </summary>
        public static void EnsureProjectStatuses(DataDbContext context)
        {
            var now = DateTime.UtcNow;
            var specs = new (long Id, string Description, int Order, string BadgeColor, bool IsClosed)[]
            {
                (1, "Planificado", 1, "gray", false),
                (2, "Activo", 2, "blue", false),
                (3, "En pausa", 3, "amber", false),
                (4, "Finalizado", 4, "green", true),
            };

            var needsProjectMigration = !context.ProjectStatus.Any(s => s.Id == 4 && s.Description == "Finalizado");

            if (needsProjectMigration)
            {
                foreach (var project in context.Project.Where(p => p.RowStatus == Active))
                {
                    project.ProjectStatusId = project.ProjectStatusId switch
                    {
                        1 => 2, // Activo (antiguo) -> Activo
                        2 => 2, // En progreso (antiguo) -> Activo
                        3 => 4, // Cerrado (antiguo) -> Finalizado
                        _ => project.ProjectStatusId
                    };
                    project.Updated = now;
                    project.UpdatedBy = 1;
                }
            }

            foreach (var spec in specs)
            {
                var status = context.ProjectStatus.FirstOrDefault(s => s.Id == spec.Id);
                if (status == null)
                {
                    context.ProjectStatus.Add(SetAudit(ProjectStatus(spec.Id, spec.Description, spec.Order, spec.BadgeColor, spec.IsClosed), now));
                    continue;
                }

                status.Description = spec.Description;
                status.Order = spec.Order;
                status.BadgeColor = spec.BadgeColor;
                status.IsClosed = spec.IsClosed;
                status.RowStatus = Active;
                status.Updated = now;
                status.UpdatedBy = 1;
            }

            context.SaveChanges();
        }

        public static void EnsurePlanificadaDevelopmentPhase(DataDbContext context)
        {
            if (context.TaskDevelopmentPhase.Any(x => x.Description == "Planificada"))
                return;

            var now = DateTime.UtcNow;
            const long plannedId = 5;

            if (!context.TaskDevelopmentPhase.Any(x => x.Id == plannedId))
                context.TaskDevelopmentPhase.Add(SetAudit(DevelopmentPhase(plannedId, "Planificada", 0, "sky"), now));
            else
                context.TaskDevelopmentPhase.Add(SetAudit(new TaskDevelopmentPhase
                {
                    Description = "Planificada",
                    Order = 0,
                    BadgeColor = "sky"
                }, now));

            context.SaveChanges();
        }

        /// <summary>
        /// Asegura columna IsClosed y valores en estados de requerimiento (bases ya pobladas).
        /// </summary>
        public static void EnsureRequirementStatusIsClosed(DataDbContext context)
        {
            if (context.Database.IsRelational())
            {
                context.Database.ExecuteSqlRaw(
                    "ALTER TABLE \"TaskManagement\".\"RequirementStatus\" ADD COLUMN IF NOT EXISTS \"IsClosed\" boolean NOT NULL DEFAULT false;");
            }

            var now = DateTime.UtcNow;
            var specs = new (long Id, string Description, int Order, string BadgeColor, bool IsClosed)[]
            {
                (1, "Pendiente", 1, "gray", false),
                (2, "En progreso", 2, "amber", false),
                (3, "Completado", 3, "green", true),
            };

            foreach (var spec in specs)
            {
                var status = context.RequirementStatus.FirstOrDefault(s => s.Id == spec.Id);
                if (status == null)
                {
                    context.RequirementStatus.Add(SetAudit(RequirementStatus(spec.Id, spec.Description, spec.Order, spec.BadgeColor, spec.IsClosed), now));
                    continue;
                }

                status.Description = spec.Description;
                status.Order = spec.Order;
                status.BadgeColor = spec.BadgeColor;
                status.IsClosed = spec.IsClosed;
                status.RowStatus = Active;
                status.Updated = now;
                status.UpdatedBy = 1;
            }

            context.SaveChanges();
        }

        /// <summary>
        /// Asegura columnas de clasificación en requerimientos (bases ya pobladas).
        /// </summary>
        public static void EnsureRequirementClassificationColumns(DataDbContext context)
        {
            if (context.Database.IsRelational())
            {
                context.Database.ExecuteSqlRaw(
                    "ALTER TABLE \"TaskManagement\".\"Requirement\" ADD COLUMN IF NOT EXISTS \"IsWithinOriginalScope\" boolean NOT NULL DEFAULT true;");
                context.Database.ExecuteSqlRaw(
                    "ALTER TABLE \"TaskManagement\".\"Requirement\" ADD COLUMN IF NOT EXISTS \"ScopeChangeReason\" smallint NULL;");
                context.Database.ExecuteSqlRaw(
                    "ALTER TABLE \"TaskManagement\".\"Requirement\" ADD COLUMN IF NOT EXISTS \"IsProductionReprocess\" boolean NOT NULL DEFAULT false;");
                context.Database.ExecuteSqlRaw(
                    "ALTER TABLE \"TaskManagement\".\"Requirement\" ADD COLUMN IF NOT EXISTS \"FreshDeskTicketNumber\" character varying(100) NULL;");
            }

            context.SaveChanges();
        }

        /// <summary>
        /// Asegura columna Hours en Security.UserTimeOff (bases PostgreSQL ya creadas).
        /// </summary>
        public static void EnsureUserTimeOffHours(DataDbContext context)
        {
            if (context.Database.IsInMemory())
                return;

            try
            {
                context.Database.ExecuteSqlRaw(
                    "ALTER TABLE \"Security\".\"UserTimeOff\" ADD COLUMN IF NOT EXISTS \"Hours\" numeric NOT NULL DEFAULT 0;");
            }
            catch
            {
                // Si el esquema no existe aún, EnsureCreated lo creará con la columna.
            }
        }

        /// <summary>
        /// Catálogo de tipos de hora y columna en TimeLog (bases ya pobladas).
        /// </summary>
        public static void EnsureHourTypeCatalog(DataDbContext context)
        {
            var now = DateTime.UtcNow;

            if (!context.Database.IsInMemory())
            {
                context.Database.ExecuteSqlRaw(
                    "CREATE TABLE IF NOT EXISTS \"TaskManagement\".\"HourType\" (" +
                    "\"Id\" bigint NOT NULL PRIMARY KEY, " +
                    "\"Description\" text NOT NULL, " +
                    "\"Scope\" text NULL, " +
                    "\"Order\" integer NOT NULL DEFAULT 0, " +
                    "\"BadgeColor\" text NOT NULL DEFAULT 'gray', " +
                    "\"RowStatus\" smallint NOT NULL DEFAULT 1, " +
                    "\"Created\" timestamp without time zone NOT NULL DEFAULT NOW(), " +
                    "\"CreatedBy\" bigint NOT NULL DEFAULT 1, " +
                    "\"Updated\" timestamp without time zone NULL, " +
                    "\"UpdatedBy\" bigint NULL);");
                context.Database.ExecuteSqlRaw(
                    "ALTER TABLE \"TaskManagement\".\"TimeLog\" ADD COLUMN IF NOT EXISTS \"HourTypeId\" bigint NULL;");
            }

            var defaults = new (long Id, string Description, string Scope, int Order, string BadgeColor)[]
            {
                (1, "Horas de Negocio", "Negocio, planificación y análisis para entender y estructurar el problema.", 1, "blue"),
                (2, "Horas Técnicas", "Desarrollo, base de datos, arquitectura e infraestructura.", 2, "amber"),
                (3, "Horas de Calidad", "Pruebas y validación antes de cerrar la tarea.", 3, "green"),
            };

            foreach (var item in defaults)
                UpsertHourType(context, item.Id, item.Description, item.Scope, item.Order, item.BadgeColor, now);

            if (!context.Database.IsInMemory())
            {
                context.Database.ExecuteSqlRaw(
                    "UPDATE \"TaskManagement\".\"TimeLog\" SET \"HourTypeId\" = 2 WHERE \"HourTypeId\" IS NULL;");
            }

            EnsureMenu(context, 18, "Tipos de Hora", 19, "/hour-type-list", 102, 37, now);
            EnsureRoleMenu(context, 1, 18, now);
            context.SaveChanges();
        }

        private static void UpsertHourType(
            DataDbContext context,
            long id,
            string description,
            string scope,
            int order,
            string badgeColor,
            DateTime now)
        {
            var item = context.HourType.FirstOrDefault(h => h.Id == id);
            if (item == null)
            {
                context.HourType.Add(SetAudit(new HourType
                {
                    Id = id,
                    Description = description,
                    Scope = scope,
                    Order = order,
                    BadgeColor = badgeColor
                }, now));
                return;
            }

            item.Description = description;
            item.Scope = scope;
            item.Order = order;
            item.BadgeColor = badgeColor;
            item.RowStatus = Active;
            item.Updated = now;
            item.UpdatedBy = 1;
        }

        /// <summary>
        /// Asegura el menú Backlog bajo Gestión, debajo de Bugs (bases ya pobladas).
        /// </summary>
        public static void EnsureBacklogMenu(DataDbContext context)
        {
            var now = DateTime.UtcNow;
            EnsureMenu(context, 21, "Backlog", 23, "/backlog", 100, 15, now);
            EnsureRoleMenu(context, 1, 21, now);

            // Mantener Backlog justo debajo de Bugs en bases ya pobladas.
            EnsureMenu(context, 16, "Desempeño", 20, "/performance", 100, 16, now);
            EnsureMenu(context, 19, "Reporte de horas", 21, "/reporte-horas", 100, 17, now);
            EnsureMenu(context, 20, "Control registro horas", 22, "/control-registro-horas", 100, 18, now);

            context.SaveChanges();
        }

        /// <summary>
        /// Asegura catálogo BacklogStatus, columna FK en BacklogItem y menú en Catálogos.
        /// </summary>
        public static void EnsureBacklogStatusCatalog(DataDbContext context)
        {
            var now = DateTime.UtcNow;

            if (!context.Database.IsInMemory())
            {
                try
                {
                    context.Database.ExecuteSqlRaw(
                        "CREATE TABLE IF NOT EXISTS \"TaskManagement\".\"BacklogStatus\" (" +
                        "\"Id\" bigint GENERATED BY DEFAULT AS IDENTITY PRIMARY KEY, " +
                        "\"Description\" text NOT NULL, " +
                        "\"Order\" integer NOT NULL DEFAULT 0, " +
                        "\"BadgeColor\" text NOT NULL DEFAULT 'gray', " +
                        "\"IsClosed\" boolean NOT NULL DEFAULT false, " +
                        "\"RowStatus\" smallint NOT NULL DEFAULT 1, " +
                        "\"Created\" timestamp without time zone NOT NULL DEFAULT NOW(), " +
                        "\"CreatedBy\" bigint NOT NULL DEFAULT 1, " +
                        "\"Updated\" timestamp without time zone NULL, " +
                        "\"UpdatedBy\" bigint NULL);");

                    context.Database.ExecuteSqlRaw(
                        "ALTER TABLE \"TaskManagement\".\"BacklogItem\" ADD COLUMN IF NOT EXISTS \"BacklogStatusId\" bigint NULL;");
                }
                catch
                {
                    // Si el esquema no existe aún, EnsureCreated lo creará.
                }
            }

            var specs = new (long Id, string Description, int Order, string BadgeColor, bool IsClosed)[]
            {
                (1, "Pendiente", 1, "amber", false),
                (2, "En análisis", 2, "sky", false),
                (3, "En requerimiento", 3, "green", true),
                (4, "Descartado", 4, "gray", true),
            };

            foreach (var spec in specs)
            {
                var existing = context.BacklogStatus.FirstOrDefault(x => x.Id == spec.Id);
                if (existing == null)
                {
                    context.BacklogStatus.Add(SetAudit(BacklogStatus(spec.Id, spec.Description, spec.Order, spec.BadgeColor, spec.IsClosed), now));
                    continue;
                }

                existing.Description = spec.Description;
                existing.Order = spec.Order;
                existing.BadgeColor = spec.BadgeColor;
                existing.IsClosed = spec.IsClosed;
                existing.RowStatus = Active;
                existing.Updated = now;
                existing.UpdatedBy = 1;
            }

            context.SaveChanges();

            // Migrar textos legacy de Status a BacklogStatusId cuando aplique.
            if (!context.Database.IsInMemory())
            {
                try
                {
                    context.Database.ExecuteSqlRaw(
                        "UPDATE \"TaskManagement\".\"BacklogItem\" SET \"BacklogStatusId\" = 1 " +
                        "WHERE \"BacklogStatusId\" IS NULL AND (\"Status\" IS NULL OR LOWER(\"Status\") = 'pendiente');");
                    context.Database.ExecuteSqlRaw(
                        "UPDATE \"TaskManagement\".\"BacklogItem\" SET \"BacklogStatusId\" = 2 " +
                        "WHERE \"BacklogStatusId\" IS NULL AND LOWER(\"Status\") IN ('en análisis', 'en analisis');");
                    context.Database.ExecuteSqlRaw(
                        "UPDATE \"TaskManagement\".\"BacklogItem\" SET \"BacklogStatusId\" = 3 " +
                        "WHERE \"BacklogStatusId\" IS NULL AND LOWER(\"Status\") IN ('aprobado', 'en requerimiento');");
                    context.Database.ExecuteSqlRaw(
                        "UPDATE \"TaskManagement\".\"BacklogItem\" SET \"BacklogStatusId\" = 4 " +
                        "WHERE \"BacklogStatusId\" IS NULL AND LOWER(\"Status\") = 'descartado';");
                    context.Database.ExecuteSqlRaw(
                        "UPDATE \"TaskManagement\".\"BacklogItem\" SET \"BacklogStatusId\" = 1 WHERE \"BacklogStatusId\" IS NULL;");
                }
                catch
                {
                    // Columna Status puede no existir en esquemas nuevos.
                }
            }

            foreach (var item in context.BacklogItem.ToList())
            {
                if (item.BacklogStatusId <= 0)
                    item.BacklogStatusId = 1;
            }

            EnsureMenu(context, 22, "Estado de Backlog", 24, "/backlog-status-list", 102, 38, now);
            EnsureRoleMenu(context, 1, 22, now);
            context.SaveChanges();
        }

        /// <summary>
        /// Asegura tabla BacklogItem y datos demo (bases PostgreSQL ya creadas).
        /// </summary>
        public static void EnsureBacklogItemTable(DataDbContext context)
        {
            var now = DateTime.UtcNow;

            if (!context.Database.IsInMemory())
            {
                try
                {
                    context.Database.ExecuteSqlRaw(
                        "CREATE TABLE IF NOT EXISTS \"TaskManagement\".\"BacklogItem\" (" +
                        "\"Id\" bigint GENERATED BY DEFAULT AS IDENTITY PRIMARY KEY, " +
                        "\"Name\" text NOT NULL, " +
                        "\"Description\" text NOT NULL DEFAULT '', " +
                        "\"BacklogStatusId\" bigint NOT NULL DEFAULT 1, " +
                        "\"CustomerId\" bigint NULL, " +
                        "\"RowStatus\" smallint NOT NULL DEFAULT 1, " +
                        "\"Created\" timestamp without time zone NOT NULL DEFAULT NOW(), " +
                        "\"CreatedBy\" bigint NOT NULL DEFAULT 1, " +
                        "\"Updated\" timestamp without time zone NULL, " +
                        "\"UpdatedBy\" bigint NULL);");
                    context.Database.ExecuteSqlRaw(
                        "ALTER TABLE \"TaskManagement\".\"BacklogItem\" ADD COLUMN IF NOT EXISTS \"CustomerId\" bigint NULL;");
                }
                catch
                {
                    // Si el esquema no existe aún, EnsureCreated lo creará con la tabla.
                }
            }

            if (context.BacklogItem.Any(x => x.RowStatus == Active))
                return;

            var nextId = context.BacklogItem.Any() ? context.BacklogItem.Max(x => x.Id) + 1 : 1;
            var demo = new (string Name, string Description, long StatusId)[]
            {
                ("Notificaciones push", "Enviar alertas al móvil cuando una tarea cambie de estado.", 1),
                ("Exportar backlog a Excel", "Permitir descargar el backlog filtrado en formato Excel.", 2),
                ("Plantillas de requerimiento", "Crear requerimientos a partir de plantillas predefinidas.", 3),
                ("Integración con Slack", "Publicar cambios de estado en un canal de Slack.", 4),
            };

            foreach (var item in demo)
            {
                context.BacklogItem.Add(new BacklogItem
                {
                    Id = nextId++,
                    Name = item.Name,
                    Description = item.Description,
                    BacklogStatusId = item.StatusId,
                    RowStatus = Active,
                    Created = now,
                    CreatedBy = 1
                });
            }

            context.SaveChanges();
        }

        /// <summary>
        /// Asegura el menú Desempeño bajo Gestión (bases ya pobladas).
        /// </summary>
        public static void EnsurePerformanceMenu(DataDbContext context)
        {
            var now = DateTime.UtcNow;
            EnsureMenu(context, 16, "Desempeño", 20, "/performance", 100, 16, now);
            EnsureRoleMenu(context, 1, 16, now);
            context.SaveChanges();
        }

        /// <summary>
        /// Asegura el menú Reporte de horas bajo Gestión (bases ya pobladas).
        /// </summary>
        public static void EnsureBillingMenu(DataDbContext context)
        {
            var now = DateTime.UtcNow;
            EnsureMenu(context, 19, "Reporte de horas", 21, "/reporte-horas", 100, 17, now);
            EnsureRoleMenu(context, 1, 19, now);
            context.SaveChanges();
        }

        /// <summary>
        /// Asegura el menú Control registro horas bajo Gestión (bases ya pobladas).
        /// </summary>
        public static void EnsureHoursReportMenu(DataDbContext context)
        {
            var now = DateTime.UtcNow;
            EnsureMenu(context, 20, "Control registro horas", 22, "/control-registro-horas", 100, 18, now);

            var gestionMenuIds = context.Menu
                .Where(m => m.RowStatus == Active && m.Parent == 100 && !string.IsNullOrWhiteSpace(m.Page))
                .Select(m => m.Id)
                .ToList();

            var roleIds = context.RoleMenu
                .Where(r => r.RowStatus == Active && r.CanView && gestionMenuIds.Contains(r.MenuId))
                .Select(r => r.RoleId)
                .Distinct()
                .ToList();

            if (!roleIds.Contains(1))
                roleIds.Add(1);

            foreach (var roleId in roleIds)
                EnsureRoleMenu(context, roleId, 20, now);

            context.SaveChanges();
        }

        /// <summary>
        /// Asegura el menú Bloqueos externos bajo Gestión (bases ya pobladas).
        /// </summary>
        public static void EnsureExternalBlockReportMenu(DataDbContext context)
        {
            var now = DateTime.UtcNow;
            EnsureMenu(context, 23, "Bloqueos externos", 25, "/bloqueos-externos", 100, 19, now);
            EnsureRoleMenu(context, 1, 23, now);
            context.SaveChanges();
        }

        /// <summary>
        /// Asegura columnas de avatar en Security.User (bases PostgreSQL ya creadas).
        /// </summary>
        public static void EnsureUserAvatarColumns(DataDbContext context)
        {
            if (context.Database.IsInMemory())
                return;

            try
            {
                context.Database.ExecuteSqlRaw(
                    "ALTER TABLE \"Security\".\"User\" ADD COLUMN IF NOT EXISTS \"AvatarFileName\" text NULL;");
                context.Database.ExecuteSqlRaw(
                    "ALTER TABLE \"Security\".\"User\" ADD COLUMN IF NOT EXISTS \"AvatarFilePath\" text NULL;");
            }
            catch
            {
                // Si el esquema no existe aún, EnsureCreated lo creará con las columnas.
            }
        }

        /// <summary>
        /// Ausentismos bajo Organización (/absence-list) y retira el menú legacy duplicado.
        /// </summary>
        public static void EnsureAbsenceManagementMenu(DataDbContext context)
        {
            var now = DateTime.UtcNow;
            EnsureMenu(context, 17, "Ausentismos", 18, "/absence-list", 101, 24, now);
            EnsureRoleMenu(context, 1, 17, now);

            foreach (var legacyRoleMenu in context.RoleMenu.Where(r => r.MenuId == 14 && r.RowStatus == Active).ToList())
            {
                var target = context.RoleMenu.FirstOrDefault(r => r.RoleId == legacyRoleMenu.RoleId && r.MenuId == 17);
                if (target == null)
                {
                    context.RoleMenu.Add(SetAudit(new RoleMenu
                    {
                        Id = GetNextRoleMenuId(context),
                        RoleId = legacyRoleMenu.RoleId,
                        MenuId = 17,
                        CanView = legacyRoleMenu.CanView,
                        CanCreate = legacyRoleMenu.CanCreate,
                        CanEdit = legacyRoleMenu.CanEdit,
                        CanDelete = legacyRoleMenu.CanDelete,
                        CanRegisterHours = legacyRoleMenu.CanRegisterHours,
                        CanFinalize = legacyRoleMenu.CanFinalize
                    }, now));
                }
                else
                {
                    target.RowStatus = Active;
                    target.CanView |= legacyRoleMenu.CanView;
                    target.CanCreate |= legacyRoleMenu.CanCreate;
                    target.CanEdit |= legacyRoleMenu.CanEdit;
                    target.CanDelete |= legacyRoleMenu.CanDelete;
                    target.CanRegisterHours |= legacyRoleMenu.CanRegisterHours;
                    target.CanFinalize |= legacyRoleMenu.CanFinalize;
                    target.Updated = now;
                    target.UpdatedBy = 1;
                }
            }

            var legacyMenu = context.Menu.FirstOrDefault(m => m.Id == 14);
            if (legacyMenu != null)
            {
                legacyMenu.RowStatus = Inactive;
                legacyMenu.Updated = now;
                legacyMenu.UpdatedBy = 1;
            }

            foreach (var roleMenu in context.RoleMenu.Where(r => r.MenuId == 14 && r.RowStatus == Active).ToList())
            {
                roleMenu.RowStatus = Inactive;
                roleMenu.Updated = now;
                roleMenu.UpdatedBy = 1;
            }

            context.SaveChanges();
        }

        /// <summary>
        /// Asegura registros de tiempo recientes para la pantalla de desempeño (bases ya pobladas).
        /// </summary>
        public static void EnsurePerformanceDemoData(DataDbContext context)
        {
            var now = DateTime.Now;
            var activeTasks = context.Task
                .Where(t => t.RowStatus == Active && t.TimeEstimationHours > 0)
                .Select(t => new { t.Id, t.UserId })
                .ToList();

            if (activeTasks.Count == 0)
                return;

            var nextTimeLogId = context.TimeLog.Any() ? context.TimeLog.Max(t => t.Id) + 1 : 1;
            var added = false;

            foreach (var task in activeTasks)
            {
                var hasRecentLog = context.TimeLog.Any(tl =>
                    tl.TaskId == task.Id
                    && tl.RowStatus == Active
                    && tl.ExecutionDate >= now.AddMonths(-6));

                if (hasRecentLog)
                    continue;

                var log = SetAudit(new TimeLog
                {
                    Id = nextTimeLogId++,
                    TaskId = task.Id,
                    UserId = task.UserId ?? 0,
                    UsedHours = 2,
                    ExecutionDate = now.AddDays(-3),
                    ProgressPercent = 50,
                    HourTypeId = 2
                }, now);
                context.TimeLog.Add(log);
                added = true;
            }

            if (added)
                context.SaveChanges();
        }

        /// <summary>
        /// Datos de ejemplo para el panel principal (/index): tareas, bugs, demoras y ausencias.
        /// </summary>
        public static void EnsureDashboardDemoData(DataDbContext context)
        {
            if (!context.User.Any())
                return;

            var now = DateTime.Now;
            var today = now.Date;
            var utcNow = DateTime.UtcNow;
            var added = false;

            EnsureDashboardTimeOffDemoData(context, today, utcNow);
            added |= EnsureDashboardAdminPanelData(context, today, utcNow);
            added |= EnsureDashboardTeamOvertimeData(context, today, utcNow);

            if (added)
                context.SaveChanges();
        }

        /// <summary>
        /// Tareas, horas y bugs del usuario admin para poblar las cards y acordeones del panel.
        /// </summary>
        private static bool EnsureDashboardAdminPanelData(DataDbContext context, DateTime today, DateTime utcNow)
        {
            const long adminUserId = 1;
            var added = false;

            var demoTasks = new (long Id, long RequirementId, decimal Hours, string Description, long StatusId, int StartDaysAgo, long PriorityId, long PhaseId)[]
            {
                (901, 1, 8, "Coordinar entrega sprint Portal Clientes", 2, 5, 1, 2),
                (902, 2, 6, "Validar mockups dashboard de saldos", 2, 4, 2, 2),
                (903, 3, 5, "Seguimiento integración pasarela", 2, 10, 1, 3),
                (904, 1, 4, "Documentar avance semanal del equipo", 2, 2, 2, 2),
                (905, 2, 10, "Planificar capacitación del equipo", 1, 0, 3, 1),
                (906, 3, 3, "Aprobar release en ambiente QA", 2, 1, 1, 2),
            };

            foreach (var spec in demoTasks)
            {
                if (UpsertDashboardTask(context, spec.Id, spec.RequirementId, adminUserId, spec.Hours, spec.Description,
                        spec.PriorityId, spec.StatusId, today.AddDays(-spec.StartDaysAgo), spec.PhaseId, utcNow))
                    added = true;
            }

            var demoLogs = new (long Id, long TaskId, decimal Hours, int DaysAgo, decimal Progress, long HourTypeId)[]
            {
                (901, 901, 2, 4, 25, 1),
                (902, 901, 2, 0, 45, 1),
                (903, 902, 3, 3, 40, 1),
                (904, 902, 3, 1, 80, 2),
                (905, 903, 4, 6, 50, 2),
                (906, 903, 3, 3, 70, 2),
                (907, 903, 2, 0, 85, 2),
                (908, 904, 2, 0, 50, 1),
                (909, 906, 1.5m, 0, 35, 3),
                (910, 5, 2, 0, 40, 1),
            };

            foreach (var spec in demoLogs)
            {
                if (UpsertDashboardTimeLog(context, spec.Id, adminUserId, spec.TaskId, spec.Hours, today.AddDays(-spec.DaysAgo), spec.Progress, spec.HourTypeId, utcNow))
                    added = true;
            }

            var demoBugs = new (long Id, long RequirementId, long TaskId, string Description, long BugStatusId, int DaysAgo)[]
            {
                (901, 1, 901, "Checklist de cierre incompleto en entrega del sprint.", 1, 2),
                (902, 3, 903, "Horas de seguimiento superan la estimación inicial.", 1, 1),
                (903, 2, 902, "Texto truncado en validación de saldos móviles.", 2, 3),
                (904, 3, 906, "Release pendiente de firma funcional.", 1, 0),
            };

            foreach (var spec in demoBugs)
            {
                if (UpsertDashboardBug(context, spec.Id, spec.RequirementId, spec.TaskId, spec.Description, spec.BugStatusId, adminUserId, today.AddDays(-spec.DaysAgo), utcNow))
                    added = true;
            }

            return added;
        }

        /// <summary>
        /// Horas extra y desviaciones del resto del equipo (visible en acordeones globales).
        /// </summary>
        private static bool EnsureDashboardTeamOvertimeData(DataDbContext context, DateTime today, DateTime utcNow)
        {
            var added = false;
            var nextTimeLogId = context.TimeLog.Any() ? context.TimeLog.Max(t => t.Id) + 1 : 1;

            if (!context.Task.Any(t => t.UserId == 1 && t.Id == 5 && t.RowStatus == Active))
            {
                var nextTaskId = context.Task.Any() ? context.Task.Max(t => t.Id) + 1 : 5;
                context.Task.Add(SetAudit(
                    Task(nextTaskId, requirementId: 1, userId: 1, hours: 6, "Revision de avance semanal", priorityId: 2, statusId: 2, start: today.AddDays(-4), developmentPhaseId: 2),
                    utcNow));
                added = true;
            }

            var activeTasks = context.Task
                .Where(t => t.RowStatus == Active && t.Id < 900)
                .Select(t => new { t.Id, t.UserId })
                .ToList();

            var monthStart = new DateTime(today.Year, today.Month, 1);
            foreach (var task in activeTasks)
            {
                var hasLogThisMonth = context.TimeLog.Any(tl =>
                    tl.TaskId == task.Id
                    && tl.RowStatus == Active
                    && tl.ExecutionDate >= monthStart);

                if (hasLogThisMonth)
                    continue;

                context.TimeLog.Add(SetAudit(new TimeLog
                {
                    Id = nextTimeLogId++,
                    TaskId = task.Id,
                    UserId = task.UserId ?? 0,
                    UsedHours = 2,
                    ExecutionDate = today,
                    ProgressPercent = 40,
                    HourTypeId = 2
                }, utcNow));
                added = true;
            }

            var overtimeTask = context.Task.FirstOrDefault(t => t.Id == 2 && t.RowStatus == Active);
            if (overtimeTask != null)
            {
                var totalHours = context.TimeLog
                    .Where(tl => tl.TaskId == overtimeTask.Id && tl.RowStatus == Active)
                    .Sum(tl => tl.UsedHours);

                if (totalHours <= overtimeTask.TimeEstimationHours)
                {
                    context.TimeLog.Add(SetAudit(new TimeLog
                    {
                        Id = nextTimeLogId++,
                        TaskId = overtimeTask.Id,
                        UserId = overtimeTask.UserId ?? 0,
                        UsedHours = overtimeTask.TimeEstimationHours - totalHours + 2,
                        ExecutionDate = today,
                        ProgressPercent = 85,
                        HourTypeId = 2
                    }, utcNow));
                    added = true;
                }
            }

            var deviatedTask = context.Task.FirstOrDefault(t => t.Id == 4 && t.RowStatus == Active);
            if (deviatedTask != null)
            {
                var totalHours = context.TimeLog
                    .Where(tl => tl.TaskId == deviatedTask.Id && tl.RowStatus == Active)
                    .Sum(tl => tl.UsedHours);

                if (totalHours < deviatedTask.TimeEstimationHours + 4)
                {
                    context.TimeLog.Add(SetAudit(new TimeLog
                    {
                        Id = nextTimeLogId++,
                        TaskId = deviatedTask.Id,
                        UserId = deviatedTask.UserId ?? 0,
                        UsedHours = 10,
                        ExecutionDate = today.AddDays(-1),
                        ProgressPercent = 90,
                        HourTypeId = 2
                    }, utcNow));
                    added = true;
                }
            }

            return added;
        }

        private static bool UpsertDashboardTask(
            DataDbContext context,
            long id,
            long requirementId,
            long userId,
            decimal hours,
            string description,
            long priorityId,
            long statusId,
            DateTime start,
            long developmentPhaseId,
            DateTime utcNow)
        {
            var task = context.Task.FirstOrDefault(t => t.Id == id);
            if (task == null)
            {
                context.Task.Add(SetAudit(
                    Task(id, requirementId, userId, hours, description, priorityId, statusId, start, developmentPhaseId: developmentPhaseId),
                    utcNow));
                return true;
            }

            var changed = task.RequirementId != requirementId
                || task.UserId != userId
                || task.TimeEstimationHours != hours
                || task.Description != description
                || task.PriorityId != priorityId
                || task.TaskStatusId != statusId
                || task.StartDate.Date != start.Date
                || task.DevelopmentPhaseId != developmentPhaseId
                || task.RowStatus != Active;

            task.RequirementId = requirementId;
            task.UserId = userId;
            task.TimeEstimationHours = hours;
            task.Description = description;
            task.PriorityId = priorityId;
            task.TaskStatusId = statusId;
            task.StartDate = start;
            task.DevelopmentPhaseId = developmentPhaseId;
            task.RowStatus = Active;
            task.Updated = utcNow;
            task.UpdatedBy = 1;
            return changed;
        }

        private static bool UpsertDashboardTimeLog(
            DataDbContext context,
            long id,
            long userId,
            long taskId,
            decimal hours,
            DateTime executionDate,
            decimal progress,
            long hourTypeId,
            DateTime utcNow)
        {
            var log = context.TimeLog.FirstOrDefault(t => t.Id == id);
            if (log == null)
            {
                context.TimeLog.Add(SetAudit(new TimeLog
                {
                    Id = id,
                    UserId = userId,
                    TaskId = taskId,
                    UsedHours = hours,
                    ExecutionDate = executionDate,
                    ProgressPercent = progress,
                    HourTypeId = hourTypeId
                }, utcNow));
                return true;
            }

            var changed = log.UserId != userId
                || log.TaskId != taskId
                || log.UsedHours != hours
                || log.ExecutionDate.Date != executionDate.Date
                || log.ProgressPercent != progress
                || log.HourTypeId != hourTypeId
                || log.RowStatus != Active;

            log.UserId = userId;
            log.TaskId = taskId;
            log.UsedHours = hours;
            log.ExecutionDate = executionDate;
            log.ProgressPercent = progress;
            log.HourTypeId = hourTypeId;
            log.RowStatus = Active;
            log.Updated = utcNow;
            log.UpdatedBy = 1;
            return changed;
        }

        private static bool UpsertDashboardBug(
            DataDbContext context,
            long id,
            long requirementId,
            long taskId,
            string description,
            long bugStatusId,
            long reportedByUserId,
            DateTime created,
            DateTime utcNow)
        {
            var bug = context.TaskBug.FirstOrDefault(b => b.Id == id);
            if (bug == null)
            {
                var entity = SetAudit(new TaskBug
                {
                    Id = id,
                    RequirementId = requirementId,
                    TaskId = taskId,
                    Description = description,
                    TaskBugStatusId = bugStatusId,
                    ReportedByUserId = reportedByUserId,
                    StartDate = created,
                    EndDate = created
                }, utcNow);
                entity.Created = created;
                entity.CreatedBy = reportedByUserId;
                context.TaskBug.Add(entity);
                return true;
            }

            var changed = bug.RequirementId != requirementId
                || bug.TaskId != taskId
                || bug.Description != description
                || bug.TaskBugStatusId != bugStatusId
                || bug.RowStatus != Active;

            if (bug.StartDate == default)
                bug.StartDate = created;
            if (bug.EndDate == default || bug.TaskBugStatusId != bugStatusId)
                bug.EndDate = utcNow;

            bug.RequirementId = requirementId;
            bug.TaskId = taskId;
            bug.Description = description;
            bug.TaskBugStatusId = bugStatusId;
            bug.ReportedByUserId = reportedByUserId;
            bug.RowStatus = Active;
            bug.Updated = utcNow;
            bug.UpdatedBy = 1;
            return changed;
        }

        /// <summary>
        /// Ausentismos de ejemplo visibles en el banner del panel principal.
        /// </summary>
        public static void EnsureDashboardTimeOffDemoData(DataDbContext context, DateTime? referenceDate = null, DateTime? auditNow = null)
        {
            var today = (referenceDate ?? DateTime.Now).Date;
            var now = auditNow ?? DateTime.UtcNow;

            UpsertTimeOff(context, 1, userId: 2, type: 1, start: today.AddDays(4), end: today.AddDays(8), hours: 40, description: "Vacaciones programadas", now);
            UpsertTimeOff(context, 2, userId: 3, type: 2, start: today, end: today, hours: 4, description: "Permiso personal", now);
            UpsertTimeOff(context, 3, userId: 2, type: 2, start: today.AddDays(-1), end: today.AddDays(2), hours: 24, description: "Permiso médico", now);
            UpsertTimeOff(context, 4, userId: 3, type: 1, start: today.AddDays(3), end: today.AddDays(6), hours: 32, description: "Vacaciones de mitad de año", now);

            context.SaveChanges();
        }

        private static void UpsertTimeOff(
            DataDbContext context,
            long id,
            long userId,
            short type,
            DateTime start,
            DateTime end,
            decimal hours,
            string description,
            DateTime now)
        {
            var item = context.UserTimeOff.FirstOrDefault(t => t.Id == id);
            if (item == null)
            {
                context.UserTimeOff.Add(SetAudit(new UserTimeOff
                {
                    Id = id,
                    UserId = userId,
                    Type = type,
                    StartDate = start,
                    EndDate = end,
                    Hours = hours,
                    Description = description
                }, now));
                return;
            }

            item.UserId = userId;
            item.Type = type;
            item.StartDate = start;
            item.EndDate = end;
            item.Hours = hours;
            item.Description = description;
            item.RowStatus = Active;
            item.Updated = now;
            item.UpdatedBy = 1;
        }

        /// <summary>
        /// Actualiza etiquetas de menú en bases ya pobladas.
        /// </summary>
        public static void EnsureMenuLabels(DataDbContext context)
        {
            var now = DateTime.UtcNow;
            var updates = new (long Id, string Description)[]
            {
                (11, "Miembros del equipo"),
                (112, "Funcionalidad"),
                (114, "Permisos"),
            };

            foreach (var (id, description) in updates)
            {
                var menu = context.Menu.FirstOrDefault(m => m.Id == id);
                if (menu == null || menu.Description == description) continue;

                menu.Description = description;
                menu.Updated = now;
                menu.UpdatedBy = 1;
            }

            context.SaveChanges();
        }

        /// <summary>
        /// Asegura columna CanDelete en RoleMenu (bases PostgreSQL ya creadas).
        /// </summary>
        public static void EnsureRoleMenuCanDelete(DataDbContext context)
        {
            if (context.Database.IsInMemory()) return;

            try
            {
                context.Database.ExecuteSqlRaw(
                    "ALTER TABLE \"Security\".\"RoleMenu\" ADD COLUMN IF NOT EXISTS \"CanDelete\" boolean NOT NULL DEFAULT false;");
                context.Database.ExecuteSqlRaw(
                    "UPDATE \"Security\".\"RoleMenu\" SET \"CanDelete\" = \"CanEdit\" WHERE \"CanDelete\" = false AND \"CanEdit\" = true;");
            }
            catch
            {
                // Si el esquema no existe aún, EnsureCreated lo creará con la columna.
            }
        }

        /// <summary>
        /// Asegura columna CanFinalize en RoleMenu (bases PostgreSQL ya creadas).
        /// </summary>
        public static void EnsureRoleMenuCanFinalize(DataDbContext context)
        {
            if (!context.Database.IsInMemory())
            {
                try
                {
                    context.Database.ExecuteSqlRaw(
                        "ALTER TABLE \"Security\".\"RoleMenu\" ADD COLUMN IF NOT EXISTS \"CanFinalize\" boolean NOT NULL DEFAULT false;");
                }
                catch
                {
                    // Si el esquema no existe aún, EnsureCreated lo creará con la columna.
                }
            }

            const long adminRoleId = 1;
            var finalizeMenuIds = new[] { 1L, 2L, 3L };
            var now = DateTime.UtcNow;

            foreach (var menuId in finalizeMenuIds)
            {
                var adminRoleMenu = context.RoleMenu.FirstOrDefault(rm => rm.RoleId == adminRoleId && rm.MenuId == menuId);
                if (adminRoleMenu == null)
                    continue;

                if (!adminRoleMenu.CanFinalize)
                {
                    adminRoleMenu.CanFinalize = true;
                    adminRoleMenu.Updated = now;
                    adminRoleMenu.UpdatedBy = 1;
                }
            }

            context.SaveChanges();
        }

        /// <summary>
        /// Asegura columna CanRegisterHours en RoleMenu (bases PostgreSQL ya creadas).
        /// </summary>
        public static void EnsureRoleMenuCanRegisterHours(DataDbContext context)
        {
            const long taskManagementMenuId = 3;

            if (!context.Database.IsInMemory())
            {
                try
                {
                    context.Database.ExecuteSqlRaw(
                        "ALTER TABLE \"Security\".\"RoleMenu\" ADD COLUMN IF NOT EXISTS \"CanRegisterHours\" boolean NOT NULL DEFAULT false;");
                    context.Database.ExecuteSqlRaw(
                        $"UPDATE \"Security\".\"RoleMenu\" SET \"CanRegisterHours\" = \"CanEdit\" WHERE \"MenuId\" = {taskManagementMenuId} AND \"CanRegisterHours\" = false AND \"CanEdit\" = true;");
                }
                catch
                {
                    // Si el esquema no existe aún, EnsureCreated lo creará con la columna.
                }
            }

            var now = DateTime.UtcNow;
            foreach (var roleMenu in context.RoleMenu.Where(rm => rm.MenuId == taskManagementMenuId && rm.RowStatus == Active))
            {
                if (roleMenu.CanRegisterHours)
                    continue;

                if (roleMenu.CanEdit)
                {
                    roleMenu.CanRegisterHours = true;
                    roleMenu.Updated = now;
                    roleMenu.UpdatedBy = 1;
                }
            }

            context.SaveChanges();
        }

        /// <summary>
        /// Solo administradores pueden editar tareas; desarrolladores conservan registro de horas.
        /// </summary>
        public static void EnsureDeveloperTaskEditPermission(DataDbContext context)
        {
            const long taskManagementMenuId = 3;
            var developerRoleId = context.Role
                .FirstOrDefault(r => r.Description == "Desarrollador" && r.RowStatus == Active)
                ?.Id;
            if (developerRoleId == null)
                return;

            var roleMenu = context.RoleMenu.FirstOrDefault(r =>
                r.RoleId == developerRoleId &&
                r.MenuId == taskManagementMenuId &&
                r.RowStatus == Active);
            if (roleMenu == null)
                return;

            if (roleMenu.CanEdit)
            {
                roleMenu.CanEdit = false;
                roleMenu.CanRegisterHours = true;
                roleMenu.Updated = DateTime.UtcNow;
                roleMenu.UpdatedBy = 1;
            }

            context.SaveChanges();
        }

        /// <summary>
        /// Asegura columnas StartDate y EndDate en TaskBug.
        /// </summary>
        public static void EnsureTaskBugDates(DataDbContext context)
        {
            if (!context.Database.IsInMemory())
            {
                try
                {
                    context.Database.ExecuteSqlRaw(
                        "ALTER TABLE \"TaskManagement\".\"TaskBug\" ADD COLUMN IF NOT EXISTS \"StartDate\" timestamp without time zone NULL;");
                    context.Database.ExecuteSqlRaw(
                        "ALTER TABLE \"TaskManagement\".\"TaskBug\" ADD COLUMN IF NOT EXISTS \"EndDate\" timestamp without time zone NULL;");
                    context.Database.ExecuteSqlRaw(
                        "UPDATE \"TaskManagement\".\"TaskBug\" SET \"StartDate\" = \"Created\" WHERE \"StartDate\" IS NULL;");
                    context.Database.ExecuteSqlRaw(
                        "UPDATE \"TaskManagement\".\"TaskBug\" SET \"EndDate\" = COALESCE(\"Updated\", \"Created\") WHERE \"EndDate\" IS NULL;");
                }
                catch
                {
                }
            }

            var now = DateTime.UtcNow;
            foreach (var bug in context.TaskBug.Where(b => b.StartDate == default || b.EndDate == default))
            {
                if (bug.StartDate == default)
                    bug.StartDate = bug.Created == default ? now : bug.Created;
                if (bug.EndDate == default)
                    bug.EndDate = bug.Updated ?? bug.StartDate;
            }

            context.SaveChanges();
        }

        /// <summary>
        /// Asegura RequirementId en TaskBug y TaskId nullable (bases PostgreSQL ya creadas).
        /// </summary>
        public static void EnsureTaskBugRequirement(DataDbContext context)
        {
            if (!context.Database.IsInMemory())
            {
                try
                {
                    context.Database.ExecuteSqlRaw(
                        "ALTER TABLE \"TaskManagement\".\"TaskBug\" ADD COLUMN IF NOT EXISTS \"RequirementId\" bigint NULL;");
                    context.Database.ExecuteSqlRaw(
                        "ALTER TABLE \"TaskManagement\".\"TaskBug\" ALTER COLUMN \"TaskId\" DROP NOT NULL;");
                    context.Database.ExecuteSqlRaw(
                        "UPDATE \"TaskManagement\".\"TaskBug\" b SET \"RequirementId\" = t.\"RequirementId\" " +
                        "FROM \"TaskManagement\".\"Task\" t WHERE b.\"TaskId\" = t.\"Id\" " +
                        "AND (b.\"RequirementId\" IS NULL OR b.\"RequirementId\" = 0);");
                }
                catch
                {
                    // Si el esquema no existe aún, EnsureCreated lo creará con las columnas.
                }
            }

            var bugsMissingRequirement = context.TaskBug
                .Where(b => b.RowStatus == Active && b.RequirementId <= 0 && b.TaskId != null)
                .ToList();

            foreach (var bug in bugsMissingRequirement)
            {
                var requirementId = context.Task
                    .Where(t => t.Id == bug.TaskId)
                    .Select(t => t.RequirementId)
                    .FirstOrDefault();
                if (requirementId > 0)
                    bug.RequirementId = requirementId;
            }

            if (bugsMissingRequirement.Count > 0)
                context.SaveChanges();
        }

        /// <summary>
        /// Asegura columna QaEnteredAt en Task y retroactiva tareas ya en fase QA o posterior.
        /// </summary>
        public static void EnsureTaskQaEnteredAt(DataDbContext context)
        {
            if (!context.Database.IsInMemory())
            {
                try
                {
                    context.Database.ExecuteSqlRaw(
                        "ALTER TABLE \"TaskManagement\".\"Task\" ADD COLUMN IF NOT EXISTS \"QaEnteredAt\" timestamp without time zone NULL;");
                }
                catch
                {
                    // Si el esquema no existe aún, EnsureCreated lo creará con la columna.
                }
            }

            var qaPhaseIds = context.TaskDevelopmentPhase
                .Where(p => p.RowStatus == Active && p.Order >= 2)
                .Select(p => p.Id)
                .ToHashSet();

            if (qaPhaseIds.Count == 0)
                return;

            var tasksInQa = context.Task
                .Where(t => t.RowStatus == Active && qaPhaseIds.Contains(t.DevelopmentPhaseId) && t.QaEnteredAt == null)
                .ToList();

            if (tasksInQa.Count == 0)
                return;

            var taskIds = tasksInQa.Select(t => t.Id).ToList();
            var firstLogByTask = context.TimeLog
                .Where(tl => taskIds.Contains(tl.TaskId) && tl.RowStatus == Active)
                .AsEnumerable()
                .GroupBy(tl => tl.TaskId)
                .ToDictionary(g => g.Key, g => g.Min(x => x.ExecutionDate));

            foreach (var task in tasksInQa)
            {
                task.QaEnteredAt = firstLogByTask.GetValueOrDefault(task.Id, task.Updated ?? task.Created);
            }

            context.SaveChanges();
        }

        /// <summary>
        /// Restaura permisos completos del rol Administrador en menús operativos.
        /// </summary>
        public static void EnsureAdminOperationalPermissions(DataDbContext context)
        {
            const long adminRoleId = 1;
            var now = DateTime.UtcNow;

            var operationalMenuIds = context.Menu
                .Where(m => m.RowStatus == Active && !string.IsNullOrWhiteSpace(m.Page))
                .Select(m => m.Id)
                .ToList();

            foreach (var menuId in operationalMenuIds)
            {
                var roleMenu = context.RoleMenu.FirstOrDefault(r => r.RoleId == adminRoleId && r.MenuId == menuId);
                if (roleMenu == null)
                {
                    context.RoleMenu.Add(SetAudit(new RoleMenu
                    {
                        Id = GetNextRoleMenuId(context),
                        RoleId = adminRoleId,
                        MenuId = menuId,
                        CanView = true,
                        CanCreate = true,
                        CanEdit = true,
                        CanDelete = true,
                        CanRegisterHours = true,
                        CanFinalize = true
                    }, now));
                    continue;
                }

                roleMenu.RowStatus = Active;
                roleMenu.CanView = true;
                roleMenu.CanCreate = true;
                roleMenu.CanEdit = true;
                roleMenu.CanDelete = true;
                roleMenu.CanRegisterHours = true;
                roleMenu.CanFinalize = true;
                roleMenu.Updated = now;
                roleMenu.UpdatedBy = 1;
            }

            context.SaveChanges();
        }

        /// <summary>
        /// Alinea secuencias de identidad con MAX(Id) tras inserts con Id explícito (seed/Ensure).
        /// </summary>
        public static void SyncPostgreSqlIdentitySequences(DataDbContext context)
        {
            if (!context.Database.IsRelational())
                return;

            context.Database.ExecuteSqlRaw(@"
                DO $sync$
                DECLARE
                    r RECORD;
                    seq_name text;
                BEGIN
                    FOR r IN
                        SELECT table_schema, table_name, column_name
                        FROM information_schema.columns
                        WHERE is_identity = 'YES'
                          AND table_schema NOT IN ('pg_catalog', 'information_schema')
                    LOOP
                        seq_name := pg_get_serial_sequence(
                            format('%I.%I', r.table_schema, r.table_name),
                            r.column_name
                        );
                        IF seq_name IS NOT NULL THEN
                            EXECUTE format(
                                'SELECT setval(%L,
                                    COALESCE((SELECT MAX(%I) FROM %I.%I), 1),
                                    (SELECT MAX(%I) IS NOT NULL FROM %I.%I)
                                )',
                                seq_name,
                                r.column_name, r.table_schema, r.table_name,
                                r.column_name, r.table_schema, r.table_name
                            );
                        END IF;
                    END LOOP;
                END
                $sync$;");
        }

        private static void EnsureMenu(DataDbContext context, long id, string description, int icon, string page, long? parent, int order, DateTime now)
        {
            var menu = context.Menu.FirstOrDefault(m => m.Id == id);
            if (menu == null)
            {
                context.Menu.Add(SetAudit(Menu(id, description, icon, page, parent, order), now));
                return;
            }

            menu.Description = description;
            menu.Icon = icon;
            menu.Page = page;
            menu.Parent = parent;
            menu.Order = order;
            menu.RowStatus = Active;
            menu.Updated = now;
            menu.UpdatedBy = 1;
        }

        private static long GetNextRoleMenuId(DataDbContext context)
        {
            var dbMax = context.RoleMenu.AsNoTracking().Select(r => (long?)r.Id).Max() ?? 0;
            var pendingMax = context.ChangeTracker.Entries<RoleMenu>()
                .Where(e => e.State == EntityState.Added)
                .Select(e => e.Entity.Id)
                .DefaultIfEmpty(0)
                .Max();
            return Math.Max(dbMax, pendingMax) + 1;
        }

        private static void EnsureRoleMenu(DataDbContext context, long roleId, long menuId, DateTime now)
        {
            var roleMenu = context.RoleMenu.FirstOrDefault(r => r.RoleId == roleId && r.MenuId == menuId);
            if (roleMenu == null)
            {
                context.RoleMenu.Add(SetAudit(new RoleMenu
                {
                    Id = GetNextRoleMenuId(context),
                    RoleId = roleId,
                    MenuId = menuId,
                    CanView = true,
                    CanCreate = true,
                    CanEdit = true,
                    CanDelete = true,
                    CanRegisterHours = true,
                    CanFinalize = true
                }, now));
                return;
            }

            roleMenu.RowStatus = Active;
            roleMenu.CanView = true;
            roleMenu.CanCreate = true;
            roleMenu.CanEdit = true;
            roleMenu.CanDelete = true;
            roleMenu.CanRegisterHours = true;
            roleMenu.CanFinalize = true;
            roleMenu.Updated = now;
            roleMenu.UpdatedBy = 1;
        }

        private static Menu Menu(long id, string description, int icon, string page, long? parent, int order)
            => new() { Id = id, Description = description, Icon = icon, Page = page, Parent = parent, Order = order };

        private static User User(long id, string name, string lastName, string email, string password, string jobTitle = "")
            => new() { Id = id, Name = name, LastName = lastName, Email = email, Password = password, JobTitle = jobTitle };

        private static Enterprise Enterprise(long id, string description)
            => new() { Id = id, Description = description };

        private static Customer Customer(long id, string description, long enterpriseId)
            => new() { Id = id, Description = description, EnterpriseId = enterpriseId };

        private static ProjectStatus ProjectStatus(long id, string description, int order, string badgeColor, bool isClosed = false)
            => new() { Id = id, Description = description, Order = order, BadgeColor = badgeColor, IsClosed = isClosed };

        private static RequirementStatus RequirementStatus(long id, string description, int order, string badgeColor = "gray", bool isClosed = false)
            => new() { Id = id, Description = description, Order = order, BadgeColor = badgeColor, IsClosed = isClosed };

        private static Priority Priority(long id, string description, string badgeColor)
            => new() { Id = id, Description = description, BadgeColor = badgeColor };

        private static Entities.TaskManagement.TaskStatus TaskStatus(long id, string description, int order, string badgeColor = "gray")
            => new() { Id = id, Description = description, Order = order, BadgeColor = badgeColor };

        private static TaskBugStatus TaskBugStatus(long id, string description, int order, string badgeColor = "gray")
            => new() { Id = id, Description = description, Order = order, BadgeColor = badgeColor };

        private static BacklogStatus BacklogStatus(long id, string description, int order, string badgeColor = "gray", bool isClosed = false)
            => new() { Id = id, Description = description, Order = order, BadgeColor = badgeColor, IsClosed = isClosed };

        private static TaskDevelopmentPhase DevelopmentPhase(long id, string description, int order, string badgeColor = "gray")
            => new() { Id = id, Description = description, Order = order, BadgeColor = badgeColor };

        private static HourType HourType(long id, string description, string scope, int order, string badgeColor = "gray")
            => new() { Id = id, Description = description, Scope = scope, Order = order, BadgeColor = badgeColor };

        private static Project Project(long id, string description, long customerId, long projectStatusId, DateTime start, DateTime? end = null)
            => new()
            {
                Id = id,
                Description = description,
                CustomerId = customerId,
                ProjectStatusId = projectStatusId,
                StartDate = start,
                EndDate = end
            };

        private static Requirement Requirement(long id, string description, long projectId, long statusId, long priorityId, DateTime start)
            => new()
            {
                Id = id,
                Description = description,
                ProjectId = projectId,
                Scope = "Alcance de prueba para " + description,
                RequirementStatusId = statusId,
                PriorityId = priorityId,
                StartDate = start,
                RequesterName = "Usuario de prueba",
                RequestDate = start.AddDays(-7),
                ImpactedSystems = "Sistema principal"
            };

        private static Entities.TaskManagement.Task Task(long id, long requirementId, long userId, decimal hours, string description, long priorityId, long statusId, DateTime start, bool withinScope = true, short? scopeReason = null, long developmentPhaseId = 1)
            => new()
            {
                Id = id,
                RequirementId = requirementId,
                UserId = userId,
                TimeEstimationHours = hours,
                Description = description,
                PriorityId = priorityId,
                TaskStatusId = statusId,
                DevelopmentPhaseId = developmentPhaseId,
                StartDate = start,
                IsWithinOriginalScope = withinScope,
                ScopeChangeReason = withinScope ? null : scopeReason
            };

        private static TimeLog TimeLog(long id, long userId, long taskId, decimal hours, DateTime date, decimal progress = 0, long hourTypeId = 2)
            => new() { Id = id, UserId = userId, TaskId = taskId, UsedHours = hours, ExecutionDate = date, ProgressPercent = progress, HourTypeId = hourTypeId };

        private static T SetAudit<T>(T entity, DateTime now) where T : Entities.AuditBaseEntity
        {
            entity.RowStatus = Active;
            entity.CreatedBy = 1;
            entity.Created = now;
            return entity;
        }
    }
}
