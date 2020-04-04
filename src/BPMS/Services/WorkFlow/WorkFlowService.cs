// using System;
// using BPMS.Domain.Model.Cartable;
// using BPMS.Services.Dto.Cartable;
// using BPMS.Services.Dto.WorkFlow;
//
// namespace BPMS.Services.WorkFlow
// {
//     public abstract class WorkFlowService<TStep>:BaseService  where TStep : Enum
//     {
//         private readonly CartableService.CartableService cartableService;
//
//         protected WorkFlowService(CartableService.CartableService cartableService)
//         {
//             this.cartableService = cartableService;
//         }
//
//         public void Start(StartWorkFlowDto<TStep> startWorkFlowDto)
//         { 
//             Enum test = Enum.Parse(typeof(TStep), startWorkFlowDto.WorkFlowInstance.StartStep.ToString()) as Enum;
//             //startWorkFlowDto.WorkFlowInstance.Next();
//             var createCaseDto = new CreateCaseDto
//             {
//                 Title = startWorkFlowDto.Title,
//                 WorkFlowTitle = startWorkFlowDto.WorkFlowInstance.Name,
//                 LastStepTitle = startWorkFlowDto.WorkFlowInstance.StartStep.ToString(),
//                 WorkFlowReference = startWorkFlowDto.WorkFlowInstance.GetType().FullName,
//                 State = CaseStates.Draft,
//                 FlowStep =Convert.ToInt32( startWorkFlowDto.WorkFlowInstance.StartStep),
//                 CreatorId = startWorkFlowDto.CurrentUserId,
//                 CurrentUserId =startWorkFlowDto.CurrentUserId,
//                 FlowParameters = startWorkFlowDto.FlowParameters
//             };
//             
//             cartableService.Create(createCaseDto);
//             
//         }
//
//         public void Next()
//         {
//             
//         }
//     }
// }