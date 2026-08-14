namespace GestionProyectos.Engine.Feature.Project.Read.Response
{
    public class ProjectReadResponse
    {
        public List<CommonObject.Project> Projects { get; set; }

        public ProjectReadResponse()
        {
            Projects = new();
        }
    }
}
