using System;
using System.ComponentModel.DataAnnotations;

namespace BPMS.Domain.Model
{
    public interface IEntity
    {
        [Key]
        int Id { get; }
    }


    public interface IGetAudit
    {
        DateTime CreatedOn { get; }
        DateTime ModifiedOn { get; }
    }


//    public interface IHaveNotes
//    {
//        IEnumerable<Note> Notes { get; }
//    }

   
}