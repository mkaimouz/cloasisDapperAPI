using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace cloasisDapperAPI.Models
{
    public class Professor
    {
        public int professor_Id { get; set; }
        public string name { get; set; }
        public string email { get; set; }
        public string gender { get; set; }
        public string office { get; set; }
        public string extension { get; set; }
        public string? imagePath { get; set; }

    }
}
