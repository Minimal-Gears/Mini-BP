namespace MiniBP.BPMS.Domain.Model.Workflow.AssignmentMethod;

public class ValueBaseAssignmentMethod : IAssignmentMethod
{
    public AssignmentMethodType AssignmentMethodType => AssignmentMethodType.ValueBase;

    public Guid SelectedUser(IList<Guid> users)
    {
        return users.First();
    }
}