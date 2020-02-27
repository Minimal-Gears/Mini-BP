using System;
using System.Collections.Generic;
using System.Linq;

namespace BPMS.Services.WorkFlow.AssignmentMethod
{
    public class ValueBaseAssignmentMethod:IAssignmentMethod
    {
        public AssignmentMethodType AssignmentMethodType => AssignmentMethodType.ValueBase;
        public Guid SelectedUser(IList<Guid> users)
        {
            return users.First();
        }
    }
}