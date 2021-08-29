using Microsoft.EntityFrameworkCore.Query.SqlExpressions;

namespace Api.Services.WorkflowRegistration
{
    public enum GettingLoanSteps
    {
        Apply = 1,
        PrimitiveCheck = 2,
        PreparingDocuments = 3,
        Payment = 4
    }
}