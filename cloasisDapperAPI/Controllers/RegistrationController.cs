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
    public class RegistrationController : Controller
    {
        public IDbConnection cloasisdbRef = HomeController.cloasisdb;

        //Random r = new Random();

        //[HttpGet("[controller]/FeedData", Name = "FeedData")]
        //public IActionResult FeedData()
        //{
        //    List<Class> classes = cloasisdbRef.Query<Class>("SELECT * FROM dbo.CLASS").ToList();
        //    List<Student> students = cloasisdbRef.Query<Student>("SELECT * FROM dbo.STUDENT").ToList();

        //    foreach (Class cls in classes)
        //    {
        //        int studentsInClass_Num = r.Next(8, 15);
        //        List<Student> studentsInClass = new List<Student>();

        //        for (int i=0; i<studentsInClass_Num; i++)
        //        {
        //            Student std = students[r.Next(students.Count())];
        //            if (studentsInClass.Contains(std))
        //            {
        //                std = students[r.Next(students.Count())];
        //            }
        //            studentsInClass.Add(std);

        //            var p = new
        //            {
        //                class_crn = cls.CRN,
        //                std_id = std.STUDENTID
        //            };

        //            cloasisdbRef.Execute(@"INSERT INTO dbo.ENROLLMENT (CRN, STUDENTID)
        //                                    VALUES (@class_crn, @std_id)", p);
        //        }
        //        studentsInClass = new List<Student>();
        //    }

        //    var data = cloasisdbRef.Query("SELECT * FROM dbo.ENROLLMENT");

        //    return Ok(data);
        //}

        [HttpGet("[controller]/GetRegistrations", Name = "GetRegistrations")]
        public IActionResult GetRegistration()
        {

            var data = cloasisdbRef.Query("SELECT en.CRN, crs.COURSE_NAME, crs.COURSE_CODE, cls.SECTION_NUM, en.STUDENTID, std.NAME FROM dbo.ENROLLMENT en INNER JOIN dbo.CLASS cls ON cls.CRN = en.CRN INNER JOIN dbo.COURSE crs ON cls.COURSE_ID = crs.COURSE_ID INNER JOIN dbo.STUDENT std ON en.STUDENTID = std.STUDENTID");

            return Ok(data);
        }

        [HttpGet("[controller]/GetStudentsInClass/{CRNorCLASSNAME}", Name = "GetStudentsInClass")]
        public IActionResult GetStudentsInClass(string CRNorCLASSNAME)
        {

            var p = new DynamicParameters();
            p.Add("@crnorclassname", CRNorCLASSNAME);

            var data = cloasisdbRef.Query("SELECT en.CRN, crs.COURSE_NAME, crs.COURSE_CODE, cls.SECTION_NUM,  en.STUDENTID, std.NAME FROM dbo.ENROLLMENT en INNER JOIN dbo.CLASS cls ON cls.CRN = en.CRN INNER JOIN dbo.COURSE crs ON cls.COURSE_ID = crs.COURSE_ID INNER JOIN dbo.STUDENT std ON en.STUDENTID = std.STUDENTID WHERE en.CRN LIKE @crnorclassname OR crs.COURSE_NAME LIKE '%' + @crnorclassname + '%'", p);

            return Ok(data);
        }

        [HttpGet("[controller]/GetClassesOfStudent/{IDorName}", Name = "GetClassesOfStudent")]
        public IActionResult GetClassesOfStudent(string IDorName)
        {

            var p = new DynamicParameters();
            p.Add("@Idorname", IDorName);

            var data = cloasisdbRef.Query("SELECT en.CRN, crs.COURSE_NAME, crs.COURSE_CODE, cls.SECTION_NUM, en.STUDENTID, std.NAME FROM dbo.ENROLLMENT en INNER JOIN dbo.CLASS cls ON cls.CRN = en.CRN INNER JOIN dbo.COURSE crs ON cls.COURSE_ID = crs.COURSE_ID INNER JOIN dbo.STUDENT std ON en.STUDENTID = std.STUDENTID WHERE en.STUDENTID LIKE @Idorname OR std.NAME LIKE '%' + @Idorname + '%'", p);

            return Ok(data);
        }

        [HttpGet("[controller]/GetStudentsNotInClass/{CRN}", Name = "GetStudentsNotInClass")]
        public IActionResult GetStudentsNotInClass(string CRN)
        {

            var p = new DynamicParameters();
            p.Add("@class_crn", CRN);

            var data = cloasisdbRef.Query(@"SELECT std.STUDENTID, std.NAME FROM STUDENT std
                                            WHERE std.STUDENTID NOT IN (
                                            	SELECT en.STUDENTID FROM ENROLLMENT en
                                            	WHERE en.CRN IN 
                                            	(
                                            		SELECT cls.CRN FROM CLASS cls
                                                    WHERE cls.COURSE_ID IN 
                                                    (
                                                        SELECT COURSE_ID FROM CLASS
                                                        GROUP BY COURSE_ID HAVING COUNT(*) = 1 OR COUNT(*) > 1
                                                    )
                                                    AND cls.COURSE_ID IN
                                                    (
                                                        SELECT COURSE_ID FROM CLASS
                                                        WHERE CLASS.CRN = @class_crn
                                                    )
                                            	)
                                            )
                                            ORDER BY std.NAME", p);

            return Ok(data);
        }

        [HttpPost("[controller]/RegisterStudent/{student_Id}", Name = "RegisterStudent")]
        public IActionResult Post([FromBody] APIPostReqs<List<string>> req, string student_Id)
        {
            if (req == null)
            {
                return BadRequest();
            }

            List<string> crns = req.data;

            var p = new DynamicParameters();
            p.Add("@stdId", student_Id);

            var isStudent = cloasisdbRef.Query("SELECT * FROM dbo.STUDENT WHERE STUDENTID = @stdId", p);


            if (isStudent.Count() == 0)
            {
                return BadRequest("Please Provide a valid StudentId");
            }

            string CRNsInString = "";
            string log = "";

            foreach (string crn in crns)
            {
                bool flag = false;

                var x = new
                {
                    class_crn = crn,
                    std_Id = student_Id
                };


                var isClass = cloasisdbRef.Query("SELECT * FROM dbo.CLASS WHERE CRN = @class_crn", x);

                if (isClass.Count() == 0)
                {
                    flag = true;
                    log = log + $"Please Provide a valid CRN because {crn} is not valid.\n";
                    continue;
                }

                int course_Id = cloasisdbRef.Query<int>("SELECT COURSE_ID FROM CLASS WHERE CRN = @class_crn", x).ToList()[0];

                List<RegistrationEntry> entriesMatched = cloasisdbRef.Query<RegistrationEntry>("SELECT * FROM dbo.ENROLLMENT WHERE STUDENTID = @stdId", p).ToList();


                foreach (RegistrationEntry ent in entriesMatched)
                {
                    int crsIdOfEnt = cloasisdbRef.Query<int>("SELECT COURSE_ID FROM CLASS WHERE CRN = @CRN", ent).ToList()[0];

                    if (ent.CRN == crn)
                    {
                        flag = true;
                        log = log + $"Student Already Registered in the Class -> {crn}\n";
                        break;
                    }

                    if (crsIdOfEnt == course_Id)
                    {
                        flag = true;
                        log = log + $"Student Already Registered in another section of this Course -> {crn}\n";
                        break;
                    }
                }

                if (!flag)
                {
                    string sql = $@"insert into dbo.ENROLLMENT (CRN, STUDENTID) 
                                values (@class_crn, @std_Id)";

                    CRNsInString = CRNsInString + crn + ", ";


                    cloasisdbRef.Execute(sql, x);
                }
                
            }

            return Ok($"Student {student_Id} is now registered in classes [{CRNsInString}]" + $"\nLog: \n{log}");
        }

        [HttpDelete("[controller]/DropStudent/{student_Id}", Name = "DropStudent")]
        public IActionResult Delete([FromBody] APIPostReqs<List<string>> req ,string student_Id)
        {

            if (req == null)
            {
                return BadRequest();
            }

            List<string> crns = req.data;

            var p = new DynamicParameters();
            p.Add("@stdId", student_Id);

            var isStudent = cloasisdbRef.Query("SELECT * FROM dbo.STUDENT WHERE STUDENTID = @stdId", p);


            if (isStudent.Count() == 0)
            {
                return BadRequest("Please Provide a valid StudentId");
            }

            string CRNsInString = "";
            string log = "";

            foreach (string crn in crns)
            {
                bool flag = false;


                var x = new
                {
                    class_crn = crn,
                    std_Id = student_Id
                };

                var isClass = cloasisdbRef.Query("SELECT * FROM dbo.CLASS WHERE CRN = @class_crn", x);

                if (isClass.Count() == 0)
                {
                    flag = true;
                    log = log + $"Please Provide a valid CRN because {crn} is not valid.\n";
                    continue;
                }

                List<RegistrationEntry> entriesMatched = cloasisdbRef.Query<RegistrationEntry>("SELECT * FROM dbo.ENROLLMENT WHERE STUDENTID = @std_Id AND CRN = @class_crn", x).ToList();

                if (entriesMatched.Count() == 0)
                {
                    flag = true;
                    log = log + $"Student Is Not Registered in the Class -> {crn}\n";
                    continue;
                }

                if (!flag)
                {
                    string sql = $@"DELETE FROM dbo.ENROLLMENT WHERE STUDENTID = @std_Id AND CRN = @class_crn";

                    CRNsInString = CRNsInString + crn + ", ";


                    cloasisdbRef.Execute(sql, x);
                }

            }

            return Ok($"Student {student_Id} is dropped from classes [{CRNsInString}]" + $"\nLog: \n{log}");
        }
    }

}