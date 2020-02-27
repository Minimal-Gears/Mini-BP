using System;
using System.Collections.Generic;
using System.Linq;

namespace BPMS.Services.WorkFlow.AssignmentMethod
{
    public class SelfServiceAssignmentMethod:IAssignmentMethod
    {
        public AssignmentMethodType AssignmentMethodType => AssignmentMethodType.SelfService;
        public Guid SelectedUser(IList<Guid> users)
        {
            return users.First();
        }
    }
}