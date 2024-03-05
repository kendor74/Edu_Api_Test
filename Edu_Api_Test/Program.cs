var builder = WebApplication.CreateBuilder(args);




builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();



var IdentityConnection = builder.Configuration.GetConnectionString("EDU_DataBase_Test");
builder.Services.AddDbContext<IdentityUserDbContext>(options => options.UseSqlServer(IdentityConnection));

var AppConnection = builder.Configuration.GetConnectionString("EDU_DataBase_Test");
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(AppConnection));


// Add services to the container.

//Authentication

// to allow some Espcial Characters for UserName
//assigning Roles to be used 
builder.Services.AddIdentity<User, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedEmail = false;
    options.User.RequireUniqueEmail = true;
    options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+#"; // Add any additional allowed characters

}).AddRoles<IdentityRole>()
  .AddEntityFrameworkStores<IdentityUserDbContext>()
  .AddDefaultTokenProviders();


//Inject Interfaces with Services


builder.Services.AddScoped<IUser, UserServices>();

builder.Services.AddScoped<IRepository<Student>, RepositoryHandler<Student>>();






//JWT Bearer Token
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;


}).AddJwtBearer(options =>
{
#pragma warning disable CS8604 // Possible null reference argument.
    options.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateActor = true,
        ValidateIssuer = true,
        ValidateAudience = true,
        RequireExpirationTime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration.GetSection("Jwt:Issuer").Value,
        ValidAudience = builder.Configuration.GetSection("Jwt:Audiance").Value,
        IssuerSigningKey =
        new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration.GetSection("Jwt:Key").Value))
    };
#pragma warning restore CS8604 // Possible null reference argument.
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();





using (var scope = app.Services.CreateScope())
{
    var _roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

    var roles = new[] { "Admin", "Student", "Tutor" };

    foreach (var role in roles)
    {
        var roleExists = await _roleManager.RoleExistsAsync(role);

        if (!roleExists)
        {
            // Role doesn't exist, create the role
            await _roleManager.CreateAsync(new IdentityRole(role));
        }
    }
}


using (var scope = app.Services.CreateScope())
{
    var _userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();

    string Email = "Admin@gmail.com";
    string Password = "Admin@123Admin#1";

    if (await _userManager.FindByEmailAsync(Email) == null)
    {
        var user = new User
        {
            Email = Email,
            FirstName = "Mohamed",
            LastName = "Abd Allateaf",
            UserName = "Ken#1",
            City = "Cairo",
            Gender = "Male",
            ImagePath = "MaleIcon.png",
            PhoneNumber = "01234567891",
            Age = 22
        };

        await _userManager.CreateAsync(user, Password);
        await _userManager.AddToRoleAsync(user, "Admin");


    }
}



app.Run();
