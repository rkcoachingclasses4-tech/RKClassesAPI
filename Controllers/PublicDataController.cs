using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using RKClassesApi.Data;
using RKClassesApi.DTOs;

namespace RKClassesApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PublicDataController : ControllerBase
    {
        private readonly SqlConnectionFactory _connectionFactory;

        public PublicDataController(SqlConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        [HttpGet("active-session")]
        public IActionResult GetActiveSession()
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                var command = connection.CreateCommand() as SqlCommand;
                command.CommandText = "SELECT TOP 1 Id, Name, IsActive FROM AcademicSessions WHERE IsActive = 1";
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return Ok(new SessionModel
                        {
                            Id = (int)reader["Id"],
                            Name = reader["Name"].ToString()!,
                            IsActive = (bool)reader["IsActive"]
                        });
                    }
                }
            }
            return Ok((SessionModel?)null);
        }

        [HttpGet("results")]
        public IActionResult GetResults(int classLevel, string subject)
        {
            var results = new List<ResultModel>();
            using (var connection = _connectionFactory.CreateConnection())
            {
                var command = connection.CreateCommand() as SqlCommand;
                // Only return results belonging to the currently active academic session
                command.CommandText = @"SELECT e.StudentName, e.RollNo, e.ClassLevel, e.Subject, e.Marks, e.OverallPercentage, e.AcademicSession, e.ResultStatus 
                                       FROM ExamResults e
                                       INNER JOIN AcademicSessions s ON e.AcademicSession = s.Name AND s.IsActive = 1
                                       WHERE e.ClassLevel = @ClassLevel AND e.Subject = @Subject
                                       ORDER BY e.Marks DESC";
                
                command.Parameters.AddWithValue("@ClassLevel", classLevel);
                command.Parameters.AddWithValue("@Subject", subject);

                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        results.Add(new ResultModel
                        {
                            StudentName = reader["StudentName"].ToString()!,
                            RollNo = reader["RollNo"].ToString()!,
                            ClassLevel = (int)reader["ClassLevel"],
                            Subject = reader["Subject"].ToString()!,
                            Marks = (int)reader["Marks"],
                            OverallPercentage = (decimal)reader["OverallPercentage"],
                            AcademicSession = reader["AcademicSession"].ToString()!,
                            ResultStatus = reader["ResultStatus"].ToString()!
                        });
                    }
                }
            }
            return Ok(results);
        }

        // [HttpGet("announcements")]
        // public IActionResult GetAnnouncements()
        // {
        //     var announcements = new List<AnnouncementModel>();
        //     using (var connection = _connectionFactory.CreateConnection())
        //     {
        //         var command = connection.CreateCommand() as SqlCommand;
        //         command.CommandText = "SELECT TitleEn, TitleHi, ContentEn, ContentHi, DatePosted FROM Announcements ORDER BY DatePosted DESC";
                
        //         connection.Open();
        //         using (var reader = command.ExecuteReader())
        //         {
        //             while (reader.Read())
        //             {
        //                 announcements.Add(new AnnouncementModel
        //                 {
        //                     TitleEn = reader["TitleEn"].ToString()!,
        //                     TitleHi = reader["TitleHi"].ToString()!,
        //                     ContentEn = reader["ContentEn"].ToString()!,
        //                     ContentHi = reader["ContentHi"].ToString()!,
        //                     DatePosted = Convert.ToDateTime(reader["DatePosted"])
        //                 });
        //             }
        //         }
        //     }
        //     return Ok(announcements);
        // }
        [HttpGet("announcements")]
public IActionResult GetAnnouncements()
{
    try
    {
        var announcements = new List<AnnouncementModel>();

        using (var connection = _connectionFactory.CreateConnection())
        {
            var command = connection.CreateCommand() as SqlCommand;
            command.CommandText = "SELECT TitleEn, TitleHi, ContentEn, ContentHi, DatePosted FROM Announcements ORDER BY DatePosted DESC";

            connection.Open();

            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    announcements.Add(new AnnouncementModel
                    {
                        TitleEn = reader["TitleEn"].ToString()!,
                        TitleHi = reader["TitleHi"].ToString()!,
                        ContentEn = reader["ContentEn"].ToString()!,
                        ContentHi = reader["ContentHi"].ToString()!,
                        DatePosted = Convert.ToDateTime(reader["DatePosted"])
                    });
                }
            }
        }

        return Ok(announcements);
    }
    catch (Exception ex)
    {
        return StatusCode(500, ex.ToString());
    }
}

        [HttpGet("classes")]
        public IActionResult GetClasses()
        {
            var classes = new List<ClassModel>();
            using (var connection = _connectionFactory.CreateConnection())
            {
                var command = connection.CreateCommand() as SqlCommand;
                command.CommandText = @"SELECT c.Id, c.Level, c.NameEn, c.NameHi,
                                              s.Id AS SubjectId, s.NameEn AS SubjectNameEn, s.NameHi AS SubjectNameHi
                                       FROM Classes c
                                       LEFT JOIN ClassSubjects s ON c.Id = s.ClassId
                                       ORDER BY c.Level, s.NameEn";

                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    var classDict = new Dictionary<int, ClassModel>();
                    while (reader.Read())
                    {
                        var classId = (int)reader["Id"];
                        if (!classDict.ContainsKey(classId))
                        {
                            classDict[classId] = new ClassModel
                            {
                                Id = classId,
                                Level = (int)reader["Level"],
                                NameEn = reader["NameEn"].ToString()!,
                                NameHi = reader["NameHi"].ToString()!,
                                Subjects = new List<SubjectModel>()
                            };
                        }

                        if (reader["SubjectId"] != DBNull.Value)
                        {
                            classDict[classId].Subjects.Add(new SubjectModel
                            {
                                Id = (int)reader["SubjectId"],
                                ClassId = classId,
                                NameEn = reader["SubjectNameEn"].ToString()!,
                                NameHi = reader["SubjectNameHi"].ToString()!
                            });
                        }
                    }
                    classes = classDict.Values.ToList();
                }
            }
            return Ok(classes);
        }

        [HttpGet("sessions")]
        public IActionResult GetSessions()
        {
            var sessions = new List<SessionModel>();
            using (var connection = _connectionFactory.CreateConnection())
            {
                var command = connection.CreateCommand() as SqlCommand;
                command.CommandText = "SELECT Id, Name, IsActive FROM AcademicSessions WHERE IsActive = 1 ORDER BY Name DESC";
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        sessions.Add(new SessionModel
                        {
                            Id = (int)reader["Id"],
                            Name = reader["Name"].ToString()!,
                            IsActive = (bool)reader["IsActive"]
                        });
                    }
                }
            }
            return Ok(sessions);
        }
    }
}
