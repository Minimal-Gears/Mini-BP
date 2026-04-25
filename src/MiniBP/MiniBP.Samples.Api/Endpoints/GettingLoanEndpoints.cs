using Common;
using Microsoft.AspNetCore.Mvc;

namespace MiniBP.Samples.Api.Endpoints;

public class GettingLoanEndpoints
{

    public static void Register(IEndpointRouteBuilder endpointBuilder)
    {
        endpointBuilder.MapGet("/loan", ApplyLoan).WithName("ApplyLoan");
        endpointBuilder.MapGet("/pre-check/{caseId}", DoPrimitiveCheck).WithName("DoPrimitiveCheck");
    }

    public static async Task<IResult> ApplyLoan(IUserContext userContext)
    {

        return TypedResults.Ok();
    }

    public static async Task<IResult> DoPrimitiveCheck([FromRoute] int caseId)
    {
     

        return TypedResults.Ok();
    }
}
