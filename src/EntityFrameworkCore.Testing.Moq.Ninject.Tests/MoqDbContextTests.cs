using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using EntityFrameworkCore.Testing.Ninject.Tests;
using Ninject;
using Ninject.MockingKernel;
using Ninject.MockingKernel.Moq;
using Xunit;

namespace EntityFrameworkCore.Testing.Moq.Ninject.Tests
{
    public class MoqDbContextTests : DbContextTests
    {

        [Fact]
        public override void Can_setup_dbset()
        {
            using (var kernel = this.CreateMockingKernel() as MoqMockingKernel)
            {
                var blogs = new List<Blog> { new Blog(), new Blog() };
                kernel.GetMock<DbSet<Blog>>().SetupData(blogs);

                var db = kernel.Get<BlogDbContext>();

                Assert.Equal(2, db.Blogs.Count());
            }
        }

        protected override MockingKernel CreateMockingKernel()
        {
            var kernel = new MoqMockingKernel();
            kernel.Load(new EntityFrameworkTestingMoqModule());
            return kernel;
        }
    }
}
