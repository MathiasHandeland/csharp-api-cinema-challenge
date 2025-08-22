using api_cinema_challenge.Data;
using api_cinema_challenge.DTOs.CustomerDTOs;
using api_cinema_challenge.DTOs.MovieDTOs;
using api_cinema_challenge.DTOs.ScreeningDTOs;
using api_cinema_challenge.Endpoints;
using api_cinema_challenge.Models;
using api_cinema_challenge.Repository;
using api_cinema_challenge.Validators.CustomerValidators;
using api_cinema_challenge.Validators.MovieValidators;
using api_cinema_challenge.Validators.ScreeningValidators;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using System.Diagnostics;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddOpenApi();
builder.Services.AddDbContext<CinemaContext>(options => {
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnectionString"));
    options.LogTo(message => Debug.WriteLine(message));
});

builder.Services.AddScoped<IRepository<Customer>, Repository<Customer>>();
builder.Services.AddScoped<IRepository<Movie>, Repository<Movie>>();
builder.Services.AddScoped<IRepository<Screening>, Repository<Screening>>();
// Register validators as services
builder.Services.AddScoped<IValidator<CustomerPostDto>, CustomerPostValidator>();
builder.Services.AddScoped<IValidator<CustomerPutDto>, CustomerPutValidator>();
builder.Services.AddScoped<IValidator<MoviePostDto>, MoviePostValidator>();
builder.Services.AddScoped<IValidator<MoviePutDto>, MoviePutValidator>();
builder.Services.AddScoped<IValidator<ScreeningPostDto>, ScreeningPostValidator>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "Demo API");
    });
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();
app.ConfigureCustomerEndpoint();
app.ConfigureMovieEndpoint();
app.Run();
