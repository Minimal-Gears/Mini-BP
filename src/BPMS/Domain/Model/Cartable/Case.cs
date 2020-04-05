using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using BPMS.Domain.Model.Workflow;
using BPMS.Services.CartableService;
using Stateless;

namespace BPMS.Domain.Model.Cartable
{
    public class Case : StateMachine<CaseStates, CartableTiggers>, IEntity// where TStep : Enum
    {
        public Case(string title,
        string workFlowTitle,
        string lastStepTitle,
        string workFlowReference,
        CaseStates state,
        int flowStep,
        Guid? creatorId,
        Guid currentUserId,
        Dictionary<string, string> flowParameters) : this(state)
        {
            Title = title;
            WorkFlowTitle = workFlowTitle;
            LastStepTitle = lastStepTitle;
            WorkFlowReference = workFlowReference;
            CreatorId = creatorId;
            FlowParameters = new List<FlowParameter>();
            Tracks = new List<CaseTracker>();
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
                true,
                string.Empty,
                DateTime.Now);
        }

        public Case(CaseStates state = CaseStates.Draft) : base(state)
        {
            this.Configure(CaseStates.Draft)
                .Permit(CartableTiggers.Route, CaseStates.ToDo);

            this.Configure(CaseStates.ToDo)
            // .OnEntry(() => Tracks.First()) //.Duty.OnEntry())
            //.OnExit(() => Tracks.First()) //.Duty.OnExit())
            .PermitReentryIf(CartableTiggers.Route, () => !IsCurrentStateFinalState)
            // ,() => !IsReadyToDone()
            //     ) // !Tracks.First().Duty.IsFinalTask && Tracks.First().Duty.BeforeExit())
            .PermitIf(CartableTiggers.Route, CaseStates.Done, () => IsCurrentStateFinalState);
            // ) //() => Tracks.First().Duty.IsFinalTask && Tracks.First().Duty.BeforeExit())
            // .Permit(CartableTiggers.Cancel, CaseStates.Canceled)
            // .Permit(CartableTiggers.Pause, CaseStates.Paused)
            // .PermitReentry(CartableTiggers.Reassign);


            this.Configure(CaseStates.Paused)
                .Permit(CartableTiggers.Restart, CaseStates.ToDo)
                .Permit(CartableTiggers.Cancel, CaseStates.Canceled);
        }

        //  public WorkflowStep<TStep> CurrentStep { get; set; }
        [NotMapped]
        public bool IsCurrentStateFinalState { get; private set; }

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

        public List<CaseTracker> Tracks { get; set; }

        public List<FlowParameter> FlowParameters { get; set; }

        public List<Note> Notes { get; private set; }


        public void Route<TStep>(WorkflowStep<TStep> wfs) where TStep : Enum
        {
            SetIsCurrentStateFinalState(wfs.IsFinal);
            Fire(CartableTiggers.Route);
            SetAreNotLatestTracks();
            LastStepTitle = wfs.Step.ToString();
            AddCaseTrack(
             Id,
             Title,
             wfs.Step.ToString(),
             State,
             Convert.ToInt32(wfs.Step),
             wfs.SelectedUser,
             DateTime.Now,
             1,
             true,
             wfs.Url,
             DateTime.Now);
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

        private void SetIsCurrentStateFinalState(bool isCurrentStateFinalState)
        {
            IsCurrentStateFinalState = isCurrentStateFinalState;
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
            bool isLatestTrack,
            string url,
            DateTime creationDate)
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
                isLatestTrack,
                url,
                creationDate);
            Tracks.Add(caseTrack);
        }

        private void SetAreNotLatestTracks()
        {
            foreach (var track in Tracks)
            {
                track.IsNotLatestTrack();
            }
        }

        // private bool IsReadyToDone<TStep>() where TStep : Enum
        // {
        //     var workflowEnum = Enum.Parse(typeof(TStep), Tracks.Single(a => a.IsLatestTrack).FlowStep.ToString()) as Enum;
        //     var workFlow = cartableService.GetEventInstance<TStep>(WorkFlowReference, workflowEnum);
        //     return true;// workFlow.IsFinalStep;
        // }
    }
}