using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using cloasisDapperAPI.Models;
using Dapper;
using Microsoft.AspNetCore.Mvc;

namespace cloasisDapperAPI.Controllers
{
    public class DeliverController : Controller
    {

        public IDbConnection cloasisdbRef = HomeController.cloasisdb;

        //Random r = new Random();

        //[HttpGet("[controller]/FeedData", Name = "FeedData")]
        //public IActionResult FeedData()
        //{
        //    List<Lecture> lectures = cloasisdbRef.Query<Lecture>("SELECT * FROM LECTURE").ToList();
        //    int i = 0;

        //    foreach (Lecture lecture in lectures)
        //    {
        //        string class_crn = lecture.CRN;

        //        var p = new DynamicParameters();
        //        p.Add(@"class_crn", class_crn);

        //        int course_id = cloasisdbRef.Query<int>("SELECT COURSE_ID FROM CLASS WHERE CRN = @class_crn", p).ToList()[0];

        //        p.Add(@"crsId", course_id);

        //        List<CLO> CLOsOfCourse = cloasisdbRef.Query<CLO>("SELECT * FROM CLO WHERE COURSE_ID = @crsId", p).ToList();

        //        if (lecture.Title == "Lecture No. 1" || lecture.Title == "Lecture No. 2" || lecture.Title == "Lecture No. 3")
        //        {
        //            var x = new
        //            {
        //                lecId = lecture.Lecture_Id,
        //                cloId = CLOsOfCourse[0].CLO_ID
        //            };

        //            cloasisdbRef.Execute("INSERT INTO dbo.DELIVERS (CLO_ID, LECTURE_ID) VALUES (@cloId, @lecId)", x);
        //            continue;
        //        }
        //        if (lecture.Title == "Lecture No. 4" || lecture.Title == "Lecture No. 5" || lecture.Title == "Lecture No. 6")
        //        {
        //            var x = new
        //            {
        //                lecId = lecture.Lecture_Id,
        //                cloId = CLOsOfCourse[1].CLO_ID
        //            };

        //            cloasisdbRef.Execute("INSERT INTO dbo.DELIVERS (CLO_ID, LECTURE_ID) VALUES (@cloId, @lecId)", x);
        //            continue;
        //        }
        //        if (lecture.Title == "Lecture No. 7" || lecture.Title == "Lecture No. 8" || lecture.Title == "Lecture No. 9")
        //        {
        //            var x = new
        //            {
        //                lecId = lecture.Lecture_Id,
        //                cloId = CLOsOfCourse[2].CLO_ID
        //            };

        //            cloasisdbRef.Execute("INSERT INTO dbo.DELIVERS (CLO_ID, LECTURE_ID) VALUES (@cloId, @lecId)", x);
        //            continue;
        //        }
        //        if (lecture.Title == "Lecture No. 10" || lecture.Title == "Lecture No. 11")
        //        {
        //            var x = new
        //            {
        //                lecId = lecture.Lecture_Id,
        //                cloId = CLOsOfCourse[3].CLO_ID
        //            };

        //            cloasisdbRef.Execute("INSERT INTO dbo.DELIVERS (CLO_ID, LECTURE_ID) VALUES (@cloId, @lecId)", x);
        //            continue;
        //        }
        //        if (lecture.Title == "Lecture No. 12" || lecture.Title == "Lecture No. 13" || lecture.Title == "Lecture No. 14" || lecture.Title == "Lecture No. 15")
        //        {
        //            var x = new
        //            {
        //                lecId = lecture.Lecture_Id,
        //                cloId = CLOsOfCourse[4].CLO_ID
        //            };

        //            cloasisdbRef.Execute("INSERT INTO dbo.DELIVERS (CLO_ID, LECTURE_ID) VALUES (@cloId, @lecId)", x);
        //            continue;
        //        }

        //        if (lecture.Title == "Lecture No. 16" || lecture.Title == "Lecture No. 17" || lecture.Title == "Lecture No. 18" || lecture.Title == "Lecture No. 19" || lecture.Title == "Lecture No. 20")
        //        {
        //            var x = new
        //            {
        //                lecId = lecture.Lecture_Id,
        //                cloId = CLOsOfCourse[5].CLO_ID
        //            };

        //            cloasisdbRef.Execute("INSERT INTO dbo.DELIVERS (CLO_ID, LECTURE_ID) VALUES (@cloId, @lecId)", x);
        //            continue;
        //        }
        //        if (lecture.Title == "Exam")
        //        {
        //            i = i + 1;

        //            if (i > 2)
        //            {
        //                i = 1;
        //            }
        //            var x = new
        //            {
        //                lecId = lecture.Lecture_Id,
        //                cloId = CLOsOfCourse[5+2+i].CLO_ID
        //            };

        //            cloasisdbRef.Execute("INSERT INTO dbo.DELIVERS (CLO_ID, LECTURE_ID) VALUES (@cloId, @lecId)", x);
        //            continue;
        //        }
        //    }

        //    var data = cloasisdbRef.Query("SELECT * FROM dbo.DELIVERS");

        //    return Ok(data);
        //}

