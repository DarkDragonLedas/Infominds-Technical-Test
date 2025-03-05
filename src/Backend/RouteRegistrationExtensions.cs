using Backend.Features.Employees;
using Backend.Features.Suppliers;
using Backend.Features.Customers;

namespace Backend;

static class RouteRegistrationExtensions
{
    public static void UseApiRoutes(this WebApplication app)
    {
        var apiGroup = app.MapGroup("api");

        apiGroup.MapGet("suppliers/list", async ([AsParameters] SupplierListQuery query, IMediator mediator) => await mediator.Send(query))
                    .WithName("GetSuppliersList")
                    .WithOpenApi();

        apiGroup.MapGet("employees/list", async ([AsParameters] EmployeesListQuery query, IMediator mediator) => await mediator.Send(query))
                    .WithName("GetEmployeesList")
                    .WithOpenApi();

        apiGroup.MapGet("customers/list", async ([AsParameters] CustomersListQuery query, IMediator mediator, HttpContext httpContext) =>
        {
            var customers = await mediator.Send(query);
            
            /*
            Check if XML format is requested.
            If I write /api/customers/list?format=xml, the endpoint returns the records
            in a XML format.
            */
            if (httpContext.Request.Query["format"].Equals("xml"))
            {
                return Results.Content(XMLExport(customers), "application/xml");
            }

            return Results.Json(customers);
        })
        .WithName("GetCustomersList")
        .WithOpenApi();

    }

    // Function that serializes data in XML format
    private static string XMLExport<T>(T obj)
    {
        var xmlSerializer = new System.Xml.Serialization.XmlSerializer(typeof(T));
        using var stringWriter = new StringWriter();
        xmlSerializer.Serialize(stringWriter, obj);
        return stringWriter.ToString();
    }
}
