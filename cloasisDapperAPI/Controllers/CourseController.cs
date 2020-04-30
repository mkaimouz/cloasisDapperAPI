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
    public class CourseController : Controller
    {
        public IDbConnection cloasisdbRef = HomeController.cloasisdb;

        [HttpGet("[controller]/GetCourses", Name = "GetCourses")]
        public IActionResult GetCourses()
        {
            var data = cloasisdbRef.Query<Course>("SELECT * FROM dbo.COURSE");

            return Ok(data);
        }

        [HttpGet("[controller]/GetCourseById/{id}", Name = "GetCourseById")]
        public IActionResult GetCoursesById(int id)
        {

            var p = new
            {
                COURSE_ID = id
            };

            var data = cloasisdbRef.Query<Course>("SELECT * FROM dbo.COURSE WHERE COURSE_ID = @COURSE_ID", p);

            return Ok(data);
        }

        [HttpGet("[controller]/FetchCourse/{searchTerm}", Name = "FetchCourse")]
        public IActionResult FetchCourse(string searchTerm)
        {

            var p = new DynamicParameters();
            p.Add("@SearchTerm", searchTerm);

            string sql = "dbo.spCourse_Search";

            var data = cloasisdbRef.Query<Course>(sql, p,
            commandType: CommandType.StoredProcedure);

            return Ok(data);
        }

        [HttpPost("[controller]/CreateCourse", Name = "CreateCourse")]
        public IActionResult Post([FromBody] Course course)
        {
            if (course == null)
            {
                return BadRequest();
            }


            string sql = $@"insert into dbo.Course (COURSE_NAME, COURSE_CODE, DESCRIPTION, CREDITS) 
                                values (@Course_Name, @Course_Code, @Description, @Credits)";

            cloasisdbRef.Execute(sql, course);

            var p = new
            {
                CourseName = course.Course_Name,
                CourseCode = course.Course_Code
            };

            sql = "SELECT * FROM dbo.COURSE WHERE COURSE_NAME = @CourseName and COURSE_CODE=@CourseCode";

            var data = cloasisdbRef.Query<Course>(sql, p);

            return CreatedAtRoute("FetchCourse", new { searchTerm = course.Course_Name }, data.ToList()[0]);
        }

        [HttpPut("[controller]/EditCourse/{CourseId}", Name = "EditCourse")]
        public IActionResult Update([FromBody]Course course, int CourseId)
        {
            var p = new
            {
                crsId = CourseId,
                crs_name = course.Course_Name,
                crs_code = course.Course_Code,
                crs_desc = course.Description,
                credits = course.Credits
            };


            string sql = $@"Update dbo.COURSE set COURSE_NAME = @crs_name, COURSE_CODE = @crs_code, DESCRIPTION = @crs_desc, CREDITS = @credits Where COURSE_ID = @crsId";

            cloasisdbRef.Execute(sql, p);

            return CreatedAtRoute("GetCourseById", new { id = CourseId }, p);
        }

        [HttpDelete("[controller]/DeleteCourse/{CourseId}", Name = "DeleteCourse")]
        public IActionResult Delete(int courseId)
        {
            var p = new
            {
                crsId = courseId,
            };


            string sql = $@"DELETE FROM  dbo.COURSE WHERE COURSE_ID = @crsId";

            cloasisdbRef.Execute(sql, p);

            return Ok();
        }
    }
}