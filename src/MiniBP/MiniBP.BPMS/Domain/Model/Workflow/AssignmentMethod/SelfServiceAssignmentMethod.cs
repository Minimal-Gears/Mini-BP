namespace MiniBP.BPMS.Domain.Model.Workflow.AssignmentMethod
{
    public class SelfServiceAssignmentMethod:IAssignmentMethod
    {
        public AssignmentMethodType AssignmentMethodType => AssignmentMethodType.SelfService;
        public Guid SelectedUser(IList<Guid> users)
        {
            return users.First();
        }
    }
}