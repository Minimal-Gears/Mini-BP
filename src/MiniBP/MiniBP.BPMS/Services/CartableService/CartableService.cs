using System.Linq.Dynamic.Core;
using Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MiniBP.BPMS.Domain.Model.Cartable;
using MiniBP.BPMS.Domain.Model.Workflow;
using MiniBP.BPMS.Domain.Model.Workflow.AssignmentMethod;
using MiniBP.BPMS.Domain.Repository;
using MiniBP.BPMS.Services.Dto.Cartable;
using MiniBP.BPMS.Services.Dto.WorkFlow;

namespace MiniBP.BPMS.Services.CartableService;

public class CartableService : BaseService //where TStep : Enum
{
    private readonly ILogger<CartableService> logger;
    private readonly IBpmsUnitOfWork bpmsUnitOfWork;
    private readonly ICaseRepository caseRepository;
    private readonly IUserContext userContext;

    public CartableService(ICaseRepository caseRepository, IBpmsUnitOfWork bpmsUnitOfWork,
                           ILogger<CartableService> logger,
                           IUserContext userContext) : base(userContext)
    {
        this.caseRepository = caseRepository;
        this.bpmsUnitOfWork = bpmsUnitOfWork;
        this.logger = logger;
        this.userContext = userContext;
    }

    public async Task<Case> Start<TStep>(StartWorkFlowDto<TStep> startWorkFlowDto) where TStep : Enum
    {
        var @case = await Create(startWorkFlowDto);
        return @case;
    }

    public async Task<Case> Route<TStep>(RouteVariable routeVariable) where TStep : Enum
    {
        //var www = await caseRepository.GetById(caseId);
        var @case = await caseRepository.Get()
                       .Include(a => a.Tracks)
                       .Include(a => a.FlowParameters)
                       .FirstAsync(b => b.Id == routeVariable.CaseId);

        var workFlow = GetFlowInstance<TStep>(@case.WorkFlowReference, @case, routeVariable);
        var nextStep = workFlow.Next();
        @case.Route<TStep>(nextStep);
        caseRepository.Update(@case);
        try {
            await bpmsUnitOfWork.CommitAsync();
        }
        catch (Exception ex) {
            logger.LogError(ex, "CaseRepository");
            throw;
        }

        return @case;
    }

    public async Task<(IEnumerable<CartableDto>, int totalCount)> GetByUser(int pageNumber, int pageSize)
    {
        var userId = userContext.CurrentUser.UserId;
        var query = caseRepository.Get().Include(a => a.Tracks).Include(b => b.FlowParameters).Where(c => c.Tracks.Any(t => t.IsLatestTrack && t.CurrentUserId == userId));
        var cartables = await query.OrderBy(t => t.Id).Page(pageNumber, pageSize).ToListAsync();
        var cartableDtos = cartables.Select(a => CartableDto.ConvertToDto(a));
        return (cartableDtos, query.Count());
    }

    public async Task<CartableDto> GetById(int caseId)
    {
        var userId = userContext.CurrentUser.UserId;
        var @case = await caseRepository.Get().Include(a => a.Tracks).Include(b => b.FlowParameters).FirstOrDefaultAsync(a => a.Id == caseId);
        return CartableDto.ConvertToDto(@case);
    }

    private WorkFlow<TStep> GetFlowInstance<TStep>(string workFlowReference, Case @case, RouteVariable routeVariable) where TStep : Enum
    {
        var workFlowRefereceType = Type.GetType(workFlowReference);
        List<IFlowParameter> flowParameters = null;
        if (routeVariable != null && routeVariable.WorkflowParameters != null && routeVariable.WorkflowParameters.Any()) {
            flowParameters = new List<IFlowParameter>();
            foreach (var workflowParameter in routeVariable.WorkflowParameters) {
                var flowParameter = new FlowParameter(routeVariable.CaseId, workflowParameter.Key, workflowParameter.Value);
                flowParameters.Add(flowParameter);
            }
        }

        var instance = (WorkFlow<TStep>)Activator.CreateInstance(workFlowRefereceType, flowParameters);
        return instance;
    }

    private async Task<Case> Create<TStep>(StartWorkFlowDto<TStep> startWorkFlowDto) where TStep : Enum
    {
        var createCaseDto = new CreateCaseDto
                                {
                                    Title = startWorkFlowDto.Title,
                                    WorkFlowTitle = startWorkFlowDto.WorkFlowInstance.Name,
                                    LastStepTitle = startWorkFlowDto.WorkFlowInstance.StartStep.Step.ToString(),
                                    WorkFlowReference = startWorkFlowDto.WorkFlowInstance.GetType().AssemblyQualifiedName,
                                    State = CaseStates.Draft,
                                    FlowStep = Convert.ToInt32(startWorkFlowDto.WorkFlowInstance.StartStep.Step),
                                    CreatorId = startWorkFlowDto.CurrentUserId,
                                    CurrentUserId = startWorkFlowDto.CurrentUserId,
                                    FlowParameters = startWorkFlowDto.FlowParameters
                                };

        var @case = await Create<TStep>(createCaseDto);
        return @case;
    }

    private async Task<Case> Create<TStep>(CreateCaseDto createCaseDto) where TStep : Enum
    {
        var cartableCase = new Case(createCaseDto.Title,
                                    createCaseDto.WorkFlowTitle,
                                    createCaseDto.LastStepTitle,
                                    createCaseDto.WorkFlowReference,
                                    createCaseDto.State,
                                    createCaseDto.FlowStep,
                                    createCaseDto.CreatorId,
                                    createCaseDto.CurrentUserId,
                                    createCaseDto.FlowParameters);

        await caseRepository.Add(cartableCase);
        try {
            await bpmsUnitOfWork.CommitAsync();
        }
        catch (Exception ex) {
            logger.LogError(ex, "RegisterNewRetailerRepository");
            throw;
        }

        return cartableCase;
    }
}
