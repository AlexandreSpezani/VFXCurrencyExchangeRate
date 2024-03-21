using Core;
using Infra;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using VFXFinancial.Middlewares;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddSwaggerGen(options =>
{
    // Enable endpoints documentation
    options.EnableAnnotations();
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "Currency Exchange Service", Version = "v1" });
    
    // To allow DTOs with same name
    options.CustomSchemaIds(s => s.FullName?.Replace("+", "."));
});

builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    // Deactivate .net DTO validation
    options.SuppressModelStateInvalidFilter = true;
});

builder.Services
    .AddHttpClient()
    .AddLogging()
    .AddApplication()
    .AddInfra(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseMiddleware<ErrorMiddleware>();

app.UseRouting();

app.MapControllers();

app.Run();