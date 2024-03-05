namespace EducationlPlatform.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUser _context;
        
        public UserController(IUser context)
        {
            _context = context;
        }


        [HttpGet("Login")]
        public async Task<IActionResult> Login(string Email, string Password, string Role)
        {
            UserDto userDto = new UserDto();
            userDto.Email = Email;
            userDto.Password = Password;
            userDto.Role = Role;

            var result = await _context.Login(userDto);

            return (result != null) ? Ok(result) : BadRequest("Invalid Login");
        }


        //Need
        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromForm]UserDto user)
        {
            if (user.Image == null)
            {
                user.ImagePath = "Default";
                string path = (user.Gender == "Male")? "D:\\repo\\EDU\\EduConsummer\\wwwroot\\Upload\\MaleIcon.png" : "D:\\repo\\EDU\\EduConsummer\\wwwroot\\Upload\\FemaleIcon.png";

                var placeholderBytes = System.IO.File.ReadAllBytes(path);
                var defaultFile = 
                    new FormFile(new MemoryStream(placeholderBytes), 0, placeholderBytes.Length, $"{user.Gender}Icon", $"{user.Gender}.png")
                {
                    Headers = new HeaderDictionary(),
                    ContentType = "image/png"
                };
                user.Image = defaultFile;
            }

            var result = await _context.SignUp(user.Image,user);

            if (ModelState.IsValid)
                return Ok(result);
            else
                return BadRequest($"Creatation denied!\n {result.Error}");
            
        }






        [HttpPost("Logout")]
        public async Task<IActionResult> Logout()
        {
            bool result = await _context.Logout();
            return (result ? Ok("Logged out") : BadRequest("No account to log out"));
        }


        [HttpGet("Get Users")]
        public async Task<IActionResult> GetUsers()
        {
            return Ok(await _context.GetUsers());
        }


        [HttpPost("Changing Password")]
        public async Task<IActionResult> ChangePassword(string Email,string CurrentPassword,string NewPassword)
        {
            var result = _context.ChangePassword(Email, CurrentPassword, NewPassword);

            return Ok(result);
        }

        [HttpPut("Edit Profile")]
        public async Task<IActionResult> EditUserProfile(IFormFile Image,UserDto user)
        {
            var result = await _context.Edit(Image,user);
            return Ok(result);
        }
    

    }
}
