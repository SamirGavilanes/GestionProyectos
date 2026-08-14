using Amazon;
using Amazon.EC2;
using Amazon.EC2.Model;
using Amazon.Runtime;

namespace GestionProyectos.Shared.AWSManager
{
    public class ScanIpAddressHelper
    {
        private readonly string secretKey;
        private readonly string accessKey;
        private readonly string regionAws;

        public ScanIpAddressHelper(string _accessKey, string _secretKey, string _regionAws)
        {
            accessKey = _accessKey;
            secretKey = _secretKey;
            regionAws = _regionAws;
        }

        #region DEVUELVE LAS DIRECCIONES IP DE INSTANCIAS FILTRADAS
        public Dictionary<string, List<string>> DnsOrIpEc2AwsInstance(List<string> instanceName)
        {

            try
            {
                Dictionary<string, List<string>> response = new();
                IAmazonEC2 amazonEC2Client;
                amazonEC2Client = new AmazonEC2Client(RegionEndpoint.GetBySystemName(regionAws));

                Console.WriteLine($"Region:{RegionEndpoint.GetBySystemName(regionAws).DisplayName}");
#if DEBUG
                AWSCredentials credentials = new BasicAWSCredentials(accessKey, secretKey);
                amazonEC2Client = new AmazonEC2Client(credentials, RegionEndpoint.GetBySystemName(regionAws));
#endif
                // ELIMINA LAS INSTANCIAS REPETIDAS
                List<string> filterInstanceName = new();
                filterInstanceName.AddRange(instanceName.Distinct().ToList());

                filterInstanceName.ForEach(instanceName =>
                {
                    DescribeInstancesRequest describeInstancesRequest = new()
                    {
                        Filters = new List<Filter>()
                        {
                            new()
                            {
                                Name = $"tag:Name",
                                Values = new List<string>()
                                {
                                    instanceName
                                }
                            }
                        }
                    };

                    var instanceResponse = amazonEC2Client.DescribeInstancesAsync(describeInstancesRequest).Result;
                    var privateIPsAddress = GetPrivateIpAddressInstance(instanceName, instanceResponse);
                    response.Add(instanceName, privateIPsAddress.ToList());

                });

                return response;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
        #endregion

        #region OBTIENE LAS DIRECCIONES IP PRIVADAS PARA LAS INSTANCIAS MENCIONADAS
        internal List<string> GetPrivateIpAddressInstance(string instanceName, DescribeInstancesResponse descriptionResponse)
        {
            var privateIPsAddress = new List<string>();
            descriptionResponse.Reservations.ForEach(reservation =>
            {
                var instancePrivateIpAddress = reservation.Instances?.FirstOrDefault()?.PrivateIpAddress;
                if (instancePrivateIpAddress != null)
                    privateIPsAddress.Add(instancePrivateIpAddress);
            });
            return privateIPsAddress;
        }
        #endregion
    }
}
