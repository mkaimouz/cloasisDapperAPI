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
    public class AttendanceController : Controller
    {

        public IDbConnection cloasisdbRef = HomeController.cloasisdb;

        //Random r = new Random();

        //[HttpGet("[controller]/FeedData", Name = "FeedData")]
        //public IActionResult FeedData()
        //{
        //    List<Lecture> lectures = cloasisdbRef.Query<Lecture>("SELECT * FROM dbo.LECTURE").ToList();

        //    foreach (Lecture lecture in lectures)
        //    {
        //        var p = new DynamicParameters();
        //        p.Add("@crn", lecture.CRN);

        //        List<Student> studentInClass = cloasisdbRef.Query<Student>(@"SELECT std.* FROM dbo.ENROLLMENT en 
        //                                        INNER JOIN dbo.CLASS cls ON cls.CRN = en.CRN INNER JOIN dbo.COURSE crs ON cls.COURSE_ID = crs.COURSE_ID 
        //                                        INNER JOIN dbo.STUDENT std ON en.STUDENTID = std.STUDENTID 
        //                                        WHERE en.CRN LIKE @crn", p).ToList();

        //        int num_values = studentInClass.Count - r.Next(0, 3);
        //        if (num_values <= 0)
        //        {
        //            num_values = studentInClass.Count - 1;
        //        }

        //        List<Student> studentsPresent = RandomTools.PickRandom<Student>(studentInClass.ToArray(), num_values);

        //        foreach (Student student in studentsPresent)
        //        {
        //            var x = new
        //            {
        //                stdId = student.STUDENTID,
        //                lectureId = lecture.Lecture_Id
        //            };

        //            cloasisdbRef.Execute("INSERT INTO dbo.ATTENDANCE (STUDENTID, LECTURE_ID) VALUES (@stdId, @lectureId)", x);
        //        }

        //    }
        //    var data = cloasisdbRef.Query("SELECT * FROM dbo.ATTENDANCE");

        //    return Ok(data);
        //}

        [HttpGet("[controller]/GetAttendance", Name = "GetAttendance")]
        public IActionResult GetAttendance()
        {
            //Returns a detailed object for all entries in the Attendance table --Takes a considerable time to compute
            List <Object> entries = new List<Object>();
            List<int> lectureIds = cloasisdbRef.Query<int>("SELECT LECTURE_ID FROM ATTENDANCE GROUP BY LECTURE_ID ORDER BY LECTURE_ID;").ToList();

            foreach (int id in lectureIds)
            {
                var p = new DynamicParameters();
                p.Add(@"lecId", id);

                Lecture lecture = cloasisdbRef.Query<Lecture>("SELECT * FROM dbo.LECTURE WHERE LECTURE_ID = @lecId", p).ToList()[0];

                int attendanceCount = cloasisdbRef.Query<int>(@"SELECT COUNT(STUDENTID) Attended 
                                                                FROM dbo.ATTENDANCE 
                                                                WHERE LECTURE_ID=@lecId 
                                                                GROUP BY LECTURE_ID", p).ToList()[0];

                p.Add(@"crn", lecture.CRN);

                var studentsAttended = cloasisdbRef.Query(@"SELECT std.STUDENTID, std.NAME 
                                                            FROM dbo.ATTENDANCE atn
                                                            INNER JOIN dbo.STUDENT std ON atn.STUDENTID = std.STUDENTID
                                                            WHERE LECTURE_ID=@lecId", p);

                Class cls = cloasisdbRef.Query<Class>(@"SELECT cls.CRN, cls.PROFESSOR_ID, crs.COURSE_NAME, cls.SECTION_NUM 
                                                        FROM dbo.LECTURE lec
                                                        INNER JOIN dbo.CLASS cls ON cls.CRN = lec.CRN
                                                        INNER JOIN dbo.COURSE crs ON crs.COURSE_ID = cls.COURSE_ID
                                                        WHERE LECTURE_ID = @lecId", p).ToList()[0];

                int totalRegistered = cloasisdbRef.Query<int>(@"SELECT COUNT(en.STUDENTID) Registered
                                                                FROM dbo.LECTURE lec
                                                                INNER JOIN dbo.ENROLLMENT en ON en.CRN = lec.CRN
                                                                WHERE lec.LECTURE_ID = @lecId
                                                                GROUP BY(LECTURE_ID)", p).ToList()[0];

                var attn_entry = new
                {
                    CourseName = cls.Course_Name,
                    LectureTitle = lecture.Title,
                    CRN = cls.CRN,
                    Lecture_Id = lecture.Lecture_Id,
                    TotalStudentRegistered = totalRegistered,
                    LectureAttendanceCount = attendanceCount,
                    StudentsAttended = studentsAttended

                };

                entries.Add(attn_entry);

            }

            return Ok(entries);
        }

        [HttpGet("[controller]/GetAttendanceOfLecture/{lectureId}", Name = "GetAttendanceOfLecture")]
        public IActionResult GetAttendaceOfLecture(int lectureId)
        {
            var p = new DynamicParameters();
            p.Add(@"lecId", lectureId);

            Lecture lecture = cloasisdbRef.Query<Lecture>("SELECT * FROM dbo.LECTURE WHERE LECTURE_ID = @lecId", p).ToList()[0];

            Class cls = cloasisdbRef.Query<Class>(@"SELECT cls.CRN, cls.PROFESSOR_ID, crs.COURSE_NAME, cls.SECTION_NUM 
                                                        FROM dbo.LECTURE lec
                                                        INNER JOIN dbo.CLASS cls ON cls.CRN = lec.CRN
                                                        INNER JOIN dbo.COURSE crs ON crs.COURSE_ID = cls.COURSE_ID
                                                        WHERE LECTURE_ID = @lecId", p).ToList()[0];

            int attendanceCount = cloasisdbRef.Query<int>(@"SELECT COUNT(STUDENTID) Attended 
                                                                FROM dbo.ATTENDANCE 
                                                                WHERE LECTURE_ID=@lecId 
                                                                GROUP BY LECTURE_ID", p).ToList()[0];

            int totalRegistered = cloasisdbRef.Query<int>(@"SELECT COUNT(en.STUDENTID) Registered
                                                                FROM dbo.LECTURE lec
                                                                INNER JOIN dbo.ENROLLMENT en ON en.CRN = lec.CRN
                                                                WHERE lec.LECTURE_ID = @lecId
                                                                GROUP BY(LECTURE_ID)", p).ToList()[0];

            var studentsAttended = cloasisdbRef.Query(@"SELECT std.STUDENTID, std.NAME 
                                                            FROM dbo.ATTENDANCE atn
                                                            INNER JOIN dbo.STUDENT std ON atn.STUDENTID = std.STUDENTID
                                                            WHERE LECTURE_ID=@lecId", p);

            var attn_entry = new
            {
                CourseName = cls.Course_Name,
                LectureTitle = lecture.Title,
                CRN = cls.CRN,
                TotalStudentRegistered = totalRegistered,
                LectureAttendanceCount = attendanceCount,
                StudentsAttended = studentsAttended

            };

            return Ok(attn_entry);
        }

        [HttpGet("[controller]/GetAttendanceOfClass/{CRN}", Name = "GetAttendanceOfClass")]
        public IActionResult GetAttendanceOfClass(string CRN)
        {

            List<Object> entries = new List<Object>();

            var p = new DynamicParameters();
            p.Add(@"class_crn", CRN);

            List<Lecture> lectures = cloasisdbRef.Query<Lecture>("SELECT * FROM dbo.LECTURE WHERE CRN = @class_crn", p).ToList();

            foreach (Lecture lecture in lectures)
            {

                p.Add(@"lecId", lecture.Lecture_Id);

                Class cls = cloasisdbRef.Query<Class>(@"SELECT cls.CRN, cls.PROFESSOR_ID, crs.COURSE_NAME, cls.SECTION_NUM 
                                                        FROM dbo.LECTURE lec
                                                        INNER JOIN dbo.CLASS cls ON cls.CRN = lec.CRN
                                                        INNER JOIN dbo.COURSE crs ON crs.COURSE_ID = cls.COURSE_ID
                                                        WHERE LECTURE_ID = @lecId", p).ToList()[0];

                int attendanceCount = cloasisdbRef.Query<int>(@"SELECT COUNT(STUDENTID) Attended 
                                                                FROM dbo.ATTENDANCE 
                                                                WHERE LECTURE_ID=@lecId 
                                                                GROUP BY LECTURE_ID", p).ToList()[0];

                int totalRegistered = cloasisdbRef.Query<int>(@"SELECT COUNT(en.STUDENTID) Registered
                                                                FROM dbo.LECTURE lec
                                                                INNER JOIN dbo.ENROLLMENT en ON en.CRN = lec.CRN
                                                                WHERE lec.LECTURE_ID = @lecId
                                                                GROUP BY(LECTURE_ID)", p).ToList()[0];

                var studentsAttended = cloasisdbRef.Query(@"SELECT std.STUDENTID, std.NAME 
                                                            FROM dbo.ATTENDANCE atn
                                                            INNER JOIN dbo.STUDENT std ON atn.STUDENTID = std.STUDENTID
                                                            WHERE LECTURE_ID=@lecId", p);

                var attn_entry = new
                {
                    CourseName = cls.Course_Name,
                    LectureTitle = lecture.Title,
                    CRN = cls.CRN,
                    TotalStudentRegistered = totalRegistered,
                    LectureAttendanceCount = attendanceCount,
                    StudentsAttended = studentsAttended

                };

                entries.Add(attn_entry);
            }

            return Ok(entries);
        }


        [HttpGet("[controller]/GetAttendanceCount", Name = "GetAttendanceCount")]
        public IActionResult GetAttendanceCount()
        {
            var data = cloasisdbRef.Query(@"SELECT Std_Attendance.STUDENTID, Std.NAME, Std_Attendance.Lectures_Attended, LecturesCounter.Lectures_Total, cls.CRN, crs.COURSE_NAME, crs.COURSE_CODE, cls.SECTION_NUM  FROM dbo.STUDENT Std
                                            INNER JOIN
                                            (
	                                            SELECT att.STUDENTID, COUNT(att.LECTURE_ID) Lectures_Attended, lec.CRN FROM ATTENDANCE att
	                                            INNER JOIN LECTURE lec ON lec.LECTURE_ID = att.LECTURE_ID
	                                            INNER JOIN dbo.STUDENT std ON std.STUDENTID = att.STUDENTID
	                                            GROUP BY att.STUDENTID, lec.CRN
                                            ) Std_Attendance
                                            ON Std_Attendance.STUDENTID = Std.STUDENTID
                                            LEFT JOIN dbo.CLASS cls ON cls.CRN = Std_Attendance.CRN
                                            LEFT JOIN dbo.COURSE crs ON crs.COURSE_ID = cls.COURSE_ID
                                            INNER JOIN
                                            (
	                                            SELECT COUNT(lec.LECTURE_ID) Lectures_Total, lec.CRN FROM LECTURE lec
	                                            GROUP BY lec.CRN
                                            ) LecturesCounter
                                            ON LecturesCounter.CRN = Std_Attendance.CRN
                                            ORDER BY Std_Attendance.CRN, Lectures_Attended;");

            return Ok(data);
        }

        [HttpGet("[controller]/GetAttendanceCountInClass/{CRN}", Name = "GetAttendanceCountInClass")]
        public IActionResult GetAttendanceCountInClass(string CRN)
        {

            var p = new DynamicParameters();
            p.Add(@"class_crn", CRN);

            var data = cloasisdbRef.Query(@"SELECT Std_Attendance.STUDENTID, Std.NAME, Std_Attendance.Lectures_Attended, LecturesCounter.Lectures_Total, cls.CRN, crs.COURSE_NAME, crs.COURSE_CODE, cls.SECTION_NUM  FROM dbo.STUDENT Std
                                            INNER JOIN
                                            (
	                                            SELECT att.STUDENTID, COUNT(att.LECTURE_ID) Lectures_Attended, lec.CRN FROM ATTENDANCE att
	                                            INNER JOIN LECTURE lec ON lec.LECTURE_ID = att.LECTURE_ID
	                                            INNER JOIN dbo.STUDENT std ON std.STUDENTID = att.STUDENTID
	                                            GROUP BY att.STUDENTID, lec.CRN
                                            ) Std_Attendance
                                            ON Std_Attendance.STUDENTID = Std.STUDENTID
                                            LEFT JOIN dbo.CLASS cls ON cls.CRN = Std_Attendance.CRN
                                            LEFT JOIN dbo.COURSE crs ON crs.COURSE_ID = cls.COURSE_ID
                                            INNER JOIN
                                            (
	                                            SELECT COUNT(lec.LECTURE_ID) Lectures_Total, lec.CRN FROM LECTURE lec
	                                            GROUP BY lec.CRN
                                            ) LecturesCounter
                                            ON LecturesCounter.CRN = Std_Attendance.CRN
                                            WHERE cls.CRN = @class_crn
                                            ORDER BY Std_Attendance.CRN, Lectures_Attended;",p);

            return Ok(data);
        }

        [HttpGet("[controller]/GetStudentAttendanceCount/{StudentId}", Name = "GetStudentAttendanceCount")]
        public IActionResult GetStudentAttendance(string StudentId)
        {
            var p = new DynamicParameters();
            p.Add(@"stdId", StudentId);

            var data = cloasisdbRef.Query(@"SELECT Std_Attendance.STUDENTID, Std.NAME, Std_Attendance.Lectures_Attended, LecturesCounter.Lectures_Total, cls.CRN, crs.COURSE_NAME, crs.COURSE_CODE, cls.SECTION_NUM  FROM dbo.STUDENT Std
                                            INNER JOIN
                                            (
	                                            SELECT att.STUDENTID, COUNT(att.LECTURE_ID) Lectures_Attended, lec.CRN FROM ATTENDANCE att
	                                            INNER JOIN LECTURE lec ON lec.LECTURE_ID = att.LECTURE_ID
	                                            INNER JOIN dbo.STUDENT std ON std.STUDENTID = att.STUDENTID
	                                            GROUP BY att.STUDENTID, lec.CRN
                                            ) Std_Attendance
                                            ON Std_Attendance.STUDENTID = Std.STUDENTID
                                            LEFT JOIN dbo.CLASS cls ON cls.CRN = Std_Attendance.CRN
                                            LEFT JOIN dbo.COURSE crs ON crs.COURSE_ID = cls.COURSE_ID
                                            INNER JOIN
                                            (
	                                            SELECT COUNT(lec.LECTURE_ID) Lectures_Total, lec.CRN FROM LECTURE lec
	                                            GROUP BY lec.CRN
                                            ) LecturesCounter
                                            ON LecturesCounter.CRN = Std_Attendance.CRN
                                            WHERE Std_Attendance.STUDENTID = @stdId
                                            ORDER BY Lectures_Attended;", p);


            return Ok(data);
        }

        [HttpGet("[controller]/GetStudentAttendanceCountInClass/{StudentId}/{CRN}", Name = "GetStudentAttendanceCountInClass")]
        public IActionResult GetStudentAttendanceCountInClass(string StudentId, string CRN)
        {
            var p = new DynamicParameters();
            p.Add(@"stdId", StudentId);
            p.Add(@"class_crn", CRN);

            List<RegistrationEntry> isRegistered = cloasisdbRef.Query<RegistrationEntry>("SELECT * FROM ENROLLMENT WHERE ENROLLMENT.CRN = @class_crn AND STUDENTID = @stdId", p).ToList();

            if (isRegistered.Count == 0)
            {
                return BadRequest("Student is not registered in the provided course");
            }

            var data = cloasisdbRef.Query(@"SELECT Std_Attendance.STUDENTID, Std.NAME, Std_Attendance.Lectures_Attended, LecturesCounter.Lectures_Total, cls.CRN, crs.COURSE_NAME, crs.COURSE_CODE, cls.SECTION_NUM  FROM dbo.STUDENT Std
                                            INNER JOIN
                                            (
	                                            SELECT att.STUDENTID, COUNT(att.LECTURE_ID) Lectures_Attended, lec.CRN FROM ATTENDANCE att
	                                            INNER JOIN LECTURE lec ON lec.LECTURE_ID = att.LECTURE_ID
	                                            INNER JOIN dbo.STUDENT std ON std.STUDENTID = att.STUDENTID
	                                            GROUP BY att.STUDENTID, lec.CRN
                                            ) Std_Attendance
                                            ON Std_Attendance.STUDENTID = Std.STUDENTID
                                            LEFT JOIN dbo.CLASS cls ON cls.CRN = Std_Attendance.CRN
                                            LEFT JOIN dbo.COURSE crs ON crs.COURSE_ID = cls.COURSE_ID
                                            INNER JOIN
                                            (
	                                            SELECT COUNT(lec.LECTURE_ID) Lectures_Total, lec.CRN FROM LECTURE lec
	                                            GROUP BY lec.CRN
                                            ) LecturesCounter
                                            ON LecturesCounter.CRN = Std_Attendance.CRN
                                            WHERE Std_Attendance.STUDENTID = @stdId AND cls.CRN = @class_crn
                                            ORDER BY Std_Attendance.CRN, Lectures_Attended;", p);


            return Ok(data);
        }

        [HttpPost("[controller]/RecordStudentAttendance/{LectureId}/{StudentId}", Name = "RecordStudentAttendance")]
        public IActionResult Post(int LectureId, string StudentId)
        {
            var p = new DynamicParameters();
            p.Add(@"stdId", StudentId);
            p.Add(@"lecId", LectureId);

            string crn = cloasisdbRef.Query<string>("SELECT CRN FROM dbo.LECTURE WHERE LECTURE_ID = @lecId", p).ToList()[0];

            p.Add(@"class_crn", crn);

            List<RegistrationEntry> isRegistered = cloasisdbRef.Query<RegistrationEntry>("SELECT * FROM ENROLLMENT WHERE ENROLLMENT.CRN = @class_crn AND STUDENTID = @stdId", p).ToList();

            if (isRegistered.Count == 0)
            {
                return BadRequest("Student is not registered in the provided course");
            }

            List<Object> attendanceRecorded = cloasisdbRef.Query("SELECT * FROM dbo.ATTENDANCE WHERE ATTENDANCE.LECTURE_ID = @lecId AND ATTENDANCE.STUDENTID = @stdId", p).ToList();

            if (attendanceRecorded.Count > 0)
            {
                return BadRequest("This Student's attendance in the lecture has already been recorded");
            }

            cloasisdbRef.Execute("INSERT INTO dbo.ATTENDANCE(STUDENTID, LECTURE_ID) VALUES (@stdId, @lecId)", p);

            var data = cloasisdbRef.Query("SELECT * FROM dbo.ATTENDANCE WHERE ATTENDANCE.LECTURE_ID = @lecId AND ATTENDANCE.STUDENTID = @stdId", p);

            return Ok(data);
        }

        [HttpDelete("[controller]/DeleteAttendanceRecord/{LectureId}/{StudentId}", Name = "DeleteAttendanceRecord")]
        public IActionResult Delete(int LectureId, string StudentId)
        {
            var p = new DynamicParameters();
            p.Add(@"stdId", StudentId);
            p.Add(@"lecId", LectureId);

            string crn = cloasisdbRef.Query<string>("SELECT CRN FROM dbo.LECTURE WHERE LECTURE_ID = @lecId", p).ToList()[0];

            p.Add(@"class_crn", crn);

            List<RegistrationEntry> isRegistered = cloasisdbRef.Query<RegistrationEntry>("SELECT * FROM ENROLLMENT WHERE ENROLLMENT.CRN = @class_crn AND STUDENTID = @stdId", p).ToList();

            if (isRegistered.Count == 0)
            {
                return BadRequest("Student is not registered in the provided course");
            }

            List<Object> attendanceRecorded = cloasisdbRef.Query("SELECT * FROM dbo.ATTENDANCE WHERE ATTENDANCE.LECTURE_ID = @lecId AND ATTENDANCE.STUDENTID = @stdId", p).ToList();

            if (attendanceRecorded.Count == 0)
            {
                return BadRequest("This Student's attendance record in the lecture does not exist");
            }


            Student std = cloasisdbRef.Query<Student>("SELECT * FROM dbo.STUDENT WHERE STUDENTID = @stdId", p).ToList()[0];
            Lecture lec = cloasisdbRef.Query<Lecture>("SELECT * FROM dbo.Lecture WHERE LECTURE_ID = @lecId", p).ToList()[0];

            cloasisdbRef.Execute("DELETE FROM dbo.ATTENDANCE WHERE STUDENTID = @stdId AND LECTURE_ID = @lecId", p);

            return Ok($"Student {std.NAME} attendance record has successfully been removed from the lecture {lec.Title}");
        }


        /***
         * I would like to add later the ability to add multiple attendance records of students in one request (Take a list of StudentIds from the body of the POST request)
         * I also would like to do the same with the DELETE requests so I can delete multiple attendance records of students in one request in a similiar manner
        ***/

    }
}