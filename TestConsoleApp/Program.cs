using System;
using System.Configuration;
using System.Linq;
using System.ServiceModel.Description;

using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Tooling.Connector;

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

            // The number of records per page to retrieve.
            int queryCount = 1;
            // Initialize the page number.
            int pageNumber = 1;
            // Initialize the number of records.
            int recordCount = 0;

            Uri oUri = new Uri(CRMPath + "/XRMServices/2011/Organization.svc");

            ClientCredentials clientCredentials = new ClientCredentials();
            clientCredentials.UserName.UserName = UserName;
            clientCredentials.UserName.Password = Password;

            var crmInstance = new OrganizationServiceProxy(oUri, null, clientCredentials, null);
            crmInstance.Timeout = TimeSpan.FromSeconds(Timeout);
            crmInstance.EnableProxyTypes();


            //Use the connection string named "MyCRMServer"
            //from the configuration file
            CrmServiceClient crmSvc = new CrmServiceClient(ConfigurationManager.ConnectionStrings["MyCRMServer"].ConnectionString);
            

            // Instantiate QueryExpression QEjnbs_location
            var QEjnbs_location = new QueryExpression("jnbs_location");
            //QEjnbs_location.TopCount = 50;
            QEjnbs_location.PageInfo = new PagingInfo();
            QEjnbs_location.PageInfo.Count = queryCount;
            QEjnbs_location.PageInfo.PageNumber = pageNumber;

            // Add columns to QEjnbs_location.ColumnSet
            QEjnbs_location.ColumnSet.AddColumns("jnbs_name", "jnbs_code", "createdon");

            while (true)
            {
                var response = crmSvc.RetrieveMultiple(QEjnbs_location);

                if (response.Entities != null)
                {
                    recordCount = recordCount + response.Entities.Count;
                    Console.WriteLine("Records found: " + response.Entities.Count.ToString(), response.Entities.Count);
                    Console.WriteLine("Records found so far... " + recordCount.ToString());
                }

                if (response.MoreRecords)
                {
                    QEjnbs_location.PageInfo.PageNumber++;
                    QEjnbs_location.PageInfo.PagingCookie = response.PagingCookie;
                }
                else
                {
                    // If no more records are in the result nodes, exit the loop.
                    break;
                }
            }

            // Add Options Set for Lead Status Test.
            var attributeRequest = new RetrieveAttributeRequest
            {
                EntityLogicalName = "lead",
                LogicalName = "jnbs_status",
                RetrieveAsIfPublished = true
            };

            var attributeResponse = (RetrieveAttributeResponse)crmSvc.Execute(attributeRequest);
            var attributeMetadata = (EnumAttributeMetadata)attributeResponse.AttributeMetadata;

            var optionList = (from o in attributeMetadata.OptionSet.Options
                              select new { Value = o.Value, Text = o.Label.UserLocalizedLabel.Label }).ToList();

            foreach(var option in optionList)
            {
                Console.WriteLine("option: " + option.Value + " - " + option.Text);
            }

            // Add Options Set for Lead Title Test.
            var attributeTitleRequest = new RetrieveAttributeRequest
            {
                EntityLogicalName = "lead",
                LogicalName = "jnbs_titlegeneral",
                RetrieveAsIfPublished = true
            };

            var attributeTitleResponse = (RetrieveAttributeResponse)crmSvc.Execute(attributeTitleRequest);
            var attributeTitleMetadata = (EnumAttributeMetadata)attributeTitleResponse.AttributeMetadata;

            var titleOptionsList = (from o in attributeTitleMetadata.OptionSet.Options
                              select new { Value = o.Value, Text = o.Label.UserLocalizedLabel.Label }).ToList();

            foreach (var titleOption in titleOptionsList)
            {
                Console.WriteLine("option: " + titleOption.Value + " - " + titleOption.Text);
            }
        }
    }
}