        [HttpGet("[controller]/GetLectureCLORelation", Name = "GetLectureCLORelation")]
        public IActionResult GetLectureCLORelation()
        {
            var data = cloasisdbRef.Query(@"SELECT del.CLO_ID, cl.DESCRIPTION, crs.COURSE_NAME, del.LECTURE_ID, lec.TITLE  FROM dbo.DELIVERS del
                                            INNER JOIN CLO cl ON cl.CLO_ID = del.CLO_ID
                                            INNER JOIN COURSE crs ON crs.COURSE_ID = cl.COURSE_ID
                                            INNER JOIN LECTURE lec ON lec.LECTURE_ID = del.LECTURE_ID");

            return Ok(data);
        }

        [HttpGet("[controller]/GetCLOsOfLecture/{Lecture_Id}", Name = "GetCLOsOfLecture")]
        public IActionResult GetCLOsOfLecture(int Lecture_Id)
        {
            var p = new DynamicParameters();
            p.Add(@"lecId", Lecture_Id);

            var data = cloasisdbRef.Query(@"SELECT del.CLO_ID, cl.DESCRIPTION, crs.COURSE_NAME, lec.TITLE  FROM dbo.DELIVERS del
                                            INNER JOIN CLO cl ON cl.CLO_ID = del.CLO_ID
                                            INNER JOIN COURSE crs ON crs.COURSE_ID = cl.COURSE_ID
                                            INNER JOIN LECTURE lec ON lec.LECTURE_ID = del.LECTURE_ID
                                            WHERE del.LECTURE_ID = @lecId",p);

            return Ok(data);
        }

        [HttpGet("[controller]/GetLecturesOfCLO/{CLO_ID}", Name = "GetLecturesOfCLO")]
        public IActionResult GetLecturesOfCLO(int CLO_ID)
        {
            var p = new DynamicParameters();
            p.Add(@"cloId", CLO_ID);

            var data = cloasisdbRef.Query(@"SELECT cl.DESCRIPTION, crs.COURSE_NAME, del.LECTURE_ID, lec.TITLE  FROM dbo.DELIVERS del
                                            INNER JOIN CLO cl ON cl.CLO_ID = del.CLO_ID
                                            INNER JOIN COURSE crs ON crs.COURSE_ID = cl.COURSE_ID
                                            INNER JOIN LECTURE lec ON lec.LECTURE_ID = del.LECTURE_ID
                                            WHERE del.CLO_ID = @cloId", p);

            return Ok(data);
        }

        [HttpPost("[controller]/LinkLectureToCLO/{Lecture_Id}/{CLO_Id}", Name = "LinkLectureToCLO")]
        public IActionResult LinkLectureToCLO(int Lecture_Id, int CLO_Id)
        {
            var p = new DynamicParameters();
            p.Add(@"cloId", CLO_Id);
            p.Add(@"lecId", Lecture_Id);

            var isCLO = cloasisdbRef.Query("SELECT * FROM CLO WHERE CLO_ID = @cloId", p);

            if (isCLO.Count() == 0)
            {
                return BadRequest("The provided CLO ID does not belong to any CLO");
            }

            var isLec = cloasisdbRef.Query("SELECT * FROM LECTURE WHERE LECTURE_ID = @lecId", p);

            if (isLec.Count() == 0)
            {
                return BadRequest("The provided Lecture ID does not belong to any Lecture");
            }

            cloasisdbRef.Execute("INSERT INTO dbo.DELIVERS (CLO_ID, LECTURE_ID) VALUES (@cloId, @lecId)",p);

            var data = cloasisdbRef.Query(@"SELECT del.CLO_ID, cl.DESCRIPTION, crs.COURSE_NAME, lec.TITLE  FROM dbo.DELIVERS del
                                            INNER JOIN CLO cl ON cl.CLO_ID = del.CLO_ID
                                            INNER JOIN COURSE crs ON crs.COURSE_ID = cl.COURSE_ID
                                            INNER JOIN LECTURE lec ON lec.LECTURE_ID = del.LECTURE_ID
                                            WHERE del.LECTURE_ID = @lecId", p);

            return Ok(data);
        }

        [HttpDelete("[controller]/DeleteLinkLectureToCLO/{Lecture_Id}/{CLO_Id}", Name = "DeleteLinkLectureToCLO")]
        public IActionResult DeleteLinkLectureToCLO(int Lecture_Id, int CLO_Id)
        {
            var p = new DynamicParameters();
            p.Add(@"cloId", CLO_Id);
            p.Add(@"lecId", Lecture_Id);

            var isCLO = cloasisdbRef.Query("SELECT * FROM CLO WHERE CLO_ID = @cloId", p);

            if (isCLO.Count() == 0)
            {
                return BadRequest("The provided CLO ID does not belong to any CLO");
            }

            var isLec = cloasisdbRef.Query("SELECT * FROM LECTURE WHERE LECTURE_ID = @lecId", p);

            if (isLec.Count() == 0)
            {
                return BadRequest("The provided Lecture ID does not belong to any Lecture");
            }

            var isLink = cloasisdbRef.Query(@"SELECT * FROM dbo.DELIVERS WHERE CLO_ID = @cloId AND LECTURE_ID = @lecId",p);

            if (isLink.Count() == 0)
            {
                return BadRequest("The link requested for deletion does not exist");
            }

            cloasisdbRef.Execute("DELETE FROM dbo.DELIVERS WHERE CLO_ID = @cloId AND LECTURE_ID = @lecId", p);

            var data = cloasisdbRef.Query(@"SELECT del.CLO_ID, cl.DESCRIPTION, crs.COURSE_NAME, lec.TITLE  FROM dbo.DELIVERS del
                                            INNER JOIN CLO cl ON cl.CLO_ID = del.CLO_ID
                                            INNER JOIN COURSE crs ON crs.COURSE_ID = cl.COURSE_ID
                                            INNER JOIN LECTURE lec ON lec.LECTURE_ID = del.LECTURE_ID
                                            WHERE del.LECTURE_ID = @lecId", p);

            return Ok(data);
        }
    }
}