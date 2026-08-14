namespace GestionProyectos.Engine.Security.Utilities

{

    public readonly struct PagePermissions

    {

        public static PagePermissions None => new(false, false, false, false, false, false);

        public static PagePermissions Full => new(true, true, true, true, true, true);



        public bool CanView { get; }

        public bool CanCreate { get; }

        public bool CanEdit { get; }

        public bool CanDelete { get; }

        public bool CanRegisterHours { get; }

        public bool CanFinalize { get; }



        public PagePermissions(bool canView, bool canCreate, bool canEdit, bool canDelete, bool canRegisterHours = false, bool canFinalize = false)

        {

            CanView = canView;

            CanCreate = canCreate;

            CanEdit = canEdit;

            CanDelete = canDelete;

            CanRegisterHours = canRegisterHours;

            CanFinalize = canFinalize;

        }

    }



    public static class PermissionHelper

    {

        public const string TaskManagementRoute = "task-management";

        public const string RequirementListRoute = "requirement-list";

        public const string ProjectListRoute = "project-list";



        public static PagePermissions Resolve(Context? context, string route)

        {

            var menu = FindMenu(context, route);

            if (menu == null)

                return PagePermissions.None;



            var canDelete = menu.CanDelete;

            if (!canDelete && menu.CanEdit)

                canDelete = true;



            return new PagePermissions(menu.CanView, menu.CanCreate, menu.CanEdit, canDelete, menu.CanView, menu.CanFinalize);

        }



        public static MenuItem? FindMenu(Context? context, string route)

        {

            if (context?.Menus == null) return null;



            var normalized = NormalizeRoute(route);

            if (normalized.StartsWith("create-requirement"))

                normalized = RequirementListRoute;

            if (normalized.StartsWith("project-detail"))

                normalized = ProjectListRoute;

            if (normalized.StartsWith("create-project"))

                normalized = ProjectListRoute;

            if (normalized.StartsWith("task-detail"))

                normalized = TaskManagementRoute;



            if (normalized.StartsWith("create-task"))

                normalized = TaskManagementRoute;



            if (normalized.StartsWith("task-management"))

                normalized = TaskManagementRoute;



            return context.Menus.FirstOrDefault(m =>

                !string.IsNullOrWhiteSpace(m.Page) &&

                NormalizeRoute(m.Page) == normalized);

        }



        public static bool CanView(Context? context, string route) => Resolve(context, route).CanView;

        public static bool CanCreate(Context? context, string route) => Resolve(context, route).CanCreate;

        public static bool CanEdit(Context? context, string route) => Resolve(context, route).CanEdit;

        public static bool CanDelete(Context? context, string route) => Resolve(context, route).CanDelete;

        public static bool CanRegisterHours(Context? context, string route) => CanView(context, route);

        public static bool CanFinalizeTask(Context? context) => Resolve(context, TaskManagementRoute).CanFinalize;

        public static bool CanFinalizeRequirement(Context? context) => Resolve(context, RequirementListRoute).CanFinalize;

        public static bool CanFinalizeProject(Context? context) => Resolve(context, ProjectListRoute).CanFinalize;



        private static string NormalizeRoute(string route) =>

            route.Trim().TrimStart('/').ToLowerInvariant().Split('?')[0];

    }

}

