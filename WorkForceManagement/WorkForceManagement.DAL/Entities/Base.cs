using System;

namespace WorkForceManagement.DAL.Entities
{
    public abstract class Base
    {
        public Guid Id { get; set; }

        public DateTime CreationDate { get; set; } = DateTime.Now;

        public string CreatorId { get; set; }

        public DateTime ChangeDate { get; set; } = DateTime.Now;

        public string UpdaterId { get; set; }
    }
}
