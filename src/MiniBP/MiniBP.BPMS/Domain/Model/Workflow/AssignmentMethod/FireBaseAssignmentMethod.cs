namespace MiniBP.BPMS.Domain.Model.Workflow.AssignmentMethod;

public class FireBaseAssignmentMethod : IAssignmentMethod
{
    public AssignmentMethodType AssignmentMethodType => AssignmentMethodType.FireBase;

    public Guid SelectedUser(IList<Guid> users)
    {
        return users.First();
    }
}
