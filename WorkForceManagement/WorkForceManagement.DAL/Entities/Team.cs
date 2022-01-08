using System.Collections.Generic;

namespace WorkForceManagement.DAL.Entities
{
    public class Team : Base
    {
        public string Name { get; set; }
        public User TeamLeader { get; set; }
        public virtual List<User> Members { get; set; } = new List<User>();
        public string Description { get; set; }
    }
}
