using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using BPMS.Domain.Model.Cartable;
using BPMS.Domain.Repository;
using BPMS.Infrastructures.DataAccess;
using BPMS.Services.Dto.Cartable;
using BPMS.Services.Dto.WorkFlow;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using BPMS.Domain.Model.Workflow;
using BPMS.Domain.Model.Workflow.AssignmentMethod;
using Common;

namespace BPMS.Services.CartableService
{
    public class CartableService : BaseService //where TStep : Enum
    {
        private readonly ILogger<CartableService> logger;
        private readonly IBpmsUnitOfWork bpmsUnitOfWork;
        private readonly ICaseRepository caseRepository;
        private readonly IUserContext userContext;

        public CartableService(ICaseRepository caseRepository, IBpmsUnitOfWork bpmsUnitOfWork,
            ILogger<CartableService> logger,
            IUserContext userContext
        ) : base(userContext)
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
            var workFlow = GetEventInstance<TStep>(@case.WorkFlowReference, @case, routeVariable);
            var currentStep = workFlow.Next();
            @case.Route<TStep>(currentStep);
            caseRepository.Update(@case);
            try
            {
                bpmsUnitOfWork.Commit();
            }
            catch (Exception ex)
            {
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

        private WorkFlow<TStep> GetEventInstance<TStep>(string workFlowReference, Case @case, RouteVariable routeVariable) where TStep : Enum
        {
            var workflowEnum = Enum.Parse(typeof(TStep), @case.Tracks.Single(a => a.IsLatestTrack).FlowStep.ToString()) as Enum;
            var workFlowRefereceType = Type.GetType(workFlowReference);
            List<IFlowParameter> flowParameters = null;
            if (routeVariable != null && routeVariable.WorkflowParameters != null && routeVariable.WorkflowParameters.Count > 0)
            {
                flowParameters = new List<IFlowParameter>();
                foreach (var workflowParameter in routeVariable.WorkflowParameters)
                {
                    var flowParameter = new FlowParameter(routeVariable.CaseId, workflowParameter.Key, workflowParameter.Value);
                    flowParameters.Add(flowParameter);
                }
            }

            var workflowStep = new WorkflowStep<TStep>((TStep)workflowEnum, new CyclicAssignmentMethod(), new List<Guid>(), string.Empty);
            var instance = (WorkFlow<TStep>)Activator.CreateInstance(workFlowRefereceType, workflowStep, flowParameters);
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
            var cartableCase = new Case(
                createCaseDto.Title,
                createCaseDto.WorkFlowTitle,
                createCaseDto.LastStepTitle,
                createCaseDto.WorkFlowReference,
                createCaseDto.State,
                createCaseDto.FlowStep,
                createCaseDto.CreatorId,
                createCaseDto.CurrentUserId,
                createCaseDto.FlowParameters);

            await caseRepository.Add(cartableCase);
            try
            {
                await bpmsUnitOfWork.CommitAsync();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "RegisterNewRetailerRepository");
                throw;
            }

            return cartableCase;
        }
    }
}