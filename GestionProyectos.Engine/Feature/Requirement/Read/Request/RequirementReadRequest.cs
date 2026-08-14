namespace GestionProyectos.Engine.Feature.Requirement.Read.Request
{
    public class RequirementReadRequest
    {
        public long EnterpriseId { get; set; }
        public long ProjectId { get; set; }
        public long CustomerId { get; set; }
        public long Id { get; set; }
        public long RequirementStatusId { get; set; }
    }
}
