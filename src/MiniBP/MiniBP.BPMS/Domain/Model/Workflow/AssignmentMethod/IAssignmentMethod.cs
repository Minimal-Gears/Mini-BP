namespace MiniBP.BPMS.Domain.Model.Workflow.AssignmentMethod
{
    public interface IAssignmentMethod
    {
        AssignmentMethodType AssignmentMethodType { get; }

        Guid SelectedUser(IList<Guid> users);
    }
}