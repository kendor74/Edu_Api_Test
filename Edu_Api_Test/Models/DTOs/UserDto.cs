namespace EducationlPlatform.Models.DTOs
{
    public class UserDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string? ImagePath { get; set; }
        public IFormFile? Image { get; set; }
        public string Password { get; set; }
        public string PhoneNumber { get; set; }
        public string Gender { get; set; }
        public string City { get; set; }
        public string Role { get; set; }
        public byte Age { get; set; }
        public string? JwtToken { get; set; }
        public string? EmailToken { get; set; }
        public List<string>? Error { get; set; }

    }
}
