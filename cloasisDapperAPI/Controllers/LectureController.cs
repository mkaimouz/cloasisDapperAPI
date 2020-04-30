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
    public class LectureController : Controller
    {
        public IDbConnection cloasisdbRef = HomeController.cloasisdb;

        //Random r = new Random();

        //[HttpGet("[controller]/FeedData", Name = "FeedData")]
        //public IActionResult FeedData()
        //{
        //    List<Class> classes = cloasisdbRef.Query<Class>("SELECT * FROM dbo.CLASS").ToList();

        //    List<List<DateTime>> firstDates = new List<List<DateTime>>();

        //    List<DateTime> dt1 = new List<DateTime>() { new DateTime(2020, 1, 23, 10, 00, 00), new DateTime(2020, 1, 23, 12, 00, 00), new DateTime(2020, 1, 23, 14, 00, 00), new DateTime(2020, 1, 23, 16, 00, 00), new DateTime(2020, 1, 23, 18, 00, 00), new DateTime(2020, 1, 23, 20, 00, 00) };
        //    List<DateTime> dt2 = new List<DateTime>() { new DateTime(2020, 1, 24, 10, 00, 00), new DateTime(2020, 1, 24, 12, 00, 00), new DateTime(2020, 1, 24, 14, 00, 00), new DateTime(2020, 1, 24, 16, 00, 00), new DateTime(2020, 1, 24, 18, 00, 00), new DateTime(2020, 1, 24, 20, 00, 00) };
        //    List<DateTime> dt3 = new List<DateTime>() { new DateTime(2020, 1, 25, 10, 00, 00), new DateTime(2020, 1, 25, 12, 00, 00), new DateTime(2020, 1, 25, 14, 00, 00), new DateTime(2020, 1, 25, 16, 00, 00), new DateTime(2020, 1, 25, 18, 00, 00), new DateTime(2020, 1, 25, 20, 00, 00) };
        //    List<DateTime> dt4 = new List<DateTime>() { new DateTime(2020, 1, 26, 10, 00, 00), new DateTime(2020, 1, 26, 12, 00, 00), new DateTime(2020, 1, 26, 14, 00, 00), new DateTime(2020, 1, 26, 16, 00, 00), new DateTime(2020, 1, 26, 18, 00, 00), new DateTime(2020, 1, 26, 20, 00, 00) };
        //    List<DateTime> dt5 = new List<DateTime>() { new DateTime(2020, 1, 27, 10, 00, 00), new DateTime(2020, 1, 27, 12, 00, 00), new DateTime(2020, 1, 27, 14, 00, 00), new DateTime(2020, 1, 27, 16, 00, 00), new DateTime(2020, 1, 27, 18, 00, 00), new DateTime(2020, 1, 27, 20, 00, 00) };

        //    firstDates.Add(dt1);
        //    firstDates.Add(dt2);
        //    firstDates.Add(dt3);
        //    firstDates.Add(dt4);
        //    firstDates.Add(dt5);

        //    foreach (Class cls in classes)
        //    {

        //        List<DateTime> randStartDate = firstDates[r.Next(firstDates.Count)];
        //        DateTime randStartTime = randStartDate[r.Next(randStartDate.Count)];

        //        DateTime lecDate = randStartTime;
        //        DateTime MidtermDate = lecDate.AddDays(20);
        //        DateTime FinalDate = lecDate.AddDays(40);

        //        for (int i=0; i < 20; i++)
        //        {

        //            Lecture newLec = new Lecture
        //            {
        //                CRN = cls.CRN,
        //                Duration = 0.85F,
        //                Room = cls.room,
        //                Title = "Lecture No. " + (i+1),
        //                Lecture_Date = lecDate,
        //                Start_Time = lecDate

        //            };

        //            cloasisdbRef.Execute("INSERT INTO dbo.LECTURE (CRN, DURATION, ROOM, START_TIME, TITLE, LECTURE_DATE) VALUES (@CRN, @Duration, @Room, @Start_Time, @Title, @Lecture_Date)", newLec);
        //            lecDate = lecDate.AddDays(2);
        //        }

        //        Lecture Midterm = new Lecture
        //        {
        //            CRN = cls.CRN,
        //            Duration = 1.5F,
        //            Room = cls.room,
        //            Title = "Exam",
        //            Lecture_Date = MidtermDate,
        //            Start_Time = MidtermDate

        //        };

        //        cloasisdbRef.Execute("INSERT INTO dbo.LECTURE (CRN, DURATION, ROOM, START_TIME, TITLE, LECTURE_DATE) VALUES (@CRN, @Duration, @Room, @Start_Time, @Title, @Lecture_Date)", Midterm);

        //        Lecture Final = new Lecture
        //        {
        //            CRN = cls.CRN,
        //            Duration = 1.5F,
        //            Room = cls.room,
        //            Title = "Exam",
        //            Lecture_Date = FinalDate,
        //            Start_Time = FinalDate

        //        };

        //        cloasisdbRef.Execute("INSERT INTO dbo.LECTURE (CRN, DURATION, ROOM, START_TIME, TITLE, LECTURE_DATE) VALUES (@CRN, @Duration, @Room, @Start_Time, @Title, @Lecture_Date)", Final);

        //    }

        //    var data = cloasisdbRef.Query("SELECT * FROM dbo.LECTURE");

        //    return Ok(data);

        //}

        [HttpGet("[controller]/GetLectures", Name = "GetLectures")]
        public IActionResult GetLectures()
        {
            var data = cloasisdbRef.Query<Lecture>("SELECT * FROM dbo.LECTURE");

            return Ok(data);
        }

        [HttpGet("[controller]/GetLectureOfId/{Lecture_Id}", Name = "GetLectureOfId")]
        public IActionResult GetLectureOfId(int Lecture_Id)
        {
            var p = new DynamicParameters();
            p.Add(@"lecId", Lecture_Id);

            var data = cloasisdbRef.Query<Lecture>("SELECT * FROM dbo.LECTURE WHERE LECTURE_ID = @lecId",p);

            return Ok(data);
        }

        [HttpGet("[controller]/GetLecturesOfClass/{CRN}", Name = "GetLecturesOfClass")]
        public IActionResult GetLecturesOfClass(string CRN)
        {

            var p = new DynamicParameters();
            p.Add(@"class_crn", CRN);

            var data = cloasisdbRef.Query<Lecture>("SELECT * FROM dbo.LECTURE WHERE CRN = @class_crn", p);

            return Ok(data);
        }

        [HttpGet("[controller]/GetExams", Name = "GetExams")]
        public IActionResult GetExams()
        {


            var data = cloasisdbRef.Query<Lecture>("SELECT * FROM dbo.LECTURE WHERE TITLE = 'EXAM'");

            return Ok(data);
        }

        [HttpGet("[controller]/GetExamsOfClass/{CRN}", Name = "GetExamsOfClass")]
        public IActionResult GetExamsOfClass(string CRN)
        {

            var p = new DynamicParameters();
            p.Add(@"class_crn", CRN);

            var data = cloasisdbRef.Query<Lecture>("SELECT * FROM dbo.LECTURE WHERE TITLE = 'EXAM' AND CRN = @class_crn", p);

            return Ok(data);
        }
        [HttpGet("[controller]/GetExamsDetailsOfClass/{CRN}", Name = "GetExamsDetailsOfClass")]
        public IActionResult GetExamsDetailsOfClass(string CRN)
        {

            var p = new DynamicParameters();
            p.Add(@"class_crn", CRN);

            var data = cloasisdbRef.Query(@"SELECT lec.*, cl.*, crs.COURSE_NAME, crs.COURSE_CODE, cls.SECTION_NUM FROM dbo.LECTURE lec
                                            INNER JOIN dbo.DELIVERS del
                                            ON del.LECTURE_ID = lec.LECTURE_ID
                                            INNER JOIN dbo.CLO cl
                                            ON cl.CLO_ID = del.CLO_ID
                                            INNER JOIN COURSE crs
                                            ON crs.COURSE_ID = cl.COURSE_ID
                                            INNER JOIN CLASS cls
                                            ON cls.CRN = lec.CRN
                                            WHERE lec.TITLE = 'EXAM' 
                                            AND lec.CRN = @class_crn", p);

            return Ok(data);
        }

        [HttpPost("[controller]/CreateLecture", Name = "CreateLecture")]
        public IActionResult CreateLecture([FromBody]Lecture lec)
        {
            if (lec == null)
            {
                return BadRequest("Please pass a lecture in the body of the request");
            }

            var p = new DynamicParameters();
            p.Add(@"class_crn", lec.CRN);

            List<Class> classes = cloasisdbRef.Query<Class>("SELECT * FROM Class WHERE CRN = @class_crn", p).ToList();

            if (classes.Count == 0)
            {
                return BadRequest("Please pass a valid CRN");
            }

            cloasisdbRef.Execute(@"INSERT INTO dbo.LECTURE (CRN, DURATION, ROOM, START_TIME, TITLE, LECTURE_DATE)
                                    VALUES (@CRN, @Duration, @Room, @Start_Time, @Title, @Lecture_Date)", lec);

            var data = cloasisdbRef.Query(@"SELECT * FROM dbo.LECTURE WHERE START_TIME = @Start_Time AND LECTURE_DATE = @Lecture_Date AND TITLE = @Title AND CRN = @CRN", lec);

            return Ok(data);
        }

        [HttpPost("[controller]/CreateLectureWithCLO/{CLO_ID}", Name = "CreateLectureWithCLO")]
        public IActionResult CreateLectureWithCLO([FromBody]Lecture lec, int CLO_ID)
        {
            if (lec == null)
            {
                return BadRequest("Please pass a lecture in the body of the request");
            }

            var p = new DynamicParameters();
            p.Add(@"class_crn", lec.CRN);

            List<Class> classes = cloasisdbRef.Query<Class>("SELECT * FROM Class WHERE CRN = @class_crn", p).ToList();

            if (classes.Count == 0)
            {
                return BadRequest("Please pass a valid CRN");
            }

            p.Add(@"cloId", CLO_ID);

            List<CLO> clos = cloasisdbRef.Query<CLO>("SELECT * FROM dbo.CLO WHERE CLO_ID = @cloId",p).ToList();

            if (clos.Count == 0)
            {
                return BadRequest("This CLO ID does not exist");
            }

            

            cloasisdbRef.Execute(@"INSERT INTO dbo.LECTURE (CRN, DURATION, ROOM, START_TIME, TITLE, LECTURE_DATE)
                                    VALUES (@CRN, @Duration, @Room, @Start_Time, @Title, @Lecture_Date)", lec);

            Lecture createdLec = cloasisdbRef.Query<Lecture>(@"SELECT * FROM dbo.LECTURE WHERE START_TIME = @Start_Time AND LECTURE_DATE = @Lecture_Date AND TITLE = @Title AND CRN = @CRN", lec).ToList()[0];

            var x = new DynamicParameters();
            x.Add(@"cloId", CLO_ID);
            x.Add(@"lecId", createdLec.Lecture_Id);

            cloasisdbRef.Execute(@"INSERT INTO dbo.DELIVERS (CLO_ID, LECTURE_ID)
                                    VALUES (@cloId, @lecId)",x);

            List<Object> returedList = new List<Object>();

            var data1 = cloasisdbRef.Query(@"SELECT * FROM dbo.LECTURE WHERE START_TIME = @Start_Time AND LECTURE_DATE = @Lecture_Date AND TITLE = @Title AND CRN = @CRN", lec);
            var data2 = cloasisdbRef.Query(@"SELECT * FROM dbo.DELIVERS WHERE CLO_ID = @cloId AND LECTURE_ID = @lecId", x);

            returedList.Add(data1);
            returedList.Add(data2);

            return Ok(returedList);
        }

        [HttpPut("[controller]/EditLecture/{Lecture_Id}", Name = "EditLecture")]
        public IActionResult EditLecture([FromBody]Lecture lec, int Lecture_Id)
        {
            if (lec == null)
            {
                return BadRequest("Please pass a lecture in the body of the request");
            }

            var p = new DynamicParameters();
            p.Add(@"lecId", Lecture_Id);

            List<Lecture> lecture = cloasisdbRef.Query<Lecture>(@"SELECT * FROM dbo.LECTURE WHERE LECTURE_ID = @lecId", p).ToList();

            if (lecture.Count == 0)
            {
                return BadRequest("Please pass a valid Lecture ID");
            }

            p.Add(@"class_crn", lec.CRN);

            List<Class> classes = cloasisdbRef.Query<Class>("SELECT * FROM Class WHERE CRN = @class_crn", p).ToList();

            if (classes.Count == 0)
            {
                return BadRequest("Please pass a valid CRN");
            }

            Lecture updatedLec = new Lecture
            {
                Lecture_Id = Lecture_Id,
                CRN = lec.CRN,
                Duration = lec.Duration,
                Lecture_Date = lec.Lecture_Date,
                Room = lec.Room,
                Start_Time = lec.Start_Time,
                Title = lec.Title
            };

            cloasisdbRef.Execute(@"UPDATE dbo.LECTURE SET CRN = @CRN , DURATION = @Duration, ROOM = @Room, START_TIME = @Start_Time, TITLE = @Title, LECTURE_DATE = @Lecture_Date WHERE LECTURE_ID = @Lecture_Id", updatedLec);

            var data = cloasisdbRef.Query(@"SELECT * FROM dbo.LECTURE WHERE LECTURE_ID = @lecId", p);

            return Ok(data);
        }

        [HttpPut("[controller]/EditLectureWithCLO/{Lecture_Id}/{CLO_ID}", Name = "EditLectureWithCLO")]
        public IActionResult EditLecture([FromBody]Lecture lec, int Lecture_Id, int CLO_ID)
        {
            if (lec == null)
            {
                return BadRequest("Please pass a lecture in the body of the request");
            }

            var p = new DynamicParameters();
            p.Add(@"lecId", Lecture_Id);

            List<Lecture> lecture = cloasisdbRef.Query<Lecture>(@"SELECT * FROM dbo.LECTURE WHERE LECTURE_ID = @lecId", p).ToList();

            if (lecture.Count == 0)
            {
                return BadRequest("Please pass a valid Lecture ID");
            }

            p.Add(@"class_crn", lec.CRN);

            List<Class> classes = cloasisdbRef.Query<Class>("SELECT * FROM Class WHERE CRN = @class_crn", p).ToList();

            if (classes.Count == 0)
            {
                return BadRequest("Please pass a valid CRN");
            }

            p.Add(@"cloId", CLO_ID);

            List<CLO> clos = cloasisdbRef.Query<CLO>("SELECT * FROM dbo.CLO WHERE CLO_ID = @cloId", p).ToList();

            if (clos.Count == 0)
            {
                return BadRequest("This CLO ID does not exist");
            }

            Lecture updatedLec = new Lecture
            {
                Lecture_Id = Lecture_Id,
                CRN = lec.CRN,
                Duration = lec.Duration,
                Lecture_Date = lec.Lecture_Date,
                Room = lec.Room,
                Start_Time = lec.Start_Time,
                Title = lec.Title
            };

            var x = new DynamicParameters();
            x.Add(@"cloId", CLO_ID);
            x.Add(@"lecId", Lecture_Id);

            cloasisdbRef.Execute(@"UPDATE dbo.DELIVERS SET CLO_ID = @cloId WHERE LECTURE_ID = @lecId", x);

            cloasisdbRef.Execute(@"UPDATE dbo.LECTURE SET CRN = @CRN , DURATION = @Duration, ROOM = @Room, START_TIME = @Start_Time, TITLE = @Title, LECTURE_DATE = @Lecture_Date WHERE LECTURE_ID = @Lecture_Id", updatedLec);

            var data1 = cloasisdbRef.Query(@"SELECT * FROM dbo.LECTURE WHERE LECTURE_ID = @lecId", p);
            var data2 = cloasisdbRef.Query(@"SELECT * FROM dbo.DELIVERS WHERE LECTURE_ID = @lecId", p);

            List<Object> returedList = new List<Object>();

            returedList.Add(data1);
            returedList.Add(data2);

            return Ok(returedList);
        }

        [HttpDelete("[controller]/DeleteLecture/{Lecture_Id}", Name = "DeleteLecture")]
        public IActionResult DeleteLecture(int Lecture_Id)
        {

            var p = new DynamicParameters();
            p.Add(@"lecId", Lecture_Id);

            List<Lecture> lecture = cloasisdbRef.Query<Lecture>(@"SELECT * FROM dbo.LECTURE WHERE LECTURE_ID = @lecId", p).ToList();

            if (lecture.Count == 0)
            {
                return BadRequest("Please pass a valid Lecture ID");
            }

            List<Object> hasCLO = cloasisdbRef.Query<Object>("SELECT * FROM dbo.DELIVERS WHERE LECTURE_ID = @lecId", p).ToList();

            if (hasCLO.Count > 0)
            {
                cloasisdbRef.Execute("DELETE FROM dbo.DELIVERS WHERE LECTURE_ID = @lecId", p);
            }



            cloasisdbRef.Execute(@"DELETE FROM dbo.LECTURE WHERE LECTURE_ID = @lecId", p);

            return Ok($"The Lecture with ID {Lecture_Id} has been deleted successfully!");
        }
    }
}