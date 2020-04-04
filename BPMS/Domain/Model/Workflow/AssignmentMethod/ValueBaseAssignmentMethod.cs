using System;
using System.Collections.Generic;
using System.Linq;

namespace BPMS.Domain.Model.Workflow.AssignmentMethod
{
    public class ValueBaseAssignmentMethod : IAssignmentMethod
    {
        public ValueBaseAssignmentMethod(List<IFlowParameter> flowParameters, string parameterName)
        {
            FlowParameters = flowParameters;
            ParameterName = parameterName;
        }

        public AssignmentMethodType AssignmentMethodType => AssignmentMethodType.ValueBase;

        public List<IFlowParameter> FlowParameters { get; }
        
        public string ParameterName { get; }

        public Guid SelectedUser(IList<Guid> users)
        {
            return users.First();
        }
    }
}