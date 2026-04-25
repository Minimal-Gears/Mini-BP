namespace MiniBP.BPMS.Domain.Model.Workflow.AssignmentMethod;

public class CyclicValueBaseAssignmentMethod:IAssignmentMethod
{
    public AssignmentMethodType AssignmentMethodType => AssignmentMethodType.CyclicValueBase;
        
    public Guid SelectedUser(IList<Guid> users)
    {
        return users.First();
    }
}