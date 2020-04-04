using System;
using System.Collections.Generic;
using BPMS.Domain.Model.Cartable;

namespace BPMS.Services.Dto.Cartable
{
    public class CreateCaseDto
    {
        public string Title{ get;  set; }
        
        public string WorkFlowTitle{ get;  set; }
        
        public string LastStepTitle{ get;  set; }
      
        public string WorkFlowReference{ get;  set; }
        
        public CaseStates State{ get;  set; }

        public int FlowStep { get; set; }
        
        public Guid? CreatorId { get;  set; }
       
        public Guid CurrentUserId{ get;  set; }
        
        public Dictionary<string, string> FlowParameters { get;  set; }
    }
}