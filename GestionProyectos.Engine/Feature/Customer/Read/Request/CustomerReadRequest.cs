namespace GestionProyectos.Engine.Feature.Customer.Read.Request
{
    public class CustomerReadRequest
    {
        public long EnterpriseId { get; set; }
        public CustomerReadRequest()
        {
            EnterpriseId = 0;
        }
    }
}
