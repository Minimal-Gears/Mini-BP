using System;
using System.Collections.Generic;
using BPMS.Services.WorkFlow;

namespace BPMS.Services.Dto.WorkFlow
{
    public class StartWorkFlowDto<TStep> where TStep:Enum
    {
        public StartWorkFlowDto(WorkFlow<TStep> workFlowInstance, string title, Guid currentUserId, Dictionary<string, string> flowParameters)
        {
            WorkFlowInstance = workFlowInstance;
            Title = title;
            CurrentUserId = currentUserId;
            FlowParameters = flowParameters;
        }

        public WorkFlow<TStep> WorkFlowInstance { get; private set; }

        public string Title { get; private set; }
        
        public  Guid CurrentUserId { get; private set; }
        
        public Dictionary<string,string> FlowParameters { get; private set; }

    }
}