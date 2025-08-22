using api_cinema_challenge.DTOs.CustomerDTOs;
using api_cinema_challenge.Models;
using api_cinema_challenge.Repository;
using Microsoft.AspNetCore.Mvc;

namespace api_cinema_challenge.Endpoints
{
    public static class CustomerEndpoints
    {
        public static void ConfigureCustomerEndpoint(this WebApplication app)
        {
            var customers = app.MapGroup("customers");

            customers.MapGet("/", GetCustomers);
            
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public static async Task<IResult> GetCustomers(IRepository<Customer> repository)
        {
            var customers = await repository.GetAll();
            if (customers == null || !customers.Any()) { return Results.NotFound("No customers found."); }

            var customerDto = customers.Select(c => new CustomerGetDto
            {
                Id = c.Id,
                Name = c.Name,
                Email = c.Email,
                Phone = c.Phone
            }).ToList();

            return Results.Ok(customerDto);

        }

    }
}
