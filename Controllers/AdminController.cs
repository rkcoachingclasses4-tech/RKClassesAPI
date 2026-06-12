using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using RKClassesApi.Data;
using RKClassesApi.DTOs;

namespace RKClassesApi.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly SqlConnectionFactory _connectionFactory;

        public AdminController(SqlConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        [HttpPost("announcement")]
        public IActionResult AddAnnouncement([FromBody] AnnouncementModel model)
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                var command = connection.CreateCommand() as SqlCommand;
                command.CommandText = @"INSERT INTO Announcements (TitleEn, TitleHi, ContentEn, ContentHi) 
                                       VALUES (@TitleEn, @TitleHi, @ContentEn, @ContentHi)";
                
                command.Parameters.AddWithValue("@TitleEn", model.TitleEn);
                command.Parameters.AddWithValue("@TitleHi", model.TitleHi);
                command.Parameters.AddWithValue("@ContentEn", (object)model.ContentEn ?? DBNull.Value);
                command.Parameters.AddWithValue("@ContentHi", (object)model.ContentHi ?? DBNull.Value);

                connection.Open();
                command.ExecuteNonQuery();
            }
            return Ok(new { Message = "Announcement published successfully!" });
        }

        [HttpGet("announcements")]
        public IActionResult GetAllAnnouncements()
        {
            var announcements = new List<AnnouncementModel>();
            using (var connection = _connectionFactory.CreateConnection())
            {
                var command = connection.CreateCommand() as SqlCommand;
                command.CommandText = "SELECT Id, TitleEn, TitleHi, ContentEn, ContentHi, DatePosted FROM Announcements ORDER BY DatePosted DESC";
                
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        announcements.Add(new AnnouncementModel
                        {
                            Id = (int)reader["Id"],
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

        [HttpPut("announcement/{id}")]
        public IActionResult UpdateAnnouncement(int id, [FromBody] AnnouncementModel model)
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                var command = connection.CreateCommand() as SqlCommand;
                command.CommandText = @"UPDATE Announcements 
                                       SET TitleEn = @TitleEn, TitleHi = @TitleHi, ContentEn = @ContentEn, ContentHi = @ContentHi 
                                       WHERE Id = @Id";
                
                command.Parameters.AddWithValue("@Id", id);
                command.Parameters.AddWithValue("@TitleEn", model.TitleEn);
                command.Parameters.AddWithValue("@TitleHi", model.TitleHi);
                command.Parameters.AddWithValue("@ContentEn", (object)model.ContentEn ?? DBNull.Value);
                command.Parameters.AddWithValue("@ContentHi", (object)model.ContentHi ?? DBNull.Value);

                connection.Open();
                var rowsAffected = command.ExecuteNonQuery();
                if (rowsAffected == 0) return NotFound(new { Message = "Announcement not found." });
            }
            return Ok(new { Message = "Announcement updated successfully!" });
        }

        [HttpDelete("announcement/{id}")]
        public IActionResult DeleteAnnouncement(int id)
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                var command = connection.CreateCommand() as SqlCommand;
                command.CommandText = "DELETE FROM Announcements WHERE Id = @Id";
                command.Parameters.AddWithValue("@Id", id);

                connection.Open();
                var rowsAffected = command.ExecuteNonQuery();
                if (rowsAffected == 0) return NotFound(new { Message = "Announcement not found." });
            }
            return Ok(new { Message = "Announcement deleted successfully!" });
        }

        [HttpPost("result")]
        public IActionResult AddResult([FromBody] ResultModel model)
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                var command = connection.CreateCommand() as SqlCommand;
                command.CommandText = @"INSERT INTO ExamResults (StudentName, RollNo, ClassLevel, Subject, Marks, OverallPercentage, AcademicSession, ResultStatus) 
                                       VALUES (@StudentName, @RollNo, @ClassLevel, @Subject, @Marks, @OverallPercentage, @AcademicSession, @ResultStatus)";
                
                command.Parameters.AddWithValue("@StudentName", model.StudentName);
                command.Parameters.AddWithValue("@RollNo", model.RollNo);
                command.Parameters.AddWithValue("@ClassLevel", model.ClassLevel);
                command.Parameters.AddWithValue("@Subject", model.Subject);
                command.Parameters.AddWithValue("@Marks", model.Marks);
                command.Parameters.AddWithValue("@OverallPercentage", model.OverallPercentage);
                command.Parameters.AddWithValue("@AcademicSession", model.AcademicSession);
                command.Parameters.AddWithValue("@ResultStatus", model.ResultStatus);

                connection.Open();
                command.ExecuteNonQuery();
            }
            return Ok(new { Message = "Result saved successfully!" });
        }

        [HttpGet("results")]
        public IActionResult GetAllResults()
        {
            var results = new List<ResultModel>();
            using (var connection = _connectionFactory.CreateConnection())
            {
                var command = connection.CreateCommand() as SqlCommand;
                command.CommandText = "SELECT Id, StudentName, RollNo, ClassLevel, Subject, Marks, OverallPercentage, AcademicSession, ResultStatus FROM ExamResults ORDER BY Id DESC";
                
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        results.Add(new ResultModel
                        {
                            Id = (int)reader["Id"],
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

        [HttpPut("result/{id}")]
        public IActionResult UpdateResult(int id, [FromBody] ResultModel model)
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                var command = connection.CreateCommand() as SqlCommand;
                command.CommandText = @"UPDATE ExamResults 
                                       SET StudentName = @StudentName, RollNo = @RollNo, ClassLevel = @ClassLevel, 
                                           Subject = @Subject, Marks = @Marks, OverallPercentage = @OverallPercentage, 
                                           AcademicSession = @AcademicSession, ResultStatus = @ResultStatus 
                                       WHERE Id = @Id";
                
                command.Parameters.AddWithValue("@Id", id);
                command.Parameters.AddWithValue("@StudentName", model.StudentName);
                command.Parameters.AddWithValue("@RollNo", model.RollNo);
                command.Parameters.AddWithValue("@ClassLevel", model.ClassLevel);
                command.Parameters.AddWithValue("@Subject", model.Subject);
                command.Parameters.AddWithValue("@Marks", model.Marks);
                command.Parameters.AddWithValue("@OverallPercentage", model.OverallPercentage);
                command.Parameters.AddWithValue("@AcademicSession", model.AcademicSession);
                command.Parameters.AddWithValue("@ResultStatus", model.ResultStatus);

                connection.Open();
                var rowsAffected = command.ExecuteNonQuery();
                if (rowsAffected == 0) return NotFound(new { Message = "Result not found." });
            }
            return Ok(new { Message = "Result updated successfully!" });
        }

        [HttpDelete("result/{id}")]
        public IActionResult DeleteResult(int id)
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                var command = connection.CreateCommand() as SqlCommand;
                command.CommandText = "DELETE FROM ExamResults WHERE Id = @Id";
                command.Parameters.AddWithValue("@Id", id);

                connection.Open();
                var rowsAffected = command.ExecuteNonQuery();
                if (rowsAffected == 0) return NotFound(new { Message = "Result not found." });
            }
            return Ok(new { Message = "Result deleted successfully!" });
        }

        // ==================== CLASS & SUBJECT MANAGEMENT ====================

        [HttpGet("classes")]
        public IActionResult GetAllClasses()
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

        [HttpPost("class")]
        public IActionResult AddClass([FromBody] ClassModel model)
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                var command = connection.CreateCommand() as SqlCommand;
                command.CommandText = @"INSERT INTO Classes (Level, NameEn, NameHi) VALUES (@Level, @NameEn, @NameHi);
                                       SELECT SCOPE_IDENTITY();";
                command.Parameters.AddWithValue("@Level", model.Level);
                command.Parameters.AddWithValue("@NameEn", model.NameEn);
                command.Parameters.AddWithValue("@NameHi", model.NameHi);

                connection.Open();
                var newId = Convert.ToInt32(command.ExecuteScalar());
                return Ok(new { Message = "Class added!", Id = newId });
            }
        }

        [HttpPut("class/{id}")]
        public IActionResult UpdateClass(int id, [FromBody] ClassModel model)
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                var command = connection.CreateCommand() as SqlCommand;
                command.CommandText = "UPDATE Classes SET Level = @Level, NameEn = @NameEn, NameHi = @NameHi WHERE Id = @Id";
                command.Parameters.AddWithValue("@Id", id);
                command.Parameters.AddWithValue("@Level", model.Level);
                command.Parameters.AddWithValue("@NameEn", model.NameEn);
                command.Parameters.AddWithValue("@NameHi", model.NameHi);

                connection.Open();
                var rows = command.ExecuteNonQuery();
                if (rows == 0) return NotFound(new { Message = "Class not found." });
            }
            return Ok(new { Message = "Class updated!" });
        }

        [HttpDelete("class/{id}")]
        public IActionResult DeleteClass(int id)
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                var command = connection.CreateCommand() as SqlCommand;
                // Delete subjects first, then the class
                command.CommandText = "DELETE FROM ClassSubjects WHERE ClassId = @Id; DELETE FROM Classes WHERE Id = @Id;";
                command.Parameters.AddWithValue("@Id", id);

                connection.Open();
                command.ExecuteNonQuery();
            }
            return Ok(new { Message = "Class and its subjects deleted!" });
        }

        [HttpPost("subject")]
        public IActionResult AddSubject([FromBody] SubjectModel model)
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                var command = connection.CreateCommand() as SqlCommand;
                command.CommandText = "INSERT INTO ClassSubjects (ClassId, NameEn, NameHi) VALUES (@ClassId, @NameEn, @NameHi)";
                command.Parameters.AddWithValue("@ClassId", model.ClassId);
                command.Parameters.AddWithValue("@NameEn", model.NameEn);
                command.Parameters.AddWithValue("@NameHi", model.NameHi);

                connection.Open();
                command.ExecuteNonQuery();
            }
            return Ok(new { Message = "Subject added!" });
        }

        [HttpDelete("subject/{id}")]
        public IActionResult DeleteSubject(int id)
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                var command = connection.CreateCommand() as SqlCommand;
                command.CommandText = "DELETE FROM ClassSubjects WHERE Id = @Id";
                command.Parameters.AddWithValue("@Id", id);

                connection.Open();
                command.ExecuteNonQuery();
            }
            return Ok(new { Message = "Subject removed!" });
        }

        // ==================== SESSION MANAGEMENT ====================

        [HttpGet("sessions")]
        public IActionResult GetAllSessions()
        {
            var sessions = new List<SessionModel>();
            using (var connection = _connectionFactory.CreateConnection())
            {
                var command = connection.CreateCommand() as SqlCommand;
                command.CommandText = "SELECT Id, Name, IsActive FROM AcademicSessions ORDER BY Name DESC";
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

        [HttpPost("session")]
        public IActionResult AddSession([FromBody] SessionModel model)
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                connection.Open();

                // If this session is active, deactivate all others first
                if (model.IsActive)
                {
                    var deactivateCmd = connection.CreateCommand() as SqlCommand;
                    deactivateCmd.CommandText = "UPDATE AcademicSessions SET IsActive = 0";
                    deactivateCmd.ExecuteNonQuery();
                }

                var command = connection.CreateCommand() as SqlCommand;
                command.CommandText = "INSERT INTO AcademicSessions (Name, IsActive) VALUES (@Name, @IsActive)";
                command.Parameters.AddWithValue("@Name", model.Name);
                command.Parameters.AddWithValue("@IsActive", model.IsActive);
                command.ExecuteNonQuery();
            }
            return Ok(new { Message = "Session added!" });
        }

        [HttpPut("session/{id}")]
        public IActionResult UpdateSession(int id, [FromBody] SessionModel model)
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                connection.Open();

                // If activating this session, deactivate all others first
                if (model.IsActive)
                {
                    var deactivateCmd = connection.CreateCommand() as SqlCommand;
                    deactivateCmd.CommandText = "UPDATE AcademicSessions SET IsActive = 0";
                    deactivateCmd.ExecuteNonQuery();
                }

                var command = connection.CreateCommand() as SqlCommand;
                command.CommandText = "UPDATE AcademicSessions SET Name = @Name, IsActive = @IsActive WHERE Id = @Id";
                command.Parameters.AddWithValue("@Id", id);
                command.Parameters.AddWithValue("@Name", model.Name);
                command.Parameters.AddWithValue("@IsActive", model.IsActive);
                var rows = command.ExecuteNonQuery();
                if (rows == 0) return NotFound(new { Message = "Session not found." });
            }
            return Ok(new { Message = "Session updated!" });
        }

        [HttpDelete("session/{id}")]
        public IActionResult DeleteSession(int id)
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                var command = connection.CreateCommand() as SqlCommand;
                command.CommandText = "DELETE FROM AcademicSessions WHERE Id = @Id";
                command.Parameters.AddWithValue("@Id", id);
                connection.Open();
                command.ExecuteNonQuery();
            }
            return Ok(new { Message = "Session deleted!" });
        }
    }
}
