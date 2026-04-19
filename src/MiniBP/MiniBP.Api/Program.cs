using Api.Model;
using Api.Services.WorkflowRegistration;
using Common;
using Microsoft.EntityFrameworkCore;
using MiniBP.BPMS.Domain.Model.Workflow;
using MiniBP.BPMS.Domain.Model.Workflow.AssignmentMethod;
using MiniBP.BPMS.Domain.Repository;
using MiniBP.BPMS.Services.CartableService;
using MiniBP.BPMS.Services.CartableService.Params;
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

app.MapGet("/test", async (CartableService cartableService) => {
                        GettingLoanFlow flow = new GettingLoanFlow([]);

                        StartWorkFlowParams<GettingLoanSteps> startParams = new StartWorkFlowParams<GettingLoanSteps>(flow, "TestTitle", Guid.NewGuid(), []);

                        var newCase = await cartableService.Start(startParams);

                        await cartableService.Route<GettingLoanSteps>(new RouteVariable() { CaseId = newCase.Id });

                        return "OK";
                    })
   .WithName("Test");

app.Run();
