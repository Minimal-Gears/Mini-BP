using System;
using System.Collections.Generic;
using System.Linq;

namespace BPMS.Domain.Model.Workflow.AssignmentMethod
{
    public class CyclicAssignmentMethod:IAssignmentMethod
    {
        public AssignmentMethodType AssignmentMethodType => AssignmentMethodType.Cyclic;
        public Guid SelectedUser(IList<Guid> users)
        {
            return users.First();
        }
    }
}