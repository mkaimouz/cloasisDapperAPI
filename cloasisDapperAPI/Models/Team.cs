using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace cloasisDapperAPI.Models
{
    public class Team
    {

        public int TEAM_ID { get; set; }
        public string? CRN { get; set; }
        public string TEAM_NAME { get; set; }
        public DateTime? CREATION_DATE { get; set; }

    }
}
