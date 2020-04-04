using System;

namespace BPMS.Domain.Model.Cartable
{
    public class Note : IEntity
    {
        public Note(int caseId, Guid? creatorId, string text)
        {
            CaseId = caseId;
            CreatorId = creatorId;
            Text = text;
        }

        public int Id { get; private set; }

        public int CaseId { get; private set; }

        public Case Case { get; private set; }

        public Guid? CreatorId { get; private set; }

        public string Text { get; private set; }
    }
}