using Api.Model;
using Api.Services.WorkflowRegistration;
using Common;
using Microsoft.EntityFrameworkCore;
using MiniBP.BPMS.Domain.Model.Workflow;
using MiniBP.BPMS.Domain.Model.Workflow.AssignmentMethod;
using MiniBP.BPMS.Domain.Repository;
using MiniBP.BPMS.Services.CartableService;
using MiniBP.BPMS.Services.Dto.WorkFlow;
using MiniBP.Infrastructure.DataAccess;
using MiniBP.Infrastructure.DataAccess.Postgres;
using MiniBP.Infrastructure.DataAccess.Repository;
using MiniBP.Infrastructure.Helper;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddDbContext<PostgresBpmsDbContext>(dbContextOptions =>
                                                         dbContextOptions.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"),
                                                                                    npgsqlOptions => {
                                                                                        npgsqlOptions.EnableRetryOnFailure(maxRetryCount: 3,
                                                                                                                           maxRetryDelay: TimeSpan.FromSeconds(10),
                                                                                                                           errorCodesToAdd: null);
                                                                                    }));

builder.Services.AddScoped<BpmsDbContext, PostgresBpmsDbContext>();
builder.Services.AddScoped<ICaseRepository, CaseRepository>();
builder.Services.AddScoped<IBpmsUnitOfWork, BpmsUnitOfWork>();
builder.Services.AddScoped<IUserContext, UserContext>();
builder.Services.AddScoped<CartableService>();
builder.Services.AddScoped<IDbExceptionHelper, PostgresExceptionHelper>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment()) {
    app.MapOpenApi();
}

app.UseHttpsRedirection();

var summaries = new[] { "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching" };

app.MapGet("/weatherforecast", () => {
                                   var forecast = Enumerable.Range(1, 5).Select(index =>
                                                                                    new WeatherForecast(DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                                                                                                        Random.Shared.Next(-20, 55),
                                                                                                        summaries[Random.Shared.Next(summaries.Length)]))
                                      .ToArray();

                                   return forecast;
                               })
   .WithName("GetWeatherForecast");

app.MapGet("/test", async (CartableService cartableService) => {
                        GettingLoanFlow flow = new GettingLoanFlow([]);

                        StartWorkFlowDto<GettingLoanSteps> startParams = new StartWorkFlowDto<GettingLoanSteps>(flow, "TestTitle", Guid.NewGuid(), []);

                        var newCase = await cartableService.Start(startParams);

                        await cartableService.Route<GettingLoanSteps>(new RouteVariable() { CaseId = newCase.Id });

                        return "OK";
                    })
   .WithName("Test");

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
