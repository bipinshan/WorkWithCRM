using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk;
using Microsoft.Crm.Sdk;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Messages;
using System.ServiceModel.Description;
using System.Net;
using XrmExtensions;
using System.ServiceModel;

namespace WorkWithCRM
{
    class Program
    {
        static IOrganizationService _orgservice;
        static void Main(string[] args)
        {
            //CrmConnection conn = CrmConnection.Parse("Url=https://mmtrail.api.crm.dynamics.com; Username=BIPIN@MMTrail.onmicrosoft.com; Password=infosys-123;");
            //OrganizationService _orgservice = new
            //    OrganizationService(conn);

            ConnectToMSCRM("bipin.kumar@movement.com", "August@7702943098", "https://mmdev.api.crm.dynamics.com/XRMServices/2011/Organization.svc");

            if (_orgservice != null)
            {
                p objp = new p();
                objp.ProcessImportRecord(_orgservice);
            }

            #region Commented code

            //Entity Contact = new Entity("contact");

            //Contact["lastname"] = "Test Entity Image13";
            //Guid id = _orgservice.Create(Contact);

            ////Get Web Resource content based on name
            //var query = new QueryExpression("webresource");
            //query.TopCount = 1;
            //query.ColumnSet.AddColumns("name", "content");
            //query.Criteria.AddCondition("name", ConditionOperator.Equal, "ims_Prospect");
            //var webResource = _orgservice.RetrieveMultiple(query);

            //if (webResource.Entities.Count > 0)
            //{
            //    if (webResource.Entities[0].Attributes.Contains("content"))
            //    {
            //        string content = webResource.Entities[0]["content"] as string;

            //        Entity objContact = new Entity("contact");
            //        objContact.Id = id;
            //        objContact["entityimage"] = Convert.FromBase64String(content);
            //        _orgservice.Update(objContact);
            //    }
            //}

            //DeploymentService service = new DeploymentService(conn);

            //ColumnSet c=new ColumnSet();
            //c.AllColumns = true;
            //RetrieveAdvancedSettingsRequest request = new RetrieveAdvancedSettingsRequest();
            //request.ConfigurationEntityName = "Deployment";
            //request.ColumnSet=c;
            ////         {
            ////ConfigurationEntityName = "Deployment";
            ////c.AllColumns = false; // Returns only writable properties.
            //// };

            //RetrieveAdvancedSettingsResponse response = (RetrieveAdvancedSettingsResponse)service.Execute(request);


            // Microsoft.Xrm.Sdk.Deployment.DeploymentServiceClient deploymentClient = Microsoft.Xrm.Sdk.Deployment.Proxy.ProxyClientHelper.CreateClient(new Uri("http://<server>:<port>/XRMDeployment/2011/Deployment.svc"));
            // deploymentClient.ClientCredentials.Windows.ClientCredential = new System.Net.NetworkCredential("<username>", "<password>", "<domain>");
            // // find org
            //// var organizations = deploymentClient.RetrieveAll(Microsoft.Xrm.Sdk.Deployment.DeploymentEntityType.Organization);
            // //var org = organizations.Where(o => o.Name.Equals("<orgname>")).SingleOrDefault();
            // // update UserRootPath setting
            // Microsoft.Xrm.Sdk.Deployment.ConfigurationEntity orgSettings = new Microsoft.Xrm.Sdk.Deployment.ConfigurationEntity
            // {
            //   //  Id = org.Id,
            //     LogicalName = "Organization"
            // };
            // orgSettings.Attributes = new Microsoft.Xrm.Sdk.Deployment.AttributeCollection();
            // orgSettings.Attributes.Add(new KeyValuePair<string, object>("UserRootPath", "LDAP://<domain>.<tld>/OU=<ou4>,OU=<ou3>,OU=<ou2>,OU=<ou1>,DC=<domain>,DC=<tld>"));
            // Microsoft.Xrm.Sdk.Deployment.UpdateAdvancedSettingsRequest reqUpdateSettings = new Microsoft.Xrm.Sdk.Deployment.UpdateAdvancedSettingsRequest
            // {
            //     Entity = orgSettings
            // };
            // Microsoft.Xrm.Sdk.Deployment.UpdateAdvancedSettingsResponse respUpdateSettings = (Microsoft.Xrm.Sdk.Deployment.UpdateAdvancedSettingsResponse)deploymentClient.Execute(reqUpdateSettings);



            ////ConfigurationEntity configEntity =response.Entity;
            //ConfigurationEntity entity = new ConfigurationEntity();
            //// entity.Id = org.Id;
            //entity.LogicalName = "Deployment";






            //entity.Attributes = new Microsoft.Xrm.Sdk.Deployment.AttributeCollection();
            //entity.Attributes.Add(new KeyValuePair<string, object>("AggregateQueryRecordLimit", 1000000));

            //UpdateAdvancedSettingsRequest request2 = new UpdateAdvancedSettingsRequest();
            //request2.Entity = entity;


            //service.Execute(request2);
            #endregion
        }

