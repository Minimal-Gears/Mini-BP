using System;
using System.Collections.Generic;
using BPMS.Domain.Model.Workflow.AssignmentMethod;

namespace BPMS.Domain.Model.Workflow
{
    public class WorkflowStep<TStep> where TStep : Enum
    {
        public WorkflowStep(TStep step, IAssignmentMethod assignmentMethod, List<Guid> users, string url, bool isFinal = false)
        {
            AssignmentMethod = assignmentMethod;
            Users = users;
            Step = step;
            Url = url;
            IsFinal = isFinal;
        }

        public IAssignmentMethod AssignmentMethod { get; private set; }

        public List<Guid> Users { get; private set; }

        public string Url { get; private set; }

        public Guid SelectedUser => AssignmentMethod.SelectedUser(Users);

        public TStep Step { get; set; }

        public bool IsFinal { get; }


        public override bool Equals(object obj)
        {
            var item = obj as WorkflowStep<TStep>;

            if (item == null)
            {
                return false;
            }

            return this.Step.Equals(item.Step) && this.IsFinal.Equals(item.IsFinal);
        }


        public override int GetHashCode()
        {
            return this.Step.GetHashCode();
        }
    }
}