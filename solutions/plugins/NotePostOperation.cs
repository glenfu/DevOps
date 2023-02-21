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
            IPluginExecutionContext context = (IPluginExecutionContext)
               serviceProvider.GetService(typeof(IPluginExecutionContext));

            try
            {
                Entity entity = (Entity)context.InputParameters["Target"];
                if (entity.LogicalName == "annotation")
                {
                    Entity annotation = (Entity)context.InputParameters["Target"];
                    Entity postImageAnnotation = (Entity)context.PostEntityImages["Image"];

                    string notetext = postImageAnnotation.GetAttributeValue<string>("notetext");

                    annotation["notetext"] = "*WEB*" + notetext;

                    // Obtain the organization service reference.
                    IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider
                    .GetService(typeof(IOrganizationServiceFactory));

                    IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);
                    //Create the task
                    service.Update(annotation);
                }
            }
            catch (Exception e)
            {
                throw new InvalidPluginExecutionException(e.Message);
            }
        }
    }
}
