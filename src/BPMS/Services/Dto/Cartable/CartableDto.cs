using System;
using System.Collections.Generic;
using System.Linq;
using BPMS.Domain.Model.Cartable;

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

        public int EntityId { get; set; }

        public CaseTrackerDto LastTrack { get; set; }

        public List<CaseTrackerDto> Tracks { get; set; }

        public List<FlowParameterDto> FlowParameters { get; set; }

        public List<NoteDto> Notes { get; set; }

        public static CartableDto ConvertToDto(Case cartable)
        {
            var lastTrack = cartable.Tracks.First(a => a.IsLatestTrack);
            return new CartableDto
            {
                Id = cartable.Id,
                CreatorId = cartable.CreatorId,
                LastStepTitle = cartable.LastStepTitle,
                State = cartable.State,
                StateTitle = ((Domain.Model.Cartable.CaseStates)cartable.State).ToString(),
                Title = cartable.Title,
                WorkFlowTitle = cartable.WorkFlowTitle,
                WorkFlowReference = cartable.WorkFlowReference,
                LastTrack = new CaseTrackerDto
                {
                    Id = lastTrack.Id,
                    CaseId = lastTrack.CaseId,
                    CurrentUserId = lastTrack.CurrentUserId,
                    PreviousUserId = lastTrack.PreviousUserId,
                    DueDate = lastTrack.DueDate,
                    FlowStep = lastTrack.FlowStep,
                    StepTitle = lastTrack.StepTitle,
                    State = lastTrack.State,
                    IsLatestTrack = lastTrack.IsLatestTrack,
                    Priority = lastTrack.Priority,
                    Url = lastTrack.Url,
                    CreationDate = lastTrack.CreationDate
                },

                EntityId = Convert.ToInt32(cartable.FlowParameters.First(a => a.Key == "EntityId").Value)
            };
        }
    }
}