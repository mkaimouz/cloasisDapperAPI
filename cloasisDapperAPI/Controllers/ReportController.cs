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
    public class ReportController : Controller
    {

        public IDbConnection cloasisdbRef = HomeController.cloasisdb;

        [HttpGet("[controller]/GetReports", Name ="GetReports")]
        public IActionResult GetReports()
        {
            var data = cloasisdbRef.Query("SELECT * FROM dbo.REPORT");

            return Ok(data);
        }

        [HttpGet("[controller]/GetReportOfId/{Report_Id}", Name = "GetReportOfId")]
        public IActionResult GetReportOfId(int Report_Id)
        {

            var p = new DynamicParameters();
            p.Add(@"rpt_id",Report_Id);

            var data = cloasisdbRef.Query("SELECT * FROM dbo.REPORT WHERE REPORT_ID = @rpt_id", p);

            if (data.Count() == 0)
            {
                return BadRequest("A report with that has the provided ID does not exist!");
            }

            return Ok(data);
        }

        [HttpGet("[controller]/GetLastReport", Name = "GetLastReport")]
        public IActionResult GetLastReport()
        {
            List<Report> reports = cloasisdbRef.Query<Report>("SELECT * FROM dbo.REPORT").ToList();

            var data = reports[reports.Count - 1];

            return Ok(data);
        }

        [HttpPost("[controller]/CreateReport", Name = "CreateReport")]
        public IActionResult CreateReport()
        {

            DateTime rpt_date = DateTime.Now;

            Report report = new Report()
            {
                Report_Date = rpt_date
            };

            cloasisdbRef.Execute("INSERT INTO dbo.REPORT (REPORT_DATE) VALUES (@Report_Date)",report);

            var data = cloasisdbRef.Query("SELECT * FROM dbo.REPORT WHERE REPORT_DATE = @Report_Date", report);

            return Ok(data);
        }

        [HttpDelete("[controller]/DeleteReport/{Report_Id}", Name = "DeleteReport")]
        public IActionResult DeleteReport(int Report_Id)
        {

            var p = new DynamicParameters();
            p.Add(@"rpt_id", Report_Id);

            var data = cloasisdbRef.Query("SELECT * FROM dbo.REPORT WHERE REPORT_ID = @rpt_id", p);

            if (data.Count() == 0)
            {
                return BadRequest("A report with that has the provided ID does not exist!");
            }

            cloasisdbRef.Execute("DELETE FROM dbo.REPORT WHERE REPORT_ID = @rpt_id", p);

            return Ok($"The report with the ID {Report_Id} has been deleted successfully!");
        }
    }
}