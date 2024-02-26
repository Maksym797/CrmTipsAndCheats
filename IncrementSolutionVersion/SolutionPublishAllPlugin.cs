using System;
using System.Linq;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace IncrementSolutionVersion;

public class SolutionPublishAllPlugin : IPlugin
{
    private const string SolutionName = "FlowersOrderRelease";

    public void Execute(IServiceProvider serviceProvider)
    {
        if (serviceProvider == null)
            return;

        var factory = (IOrganizationServiceFactory) serviceProvider.GetService(typeof(IOrganizationServiceFactory));
        var systemUserService = factory.CreateOrganizationService(null);

        var solution = RetrieveSolution(systemUserService);

        if (solution == default)
            return;

        var solutionVersion = solution.GetAttributeValue<string>("version");

        var majorVersion = solutionVersion.Split('.')[0];
        var minorVersion = solutionVersion.Split('.')[1];
        var maintenanceVersion = solutionVersion.Split('.')[2];
        var buildVersion = solutionVersion.Split('.')[3];

        Update(systemUserService, new Entity(solution.LogicalName, solution.Id)
        {
            ["version"] =
                $"{majorVersion}.{minorVersion}.{maintenanceVersion}.{IncrementVersion(buildVersion)}",
        });
    }

    private static string IncrementVersion(string version) =>
        int.TryParse(version, out var r) ? (r + 1).ToString() : version;

    private static Entity RetrieveSolution(IOrganizationService service)
    {
        var query = new QueryExpression("solution");
        query.ColumnSet.AddColumns("version");
        query.Criteria.AddCondition("uniquename", ConditionOperator.Equal, SolutionName);

        return service.RetrieveMultiple(query).Entities.FirstOrDefault();
    }

    private static void Update(IOrganizationService service, Entity entity) => service.Update(entity);
}