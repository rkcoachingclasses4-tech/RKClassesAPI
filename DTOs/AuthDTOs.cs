namespace RKClassesApi.DTOs
{
    public class RegisterModel
    {
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Role { get; set; } = "User"; // Default role
    }

    public class LoginModel
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class AuthResponse
    {
        public string Token { get; set; } = string.Empty;
        public DateTime Expiration { get; set; }
        public string Message { get; set; } = string.Empty;
    }

    public class AnnouncementModel
    {
        public int Id { get; set; }
        public string TitleEn { get; set; } = string.Empty;
        public string TitleHi { get; set; } = string.Empty;
        public string ContentEn { get; set; } = string.Empty;
        public string ContentHi { get; set; } = string.Empty;
        public DateTime DatePosted { get; set; }
    }

    public class ResultModel
    {
        public int Id { get; set; }
        public string StudentName { get; set; } = string.Empty;
        public string RollNo { get; set; } = string.Empty;
        public int ClassLevel { get; set; }
        public string Subject { get; set; } = string.Empty;
        public int Marks { get; set; }
        public decimal OverallPercentage { get; set; }
        public string AcademicSession { get; set; } = string.Empty;
        public string ResultStatus { get; set; } = string.Empty;
    }

    public class ClassModel
    {
        public int Id { get; set; }
        public int Level { get; set; }            // 9, 10, 11, 12
        public string NameEn { get; set; } = string.Empty;  // "10th Class"
        public string NameHi { get; set; } = string.Empty;  // "10वीं कक्षा"
        public List<SubjectModel> Subjects { get; set; } = new();
    }

    public class SubjectModel
    {
        public int Id { get; set; }
        public int ClassId { get; set; }
        public string NameEn { get; set; } = string.Empty;
        public string NameHi { get; set; } = string.Empty;
    }

    public class SessionModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;       // "2024-25"
        public bool IsActive { get; set; } = true;
    }
}
