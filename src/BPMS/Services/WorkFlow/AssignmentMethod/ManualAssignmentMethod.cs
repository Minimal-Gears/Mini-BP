using System;
using System.Collections.Generic;
using System.Linq;

namespace BPMS.Services.WorkFlow.AssignmentMethod
{
    public class ManualAssignmentMethod:IAssignmentMethod
    {
        public AssignmentMethodType AssignmentMethodType => AssignmentMethodType.Manual;
        public Guid SelectedUser(IList<Guid> users)
        {
            return users.First();
        }
    }
}