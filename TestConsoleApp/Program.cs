using System;
using System.Configuration;
using System.Linq;
using System.ServiceModel.Description;

using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Sdk.Query;

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

            // Add Options Set for Lead Test.
            var attributeRequest = new RetrieveAttributeRequest
            {
                EntityLogicalName = "lead",
                LogicalName = "jnbs_status",
                RetrieveAsIfPublished = true
            };

            var attributeResponse = (RetrieveAttributeResponse)crmInstance.Execute(attributeRequest);
            var attributeMetadata = (EnumAttributeMetadata)attributeResponse.AttributeMetadata;

            var optionList = (from o in attributeMetadata.OptionSet.Options
                              select new { Value = o.Value, Text = o.Label.UserLocalizedLabel.Label }).ToList();
        }
    }
}