        public static void ConnectToMSCRM(string UserName, string Password, string SoapOrgServiceUri)
        {
            try
            {
                ClientCredentials credentials = new ClientCredentials();
                credentials.UserName.UserName = UserName;
                credentials.UserName.Password = Password;
                Uri serviceUri = new Uri(SoapOrgServiceUri);
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                OrganizationServiceProxy proxy = new OrganizationServiceProxy(serviceUri, null, credentials, null);
                proxy.EnableProxyTypes();
                _orgservice = (IOrganizationService)proxy;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error while connecting to CRM " + ex.Message);
                Console.ReadKey();
            }
        }
    }

    public class p
    {
        List<Common.Mapping> mappings = new List<Common.Mapping>();
        //string validationMessage = string.Empty;
        //string infoMessage = string.Empty;
        string errorMessage = string.Empty;
        //string defaultMessage = string.Empty;
        Dictionary<string, string> dcConfigDetails = new Dictionary<string, string>();
        bool validationStatus = true;
        bool canReturn = false;
        //string errorLog = string.Empty;
        bool errorLogConfig = false;

        public void ProcessImportRecord(IOrganizationService service)
        {
            ////Extract the tracing service for use in debugging sandboxed plug-ins.
            //ITracingService tracingService =
            //    (ITracingService)serviceProvider.GetService(typeof(ITracingService));

            //// Obtain the execution context from the service provider.
            //Microsoft.Xrm.Sdk.IPluginExecutionContext context = (Microsoft.Xrm.Sdk.IPluginExecutionContext)
            //    serviceProvider.GetService(typeof(Microsoft.Xrm.Sdk.IPluginExecutionContext));

            //// Obtain the organization service reference.
            //IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            //IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);
            Entity leadStaging = null;
            Common objCommon = new Common();
            //if (context.MessageName.ToLower() == "update" && context.Depth > 1)
            //    return;
            try
            {
                //if (context.PostEntityImages.Contains("PostImage") && context.PostEntityImages["PostImage"] is Entity)
                //{
                //    leadStaging = (Entity)context.PostEntityImages["PostImage"];
                //}
                //else if (context.InputParameters.Contains("Target") && context.InputParameters["Target"] is Entity)
                //{
                //    leadStaging = (Entity)context.InputParameters["Target"];
                //}
                //if (leadStaging.LogicalName != LeadStaging.EntityName)
                //    return;
                //Context.FetchConfigDetails()

                // Define Condition Values
                var QEims_leadstaging_ims_email = "john.doeedw@test.com";

                // Instantiate QueryExpression QEims_leadstaging
                var QEims_leadstaging = new QueryExpression("ims_leadstaging");

                // Add all columns to QEims_leadstaging.ColumnSet
                QEims_leadstaging.ColumnSet.AllColumns = true;

                // Define filter QEims_leadstaging.Criteria
                QEims_leadstaging.Criteria.AddCondition("ims_email", ConditionOperator.Equal, QEims_leadstaging_ims_email);

                EntityCollection ecLeadStagings = service.RetrieveMultiple(QEims_leadstaging);
                if (ecLeadStagings != null && ecLeadStagings.Entities.Count > 0)
                {
                    leadStaging = ecLeadStagings.Entities[0];
                }

                UpsertLeadDetails(leadStaging, service, ref errorMessage);
                if (errorLogConfig)
                {
                    objCommon.CreateErrorLog(leadStaging.Id.ToString(), errorMessage, service);
                }
            }
            catch (FaultException<OrganizationServiceFault> ex)
            {
                throw new InvalidPluginExecutionException(ex.Message);
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException(ex.Message);
            }
        }

