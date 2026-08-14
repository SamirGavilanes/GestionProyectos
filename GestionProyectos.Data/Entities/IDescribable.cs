namespace GestionProyectos.Data.Entities
{
    /// <summary>
    /// Catálogos simples que se administran solo por su descripción.
    /// </summary>
    public interface IDescribable
    {
        string Description { get; set; }
    }
}
