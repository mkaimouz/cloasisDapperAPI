using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace cloasisDapperAPI.Models
{
    public class Lecture
    {
        public int Lecture_Id { get; set; }
        public string CRN { get; set; }
        public float Duration { get; set; }
        public string Room { get; set; }
        public DateTime Start_Time { get; set; }
        public string Title { get; set; }
        public DateTime Lecture_Date { get; set; }
    }
}
