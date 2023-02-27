using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace plugins
{
    public class NotePostOperation : IPlugin
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
                    if (entity.LogicalName.ToLower().Equals("annotation"))
                    {
                        try
                        {
                            Entity annotation = (Entity)context.InputParameters["Target"];
                            Entity postImageAnnotation = (Entity)context.PostEntityImages["Image"];

                            string noteText = postImageAnnotation.GetAttributeValue<string>("notetext");
                            // Check if notetext starts with *WEB*
                            if (!noteText.StartsWith("*WEB*"))
                            {
                                annotation["notetext"] = "*WEB*" + noteText;

                                // Obtain the organization service reference.
                                IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider
                                .GetService(typeof(IOrganizationServiceFactory));

                                IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);
                                //Update notetext with prefix *WEB*
                                service.Update(annotation);
                            }
                        }
                        catch (Exception ex)
                        {
                            tracingService.Trace("NotePostOperation: {0}", ex.ToString());
                            throw;
                        }
                    }
                }
            }
        }
    }
}
