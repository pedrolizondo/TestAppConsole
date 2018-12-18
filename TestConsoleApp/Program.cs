using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.ServiceModel.Description;
using System.Text;
using System.Threading.Tasks;

namespace TestConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            string CRMPath = ConfigurationManager.AppSettings.Get("CRMPath");
            string UserName = ConfigurationManager.AppSettings.Get("UserName");
            string Password = ConfigurationManager.AppSettings.Get("Password");
            double Timeout = Convert.ToDouble(ConfigurationManager.AppSettings.Get("CRMTimeout"));

            Uri oUri = new Uri(CRMPath + "/XRMServices/2011/Organization.svc");

            ClientCredentials clientCredentials = new ClientCredentials();
            clientCredentials.UserName.UserName = UserName;
            clientCredentials.UserName.Password = Password;

            var crmInstance = new OrganizationServiceProxy(oUri, null, clientCredentials, null);
            crmInstance.Timeout = TimeSpan.FromSeconds(Timeout);
            crmInstance.EnableProxyTypes();

            // Instantiate QueryExpression QEjnbs_location
            var QEjnbs_location = new QueryExpression("jnbs_location");
            QEjnbs_location.TopCount = 50;

            // Add columns to QEjnbs_location.ColumnSet
            QEjnbs_location.ColumnSet.AddColumns("jnbs_name", "jnbs_code", "createdon");

            var response = crmInstance.RetrieveMultiple(QEjnbs_location);
        }
    }
}
