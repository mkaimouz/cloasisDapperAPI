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
    public class CLOController : Controller
    {

        public IDbConnection cloasisdbRef = HomeController.cloasisdb;

        //Random r = new Random();

        //[HttpGet("[controller]/FeedData", Name = "FeedData")]
        //public IActionResult FeedData()
        //{
        //    List<Course> courses = cloasisdbRef.Query<Course>("SELECT * FROM dbo.COURSE").ToList();

        //    foreach (Course course in courses)
        //    {
        //        for (int i=0; i<6; i++)
        //        {
        //            var p = new
        //            {
        //                clo_desc = $"Course Learning OutCome of Course \"{course.Course_Name}\" number: " + (i+1),
        //                crs_id = course.Course_Id,
        //            };

        //            cloasisdbRef.Execute("INSERT INTO dbo.CLO (DESCRIPTION, COURSE_ID) VALUES (@clo_Desc, @crs_id)", p);
        //        }

        //        Report rprt = new Report
        //        {
        //            Report_Date = DateTime.Now
        //        };
        //        cloasisdbRef.Execute("INSERT INTO dbo.REPORT (REPORT_DATE) VALUES (@Report_Date)", rprt);

        //        List<Report> rprts = cloasisdbRef.Query<Report>("SELECT * FROM dbo.REPORT").ToList<Report>();
        //        Report rprtToUse = rprts[rprts.Count - 1];

        //        var x = new
        //        {
        //            clo_desc = "Assignment " + 1,
        //            crs_id = course.Course_Id,
        //            report_id = rprtToUse.Report_Id
        //        };

        //        cloasisdbRef.Execute("INSERT INTO dbo.CLO (COURSE_ID, REPORT_ID, DESCRIPTION) VALUES (@crs_id, @report_id, @clo_Desc)", x);

        //        rprt = new Report
        //        {
        //            Report_Date = DateTime.Now
        //        };
        //        cloasisdbRef.Execute("INSERT INTO dbo.REPORT (REPORT_DATE) VALUES (@Report_Date)", rprt);

        //        rprts = cloasisdbRef.Query<Report>("SELECT * FROM dbo.REPORT").ToList<Report>();
        //        rprtToUse = rprts[rprts.Count - 1];

        //        x = new
        //        {
        //            clo_desc = "Assignment " + 2,
        //            crs_id = course.Course_Id,
        //            report_id = rprtToUse.Report_Id
        //        };

        //        cloasisdbRef.Execute("INSERT INTO dbo.CLO (COURSE_ID, REPORT_ID, DESCRIPTION) VALUES (@crs_id, @report_id, @clo_Desc)", x);

        //        rprt = new Report
        //        {
        //            Report_Date = DateTime.Now
        //        };
        //        cloasisdbRef.Execute("INSERT INTO dbo.REPORT (REPORT_DATE) VALUES (@Report_Date)", rprt);

        //        rprts = cloasisdbRef.Query<Report>("SELECT * FROM dbo.REPORT").ToList<Report>();
        //        rprtToUse = rprts[rprts.Count - 1];

        //        x = new
        //        {
        //            clo_desc = "Midterm",
        //            crs_id = course.Course_Id,
        //            report_id = rprtToUse.Report_Id
        //        };

        //        cloasisdbRef.Execute("INSERT INTO dbo.CLO (COURSE_ID, REPORT_ID, DESCRIPTION) VALUES (@crs_id, @report_id, @clo_Desc)", x);

        //        rprt = new Report
        //        {
        //            Report_Date = DateTime.Now
        //        };
        //        cloasisdbRef.Execute("INSERT INTO dbo.REPORT (REPORT_DATE) VALUES (@Report_Date)", rprt);

        //        rprts = cloasisdbRef.Query<Report>("SELECT * FROM dbo.REPORT").ToList<Report>();
        //        rprtToUse = rprts[rprts.Count - 1];

        //        x = new
        //        {
        //            clo_desc = "Final",
        //            crs_id = course.Course_Id,
        //            report_id = rprtToUse.Report_Id
        //        };

        //        cloasisdbRef.Execute("INSERT INTO dbo.CLO (COURSE_ID, REPORT_ID, DESCRIPTION) VALUES (@crs_id, @report_id, @clo_Desc)", x);
        //    }

        //    var data = cloasisdbRef.Query("SELECT * FROM dbo.CLO");

        //    return Ok(data);
        //}

        [HttpGet("[controller]/GetCLOs", Name = "GetCLOs")]
        public IActionResult GetCLOs()
        {

            var data = cloasisdbRef.Query("SELECT * FROM dbo.CLO");

            return Ok(data);
        }

        [HttpGet("[controller]/GetCLOofId/{CLO_ID}", Name = "GetCLOofId")]
        public IActionResult GetCLOofId(int CLO_ID)
        {

            var p = new DynamicParameters();
            p.Add(@"cloId", CLO_ID);

            var data = cloasisdbRef.Query("SELECT * FROM dbo.CLO WHERE CLO_ID = @cloId", p);

            return Ok(data);
        }

        [HttpGet("[controller]/GetCLOsOfCourse/{Course_Id}", Name = "GetCLOsOfCourse")]
        public IActionResult GetCLOsOfCourse(int Course_Id)
        {

            var p = new DynamicParameters();
            p.Add(@"crsId", Course_Id);

            var isCourse = cloasisdbRef.Query("SELECT * FROM dbo.COURSE WHERE COURSE_ID = @crsId", p);

            if (isCourse.Count() == 0)
            {
                return BadRequest("Please pass a valid course");
            }

            var data = cloasisdbRef.Query("SELECT * FROM dbo.CLO WHERE COURSE_ID = @crsId", p);

            return Ok(data);
        }

        [HttpGet("[controller]/GetAssessmentsCLOs/{Course_Id}", Name = "GetAssessmentsCLOs")]
        public IActionResult GetAssessmentsCLOs(int Course_Id)
        {

            var p = new DynamicParameters();
            p.Add(@"crsId", Course_Id);

            var isCourse = cloasisdbRef.Query("SELECT * FROM dbo.COURSE WHERE COURSE_ID = @crsId", p);

            if (isCourse.Count() == 0)
            {
                return BadRequest("Please pass a valid course");
            }

            var data = cloasisdbRef.Query("SELECT * FROM dbo.CLO WHERE COURSE_ID = @crsId AND REPORT_ID IS NOT NULL", p);

            return Ok(data);
        }

        [HttpGet("[controller]/GetAssignmentsCLOs/{Course_Id}", Name = "GetAssignmentsCLOs")]
        public IActionResult GetAssignmentsCLOs(int Course_Id)
        {

            var p = new DynamicParameters();
            p.Add(@"crsId", Course_Id);

            var isCourse = cloasisdbRef.Query("SELECT * FROM dbo.COURSE WHERE COURSE_ID = @crsId", p);

            if (isCourse.Count() == 0)
            {
                return BadRequest("Please pass a valid course");
            }

            var data = cloasisdbRef.Query("SELECT * FROM dbo.CLO WHERE COURSE_ID = @crsId AND REPORT_ID IS NOT NULL AND (DESCRIPTION LIKE '%' + 'assignment' + '%' OR DESCRIPTION LIKE '%' + 'asst' + '%')", p);

            return Ok(data);
        }

        [HttpGet("[controller]/GetCLOsOfClass/{CRN}", Name = "GetCLOsOfClass")]
        public IActionResult GetCLOsOfClass(string CRN)
        {

            var p = new DynamicParameters();
            p.Add(@"class_crn", CRN);

            var isClass = cloasisdbRef.Query("SELECT * FROM dbo.CLASS WHERE CRN = @class_crn", p);

            if (isClass.Count() == 0)
            {
                return BadRequest("Please pass a valid class");
            }

            int course_Id = cloasisdbRef.Query<int>("SELECT COURSE_ID FROM dbo.CLASS WHERE CRN = @class_crn", p).ToList()[0];

            p.Add(@"crsId", course_Id);

            var data = cloasisdbRef.Query("SELECT * FROM dbo.CLO WHERE COURSE_ID = @crsId", p);

            return Ok(data);
        }

        [HttpGet("[controller]/GetCLOsOfReport/{Report_Id}", Name = "GetCLOsOfReport")]
        public IActionResult GetCLOsOfReport(int Report_Id)
        {

            var p = new DynamicParameters();
            p.Add(@"rptId", Report_Id);

            var isReport = cloasisdbRef.Query("SELECT * FROM dbo.REPORT WHERE REPORT_ID = @rptId", p);

            if (isReport.Count() == 0)
            {
                return BadRequest("Please pass a valid Report");
            }

            var data = cloasisdbRef.Query("SELECT * FROM dbo.CLO WHERE REPORT_ID = @rptId", p);

            return Ok(data);
        }

        [HttpPost("[controller]/CreateCLO", Name = "CreateCLO")]
        public IActionResult CreateCLO([FromBody]CLO clo)
        {

            var isCourse = cloasisdbRef.Query("SELECT * FROM dbo.COURSE WHERE COURSE_ID = @COURSE_ID", clo);

            if (isCourse.Count() == 0)
            {
                return BadRequest("Please pass a valid Course ID");
            }

            if (clo.REPORT_ID != null)
            {
                var isReport = cloasisdbRef.Query("SELECT * FROM dbo.REPORT WHERE REPORT_ID = @REPORT_ID", clo);

                if (isReport.Count() == 0)
                {
                    return BadRequest("Please pass a valid Report");
                }

                var reportUsed = cloasisdbRef.Query("SELECT * FROM dbo.CLO WHERE REPORT_ID = @REPORT_ID", clo);

                if (reportUsed.Count() > 0)
                {
                    return BadRequest("This Report is already used in another CLO please pass a novel Report_ID");
                }

            }

            cloasisdbRef.Execute(@"INSERT INTO dbo.CLO (COURSE_ID, REPORT_ID, DESCRIPTION)
                                                VALUES (@COURSE_ID, @REPORT_ID, @DESCRIPTION)", clo);


            if (clo.REPORT_ID != null)
            {
                var data = cloasisdbRef.Query("SELECT * FROM dbo.CLO WHERE COURSE_ID = @COURSE_ID AND REPORT_ID = @REPORT_ID AND DESCRIPTION = @DESCRIPTION", clo);
                return Ok(data);

            }

            var data2 = cloasisdbRef.Query("SELECT * FROM dbo.CLO WHERE COURSE_ID = @COURSE_ID AND DESCRIPTION = @DESCRIPTION", clo);
            return Ok(data2);
        }

        [HttpPut("[controller]/EditCLO/{CLO_ID}", Name = "EditCLO")]
        public IActionResult EditCLO([FromBody]CLO clo, int CLO_ID)
        {

            var p = new DynamicParameters();
            p.Add(@"cloId", CLO_ID);

            var IsCLO = cloasisdbRef.Query("SELECT * FROM dbo.CLO WHERE CLO_ID = @cloId", p);

            if (IsCLO.Count() == 0)
            {
                return BadRequest("Please pass an existant CLO ID");
            }

            var isCourse = cloasisdbRef.Query("SELECT * FROM dbo.COURSE WHERE COURSE_ID = @COURSE_ID", clo);

            if (isCourse.Count() == 0)
            {
                return BadRequest("Please pass a valid Course ID");
            }


            if (clo.REPORT_ID != null)
            {
                var isReport = cloasisdbRef.Query("SELECT * FROM dbo.REPORT WHERE REPORT_ID = @REPORT_ID", clo);

                if (isReport.Count() == 0)
                {
                    return BadRequest("Please pass a valid Report");
                }

            }

            cloasisdbRef.Execute(@"UPDATE dbo.CLO SET COURSE_ID = @COURSE_ID, REPORT_ID = @REPORT_ID, DESCRIPTION = @DESCRIPTION WHERE CLO_ID = @cloId", p);

            var data = cloasisdbRef.Query("SELECT * FROM dbo.CLO WHERE CLO_ID = @cloId", p);

            return Ok(data);
        }

        [HttpDelete("[controller]/DeleteCLO/{CLO_ID}", Name = "DeleteCLO")]
        public IActionResult DeleteCLO(int CLO_ID)
        {

            var p = new DynamicParameters();
            p.Add(@"cloId", CLO_ID);

            var IsCLO = cloasisdbRef.Query("SELECT * FROM dbo.CLO WHERE CLO_ID = @cloId", p);

            if (IsCLO.Count() == 0)
            {
                return BadRequest("Please pass an existant CLO ID");
            }

            cloasisdbRef.Execute(@"DELETE FROM dbo.CLO WHERE CLO_ID = @cloId", p);

            return Ok($"THE CLO with the ID {CLO_ID} has been deleted successfully!");
        }

        //[HttpGet("[controller]/FIX", Name = "FIX")]
        //public IActionResult FIX()
        //{
        //    List<CLO> clos = cloasisdbRef.Query<CLO>("SELECT * FROM dbo.CLO ORDER BY CLO_ID").ToList();
        //    List<Course> courses = cloasisdbRef.Query<Course>("SELECT * FROM dbo.COURSE ORDER BY COURSE_ID").ToList();
        //    List<Report> reports = cloasisdbRef.Query<Report>("SELECT * FROM dbo.REPORT ORDER BY REPORT_ID").ToList();
        //    int i = 0;
        //    int j = 0;
        //    int n = 0;

        //    foreach(CLO clo in clos)
        //    {
        //        i = i + 1;
        //        if ( i > 0 && i < 7)
        //        {
        //            CLO c = new CLO
        //            {
        //                CLO_ID = clo.CLO_ID,
        //                COURSE_ID = courses[j].Course_Id,
        //                DESCRIPTION = $"Course Learning OutCome of Course \"{courses[j].Course_Name}\" number: " + (i)
        //            };

        //            cloasisdbRef.Execute("UPDATE dbo.CLO SET COURSE_ID = @COURSE_ID, DESCRIPTION = @DESCRIPTION WHERE CLO_ID = @CLO_ID", c);

        //            continue;
        //        }
        //        if (i == 7)
        //        {
        //            CLO c = new CLO
        //            {
        //                CLO_ID = clo.CLO_ID,
        //                COURSE_ID = courses[j].Course_Id,
        //                DESCRIPTION = "Assignment 1",
        //                REPORT_ID = reports[n].Report_Id
                        
        //            };
        //            n = n + 1;
        //            cloasisdbRef.Execute("UPDATE dbo.CLO SET COURSE_ID = @COURSE_ID, REPORT_ID = @REPORT_ID, DESCRIPTION = @DESCRIPTION WHERE CLO_ID = @CLO_ID",c);

        //            continue;
        //        }
        //        if (i == 8)
        //        {

        //            CLO c = new CLO
        //            {
        //                CLO_ID = clo.CLO_ID,
        //                COURSE_ID = courses[j].Course_Id,
        //                DESCRIPTION = "Assignment 2",
        //                REPORT_ID = reports[n].Report_Id
        //            };
        //            n = n + 1;
        //            cloasisdbRef.Execute("UPDATE dbo.CLO SET COURSE_ID = @COURSE_ID, REPORT_ID = @REPORT_ID, DESCRIPTION = @DESCRIPTION WHERE CLO_ID = @CLO_ID",c);
        //            continue;
        //        }
        //        if (i == 9)
        //        {

        //            CLO c = new CLO
        //            {
        //                CLO_ID = clo.CLO_ID,
        //                COURSE_ID = courses[j].Course_Id,
        //                DESCRIPTION = "Midterm",
        //                REPORT_ID = reports[n].Report_Id
        //            };
        //            n = n + 1;
        //            cloasisdbRef.Execute("UPDATE dbo.CLO SET COURSE_ID = @COURSE_ID, REPORT_ID = @REPORT_ID, DESCRIPTION = @DESCRIPTION WHERE CLO_ID = @CLO_ID",c);
        //            continue;
        //        }
        //        if (i == 10)
        //        {

        //            CLO c = new CLO
        //            {
        //                CLO_ID = clo.CLO_ID,
        //                COURSE_ID = courses[j].Course_Id,
        //                DESCRIPTION = "Final",
        //                REPORT_ID = reports[n].Report_Id
        //            };
        //            n = n + 1;
        //            cloasisdbRef.Execute("UPDATE dbo.CLO SET COURSE_ID = @COURSE_ID, REPORT_ID = @REPORT_ID, DESCRIPTION = @DESCRIPTION WHERE CLO_ID = @CLO_ID",c);

        //            i = 0;
        //            j = j + 1;
        //            continue;
        //        }
        //    }
        //    return Ok();
        //}


    }
}