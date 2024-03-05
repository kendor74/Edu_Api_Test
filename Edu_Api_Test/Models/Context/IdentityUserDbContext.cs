namespace EducationlPlatform.Models
{
    public class IdentityUserDbContext : IdentityDbContext<User,IdentityRole,string>
    {
        public IdentityUserDbContext(DbContextOptions<IdentityUserDbContext> option) : base(option)
        {
        }

    }
}
