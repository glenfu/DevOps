using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Linq;
using System.ServiceModel;
using System.Xml.Linq;

namespace plugins
{
    public class CaseAgePreOperation : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            // Obtain the tracing service
            ITracingService tracingService = (ITracingService)serviceProvider.GetService(typeof(ITracingService));

            // Obtain the execution context from the service provider.  
            IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));

            // The InputParameters collection contains all the data passed in the message request.  
            if (context.InputParameters.Contains("Query"))
            {
                // Obtain the organization service reference which you will need for  
                // web service calls.  
                IOrganizationServiceFactory serviceFactory =
                    (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
                IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);

                try
                {
                    var query = (QueryBase)context.InputParameters["Query"];
                    var fetchQuery = query as FetchExpression;
                    if (fetchQuery != null)
                    {
                        tracingService.Trace("Found FetchExpression Query");

                        XDocument fetchXmlDoc = XDocument.Parse(fetchQuery.Query);
                        //The required entity element
                        var entityElement = fetchXmlDoc.Descendants("entity").FirstOrDefault();
                        var entityName = entityElement.Attributes("name").FirstOrDefault().Value;

                        //Only applying to the account entity
                        if (entityName == "incident")
                        {
                            tracingService.Trace("Query on Case confirmed");

                            //Get all filter elements
                            var filterElements = entityElement.Descendants("filter");

                            //Find any existing statecode conditions
                            var stateCodeConditions = from c in filterElements.Descendants("condition")
                                                      where c.Attribute("attribute").Value.Equals("adx_publishtoweb") && c.Attribute("value").Value.Equals("1")
                                                      select c;

                            if (stateCodeConditions.Count() > 0)
                            {
                                tracingService.Trace("Removing existing statecode filter conditions.");
                            }
                            //Remove statecode conditions
                            stateCodeConditions.ToList().ForEach(x => x.Remove());


                            //Add the condition you want in a new filter
                            entityElement.Add(
                                new XElement("filter",
                                    new XElement("condition",
                                        new XAttribute("attribute", "createdon"),
                                        new XAttribute("operator", "olderthan-x-minutes"), //not equal
                                        new XAttribute("value", "240") //Inactive
                                        )
                                    )
                                );
                        }

                        fetchQuery.Query = fetchXmlDoc.ToString();
                    }
                }

                catch (FaultException<OrganizationServiceFault> ex)
                {
                    throw new InvalidPluginExecutionException("An error occurred in CaseAgePreOperation.", ex);
                }

                catch (Exception ex)
                {
                    tracingService.Trace("CaseAgePreOperation: {0}", ex.ToString());
                    throw;
                }
            }
        }
    }
}
