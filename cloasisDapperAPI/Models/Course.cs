using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace cloasisDapperAPI.Models
{
    public class Course
    {
        public int Course_Id { get; set; }
        public string Course_Name { get; set; }
        public string Course_Code { get; set; }
        public string Description { get; set; }
        public int Credits { get; set; }

    }
}
