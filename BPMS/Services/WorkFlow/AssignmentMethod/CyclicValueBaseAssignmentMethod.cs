using System;
using System.Collections.Generic;
using System.Linq;

namespace BPMS.Services.WorkFlow.AssignmentMethod
{
    public class CyclicValueBaseAssignmentMethod:IAssignmentMethod
    {
        public AssignmentMethodType AssignmentMethodType => AssignmentMethodType.CyclicValueBase;
        
        public Guid SelectedUser(IList<Guid> users)
        {
            return users.First();
        }
    }
}