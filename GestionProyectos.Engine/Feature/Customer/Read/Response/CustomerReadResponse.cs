namespace GestionProyectos.Engine.Feature.Customer.Read.Response
{
    public class CustomerReadResponse
    {
        public List<CommonObject.Customer> Customers { get; set; }

        public CustomerReadResponse()
        {
            Customers = new();
        }
    }
}
