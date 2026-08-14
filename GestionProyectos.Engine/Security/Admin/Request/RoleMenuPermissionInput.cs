namespace GestionProyectos.Engine.Security.Admin.Request
{
    public class RoleMenuPermissionInput
    {
        public long MenuId { get; set; }
        public bool CanView { get; set; }
        public bool CanCreate { get; set; }
        public bool CanEdit { get; set; }
        public bool CanDelete { get; set; }
        public bool CanRegisterHours { get; set; }
        public bool CanFinalize { get; set; }
    }
}
