using System.Data.Entity;
using System.Linq;
using Ninject;
using Ninject.MockingKernel;
using Xunit;

namespace EntityFrameworkCore.Testing.Ninject.Tests
{
    public abstract class DbContextTests
    {
        [Fact]
        public void Can_auto_mock_dbset()
        {
            using (var kernel = this.CreateMockingKernel())
            {
                var db = kernel.Get<BlogDbContext>();

                Assert.NotNull(db.Blogs);
            }
        }

        [Fact]
        public abstract void Can_setup_dbset();

        [Fact]
        public void Can_query_dbset_if_not_setup()
        {
            using (var kernel = this.CreateMockingKernel())
            {
                var db = kernel.Get<BlogDbContext>();

                Assert.Equal(0, db.Blogs.Count());
            }
        }

        protected abstract MockingKernel CreateMockingKernel();

        public class Blog
        {
            public int BlogId { get; set; }
            public string Url { get; set; }
        }

        public class BlogDbContext : DbContext
        {
            public virtual DbSet<Blog> Blogs { get; set; }
        }
    }
}
