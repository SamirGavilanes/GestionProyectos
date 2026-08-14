using GestionProyectos.Engine.Feature.Customer.Read.Request;
using GestionProyectos.Engine.Feature.Customer.Read.Response;
using GestionProyectos.Shared.Message;

namespace GestionProyectos.Engine.Feature.Customer.Read
{
    public interface ICustomerReadEngine
    {
        OperationResult<CustomerReadResponse> GetCustomers();
        OperationResult<CustomerReadResponse> GetCustomersByEnterprise(CustomerReadRequest request);
    }
}
