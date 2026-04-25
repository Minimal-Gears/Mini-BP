namespace MiniBP.BPMS.Abstractions;

public interface IAssignmentRule
{
    string? ResolveAssignee(AssignmentContext context);
}

public static class AssignmentRules
{
    public static IAssignmentRule Claiming() => new DelegateAssignmentRule(_ => null);

    public static IAssignmentRule Fixed(string userId) => new DelegateAssignmentRule(_ => userId);

    public static IAssignmentRule Conditional(Func<AssignmentContext, string?> resolver) => new DelegateAssignmentRule(resolver);

    public static IAssignmentRule Cyclic(params string[] userIds) => new CyclicAssignmentRule(userIds);

    private sealed class DelegateAssignmentRule(Func<AssignmentContext, string?> resolver) : IAssignmentRule
    {
        public string? ResolveAssignee(AssignmentContext context) => resolver(context);
    }

    private sealed class CyclicAssignmentRule(IReadOnlyList<string> userIds) : IAssignmentRule
    {
        private int _index = -1;

        public string? ResolveAssignee(AssignmentContext context)
        {
            if (userIds.Count == 0) {
                return null;
            }

            var next = Interlocked.Increment(ref _index);
            return userIds[next % userIds.Count];
        }
    }
}
