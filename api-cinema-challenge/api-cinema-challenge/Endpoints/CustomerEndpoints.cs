using api_cinema_challenge.DTOs.CustomerDTOs;
using api_cinema_challenge.Models;
using api_cinema_challenge.Repository;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace api_cinema_challenge.Endpoints
{
    public static class CustomerEndpoints
    {
        public static void ConfigureCustomerEndpoint(this WebApplication app)
        {
            var customers = app.MapGroup("customers");

            customers.MapGet("/{id}", GetCustomerById);
            customers.MapGet("/", GetCustomers);
            customers.MapPost("/", AddCustomer);
            customers.MapDelete("/{id}", DeleteCustomer);
            customers.MapPut("/{id}", UpdateCustomer);

        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public static async Task<IResult> GetCustomerById(int id, IRepository<Customer> repository)
        {
            var customers = await repository.GetAll();
            var targetCustomer = customers.FirstOrDefault(c => c.Id == id);
            if (targetCustomer == null) { return TypedResults.NotFound($"Customer with id {id} not found."); }

            var customerDto = new CustomerDto
            {
                Id = targetCustomer.Id,
                Name = targetCustomer.Name,
                Email = targetCustomer.Email,
                Phone = targetCustomer.Phone
            };

            return TypedResults.Ok(customerDto);
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
        public static async Task<IResult> AddCustomer(IRepository<Customer> repository, [FromBody] CustomerPostDto model, IValidator<CustomerPostDto> validator, HttpRequest request)
        {
            if (model == null) { return TypedResults.BadRequest("Invalid customer data"); }

            var validationResult = await validator.ValidateAsync(model);
            if (!validationResult.IsValid)
            {
                var errors = string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage));
                return TypedResults.BadRequest(errors);
            }

            var newCustomer = new Customer { Name = model.Name, Email = model.Email, Phone = model.Phone };
            var addedCustomer = await repository.Add(newCustomer);

            var customerDto = new CustomerDto { Id = addedCustomer.Id, Name = addedCustomer.Name, Email=addedCustomer.Email, Phone = addedCustomer.Phone };

            var baseUrl = $"{request.Scheme}://{request.Host}{request.PathBase}";
            var location = $"{baseUrl}/customers/{addedCustomer.Id}";
            return TypedResults.Created(location, customerDto);
        
        }

        public static async Task<IResult> DeleteCustomer(int id, IRepository<Customer> repository)
        {
            var targetCustomer = await repository.GetById(id);
            if (targetCustomer == null) {  return TypedResults.NotFound($"Customer with id {id} not found."); }

            var deletedCustomer = await repository.Delete(id);

            var customerDto = new CustomerDto
            {
                Id = deletedCustomer.Id,
                Name = deletedCustomer.Name,
                Email = deletedCustomer.Email,
                Phone = deletedCustomer.Phone
            };

            return TypedResults.Ok(customerDto);
        }


        public static async Task<IResult> UpdateCustomer(int id, IRepository<Customer> repository, [FromBody] CustomerPutDto model, IValidator<CustomerPutDto> validator, HttpRequest request)
        {
            // check if the customer we want to update exists
            var existingCustomer = await repository.GetById(id);
            if (existingCustomer == null) { return TypedResults.NotFound($"The customer you want to update with ID {id} does not exist"); }
            
            if (model == null) { return TypedResults.BadRequest("Invalid customer data"); }

            var validationResult = await validator.ValidateAsync(model);
            if (!validationResult.IsValid)
            {
                var errors = string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage));
                return TypedResults.BadRequest(errors);
            }

            // check if the new name already exists for another customer
            var allCustomers = await repository.GetAll();
            var duplicateNameCustomer = allCustomers.FirstOrDefault(
                c => c.Name == model.Name && c.Id != id);
            if (duplicateNameCustomer != null) { return TypedResults.BadRequest($"A customer with the name '{model.Name}' already exists."); }

            // update the customer
            existingCustomer.Name = model.Name;
            existingCustomer.Email = model.Email;
            existingCustomer.Phone = model.Phone;

            var updatedCustomer = await repository.Update(id, existingCustomer);

            // generate respone dto
            var customerDto = new CustomerDto { Id = updatedCustomer.Id, Name = updatedCustomer.Name, Email = updatedCustomer.Email, Phone = updatedCustomer.Phone };

            var baseUrl = $"{request.Scheme}://{request.Host}{request.PathBase}";
            var location = $"{baseUrl}/customers/{updatedCustomer.Id}";
            return TypedResults.Created(location, customerDto);

        }
    }
}
