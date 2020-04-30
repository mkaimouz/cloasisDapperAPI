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
    public class TeamController : Controller
    {

        public IDbConnection cloasisdbRef = HomeController.cloasisdb;

        [HttpGet("[controller]/GetTeams", Name = "GetTeams")]
        public IActionResult GetTeams()
        {
            var data = cloasisdbRef.Query("SELECT tm.TEAM_ID, cr.COURSE_NAME, cr.COURSE_CODE, cls.SECTION_NUM, tm.TEAM_NAME, tm.CREATION_DATE FROM dbo.TEAM tm LEFT JOIN dbo.CLASS cls ON cls.CRN = tm.CRN LEFT JOIN dbo.COURSE cr ON cls.COURSE_ID = cr.COURSE_ID");

            return Ok(data);
        }

        [HttpGet("[controller]/FetchTeam/{searchTerm}", Name = "FetchTeam")]
        public IActionResult FetchTeam(string searchTerm)
        {

            var p = new DynamicParameters();
            p.Add("@SearchTerm", searchTerm);

            string sql = @"SELECT tm.TEAM_ID, cr.COURSE_NAME, cr.COURSE_CODE, cls.SECTION_NUM, tm.TEAM_NAME, tm.CREATION_DATE FROM dbo.TEAM tm 
                            LEFT JOIN dbo.CLASS cls ON cls.CRN = tm.CRN 
                            LEFT JOIN dbo.COURSE cr ON cls.COURSE_ID = cr.COURSE_ID
                               WHERE tm.TEAM_ID LIKE @searchTerm 
                                OR cr.COURSE_NAME LIKE @searchTerm
                                OR cr.COURSE_CODE LIKE '%' + @searchTerm + '%'
                                OR tm.TEAM_NAME LIKE '%' + @searchTerm + '%'
                                OR tm.CREATION_DATE LIKE '%' + @searchTerm + '%'";

            var data = cloasisdbRef.Query(sql, p);

            return Ok(data);
        }

        [HttpPost("[controller]/CreateTeam", Name = "CreateTeam")]
        public IActionResult Post([FromBody] Team team)
        {
            if (team == null)
            {
                return BadRequest();
            }

            if (team.CREATION_DATE == null)
            {
                team.CREATION_DATE = DateTime.Now;
            }

            string sql = $@"insert into dbo.TEAM (TEAM_NAME, CRN, CREATION_DATE) 
                                values (@TEAM_NAME, @CRN, @CREATION_DATE)";

            cloasisdbRef.Execute(sql, team);

            var p = new DynamicParameters();
            p.Add("@SearchTerm", team.TEAM_NAME);

            sql = @"SELECT tm.TEAM_ID, cr.COURSE_NAME, cr.COURSE_CODE, cls.SECTION_NUM, tm.TEAM_NAME, tm.CREATION_DATE FROM dbo.TEAM tm 
                            LEFT JOIN dbo.CLASS cls ON cls.CRN = tm.CRN 
                            LEFT JOIN dbo.COURSE cr ON cls.COURSE_ID = cr.COURSE_ID
                               WHERE cr.COURSE_NAME LIKE @searchTerm
                                OR cr.COURSE_CODE LIKE '%' + @searchTerm + '%'
                                OR tm.TEAM_NAME LIKE '%' + @searchTerm + '%'
                                OR tm.CREATION_DATE LIKE '%' + @searchTerm + '%'";

            var data = cloasisdbRef.Query(sql, p);

            return CreatedAtRoute("FetchTeam", new { searchTerm = team.TEAM_NAME }, data.ToList()[0]);
        }

        [HttpPut("[controller]/EditTeam/{Team_Id}", Name = "EditTeam")]
        public IActionResult Update([FromBody]Team team, int Team_Id)
        {
            var p = new
            {
                tm_Id = Team_Id,
                tm_Name = team.TEAM_NAME,
                tm_CRN = team.CRN,
                tm_createDate = team.CREATION_DATE
            };


            string sql = $@"Update dbo.TEAM set TEAM_NAME = @tm_Name, CRN = @tm_CRN, CREATION_DATE = @tm_createDate Where TEAM_ID = @tm_Id";

            cloasisdbRef.Execute(sql, p);

            return CreatedAtRoute("FetchTeam", new { searchTerm = team.TEAM_NAME }, p);
        }

        [HttpDelete("[controller]/DeleteTeam/{teamId}", Name = "DeleteTeam")]
        public IActionResult Delete(int teamId)
        {
            var p = new
            {
                tmId = teamId,
            };


            string sql = $@"DELETE FROM  dbo.TEAM WHERE TEAM_ID = @tmId";

            cloasisdbRef.Execute(sql, p);

            return Ok();
        }
    }
}