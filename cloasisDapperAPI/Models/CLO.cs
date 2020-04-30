using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace cloasisDapperAPI.Models
{
    public class CLO
    {
        public int CLO_ID { get; set; }
        public int COURSE_ID { get; set; }
        public int? REPORT_ID { get; set; }
        public string DESCRIPTION { get; set; }
    }
}
