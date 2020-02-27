using System;
using System.Collections.Generic;

namespace BPMS.Services.WorkFlow.AssignmentMethod
{
    public interface IAssignmentMethod
    {
        AssignmentMethodType AssignmentMethodType { get; }

        Guid SelectedUser(IList<Guid> users);
    }
}