using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace plugins
{
    public class RouteCasePostOperation : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            // Obtain the tracing service 
            ITracingService tracingService = (ITracingService)serviceProvider.GetService(typeof(ITracingService));
            // Obtain the execution context from the service provider 
            IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
            // Check if create message 
            if (context.MessageName.ToLower().Equals("create"))
            {
                // The InputParameters collection contains all the data passed in the message request 
                if (context.InputParameters.Contains("Target") && context.InputParameters["Target"] is Entity)
                {
                    // Obtain the target entity from the input parameters 
                    Entity entity = (Entity)context.InputParameters["Target"];
                    // Target is an email 
                    if (entity.LogicalName.ToLower().Equals("incident"))
                    {
                        try
                        {
                            // Obtain the organization service reference that you'll need for web service calls 
                            IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
                            IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);
                            // Execute msdyn_ApplyRoutingRuleEntityRecord request 
                            OrganizationRequest request = new OrganizationRequest("msdyn_ApplyRoutingRuleEntityRecord");
                            request["Target"] = new EntityReference("incident", entity.Id);
                            service.Execute(request);
                        }
                        catch (Exception ex)
                        {
                            tracingService.Trace("RouteCasePostOperation: {0}", ex.ToString());
                            throw;
                        }
                    }
                }
            }
        }
    }
}