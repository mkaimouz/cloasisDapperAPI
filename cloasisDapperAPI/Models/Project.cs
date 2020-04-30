using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace cloasisDapperAPI.Models
{
    public class Project
    {
        public int Project_ID { get; set; }
        public string? Team { get; set; }
        public string Project_Title { get; set; }
        public string Project_Desc { get; set; }
    }
}
