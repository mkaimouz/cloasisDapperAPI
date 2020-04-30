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
    public class GradeController : Controller
    {
        public IDbConnection cloasisdbRef = HomeController.cloasisdb;

        //    Random r = new Random();

        //    [HttpGet("[controller]/FeedData", Name = "FeedData")]
        //    public IActionResult FeedData()
        //    {
        //        List<Report> reports = cloasisdbRef.Query<Report>("SELECT * FROM dbo.REPORT").ToList();

        //        foreach(Report report in reports)
        //        {
        //            CLO CLOofReport = cloasisdbRef.Query<CLO>("SELECT * FROM dbo.CLO WHERE REPORT_ID=@Report_Id", report).ToList()[0];

        //            var p = new DynamicParameters();
        //            p.Add(@"crs_id",CLOofReport.COURSE_ID);

        //            List<Class> classesWithID = cloasisdbRef.Query<Class>("SELECT * FROM dbo.CLASS WHERE COURSE_ID=@crs_id", p).ToList();

        //            if (classesWithID.Count > 0)
        //            {
        //                foreach (Class cls in classesWithID)
        //                {
        //                    p.Add(@"crn", cls.CRN);
        //                    List<Student> studentInClass = cloasisdbRef.Query<Student>(@"SELECT std.* FROM dbo.ENROLLMENT en 
        //                                                                                INNER JOIN dbo.CLASS cls ON cls.CRN = en.CRN INNER JOIN dbo.COURSE crs ON cls.COURSE_ID = crs.COURSE_ID 
        //                                                                                INNER JOIN dbo.STUDENT std ON en.STUDENTID = std.STUDENTID 
        //                                                                                WHERE en.CRN LIKE @crn", p).ToList();

        //                    foreach (Student student in studentInClass)
        //                    {
        //                        var x = new
        //                        {
        //                            reportId = report.Report_Id,
        //                            studentId = student.STUDENTID,
        //                            grade = r.Next(70, 100),
        //                            gradeDesc = CLOofReport.DESCRIPTION,
        //                        };
        //                        cloasisdbRef.Execute("INSERT INTO dbo.GRADE (REPORT_ID, STUDENTID, GRADE, GRADE_DESC) VALUES (@reportId, @studentId, @grade, @gradeDesc)", x);

        //                    }
        //                }
        //            }
        //        }
        //        var data = cloasisdbRef.Query("SELECT * FROM dbo.GRADE");
        //        return Ok(data);
        //    }

        [HttpGet("[controller]/GetGrades", Name = "GetGrades")]
        public IActionResult GetGrades()
        {
            var data = cloasisdbRef.Query(@"SELECT gr.GRADE_ID, gr.REPORT_ID, std.STUDENTID, std.NAME, gr.GRADE, gr.GRADE_DESC, crs.COURSE_NAME, crs.COURSE_CODE 
                                            FROM GRADE gr LEFT JOIN CLO cl ON gr.REPORT_ID = cl.REPORT_ID
                                            INNER JOIN dbo.STUDENT std ON std.STUDENTID = gr.STUDENTID
                                            INNER JOIN COURSE crs ON crs.COURSE_ID = cl.COURSE_ID
                                            ORDER BY gr.REPORT_ID, gr.GRADE DESC");

            return Ok(data);
        }
        [HttpGet("[controller]/GetGradesOfCourse/{Course_Id}", Name = "GetGradesOfCourse")]
        public IActionResult GetGradesOfCourse(int Course_Id)
        {

            var p = new DynamicParameters();
            p.Add(@"crsId", Course_Id);

            List<Class> isClass = cloasisdbRef.Query<Class>("SELECT * FROM dbo.CLASS WHERE COURSE_ID = @crsId", p).ToList();

            if (isClass.Count == 0)
            {
                return BadRequest("This Course has no classes associated with it");
            }

            var data = cloasisdbRef.Query(@"SELECT gr.GRADE_ID, gr.REPORT_ID, std.STUDENTID, std.NAME, gr.GRADE, gr.GRADE_DESC, crs.COURSE_NAME, crs.COURSE_CODE 
                                            FROM GRADE gr LEFT JOIN CLO cl ON gr.REPORT_ID = cl.REPORT_ID
                                            INNER JOIN dbo.STUDENT std ON std.STUDENTID = gr.STUDENTID
                                            INNER JOIN COURSE crs ON crs.COURSE_ID = cl.COURSE_ID
                                            WHERE crs.COURSE_ID = @crsId
                                            ORDER BY gr.REPORT_ID, gr.GRADE DESC", p);

            return Ok(data);
        }

        [HttpGet("[controller]/GetGradesOfClass/{CRN}", Name = "GetGradesOfClass")]
        public IActionResult GetGradesOfClass(string CRN)
        {

            var p = new DynamicParameters();
            p.Add(@"class_crn", CRN);

            List<Class> isClass = cloasisdbRef.Query<Class>("SELECT * FROM dbo.CLASS WHERE CRN = @class_crn", p).ToList();

            if (isClass.Count == 0)
            {
                return BadRequest("This class does not exist");
            }

            var data = cloasisdbRef.Query(@"SELECT gr.GRADE_ID, gr.REPORT_ID, std.STUDENTID, std.NAME, gr.GRADE, gr.GRADE_DESC, crs.COURSE_NAME, crs.COURSE_CODE, cls.SECTION_NUM, cls.CRN
                                            FROM GRADE gr LEFT JOIN CLO cl ON gr.REPORT_ID = cl.REPORT_ID
                                            INNER JOIN dbo.STUDENT std ON std.STUDENTID = gr.STUDENTID
                                            INNER JOIN COURSE crs ON crs.COURSE_ID = cl.COURSE_ID
                                            INNER JOIN CLASS cls ON cls.COURSE_ID = crs.COURSE_ID
                                            INNER JOIN ENROLLMENT en ON en.STUDENTID = gr.STUDENTID AND en.CRN = cls.CRN
                                            WHERE en.CRN = @class_crn
                                            ORDER BY gr.REPORT_ID, gr.GRADE DESC", p);

            return Ok(data);
        }

        [HttpGet("[controller]/GetGradesOfReport/{Report_Id}", Name = "GetGradesOfReport")]
        public IActionResult GetGradesOfReport(int Report_Id)
        {

            var p = new DynamicParameters();
            p.Add(@"report_id", Report_Id);

            List<Report> isReport = cloasisdbRef.Query<Report>("SELECT * FROM dbo.REPORT WHERE REPORT_ID = @report_id", p).ToList();

            if (isReport.Count == 0)
            {
                return BadRequest("This Report Id does not exist please create a report with that ID");
            }

            var data = cloasisdbRef.Query(@"SELECT gr.GRADE_ID, gr.REPORT_ID, std.STUDENTID, std.NAME, gr.GRADE, gr.GRADE_DESC, crs.COURSE_NAME, crs.COURSE_CODE, cls.SECTION_NUM, cls.CRN
                                            FROM GRADE gr LEFT JOIN CLO cl ON gr.REPORT_ID = cl.REPORT_ID
                                            INNER JOIN dbo.STUDENT std ON std.STUDENTID = gr.STUDENTID
                                            INNER JOIN COURSE crs ON crs.COURSE_ID = cl.COURSE_ID
                                            INNER JOIN CLASS cls ON cls.COURSE_ID = crs.COURSE_ID
                                            INNER JOIN ENROLLMENT en ON en.STUDENTID = gr.STUDENTID AND en.CRN = cls.CRN
                                            WHERE gr.REPORT_ID = @report_id
                                            ORDER BY gr.REPORT_ID, gr.GRADE DESC", p);

            return Ok(data);
        }

        [HttpGet("[controller]/GetGradesOfStudent/{StudentId}", Name = "GetGradesOfStudent")]
        public IActionResult GetGradesOfStudent(string StudentId)
        {

            var p = new DynamicParameters();
            p.Add(@"stdId", StudentId);

            List<Class> isEnrolled = cloasisdbRef.Query<Class>("SELECT * FROM ENROLLMENT WHERE STUDENTID = @stdId", p).ToList();

            if (isEnrolled.Count == 0)
            {
                return BadRequest("This student is not enrolled in any of the classes currently");
            }

            var data = cloasisdbRef.Query(@"SELECT gr.GRADE_ID, gr.REPORT_ID, std.STUDENTID, std.NAME, gr.GRADE, gr.GRADE_DESC, crs.COURSE_NAME, crs.COURSE_CODE, cls.SECTION_NUM, cls.CRN
                                            FROM GRADE gr LEFT JOIN CLO cl ON gr.REPORT_ID = cl.REPORT_ID
                                            INNER JOIN dbo.STUDENT std ON std.STUDENTID = gr.STUDENTID
                                            INNER JOIN COURSE crs ON crs.COURSE_ID = cl.COURSE_ID
                                            INNER JOIN CLASS cls ON cls.COURSE_ID = crs.COURSE_ID
                                            INNER JOIN ENROLLMENT en ON en.STUDENTID = gr.STUDENTID AND en.CRN = cls.CRN
                                            WHERE std.STUDENTID = @stdId
                                            ORDER BY gr.REPORT_ID, gr.GRADE DESC", p);

            return Ok(data);
        }

        [HttpGet("[controller]/GetGradesOfStudentInClass/{StudentId}/{CRN}", Name = "GetGradesOfStudentInClass")]
        public IActionResult GetGradesOfStudentInClass(string StudentId, string CRN)
        {

            var p = new DynamicParameters();
            p.Add(@"stdId", StudentId);
            p.Add(@"class_crn", CRN);

            List<Class> isEnrolled = cloasisdbRef.Query<Class>("SELECT * FROM ENROLLMENT WHERE STUDENTID = @stdId AND CRN = @class_crn", p).ToList();

            if (isEnrolled.Count == 0)
            {
                return BadRequest("This student is not enrolled in that class");
            }

            var data = cloasisdbRef.Query(@"SELECT gr.GRADE_ID, gr.REPORT_ID, std.STUDENTID, std.NAME, gr.GRADE, gr.GRADE_DESC, crs.COURSE_NAME, crs.COURSE_CODE, cls.SECTION_NUM, cls.CRN
                                            FROM GRADE gr LEFT JOIN CLO cl ON gr.REPORT_ID = cl.REPORT_ID
                                            INNER JOIN dbo.STUDENT std ON std.STUDENTID = gr.STUDENTID
                                            INNER JOIN COURSE crs ON crs.COURSE_ID = cl.COURSE_ID
                                            INNER JOIN CLASS cls ON cls.COURSE_ID = crs.COURSE_ID
                                            INNER JOIN ENROLLMENT en ON en.STUDENTID = gr.STUDENTID AND en.CRN = cls.CRN
                                            WHERE std.STUDENTID = @stdId
                                            AND cls.CRN = @class_crn
                                            ORDER BY gr.REPORT_ID, gr.GRADE DESC", p);

            return Ok(data);
        }

        [HttpGet("[controller]/GetGradesStatsOfCourse/{Course_Id}", Name = "GetGradesStatsOfCourse")]
        public IActionResult GetGradesStatsOfCourse(string Course_Id)
        {

            var p = new DynamicParameters();
            p.Add(@"crsId", Course_Id);

            List<Class> isClass = cloasisdbRef.Query<Class>("SELECT * FROM dbo.CLASS WHERE COURSE_ID = @crsId", p).ToList();

            if (isClass.Count == 0)
            {
                return BadRequest("This Course has no classes associated with it");
            }

            var data = cloasisdbRef.Query(@"SELECT gr.REPORT_ID, gr.GRADE_DESC, MAX(gr.GRADE) Maximum_Grade ,AVG(gr.GRADE) Average, MIN(gr.GRADE) Minimum_Grade, STDEV(gr.GRADE) Standard_Deviation
                                            FROM GRADE gr LEFT JOIN CLO cl ON gr.REPORT_ID = cl.REPORT_ID
                                            INNER JOIN COURSE crs ON crs.COURSE_ID = cl.COURSE_ID
                                            WHERE crs.COURSE_ID = @crsId
                                            GROUP BY gr.REPORT_ID, gr.GRADE_DESC", p);

            return Ok(data);
        }

        [HttpGet("[controller]/GetGradesStatsOfClass/{CRN}", Name = "GetGradesStatsOfClass")]
        public IActionResult GetGradesStatsOfClass(string CRN)
        {

            var p = new DynamicParameters();
            p.Add(@"class_crn", CRN);

            List<Class> isClass = cloasisdbRef.Query<Class>("SELECT * FROM dbo.CLASS WHERE CRN = @class_crn", p).ToList();

            if (isClass.Count == 0)
            {
                return BadRequest("This class does not exist");
            }

            var data = cloasisdbRef.Query(@"SELECT gr.REPORT_ID, gr.GRADE_DESC, MAX(gr.GRADE) Maximum_Grade ,AVG(gr.GRADE) Average, MIN(gr.GRADE) Minimum_Grade, STDEV(gr.GRADE) Standard_Deviation
                                            FROM GRADE gr LEFT JOIN CLO cl ON gr.REPORT_ID = cl.REPORT_ID
                                            INNER JOIN COURSE crs ON crs.COURSE_ID = cl.COURSE_ID
                                            INNER JOIN CLASS cls ON cls.COURSE_ID = crs.COURSE_ID
                                            INNER JOIN ENROLLMENT en ON en.STUDENTID = gr.STUDENTID AND en.CRN = cls.CRN
                                            WHERE cls.CRN = @class_crn
                                            GROUP BY gr.REPORT_ID, gr.GRADE_DESC", p);

            return Ok(data);
        }

        [HttpGet("[controller]/GetGradesStatsOfReport/{Report_Id}", Name = "GetGradesStatsOfReport")]
        public IActionResult GetGradesStatsOfReport(int Report_Id)
        {

            var p = new DynamicParameters();
            p.Add(@"rptId", Report_Id);

            List<Report> isReport = cloasisdbRef.Query<Report>("SELECT * FROM dbo.REPORT WHERE REPORT_ID = @rptId", p).ToList();

            if (isReport.Count == 0)
            {
                return BadRequest("This Report does not exist");
            }

            var data = cloasisdbRef.Query(@"SELECT gr.REPORT_ID, gr.GRADE_DESC, MAX(gr.GRADE) Maximum_Grade ,AVG(gr.GRADE) Average, MIN(gr.GRADE) Minimum_Grade, STDEV(gr.GRADE) Standard_Deviation
                                            FROM GRADE gr LEFT JOIN CLO cl ON gr.REPORT_ID = cl.REPORT_ID
                                            INNER JOIN COURSE crs ON crs.COURSE_ID = cl.COURSE_ID
											WHERE gr.REPORT_ID = @rptId
                                            GROUP BY gr.REPORT_ID, gr.GRADE_DESC", p);

            return Ok(data);
        }

        [HttpPost("[controller]/CreateGrade", Name = "CreateGrade")]
        public IActionResult CreateGrade([FromBody]Grade grade)
        {

            if (grade == null)
            {
                return BadRequest("No grade object is passed");
            }

            int report_id = grade.Report_Id;

            var p = new DynamicParameters();
            p.Add(@"rptId", report_id);
            p.Add(@"stdId", grade.StudentId);

            var isReport = cloasisdbRef.Query("SELECT * FROM dbo.REPORT WHERE REPORT_ID = @rptId", p);

            if (isReport.Count() == 0)
            {
                return BadRequest("The Provided Report ID does not exist!");
            }

            var isLinkedToCLO = cloasisdbRef.Query("SELECT * FROM dbo.CLO WHERE REPORT_ID = @rptId", p);

            if (isLinkedToCLO.Count() == 0)
            {
                return BadRequest("This Report, although exists, has not been linked to any CLO");
            }

            var isStudent = cloasisdbRef.Query("SELECT * FROM dbo.STUDENT WHERE STUDENTID = @stdId", p);

            if (isStudent.Count() == 0)
            {
                return BadRequest("The studentId passed does not belong to any student!");
            }

            //var isRegistered = cloasisdbRef.Query(@"SELECT * FROM GRADE gr
            //                                        INNER JOIN CLO cl
            //                                        ON cl.REPORT_ID = gr.REPORT_ID
            //                                        INNER JOIN COURSE crs
            //                                        ON crs.COURSE_ID = cl.COURSE_ID
            //                                        WHERE gr.REPORT_ID = @rptId
            //                                        AND gr.STUDENTID = @stdId", p);

            //if (isRegistered.Count() == 0)
            //{
            //    return BadRequest("This student is not registered in the course that the report is linked to!");
            //}

            var alreadyAssigned = cloasisdbRef.Query(@"SELECT * FROM dbo.GRADE WHERE STUDENTID = @stdId AND REPORT_ID = @rptId", p);

            if (alreadyAssigned.Count() > 0)
            {
                return BadRequest("This student has already been assigned a grade in the provided report!");
            }

            cloasisdbRef.Execute("INSERT INTO dbo.GRADE (REPORT_ID, STUDENTID, GRADE, GRADE_DESC) VALUES (@Report_Id, @StudentId, @Grade_Value, @Grade_Desc)", grade);

            var data = cloasisdbRef.Query("SELECT * FROM dbo.GRADE WHERE REPORT_ID = @Report_Id AND STUDENTID = @STUDENTID AND GRADE = @Grade_Value", grade);

            return Ok(data);
        }

        [HttpPut("[controller]/EditGrade/{grade_Id}", Name = "EditGrade")]
        public IActionResult EditGrade([FromBody]Grade grade, int grade_Id)
        {

            if (grade == null)
            {
                return BadRequest("No grade object is passed");
            }

            int report_id = grade.Report_Id;

            var p = new DynamicParameters();
            p.Add(@"rptId", report_id);
            p.Add(@"stdId", grade.StudentId);
            p.Add(@"grdId", grade_Id);

            var isGrade = cloasisdbRef.Query("SELECT * FROM dbo.GRADE WHERE GRADE_ID = @grdId", p);

            if (isGrade.Count() == 0)
            {
                return BadRequest("The Provided Grade ID does not exist!");
            }

            var isReport = cloasisdbRef.Query("SELECT * FROM dbo.REPORT WHERE REPORT_ID = @rptId", p);

            if (isReport.Count() == 0)
            {
                return BadRequest("The Provided Report ID does not exist!");
            }

            var isLinkedToCLO = cloasisdbRef.Query("SELECT * FROM dbo.CLO WHERE REPORT_ID = @rptId", p);

            if (isLinkedToCLO.Count() == 0)
            {
                return BadRequest("This Report, although exists, has not been linked to any CLO");
            }

            var isStudent = cloasisdbRef.Query("SELECT * FROM dbo.STUDENT WHERE STUDENTID = @stdId", p);

            if (isStudent.Count() == 0)
            {
                return BadRequest("The studentId passed does not belong to any student!");
            }

            //var isRegistered = cloasisdbRef.Query(@"SELECT * FROM GRADE gr
            //                                        INNER JOIN CLO cl
            //                                        ON cl.REPORT_ID = gr.REPORT_ID
            //                                        INNER JOIN COURSE crs
            //                                        ON crs.COURSE_ID = cl.COURSE_ID
            //                                        WHERE gr.REPORT_ID = @rptId
            //                                        AND gr.STUDENTID = @stdId", p);

            //if (isRegistered.Count() == 0)
            //{
            //    return BadRequest("This student is not registered in the course that the report is linked to!");
            //}

            var alreadyAssigned = cloasisdbRef.Query(@"SELECT * FROM dbo.GRADE WHERE STUDENTID = @stdId AND REPORT_ID = @rptId", p);

            if (alreadyAssigned.Count() == 0)
            {
                return BadRequest("This student does not have a grade in this report!");
            }

            Grade updatedGrade = new Grade()
            {
                Grade_Id = grade_Id,
                Report_Id = grade.Report_Id,
                Grade_Value = grade.Grade_Value,
                StudentId = grade.StudentId,
                Grade_Desc = grade.Grade_Desc
            };

            cloasisdbRef.Execute("UPDATE dbo.GRADE SET REPORT_ID = @Report_Id, STUDENTID = @StudentId, GRADE = @Grade_Value, GRADE_DESC = @Grade_Desc WHERE GRADE_ID = @Grade_Id", updatedGrade);

            var data = cloasisdbRef.Query("SELECT * FROM dbo.GRADE WHERE GRADE_ID = @grdId", p);

            return Ok(data);
        }

        [HttpDelete("[controller]/DeleteGrade/{grade_Id}", Name = "DeleteGrade")]
        public IActionResult DeleteGrade(int grade_Id)
        {

            var p = new DynamicParameters();
            p.Add(@"grdId", grade_Id);

            var isGrade = cloasisdbRef.Query("SELECT * FROM dbo.GRADE WHERE GRADE_ID = @grdId", p);

            if (isGrade.Count() == 0)
            {
                return BadRequest("The Provided Grade ID does not exist!");
            }

            cloasisdbRef.Execute("DELETE FROM dbo.GRADE WHERE GRADE_ID = @grdId", p);

            return Ok($"The grade with ID {grade_Id} has been deleted successfully!");
        }






    }
}