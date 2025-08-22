using api_cinema_challenge.DTOs.CustomerDTOs;
using api_cinema_challenge.Models;
using api_cinema_challenge.Repository;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace api_cinema_challenge.Endpoints
{
    public static class CustomerEndpoints
    {
        public static void ConfigureCustomerEndpoint(this WebApplication app)
        {
            var customers = app.MapGroup("customers");

            customers.MapGet("/", GetCustomers);
            customers.MapPost("/", AddCustomer);

        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public static async Task<IResult> GetCustomers(IRepository<Customer> repository)
        {
            var customers = await repository.GetAll();
            if (customers == null || !customers.Any()) { return Results.NotFound("No customers found."); }

            var customerDto = customers.Select(c => new CustomerDto
            {
                Id = c.Id,
                Name = c.Name,
                Email = c.Email,
                Phone = c.Phone
            }).ToList();

            return TypedResults.Ok(customerDto);

        }

        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public static async Task<IResult> AddCustomer(IRepository<Customer> repository, [FromBody] CustomerPostDto model, HttpRequest request)
        {
            if (model == null) return TypedResults.BadRequest("Invalid customer data");
            if (string.IsNullOrWhiteSpace(model.Name) || string.IsNullOrWhiteSpace(model.Email)) return TypedResults.BadRequest("Invalid customer data");
            
            var phoneAttribute = new PhoneAttribute();
            if (!phoneAttribute.IsValid(model.Phone))
                return TypedResults.BadRequest("Invalid phone number format.");

            var newCustomer = new Customer { Name = model.Name, Email = model.Email, Phone = model.Phone };
            var addedCustomer = await repository.Add(newCustomer);

            var customerDto = new CustomerDto { Id = addedCustomer.Id, Name = addedCustomer.Name, Email=addedCustomer.Email, Phone = addedCustomer.Phone };

            var baseUrl = $"{request.Scheme}://{request.Host}{request.PathBase}";
            var location = $"{baseUrl}/patients/{addedCustomer.Id}";
            return TypedResults.Created(location, customerDto);

        }

    }
}
