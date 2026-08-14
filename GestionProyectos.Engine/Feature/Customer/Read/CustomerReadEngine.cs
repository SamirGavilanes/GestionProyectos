using GestionProyectos.Data;
using GestionProyectos.Engine.Feature.Customer.Read.Request;
using GestionProyectos.Engine.Feature.Customer.Read.Response;
using GestionProyectos.Engine.Feature.Project.Read.Request;
using GestionProyectos.Engine.Feature.Project.Read.Response;
using GestionProyectos.Shared.Enums;
using GestionProyectos.Shared.Message;

namespace GestionProyectos.Engine.Feature.Customer.Read
{
    public class CustomerReadEngine : ICustomerReadEngine
    {
        private readonly DataDbContext dbContext;

        public CustomerReadEngine(DataDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public OperationResult<CustomerReadResponse> GetCustomers()
        {
            try
            {
                CustomerReadResponse response = new();

                var customers = dbContext.Customer.Where(c => c.RowStatus == (short)RowStatus.Active)
                                                  .OrderBy(c => c.Description)
                                                  .ToList();

                // SE MAPEA LA RESPUESTA
                List<CommonObject.Customer> customerList = new();
                foreach (var customer in customers)
                {

                    CommonObject.Customer customerItem = new()
                    {
                        Id = customer.Id,
                        Description = customer.Description,
                        EnterpriseId = customer.EnterpriseId,
                        Enterprise = customer.Enterprise.Description
                    };

                    customerList.Add(customerItem);
                }

                // RESPUESTA
                response.Customers = customerList;
                return OperationResult<CustomerReadResponse>.CreateSuccessResult(response);
            }
            catch (Exception ex)
            {
                return OperationResult<CustomerReadResponse>.CreateFailureResult(ex);
            }
        }

        public OperationResult<CustomerReadResponse> GetCustomersByEnterprise(CustomerReadRequest request)
        {
            try
            {
                CustomerReadResponse response = new();

                var customers = dbContext.Customer.Where(c => c.RowStatus == (short)RowStatus.Active 
                                                           && c.EnterpriseId == request.EnterpriseId)
                                                .OrderBy(p => p.Description)
                                                .ToList();

                // SE MAPEA LA RESPUESTA
                List<CommonObject.Customer> customerList = new();
                foreach (var customer in customers)
                {
                    CommonObject.Customer customerItem = new()
                    {
                        Id = customer.Id,
                        Description = customer.Description,
                        EnterpriseId = customer.EnterpriseId,
                        Enterprise = customer.Enterprise.Description
                    };

                    customerList.Add(customerItem);
                }

                // RESPUESTA
                response.Customers = customerList;
                return OperationResult<CustomerReadResponse>.CreateSuccessResult(response);
            }
            catch (Exception ex)
            {
                return OperationResult<CustomerReadResponse>.CreateFailureResult(ex);
            }
        }
    }
}
