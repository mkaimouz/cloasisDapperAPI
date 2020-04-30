using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace cloasisDapperAPI.Models
{
    public class Student
    {
        public string STUDENTID { get; set; }
        public int? TEAM_ID { get; set; }
        public string NAME { get; set; }
        public string EMAIL { get; set; }
        public string PHONE { get; set; }
        public DateTime DOB { get; set; }
        public string GENDER { get; set; }

    }
}
