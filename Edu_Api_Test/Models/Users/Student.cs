namespace EducationlPlatform.Models.Users
{
    public class Student 
    {
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int StudentId { get; set; }
        
        //public virtual ICollection<StudentRoom>? Rooms { get; set; }

        [ForeignKey(nameof(UserId))]        
        public string UserId { get; set; }
        public virtual User User { get; set; }
    }
}
