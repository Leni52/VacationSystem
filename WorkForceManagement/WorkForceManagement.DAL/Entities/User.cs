using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace WorkForceManagement.DAL.Entities
{
    public class User : IdentityUser
    {
        public virtual ICollection<Team> Teams { get; set; } = new List<Team>();
        public DateTime CreationDate { get; set; } = DateTime.Now;
        public string CreatorId { get; set; }
        public DateTime ChangeDate { get; set; } = DateTime.Now;
        public string UpdaterId { get; set; }
    }
}
