using System;
using BPMS.Domain.Model.Cartable;

namespace BPMS.Services.Dto.Cartable
{
    public class CaseTrackerDto
    {
        public int Id { get; set; }

        public int CaseId { get; set; }

        public string StepTitle { get; set; }

        public CaseStates State { get; set; }

        public int FlowStep { get; set; }

        public Guid CurrentUserId { get; set; }


        public Guid? PreviousUserId { get; set; }

        public DateTime DueDate { get; set; }

        public int Priority { get; set; }

        public bool IsLatestTrack { get; set; }
    }
}