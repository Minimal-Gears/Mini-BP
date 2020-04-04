using System;
using System.Collections.Generic;

namespace BPMS.Domain.Model.Workflow.AssignmentMethod
{
    public interface IAssignmentMethod
    {
        AssignmentMethodType AssignmentMethodType { get; }

        Guid SelectedUser(IList<Guid> users);
    }
}