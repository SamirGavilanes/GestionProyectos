namespace GestionProyectos.Engine.Feature.Customer.Read.CommonObject
{
    public class Customer
    {
        public long Id { get; set; }
        public string Description { get; set; } = string.Empty;
        public long EnterpriseId { get; set; }
        public string Enterprise { get; set; } = string.Empty;
    }
}
