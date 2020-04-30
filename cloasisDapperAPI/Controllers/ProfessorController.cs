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
    public class ProfessorController : Controller
    {
        public IDbConnection cloasisdbRef = HomeController.cloasisdb;

        [HttpGet("[controller]/GetProfessors", Name = "GetProfessors")]
        public IActionResult GetProfessors()
        {
            var data = cloasisdbRef.Query<Professor>("SELECT * FROM dbo.PROFESSOR");

            return Ok(data);
        }

        [HttpGet("[controller]/FetchProfessor/{searchTerm}", Name = "FetchProfessor")]
        public IActionResult FetchProfessor(string searchTerm)
        {

            var p = new DynamicParameters();
            p.Add("@SearchTerm", searchTerm);

            string sql = "dbo.spProfessor_Search";

            var data = cloasisdbRef.Query<Professor>(sql, p,
            commandType: CommandType.StoredProcedure);

            return Ok(data);
        }

        [HttpPost("[controller]/CreateProfessor", Name = "CreateProfessor")]
        public IActionResult Post([FromBody] Professor prof)
        {
            if (prof == null)
            {
                return BadRequest();
            }


            string sql = $@"insert into dbo.PROFESSOR (NAME, EMAIL, GENDER, OFFICE, EXTENSION, IMAGEPATH) 
                                values (@name, @email, @gender, @office, @extension, @imagePath)";

            cloasisdbRef.Execute(sql, prof);

            var p = new DynamicParameters();
            p.Add("@SearchTerm", prof.name);

            sql = "dbo.spProfessor_Search";

            var data = cloasisdbRef.Query<Professor>(sql, p,
            commandType: CommandType.StoredProcedure);

            return CreatedAtRoute("FetchCourse", new { searchTerm = prof.name }, data.ToList()[0]);
        }
        [HttpPut("[controller]/EditProfessor/{professorId}", Name = "EditProfessor")]
        public IActionResult Update([FromBody]Professor professor, int professorId)
        {
            var p = new
            {
                profId = professorId,
                profName = professor.name,
                profEmail = professor.email,
                profGender = professor.gender,
                profOffice = professor.office,
                profExtension = professor.extension,
                profImage = professor.imagePath
            };


            string sql = $@"Update dbo.PROFESSOR set NAME = @profName, EMAIL = @profEmail, GENDER = @profGender, OFFICE = @profOffice, EXTENSION = @profExtension, IMAGEPATH = @profImage Where PROFESSOR_ID = @profId";

            cloasisdbRef.Execute(sql, p);

            return CreatedAtRoute("FetchProfessor", new { searchTerm = professorId }, p);
        }

        [HttpDelete("[controller]/DeleteProfessor/{professorId}", Name = "DeleteProfessor")]
        public IActionResult Delete(int professorId)
        {
            var p = new
            {
                profId = professorId,
            };


            string sql = $@"DELETE FROM  dbo.PROFESSOR WHERE PROFESSOR_ID = @profId";

            cloasisdbRef.Execute(sql, p);

            return Ok();
        }
    }
}