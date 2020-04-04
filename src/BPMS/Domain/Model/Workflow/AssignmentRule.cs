using System;
using System.Collections.Generic;
using BPMS.Domain.Model.Workflow.AssignmentMethod;

namespace BPMS.Domain.Model.Workflow
{
    public class AssignmentRule<T> where T : Enum
    {
        public AssignmentRule(IAssignmentMethod assignmentMethod, List<Guid> users, T step, string url)
        {
            AssignmentMethod = assignmentMethod;
            Users = users;
            Step = step;
            Url = url;
        }
        public IAssignmentMethod AssignmentMethod { get; private set; }

        public List<Guid> Users { get; private set; }

        public T Step { get; private set; }

        public string Url { get; private set; }

        public Guid SelectedUser => AssignmentMethod.SelectedUser(Users);
    }
}