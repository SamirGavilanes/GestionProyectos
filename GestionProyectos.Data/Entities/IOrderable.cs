namespace GestionProyectos.Data.Entities;

/// <summary>
/// Catálogos con orden explícito (p. ej. columnas Kanban).
/// </summary>
public interface IOrderable
{
    int Order { get; set; }
}
