using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using EntityFrameworkCore.Testing.Ninject.Tests;
using Ninject;
using Ninject.MockingKernel;
using Ninject.MockingKernel.FakeItEasy;
using Xunit;

namespace EntityFrameworkCore.Testing.FakeItEasy.Ninject.Tests
{
    public class FakeItEasyDbContextTests : DbContextTests
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
            var kernel = new FakeItEasyMockingKernel();
            kernel.Load(new EntityFrameworkTestingFakeItEasyModule());
            return kernel;
        }
    }
}
