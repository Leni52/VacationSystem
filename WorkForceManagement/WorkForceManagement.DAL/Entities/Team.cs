using System.Collections.Generic;

namespace WorkForceManagement.DAL.Entities
{
    public class Team : Base
    {
        public string Name { get; set; }
        public virtual User TeamLeader { get; set; }
        public virtual List<User> Members { get; set; }
        public string Description { get; set; }
    }
}
