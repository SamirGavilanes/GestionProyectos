namespace GestionProyectos.Engine.Feature.Requirement.Read.Response
{
    public class RequirementReadResponse
    {
        public List<CommonObject.Requirement> Tickets { get; set; }

        public RequirementReadResponse()
        {
            Tickets = new();
        }
    }
}
