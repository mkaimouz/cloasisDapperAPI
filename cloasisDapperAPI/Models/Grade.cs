using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace cloasisDapperAPI.Models
{
    public class Grade
    {
        public int Grade_Id { get; set; }
        public int Report_Id { get; set; }
        public string StudentId { get; set; }
        public float Grade_Value { get; set; }
        public string Grade_Desc { get; set; }
    }
}
