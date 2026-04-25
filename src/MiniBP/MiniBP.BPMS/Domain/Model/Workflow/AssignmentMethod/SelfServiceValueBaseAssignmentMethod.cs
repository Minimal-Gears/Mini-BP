namespace MiniBP.BPMS.Domain.Model.Workflow.AssignmentMethod;

public class SelfServiceValueBaseAssignmentMethod:IAssignmentMethod
{
    public AssignmentMethodType AssignmentMethodType => AssignmentMethodType.SelfServiceValueBase;
    
    public Guid SelectedUser(IList<Guid> users)
    {
        return users.First();
    }
}