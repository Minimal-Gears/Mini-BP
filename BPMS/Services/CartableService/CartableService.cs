using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using BPMS.Domain.Model.Cartable;
using BPMS.Domain.Repository;
using BPMS.Infrastructures.DataAccess;
using BPMS.Services.Dto.Cartable;
using BPMS.Services.Dto.WorkFlow;
using BPMS.Services.WorkFlow;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using AutoMapper;
using System.Collections.Generic;
using Common;

namespace BPMS.Services.CartableService
{
    public class CartableService<TStep> : BaseService where TStep : Enum
    {
        private readonly ILogger<CartableService<TStep>> logger;
        private readonly IUnitOfWork unitOfWork;
        private readonly ICaseRepository caseRepository;
        private readonly IMapper mapper;
        private readonly IUserContext userContext;

        public CartableService(ICaseRepository caseRepository, IUnitOfWork unitOfWork,
            ILogger<CartableService<TStep>> logger,
         Common.IUserContext userContext
        )
        {
            this.caseRepository = caseRepository;
            this.unitOfWork = unitOfWork;
            this.logger = logger;
            this.userContext = userContext;
        }

        public async Task<Case> Start(StartWorkFlowDto<TStep> startWorkFlowDto)
        {
            //Enum test = Enum.Parse(typeof(TStep), startWorkFlowDto.WorkFlowInstance.StartStep.ToString()) as Enum;
            var @case = await Create(startWorkFlowDto);
            return @case;
        }

        public async Task Route(int caseId)
        {
            var @case = await caseRepository.GetById(@caseId);
            Route(@case);
        }

        public void Route(Case @case)
        {
            var workflowEnum = Enum.Parse(typeof(TStep), @case.Tracks.LastOrDefault().FlowStep.ToString()) as Enum;
            var workFlow = GetEventInstance(@case.WorkFlowReference, workflowEnum);
            var currentStep = workFlow.Next();
            @case.Route(Convert.ToInt32(currentStep.Step), "ddd", currentStep.SelectedUser);
        }

        public async Task<(IEnumerable<CartableDto>, int totalCount)> GetByUser(int pageNumber, int pageSize)
        {
            var userId = userContext.CurrentUser.UserId;
            var query = caseRepository.Get().Where(a => a.CreatorId == userId);
            var cartables = await query.OrderBy(t => t.Id).Page(pageNumber, pageSize).ToListAsync();
            var cartableDtos = cartables.Select(a => new CartableDto
            {
                Id = a.Id,
                CreatorId = a.CreatorId,
                LastStepTitle = a.LastStepTitle,
                State = a.State,
                StateTitle = ((Domain.Model.Cartable.CaseStates)a.State).ToString(),
                Title = a.Title,
                WorkFlowTitle = a.WorkFlowTitle,
                WorkFlowReference = a.WorkFlowReference
            });

            return (cartableDtos, query.Count());
        }

        private WorkFlow<TStep> GetEventInstance(string workFlowReference, Enum lastStep)
        {
            var workFlowRefereceType = Type.GetType(workFlowReference);
            var instance = (WorkFlow<TStep>)Activator.CreateInstance(workFlowRefereceType, lastStep);
            return instance;
        }

        private async Task<Case> Create(StartWorkFlowDto<TStep> startWorkFlowDto)
        {
            var createCaseDto = new CreateCaseDto
            {
                Title = startWorkFlowDto.Title,
                WorkFlowTitle = startWorkFlowDto.WorkFlowInstance.Name,
                LastStepTitle = startWorkFlowDto.WorkFlowInstance.StartStep.ToString(),
                WorkFlowReference = startWorkFlowDto.WorkFlowInstance.GetType().AssemblyQualifiedName,
                State = CaseStates.Draft,
                FlowStep = Convert.ToInt32(startWorkFlowDto.WorkFlowInstance.StartStep),
                CreatorId = startWorkFlowDto.CurrentUserId,
                CurrentUserId = startWorkFlowDto.CurrentUserId,
                FlowParameters = startWorkFlowDto.FlowParameters
            };

            var @case = await Create(createCaseDto);
            return @case;
        }

        private async Task<Case> Create(CreateCaseDto createCaseDto)
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
                await unitOfWork.CommitAsync();
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