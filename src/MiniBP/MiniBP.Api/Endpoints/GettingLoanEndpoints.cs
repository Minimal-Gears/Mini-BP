using Api.Services.WorkflowRegistration;
using Common;
using Microsoft.AspNetCore.Mvc;
using MiniBP.BPMS.Services.CartableService;
using MiniBP.BPMS.Services.CartableService.Params;

namespace Api.Model.Endpoints;

public class GettingLoanEndpoints
{
    // public GettingLoanEndpoints(IEndpointRouteBuilder endpointBuilder)
    // {
    //     endpointBuilder.MapGet("/loan", async (CartableService cartableService) => {
    //                                         GettingLoanFlow flow = new GettingLoanFlow([]);
    //
    //                                         StartWorkFlowParams<GettingLoanSteps> startParams = new StartWorkFlowParams<GettingLoanSteps>(flow, "TestTitle", Guid.NewGuid(),
    //                                              new Dictionary<string, string>() { { "EntityId", "1" } });
    //
    //                                         var newCase = await cartableService.Start(startParams);
    //
    //                                         await cartableService.Route<GettingLoanSteps>(new RouteVariable() { CaseId = newCase.Id });
    //
    //                                         return "OK";
    //                                     }).WithName("StartLoan");
    // }

    public static void Register(IEndpointRouteBuilder endpointBuilder)
    {
        endpointBuilder.MapGet("/loan", ApplyLoan).WithName("ApplyLoan");
        endpointBuilder.MapGet("/pre-check/{caseId}", DoPrimitiveCheck).WithName("DoPrimitiveCheck");
    }

    public static async Task<IResult> ApplyLoan(CartableService cartableService, IUserContext userContext)
    {
        GettingLoanFlow flow = new GettingLoanFlow([], null);

        StartWorkFlowParams<GettingLoanSteps> startParams = new StartWorkFlowParams<GettingLoanSteps>(flow, "TestTitle", userContext.CurrentUser.UserId,
                                                                                                      new Dictionary<string, string>() { { "EntityId", "1" } });

        var newCase = await cartableService.Start(startParams);

        var @case = await cartableService.Route<GettingLoanSteps>(new RouteVariable() { CaseId = newCase.Id });

        return TypedResults.Ok(@case.Id);
    }

    public static async Task<IResult> DoPrimitiveCheck([FromRoute] int caseId, CartableService cartableService)
    {
        var newCase = await cartableService.GetById(caseId);

        await cartableService.Route<GettingLoanSteps>(new RouteVariable() { CaseId = newCase.Id });

        return TypedResults.Ok();
    }
}
