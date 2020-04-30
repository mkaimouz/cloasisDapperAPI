using cloasisDapperAPI.Models;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace cloasisDapperAPI.Controllers
{

    public class StudentController : Controller
    {
        public IDbConnection cloasisdbRef = HomeController.cloasisdb;

        [HttpGet("[controller]/GetStudents", Name = "GetStudents")]
        public IActionResult GetStudents()
        {
            var data = cloasisdbRef.Query<Student>("SELECT * FROM dbo.STUDENT");

            return Ok(data);
        }

        [HttpGet("[controller]/FetchStudent/{searchTerm}", Name = "FetchStudent")]
        public IActionResult FetchStudent(string searchTerm)
        {

            var p = new DynamicParameters();
            p.Add("@SearchTerm", searchTerm);

            string sql = "dbo.spStudent_Search";

            var data = cloasisdbRef.Query<Student>(sql, p,
            commandType: CommandType.StoredProcedure);

            return Ok(data);
        }

        [HttpPost("[controller]/CreateStudent", Name = "CreateStudent")]
        public IActionResult Post([FromBody] Student student)
        {
            if (student == null)
            {
                return BadRequest();
            }


            var p = new DynamicParameters();
            p.Add("@SearchTerm", student.STUDENTID);
            
            string sql = "dbo.spStudent_Search";
            var data = cloasisdbRef.Query<Student>(sql, p,
            commandType: CommandType.StoredProcedure);

            p.Add("@team_id", student.TEAM_ID);
            var isTeam = cloasisdbRef.Query("SELECT * FROM dbo.TEAM WHERE TEAM_ID = @team_id", p);

            if (data.Count() > 0)
            {
                return BadRequest(new {error = "This STUDENTID Exists in the DB" });
            }

            if (isTeam.Count() == 0 && student.TEAM_ID != null)
            {
                return BadRequest(new { error = "This TEAM_ID Does Not Exist in the DB" });
            }

            sql = $@"insert into dbo.Student (STUDENTID, TEAM_ID, NAME, EMAIL, PHONE, DOB, GENDER) 
                                values (@STUDENTID, @TEAM_ID, @NAME, @EMAIL, @PHONE, @DOB, @GENDER)";

            cloasisdbRef.Execute(sql, student);

            return CreatedAtRoute("FetchStudent", new { searchTerm = student.STUDENTID }, student);
        }

        [HttpPut("[controller]/EditStudent/{StudentId}", Name = "EditStudent")]
        public IActionResult Update([FromBody]Student student, string StudentId)
        {
            var p = new
            {
                stdID = StudentId,
                name = student.NAME,
                team_id = student.TEAM_ID,
                email = student.EMAIL,
                phone = student.PHONE,
                dob = student.DOB,
                gender = student.GENDER
            };

            var isTeam = cloasisdbRef.Query("SELECT * FROM dbo.TEAM WHERE TEAM_ID = @team_id",p);

            string sql;

            if (isTeam.Count() == 0 && student.TEAM_ID != null)
            {
                var x = new
                {
                    stdID = StudentId,
                    name = student.NAME,
                    team_id = "TEAM_ID DOES NOT EXIST in the DB",
                    email = student.EMAIL,
                    phone = student.PHONE,
                    dob = student.DOB,
                    gender = student.GENDER
                };

                sql = $@"Update dbo.STUDENT set NAME = @name, EMAIL = @email, PHONE = @phone, DOB = @dob, GENDER = @gender Where STUDENTID = @stdId";

                cloasisdbRef.Execute(sql, x);

                return CreatedAtRoute("FetchStudent", new { searchTerm = StudentId }, x);

            }

            sql = $@"Update dbo.STUDENT set NAME = @name, TEAM_ID = @team_id, EMAIL = @email, PHONE = @phone, DOB = @dob, GENDER = @gender Where STUDENTID = @stdId";

            cloasisdbRef.Execute(sql, p);

            return CreatedAtRoute("FetchStudent", new { searchTerm = StudentId }, p);
        }

        [HttpDelete("[controller]/DeleteStudent/{StudentId}", Name = "DeleteStudent")]
        public IActionResult Delete(string StudentId)
        {
            var p = new
            {
                stdID = StudentId,
            };


            string sql = $@"DELETE FROM  dbo.STUDENT WHERE STUDENTID = @stdId";

            cloasisdbRef.Execute(sql, p);

            return Ok();
        }




    }
}
