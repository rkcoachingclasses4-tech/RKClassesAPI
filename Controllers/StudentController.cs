using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using RKClassesApi.Data;
using System.Data;

namespace RKClassesApi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class StudentController : ControllerBase
    {
        private readonly SqlConnectionFactory _connectionFactory;

        public StudentController(SqlConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        [HttpGet]
        public IActionResult GetAllStudents()
        {
            var students = new List<object>();
            using (var connection = _connectionFactory.CreateConnection())
            {
                var command = connection.CreateCommand();
                command.CommandText = "SELECT StudentId, FullName, Email, Phone, CurrentClass, Stream, SchoolName FROM Students";
                connection.Open();

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        students.Add(new
                        {
                            StudentId = reader["StudentId"],
                            FullName = reader["FullName"],
                            Email = reader["Email"],
                            Phone = reader["Phone"],
                            CurrentClass = reader["CurrentClass"],
                            Stream = reader["Stream"],
                            SchoolName = reader["SchoolName"]
                        });
                    }
                }
            }
            return Ok(students);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult CreateStudent(string fullName, string email, string phone, int currentClass, string stream, string schoolName)
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                var command = connection.CreateCommand();
                command.CommandText = @"INSERT INTO Students (FullName, Email, Phone, CurrentClass, Stream, SchoolName) 
                                       VALUES (@FullName, @Email, @Phone, @CurrentClass, @Stream, @SchoolName)";
                
                command.Parameters.Add(new SqlParameter("@FullName", fullName));
                command.Parameters.Add(new SqlParameter("@Email", email ?? (object)DBNull.Value));
                command.Parameters.Add(new SqlParameter("@Phone", phone ?? (object)DBNull.Value));
                command.Parameters.Add(new SqlParameter("@CurrentClass", currentClass));
                command.Parameters.Add(new SqlParameter("@Stream", stream ?? "General"));
                command.Parameters.Add(new SqlParameter("@SchoolName", schoolName ?? (object)DBNull.Value));

                connection.Open();
                command.ExecuteNonQuery();
            }
            return Ok(new { Message = "Student registered successfully using ADO.NET!" });
        }
    }
}
