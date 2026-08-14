namespace GestionProyectos.Engine.Security.Utilities
{
    public class Context
    {
        public string Email { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public long UserId { get; set; }
        public List<MenuItem> Menus { get; set; } = new();
    }
    public class MenuItem
    {
        public long Id { get; set; }
        public string Description { get; set; } = string.Empty;
        public int Icon { get; set; }
        public string Page { get; set; } = string.Empty;
        public long? Parent { get; set; }
        public int Oder { get; set; }
        public bool CanView { get; set; } = true;
        public bool CanCreate { get; set; } = true;
        public bool CanEdit { get; set; } = true;
        public bool CanDelete { get; set; } = true;
        public bool CanRegisterHours { get; set; } = true;
        public bool CanFinalize { get; set; }
    }
}
