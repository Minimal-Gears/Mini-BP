using System;
using System.Collections.Generic;
using System.Linq;

namespace BPMS.Services.WorkFlow.AssignmentMethod
{
    public class SelfServiceValueBaseAssignmentMethod:IAssignmentMethod
    {
        public AssignmentMethodType AssignmentMethodType => AssignmentMethodType.SelfServiceValueBase;
        public Guid SelectedUser(IList<Guid> users)
        {
            return users.First();
        }
    }
}