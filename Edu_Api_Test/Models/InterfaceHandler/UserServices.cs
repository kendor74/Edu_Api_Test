namespace EducationlPlatform.Models.InterfaceHandler
{
    public class UserServices : IUser
    {

        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IConfiguration _config;
        //byte[] defualtImage = File.ReadAllBytes("D:\\images\\1.jpg");
        private readonly ApplicationDbContext _studentRepository;
        private readonly IdentityUserDbContext _identityUserDbContext;



        public UserServices(UserManager<User> userManager, IConfiguration config
            , SignInManager<User> signInManager
            , ApplicationDbContext studentRepository)
        {
            _userManager = userManager;
            _config = config;
            _signInManager = signInManager;
            _studentRepository = studentRepository;
            _identityUserDbContext = identityUserDbContext;
        }


        private string JwtToken(UserDto user)
        {
            IEnumerable<Claim> claims = new List<Claim>
            {
                    new Claim(ClaimTypes.Email , user.UserName),
            };

#pragma warning disable CS8604 // Possible null reference argument.
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.GetSection("Jwt:Key").Value));

            var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha512Signature);

            var securityToken = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.UtcNow.AddDays(1),
                issuer: _config.GetSection("Jwt:Issuer").Value,
                audience: _config.GetSection("Jwt:Audiance").Value,
                signingCredentials: signingCredentials);

            string tokenString = new JwtSecurityTokenHandler().WriteToken(securityToken);

            return tokenString;
        }

        public async Task<UserDto> Login(UserDto user)
        {
            var userIdentity = await _userManager.FindByEmailAsync(user.Email);

            if (userIdentity != null)
            {

                //add role
                //user.Role = await _userManager.getR
                await _userManager.CheckPasswordAsync(userIdentity, user.Password);
                user.Email = user.Email;
                user.Password = user.Password;
                user.UserName = userIdentity.UserName;
                //user.Role = role.FirstOrDefault();
                user.JwtToken = JwtToken(user);
                return user;
            }
#pragma warning disable CS8603 // Possible null reference return.
            return null;
        }

        public async Task<bool> Logout()
        {
            try
            {
                await _signInManager.SignOutAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<UserDto> SignUp(IFormFile Image, UserDto user)
        {
            IdentityResult result;
            var identityUser = new User
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                UserName = user.UserName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                City = user.City,
                Age = user.Age,
                Gender = user.Gender,
                EmailConfirmed = true
            };

           


            try
            {
                //save image into wwwroot ?? directory and display the path
                if (Image !=null && user.ImagePath == "" )
                {
                    string imageName = await UploadImage(Image, identityUser.UserName, identityUser.Id);
                    identityUser.ImagePath = imageName;
                    user.ImagePath = imageName;

                }
                else
                {
                    if (user.Gender == "Female")
                    {

                        identityUser.ImagePath = "FemaleIcon.png";
                    }
                    else
                    {
                        identityUser.ImagePath = "MaleIcon.png";
                    }
                }
                result = await _userManager.CreateAsync(identityUser, user.Password);

                //creating rules
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(identityUser, user.Role);

                    if (user.Role == "Student")
                    {
                        //adding in the Student Table
                        Student student = new Student()
                        {
                            UserId = identityUser.Id,
                        };

                        var student_result = await _studentRepository.Students.AddAsync(student);
                        await _studentRepository.SaveChangesAsync();
                    }
                   

                }
                else
                {
                    foreach (var item in result.Errors)
                    {
                        user.Error.Add(item.ToString());
                    }
                }
                var Emailtoken = await _userManager.GenerateEmailConfirmationTokenAsync((User)identityUser);
                user.EmailToken = Emailtoken;
                user.JwtToken = JwtToken(user);


                return user;
            }
            catch(Exception ex) {

                
                user.Error.Add(ex.Message);
                
                return null;
            }

            
        }

        public async Task<IEnumerable<IdentityUser>> GetUsers()
        {
            var list = await _userManager.Users.ToListAsync();

            
            return list;
        }

        //TODO
        public async Task<UserDto> Edit(IFormFile Image, UserDto userDto)
        {
            var user = await _userManager.FindByEmailAsync(userDto.Email);

            user.PhoneNumber = userDto.PhoneNumber;
            user.City = userDto.City;
            user.Gender = userDto.Gender;
            user.Age = userDto.Age;
            user.ImagePath = await UploadImage(Image,userDto.UserName,user.Id);

            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                foreach (var item in result.Errors)
                {
                    userDto.Error.Add(item.ToString());
                }
            }
            return userDto;
        }

        //TODO
        public async Task<string> ChangePassword(string Email, string currentPassword , string newPassword)
        {
            var user = await _userManager.FindByEmailAsync(Email);
            var checkPassword = await _userManager.CheckPasswordAsync(user, currentPassword);
            if (checkPassword)
            {
                var result = await _userManager.ChangePasswordAsync(user,currentPassword,newPassword);
                if (result.Succeeded)
                {
                    return newPassword;
                }
                else
                { 
                    return result.Errors.ToString(); 
                }
            }

            return "The Password is Invalid !";
        }
       
        private async Task<string> UploadImage(IFormFile Image , string UserName , string UserId)
        {

            var imageName = $"{UserName}_{UserId}_{Guid.NewGuid().ToString()}.jpg";
            var imagePath = Path.Combine(@"D:\repo\EDU\EduConsummer\wwwroot\Upload\", imageName);

            if (File.Exists(imagePath))
            {
                File.Delete(imagePath);
            }
            // Save the image to the folder
            using (var stream = new FileStream(imagePath, FileMode.Create))
            {
                await Image.CopyToAsync(stream);
            }       


            return imageName;
        }

        
    }
}
