using System;
using System.Collections.Generic;
using BPMS.Services.WorkFlow.AssignmentMethod;

namespace BPMS.Services.WorkFlow
{
    public  class AssignmentRule<T> where T:Enum
    {
        public IAssignmentMethod AssignmentMethod { get; set; }

        public List<Guid> Users { get; set; }

        public T Step { get; set; }

        public Guid SelectedUser => AssignmentMethod.SelectedUser(Users);
    }
}