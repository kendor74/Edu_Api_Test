namespace EducationlPlatform.Models.Context
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);


            modelBuilder.Entity<Student>()
               .HasOne(s => s.User)
               .WithOne()
               .HasForeignKey<Student>(s => s.UserId)
               .IsRequired();
        }

        public DbSet<Student> Students { get; set; }



    }
}
