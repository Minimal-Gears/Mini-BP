using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BPMS.Domain.Model.Cartable
{
    public class CaseTracker : IEntity
    {
        public CaseTracker(
            int caseId,
            string title,
            string stepTitle,
            CaseStates state,
            int flowStep,
            Guid currentUserId,
            DateTime dueDate,
            int priority,
            bool isLatestTrack,
            string url,
            DateTime creationDate)
        {
            CaseId = caseId;
            StepTitle = stepTitle;
            State = state;
            FlowStep = flowStep;
            CurrentUserId = currentUserId;
            DueDate = dueDate;
            Priority = priority;
            IsLatestTrack = isLatestTrack;
            Url = url;
            CreationDate = creationDate;
        }

        private CaseTracker()
        {

        }

        public int Id { get; private set; }

        public int CaseId { get; private set; }

        public Case Case { get; private set; }

        [StringLength(50), Column(TypeName = "VARCHAR(50)")]
        public string StepTitle { get; private set; }

        public CaseStates State { get; private set; }

        public int FlowStep { get; private set; }

        public Guid CurrentUserId { get; private set; }

        public Guid? PreviousUserId { get; private set; }

        public DateTime CreationDate { get; private set; }

        public DateTime DueDate { get; private set; }

        public int Priority { get; private set; }

        public bool IsLatestTrack { get; private set; }

        public string Url { get; private set; }

        public void IsNotLatestTrack() => IsLatestTrack = false;
    }
}