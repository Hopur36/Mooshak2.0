using Mooshack_2.Models;
using System.Data.Entity;
using WebApplication1.Models.Entities;
using Microsoft.AspNet.Identity.EntityFramework;


namespace Mooshak2._0Test
{
    /// <summary>
    /// This is an example of how we'd create a fake database by implementing the 
    /// same interface that the BookeStoreEntities class implements.
    /// </summary>
    public class MockDatabase : IMyDataContext
    {
        /// <summary>
        /// Sets up the fake database.
        /// </summary>
        public MockDatabase()
        {
            // We're setting our DbSets to be InMemoryDbSets rather than using SQL Server.
            this.Courses = new InMemoryDbSet<Course>();
            this.Assignments = new InMemoryDbSet<Assignment>();
            this.CourseStudent = new InMemoryDbSet<CourseStudent>();
            this.CourseTeacher = new InMemoryDbSet<CourseTeacher>();
            this.Milestones = new InMemoryDbSet<Milestone>();
            this.Submissions = new InMemoryDbSet<Submission>();
            this.Users = new InMemoryDbSet<ApplicationUser>();

        }

        public IDbSet<Course> Courses { get; set; }
        public IDbSet<Assignment> Assignments { get; set; }
        public IDbSet<CourseStudent> CourseStudent { get; set; }
        public IDbSet<CourseTeacher> CourseTeacher { get; set; }
        public IDbSet<Milestone> Milestones { get; set; }
        public IDbSet<Submission> Submissions { get; set; }
        public IDbSet<ApplicationUser> Users { get; set; }
        public IDbSet<IdentityRole> Roles { get; set; }

        public int SaveChanges()
        {
            // Pretend that each entity gets a database id when we hit save.
            int changes = 0;
            //changes += DbSetHelper.IncrementPrimaryKey<Author>(x => x.AuthorId, this.Authors);
            //changes += DbSetHelper.IncrementPrimaryKey<Book>(x => x.BookId, this.Books);

            return changes;
        }

        public void Dispose()
        {
            // Do nothing!
        }
    }
}
