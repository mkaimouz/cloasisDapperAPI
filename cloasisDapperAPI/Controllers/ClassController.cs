using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using cloasisDapperAPI.Models;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace cloasisDapperAPI.Controllers
{
    public class ClassController : Controller
    {
        public IDbConnection cloasisdbRef = HomeController.cloasisdb;

        [HttpGet("[controller]/GetClasses", Name = "GetClasses")]
        public IActionResult GetClasses()
        {
            var data = cloasisdbRef.Query(@"SELECT CRN, pr.NAME 'Professor''s Name', pr.EMAIL 'Professor''s Email', pr.OFFICE 'Professor''s Office',
                                                  cr.COURSE_NAME 'Course''s Name', cr.COURSE_CODE 'Course''s Code', cr.DESCRIPTION 'Course''s Description',
                                                   cr.CREDITS, cs.PROGRESS, cs.ROOM, cs.SECTION_NUM, cs.TEACHING_SEMESTER
                                                   FROM CLASS cs
                                                   LEFT JOIN dbo.PROFESSOR pr
                                                   ON cs.PROFESSOR_ID = pr.PROFESSOR_ID
                                                   LEFT JOIN dbo.COURSE cr
                                                   ON cs.COURSE_ID = cr.COURSE_ID");

            return Ok(data);
        }

        [HttpGet("[controller]/FetchClass/{searchTerm}", Name = "FetchClass")]
        public IActionResult FetchClass(string searchTerm)
        {

            var p = new DynamicParameters();
            p.Add("@SearchTerm", searchTerm);

            string sql = @"SELECT CRN, pr.NAME 'Professor''s Name', pr.EMAIL 'Professor''s Email', pr.OFFICE 'Professor''s Office',
                                                  cr.COURSE_NAME 'Course''s Name', cr.COURSE_CODE 'Course''s Code', cr.DESCRIPTION 'Course''s Description',
                                                   cr.CREDITS, cs.PROGRESS, cs.ROOM, cs.SECTION_NUM, cs.TEACHING_SEMESTER
                                                   FROM CLASS cs
                                                   LEFT JOIN dbo.PROFESSOR pr
                                                   ON cs.PROFESSOR_ID = pr.PROFESSOR_ID
                                                   LEFT JOIN dbo.COURSE cr
                                                   ON cs.COURSE_ID = cr.COURSE_ID
                                                   WHERE
                                                   CRN LIKE @searchTerm 
			                                       OR TEACHING_SEMESTER LIKE '%' + @searchTerm + '%'
			                                       OR ROOM LIKE '%' + @searchTerm + '%'
                                                   OR pr.NAME LIKE '%' + @searchTerm + '%'
                                                   OR cr.COURSE_NAME LIKE '%' + @searchTerm + '%'
                                                   OR cr.COURSE_CODE LIKE  @searchTerm";

            var data = cloasisdbRef.Query(sql, p);

            return Ok(data);
        }

        [HttpPost("[controller]/CreateClass", Name = "CreateClass")]
        public IActionResult Post([FromBody] Class cls)
        {
            if (cls == null)
            {
                return BadRequest(new {error = "You passed a null object"});
            }


            var prof_name = cls.Professor_Name;

            var p = new DynamicParameters();
            p.Add("@prof_name", prof_name);

            Professor prof = cloasisdbRef.Query<Professor>(@"SELECT * FROM dbo.PROFESSOR WHERE NAME LIKE @prof_name", p).ToList<Professor>()[0];


            var course_name = cls.Course_Name;
            p.Add("@course_name", course_name);

            Course crs = cloasisdbRef.Query<Course>(@"SELECT * FROM dbo.COURSE WHERE COURSE_NAME LIKE @course_name", p).ToList<Course>()[0];

            p.Add("@course_id", crs.Course_Id);

            if (prof == null || crs == null)
            {
                return BadRequest(new { error = "Either the professor name or the course name deos not exist in the database." });
            }

            List<Class> classes = cloasisdbRef.Query<Class>(@"SELECT * FROM dbo.CLASS WHERE COURSE_ID = @course_id", p).ToList<Class>();

            int sectionCount = 0;

            foreach (Class cl in classes)
            {
                if (cl.Teaching_Semester == cls.Teaching_Semester)
                {
                    sectionCount++;
                }
            }

            

            var newClass = new 
            {
                CRN = cls.CRN,
                Professor_Id = prof.professor_Id,
                Course_Id = crs.Course_Id,
                room = cls.room,
                Section_Num = sectionCount + 1,
                progress = cls.progress,
                Teaching_Semester = cls.Teaching_Semester
            };

            string sql = @"insert into dbo.CLASS (CRN, PROFESSOR_ID, COURSE_ID, SECTION_NUM, TEACHING_SEMESTER, PROGRESS, ROOM)
                            VALUES (@CRN, @Professor_Id, @Course_Id, @Section_Num, @Teaching_Semester, @progress, @room)";

            cloasisdbRef.Execute(sql, newClass);


            return CreatedAtRoute("FetchClass", new { searchTerm = cls.CRN }, newClass);
        }

        [HttpPut("[controller]/EditClass/{CRN}", Name = "EditClass")]
        public IActionResult Update([FromBody]Class cls, string CRN)
        {
            

            var prof_name = cls.Professor_Name;

            var p = new DynamicParameters();
            p.Add("@prof_name", prof_name);

            Professor prof = cloasisdbRef.Query<Professor>(@"SELECT * FROM dbo.PROFESSOR WHERE NAME LIKE @prof_name", p).ToList<Professor>()[0];


            var course_name = cls.Course_Name;
            p.Add("@course_name", course_name);

            Course crs = cloasisdbRef.Query<Course>(@"SELECT * FROM dbo.COURSE WHERE COURSE_NAME LIKE @course_name", p).ToList<Course>()[0];

            if (prof == null || crs == null)
            {
                return BadRequest(new { error = "Either the professor name or the course name deos not exist in the database." });
            }

            var updateClass = new
            {
                class_crn = CRN,
                profId = prof.professor_Id,
                courseId = crs.Course_Id,
                secNumb = cls.Section_Num,
                teaching_sem = cls.Teaching_Semester,
                prgrs = cls.progress,
                rm = cls.room
            };
            string sql = $@"Update dbo.CLASS set  PROFESSOR_ID = @profId, COURSE_ID = @courseId, SECTION_NUM = @secNumb, TEACHING_SEMESTER = @teaching_sem, PROGRESS = @prgrs, ROOM = @rm Where CRN = @class_crn";

            cloasisdbRef.Execute(sql, updateClass);

            return CreatedAtRoute("FetchClass", new { searchTerm = CRN }, updateClass);
        }

        [HttpDelete("[controller]/DeleteClass/{CRN}", Name = "DeleteClass")]
        public IActionResult Delete(string CRN)
        {
            var p = new
            {
                class_crn = CRN,
            };


            string sql = $@"DELETE FROM  dbo.CLASS WHERE CRN = @class_crn";

            cloasisdbRef.Execute(sql, p);

            return Ok();
        }





    }
}