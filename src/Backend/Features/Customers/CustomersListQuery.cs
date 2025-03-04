namespace Backend.Features.Customers;

public class CustomersListQuery : IRequest<List<CustomersListQueryResponse>>
{
    public string? Name { get; set; }
    public string? SearchText { get; set; }
    public string? SortBy { get; set; }     //Field that will order by Name and Email

    public int? Skip { get; set; }
    public int? Take { get; set; }
}

public class CustomersListQueryResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string Address { get; set; } = "";
    public string Email { get; set; } = "";
    public string Phone { get; set; } = "";
    public string Iban { get; set; } = "";

    public CustomersListQueryResponseCustomerCategory? CustomerCategory { get; set; }
}

public class CustomersListQueryResponseCustomerCategory
{
    public string Code { get; set; } = "";
    public string Description { get; set; } = "";
}

internal class CustomersListQueryHandler(BackendContext context) : IRequestHandler<CustomersListQuery, List<CustomersListQueryResponse>>
{
    private readonly BackendContext context = context;

    public async Task<List<CustomersListQueryResponse>> Handle(CustomersListQuery request, CancellationToken cancellationToken)
    {
        var query = context.Customers.AsQueryable();

        /*
        Filter SearchText will filter on Name and Email. 
        If the field is null, the query will show all the Customers
        */
        if (!string.IsNullOrEmpty(request.SearchText))
            query = query.Where(q => q.Name.ToLower().Contains(request.SearchText.ToLower()) || q.Email.ToLower().Contains(request.SearchText.ToLower()));

        /*
        It will order by Name or Email depending on the
        value of SortBy:
        /api/customers/list?SortBy=Name -> will order by Name
        /api/customers/list?SortBy=Email -> will order by Email
        */
        if (!string.IsNullOrEmpty(request.SortBy))
        {
            if (request.SortBy.Equals("Name", StringComparison.OrdinalIgnoreCase))
            {
                query = query.OrderBy(q => q.Name);
            }
            else if (request.SortBy.Equals("Email", StringComparison.OrdinalIgnoreCase))
            {
                query = query.OrderBy(q => q.Email);
            }
        }
            
        /*
        Checks if Skip and Take are null.
        If they are, default values are assigned.
        Shows all records.
        */
        if (!request.Skip.HasValue)
            request.Skip = 0;

        if (!request.Take.HasValue)
            request.Take = int.MaxValue;

        query = query.Skip(request.Skip.Value).Take(request.Take.Value);

        var data = await query.ToListAsync(cancellationToken);
        var result = new List<CustomersListQueryResponse>();

        foreach (var item in data)
        {
            var resultItem = new CustomersListQueryResponse
            {
                Id = item.Id,
                Name = item.Name,
                Address = item.Address,
                Email = item.Email,
                Phone = item.Phone,
                Iban = item.Iban
            };

            var customerCategory = await context.CustomerCategories.SingleOrDefaultAsync(q => q.Id == item.CustomerCategoryId, cancellationToken);
            if (customerCategory is not null)
                resultItem.CustomerCategory = new CustomersListQueryResponseCustomerCategory
                {
                    Code = customerCategory.Code,
                    Description = customerCategory.Description
                };

            result.Add(resultItem);
        }

        return result;
    }
}