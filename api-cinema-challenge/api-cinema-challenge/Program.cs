using api_cinema_challenge.Data;
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

app.Run();
