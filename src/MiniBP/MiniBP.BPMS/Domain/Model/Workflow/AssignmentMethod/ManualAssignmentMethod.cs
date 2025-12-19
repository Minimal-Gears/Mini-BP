namespace MiniBP.BPMS.Domain.Model.Workflow.AssignmentMethod
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