        public void UpsertLeadDetails(Entity leadStaging, IOrganizationService service, ref string errorMessage)
        {
            //Fetch Mapping Details [ImportDetailsMapping]
            Guid importMasterDataId = new Guid("e0d56b46-05fe-e911-a811-000d3a4f6fce");//EDW Lead [Import Data Master]
            Entity objLeadStaging = null;
            Common objCommon = new Common();
            if (!objCommon.FetchMappings(importMasterDataId, ref mappings, service, ref errorMessage))
            {
                objCommon.UpdateStagingLog(leadStaging.Id, Guid.Empty, LeadStaging.EntityName, errorMessage, service);
                return;
            }
            //Check for Mandatory fields and Max Length Allowed
            objCommon.MandatoryValidation(leadStaging, mappings, ref validationStatus, ref canReturn, ref errorMessage);
            objLeadStaging = objCommon.FormTargetEntityObject(Lead.EntityName, leadStaging, mappings, service, ref validationStatus, ref canReturn, ref errorMessage);
            if (canReturn)
            {
                objCommon.UpdateStagingLog(leadStaging.Id, Guid.Empty, LeadStaging.EntityName, errorMessage, service);
                return;
            }
            Guid leadId = Guid.Empty;
            //Check for Existing Active Lead record
            bool existingLead = false;
            Entity objExistingLead = GetPreviousLead("", service);
            if (objExistingLead != null && objExistingLead.Id != Guid.Empty)
            {
                existingLead = true;
                leadId = objExistingLead.Id;
            }
            //Existing Active Lead record present in SYSTEM. update dirty fields
            if (existingLead)
            {
                objCommon.UpdateRecordIfDirty(objExistingLead, objLeadStaging, Lead.EntityName, mappings, service, ref validationStatus, ref canReturn, ref errorMessage);
                if (canReturn)
                {
                    objCommon.UpdateStagingLog(leadStaging.Id, Guid.Empty, LeadStaging.EntityName, errorMessage, service);
                    return;
                }
            }
            //No Existing Active Lead Present in SYSTEM. Created new Lead with all details
            else
            {
                try
                {
                    leadId = service.Create(objLeadStaging);
                }
                catch (Exception ex)
                {
                    objCommon.UpdateValidationMessage("Error while create Lead record " + ex.Message, ref errorMessage);
                    validationStatus = false;
                    canReturn = true;
                }
            }
            if (canReturn)
            {
                objCommon.UpdateStagingLog(leadStaging.Id, Guid.Empty, LeadStaging.EntityName, errorMessage, service);
                return;
            }
            if (leadId != Guid.Empty)
                objCommon.UpdateStagingLog(leadStaging.Id, leadId, LeadStaging.EntityName, errorMessage, service);
        }

        public Entity GetPreviousLead(string externalId, IOrganizationService service)
        {
            Entity objExistingLead = null;
            //// Define Condition Values
            //var QEims_leadstaging_ims_email = "john.doeryan@test.com";

            //// Instantiate QueryExpression QEims_leadstaging
            //var QEims_leadstaging = new QueryExpression("lead");

            //// Add all columns to QEims_leadstaging.ColumnSet
            //QEims_leadstaging.ColumnSet.AllColumns = true;

            //// Define filter QEims_leadstaging.Criteria
            //QEims_leadstaging.Criteria.AddCondition("emailaddress1", ConditionOperator.Equal, QEims_leadstaging_ims_email);

            //EntityCollection ecLeadStagings = service.RetrieveMultiple(QEims_leadstaging);
            //if (ecLeadStagings != null && ecLeadStagings.Entities.Count > 0)
            //{
            //    objExistingLead = ecLeadStagings.Entities[0];
            //}
            return objExistingLead;
        }
    }
}