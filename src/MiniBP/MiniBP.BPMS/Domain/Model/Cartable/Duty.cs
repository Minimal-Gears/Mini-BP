using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MiniBP.BPMS.Domain.Model.Cartable;

public class Duty : IEntity
{
    public int Id { get; private set; }

    public int CaseId { get; private set; }

    public Case Case { get; private set; }

    [StringLength(50), Column(TypeName = "VARCHAR(50)")]
    public string StepTitle { get; private set; }
}