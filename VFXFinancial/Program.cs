using Core;
using Infra;
using Microsoft.AspNetCore.Mvc;
using VFXFinancial.Middlewares;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddSwaggerGen(options =>
{
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