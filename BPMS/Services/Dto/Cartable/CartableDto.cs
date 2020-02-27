using System;
using System.Collections.Generic;

namespace BPMS.Services.Dto.Cartable
{
    public class CartableDto
    {
        public int Id { get; set; }


        public string Title { get; set; }

        public string WorkFlowTitle { get; set; }

        public Domain.Model.Cartable.CaseStates State { get; set; }

        public string StateTitle { get; set; }

        public string LastStepTitle { get; set; }

        public string WorkFlowReference { get; set; }

        public Guid? CreatorId { get; set; }

        public List<CaseTrackerDto> Tracks { get; set; }

        public List<FlowParameterDto> FlowParameters { get; set; }

        public List<NoteDto> Notes { get; set; }
    }
}