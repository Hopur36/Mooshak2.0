using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using WebApplication1.Models.Entities;

namespace Mooshack_2.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    } 

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {


        public DbSet<Course>        Courses         { get; set; }
        public DbSet<Assignment>    Assignments     { get; set; }
        public DbSet<CourseStudent> CourseStudent  { get; set; }
        public DbSet<CourseTeacher> CourseTeacher  { get; set; }
        public DbSet<Milestone>     Milestones      { get; set; }
        public DbSet<Submission>    Submissions     { get; set; }

        
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        public System.Data.Entity.DbSet<Mooshack_2.Models.ViewModels.AssignmentViewModel> AssignmentViewModels { get; set; }

        public System.Data.Entity.DbSet<Mooshack_2.Models.ViewModels.CreateAssignmentViewModel> CreateAssignmentViewModels { get; set; }
    }
}