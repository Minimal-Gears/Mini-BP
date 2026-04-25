using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MiniBP.BPMS.Domain.Model.Workflow;
using Stateless;

namespace MiniBP.BPMS.Domain.Model.Cartable;

public class Case 
{
    public int Id { get; private set; }

    [StringLength(50), Column(TypeName = "VARCHAR(50)")]
    public string Title { get; private set; }

    [StringLength(50), Column(TypeName = "VARCHAR(50)")]
    public string WorkFlowTitle { get; private set; }
    
    [StringLength(50), Column(TypeName = "VARCHAR(50)")]
    public string LastStepTitle { get; private set; }

    public string WorkFlowReference { get; private set; }

    public Guid? CreatorId { get; private set; }

    public List<Duty> Tracks { get; set; }

    public List<FlowParameter> FlowParameters { get; set; }

    public List<Note> Notes { get; private set; }
}