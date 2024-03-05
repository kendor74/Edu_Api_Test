namespace EducationlPlatform.Models.Interfaces.UserInterfaces
{
    public interface IUser
    {
        Task<UserDto> Login(UserDto user);
        Task<UserDto> SignUp(IFormFile Image,UserDto user);
        Task<bool> Logout();
        Task<IEnumerable<IdentityUser>> GetUsers();
        Task<UserDto> Edit(IFormFile Image,UserDto userDto);
        Task<string> ChangePassword(string Email, string currentPassword, string newPassword);

        //Task<string> JwtToken(UserDto user);
    }
}
