using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Stateless;

namespace BPMS.Domain.Model.Cartable
{
    public class Case : StateMachine<CaseStates, CartableTiggers>, IEntity
    {
        public Case(string title,
            string workFlowTitle,
            string lastStepTitle,
            string workFlowReference,
            CaseStates state,
            int flowStep,
            Guid? creatorId,
            Guid currentUserId,
            Dictionary<string, string> flowParameters
        ) : this(state)
        {
            Title = title;
            WorkFlowTitle = workFlowTitle;
            LastStepTitle = lastStepTitle;
            WorkFlowReference = workFlowReference;
            CreatorId = creatorId;
            FlowParameters=new List<FlowParameter>();
            Tracks=new List<CaseTracker>();
            AddFlowParameters(flowParameters);
            AddCaseTrack(
                Id,
                title,
                lastStepTitle,
                CaseStates.Draft,
                flowStep,
                currentUserId,
                DateTime.Now,
                1,
                true);
        }

        public Case(CaseStates state = CaseStates.Draft) : base(state)
        {
            this.Configure(CaseStates.Draft)
                .Permit(CartableTiggers.Route, CaseStates.ToDo);

            this.Configure(CaseStates.ToDo)
                .OnEntry(() => Tracks.First()) //.Duty.OnEntry())
                .OnExit(() => Tracks.First()) //.Duty.OnExit())
                .PermitReentryIf(CartableTiggers.Route,
                    () => true) // !Tracks.First().Duty.IsFinalTask && Tracks.First().Duty.BeforeExit())
                .PermitIf(CartableTiggers.Route, CaseStates.Done,
                    () => true) //() => Tracks.First().Duty.IsFinalTask && Tracks.First().Duty.BeforeExit())
                .Permit(CartableTiggers.Cancel, CaseStates.Canceled)
                .Permit(CartableTiggers.Pause, CaseStates.Paused)
                .PermitReentry(CartableTiggers.Reassign);

            this.Configure(CaseStates.Paused)
                .Permit(CartableTiggers.Restart, CaseStates.ToDo)
                .Permit(CartableTiggers.Cancel, CaseStates.Canceled);
        }

        public int Id { get; private set; }

        [StringLength(50), Column(TypeName = "VARCHAR(50)")]
        public string Title { get; private set; }

        [StringLength(50), Column(TypeName = "VARCHAR(50)")]
        public string WorkFlowTitle { get; private set; }
        
       // public CaseStates State { get; private set; }
        
        [StringLength(50), Column(TypeName = "VARCHAR(50)")]
        public string LastStepTitle { get; private set; }
        
        public string WorkFlowReference { get; private set; }

        public Guid? CreatorId { get; private set; }

        public List<CaseTracker> Tracks { get;  set; }

        public List<FlowParameter> FlowParameters { get;  set; }

        public List<Note> Notes { get; private set; }

        

        #region Methodes

        
        
        public void Route(int currentStep,string stepTitle,Guid currentUserId)
        {
            Fire(CartableTiggers.Route);
           
            AddCaseTrack(
             Id,
             Title,
             stepTitle,
             State,
             currentStep,
             currentUserId, 
             DateTime.Now,
             1,
             true);
        }

        public void Cancel()
        {
        }

        public void Pause()
        {
        }

        public void Reassign()
        {
        }

        public void Restart()
        {
        }

        private void AddFlowParameters(Dictionary<string, string> flowParameters)
        {
            foreach (var parameter in flowParameters)
            {
                var flowParameter = new FlowParameter(Id, parameter.Key, parameter.Value);
                FlowParameters.Add(flowParameter);
            }
        }

        private void AddCaseTrack(
            int caseId,
            string title,
            string stepTitle,
            CaseStates state,
            int flowStep,
            Guid currentUserId,
            DateTime dueDate,
            int priority,
            bool isLatestTrack)
        {
            var caseTrack = new CaseTracker(
                caseId,
                title,
                stepTitle,
                state,
                flowStep,
                currentUserId,
                dueDate,
                priority,
                isLatestTrack
            );
            Tracks.Add(caseTrack);
        }

        
        #endregion
    }
}