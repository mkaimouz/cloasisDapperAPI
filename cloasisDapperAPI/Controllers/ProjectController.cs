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
    public class ProjectController : Controller
    {

        public IDbConnection cloasisdbRef = HomeController.cloasisdb;

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet("[controller]/GetProjects", Name = "GetProjects")]
        public IActionResult GetProjects()
        {
            string sql = @"SELECT PROJECT_ID Project_ID, dbo.TEAM.TEAM_NAME team, PROJECT_TITLE Project_Title, PROJECT_DESC Project_Desc
                           FROM dbo.PROJECT
                           LEFT JOIN dbo.TEAM ON dbo.PROJECT.TEAM_ID = dbo.TEAM.TEAM_ID";

            var data = cloasisdbRef.Query<Project>(sql);

            return Ok(data);
        }

        [HttpGet("[controller]/FetchProject/{searchTerm}", Name = "FetchProject")]
        public IActionResult FetchProject(string searchTerm)
        {

            var p = new DynamicParameters();
            p.Add("@SearchTerm", searchTerm);

            string sql = @"SELECT PROJECT_ID Project_ID, dbo.TEAM.TEAM_NAME team, PROJECT_TITLE Project_Title, PROJECT_DESC Project_Desc
                           FROM dbo.PROJECT
                           LEFT JOIN dbo.TEAM ON dbo.PROJECT.TEAM_ID = dbo.TEAM.TEAM_ID
                               WHERE PROJECT_ID LIKE @searchTerm
                                OR dbo.TEAM.TEAM_NAME LIKE '%' + @searchTerm + '%'
                                OR PROJECT_TITLE LIKE '%' + @searchTerm + '%'";

            var data = cloasisdbRef.Query(sql, p);

            return Ok(data);
        }

        [HttpPost("[controller]/CreateProject", Name = "CreateProject")]
        public IActionResult Post([FromBody] Project prj)
        {
            if (prj == null)
            {
                return BadRequest();
            }

            
            string teamName = prj.Team;

            var p = new DynamicParameters();
            p.Add("@teamName", teamName);

            string sql = @"SELECT * FROM dbo.TEAM WHERE TEAM_NAME LIKE @teamName";

            Team team = cloasisdbRef.Query<Team>(sql, p).ToList()[0];

            var newPrj = new 
            {
                ProjectTitle = prj.Project_Title,
                ProjectDesc = prj.Project_Desc,
                TeamId = team.TEAM_ID
            };

            sql = $@"insert into dbo.PROJECT (TEAM_ID, PROJECT_TITLE, PROJECT_DESC) 
                                values (@TeamId, @ProjectTitle, @ProjectDesc)";


            cloasisdbRef.Execute(sql, newPrj);

            p = new DynamicParameters();
            p.Add("@SearchTerm", prj.Project_Title);

            sql = @"SELECT PROJECT_ID Project_ID, dbo.TEAM.TEAM_NAME team, PROJECT_TITLE Project_Title, PROJECT_DESC Project_Desc
                           FROM dbo.PROJECT
                           LEFT JOIN dbo.TEAM ON dbo.PROJECT.TEAM_ID = dbo.TEAM.TEAM_ID
                               WHERE PROJECT_ID LIKE @searchTerm
                                OR dbo.TEAM.TEAM_NAME LIKE '%' + @searchTerm + '%'
                                OR PROJECT_TITLE LIKE '%' + @searchTerm + '%'";

            var data = cloasisdbRef.Query(sql, p);

            return CreatedAtRoute("FetchTeam", new { searchTerm = team.TEAM_NAME }, data.ToList()[0]);
        }

        [HttpPut("[controller]/EditProject/{ProjectId}", Name = "EditProject")]
        public IActionResult Update([FromBody]Project prj, int ProjectId)
        {

            string teamName = prj.Team;

            var x = new DynamicParameters();
            x.Add("@teamName", teamName);

            string sql = @"SELECT * FROM dbo.TEAM WHERE TEAM_NAME=@teamName";

            Team team = cloasisdbRef.Query<Team>(sql, x).ToList()[0];

            var p = new
            {
                prj_Id = ProjectId,
                prj_Title = prj.Project_Title,
                tm_Id = team.TEAM_ID,
                prj_desc = prj.Project_Desc
            };


            sql = $@"Update dbo.PROJECT set PROJECT_TITLE = @prj_Title, TEAM_ID = @tm_Id, PROJECT_DESC = @prj_desc Where PROJECT_ID = @prj_Id";

            cloasisdbRef.Execute(sql, p);

            return CreatedAtRoute("FetchProject", new { searchTerm = prj.Project_Title }, p);
        }

        [HttpDelete("[controller]/DeleteProject/{ProjectId}", Name = "DeleteProject")]
        public IActionResult Delete(int ProjectId)
        {
            var p = new
            {
                prj_Id = ProjectId,
            };


            string sql = $@"DELETE FROM  dbo.PROJECT WHERE PROJECT_ID = @prj_Id";

            cloasisdbRef.Execute(sql, p);

            return Ok();
        }
    }
}