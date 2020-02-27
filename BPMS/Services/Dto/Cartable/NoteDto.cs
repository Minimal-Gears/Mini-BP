using System;

namespace BPMS.Services.Dto.Cartable
{
    public class NoteDto
    {
        public int Id { get; set; }

        public int CaseId { get; set; }

        public Guid? CreatorId { get; set; }

        public string Text { get; set; }
    }
}