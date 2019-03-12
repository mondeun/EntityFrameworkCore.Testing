using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using EntityFrameworkCore.Testing.Ninject.Tests;
using Ninject;
using Ninject.MockingKernel;
using Ninject.MockingKernel.NSubstitute;
using Xunit;

namespace EntityFrameworkCore.Testing.NSubstitute.Ninject.Tests
{
    public class NSubstituteDbContextTests : DbContextTests
    {
        [Fact]
        public override void Can_setup_dbset()
        {
            using (var kernel = this.CreateMockingKernel())
            {
                var blogs = new List<Blog> { new Blog(), new Blog() };
                kernel.Get<DbSet<Blog>>().SetupData(blogs);

                var db = kernel.Get<BlogDbContext>();

                Assert.Equal(2, db.Blogs.Count());
            }
        }

        protected override MockingKernel CreateMockingKernel()
        {
            var kernel = new NSubstituteMockingKernel();
            kernel.Load(new EntityFrameworkTestingNSubstituteModule());
            return kernel;
        }
    }
}
