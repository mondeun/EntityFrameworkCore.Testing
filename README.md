## EntityFrameworkCore.Testing [![NuGet Version](http://img.shields.io/nuget/v/EntityFrameworkTesting.svg?style=flat)](https://www.nuget.org/packages/EntityFrameworkTesting/) [![Build status](https://ci.appveyor.com/api/projects/status/8dam29eko6pyot3v?svg=true)](https://ci.appveyor.com/project/scott-xu/entityframework-testing)

**EntityFrameworkCore.Testing** provides an implementation of `DbAsyncQueryProvider` that can be used when testing a component that uses async queries with EntityFramework.

The project is cut from EntityFrameworks' [source code](http://entityframework.codeplex.com/SourceControl/latest#test/EntityFramework/FunctionalTests/TestDoubles/). Some changes are made to be compliant with StyleCop/CodeAnalysis

All the following mocked `DbSet` operations work as expected:

- `Add`
- `AddRange`
- `AsNoTracking`
- `Attach`
- `Create`
- `Find`
- `FindAsync`
- `Include`
- `Remove`
- `RemoveRange`

The following samples are based on this Controller：
```C#
public class BlogsController : Controller
{
    private readonly BloggingContext db;

    public BlogsController(BloggingContext context)
    {
        db = context;
    }

    public async Task<ViewResult> Index()
    {
        var query = db.Blogs.OrderBy(b => b.Name);

        return View(await query.ToListAsync());
    }
}
```

### EntityFrameworkCore.Testing.Moq [![NuGet Version](http://img.shields.io/nuget/v/EntityFrameworkTesting.Moq.svg?style=flat)](https://www.nuget.org/packages/EntityFrameworkTesting.Moq/) 

**EntityFrameworkCore.Testing.Moq** provides a helpful extension method to mock EntityFramework's DbSets using Moq. 

You can write a unit test against a mock context as follows. `SetupData` extension method is part of EntityFrameworkCore.Testing.Moq.

```C#
[TestMethod]
public async Task Index_returns_blogs_ordered_by_name()
{
    // Create some test data
    var data = new List<Blog>
    {
        new Blog{ Name = "BBB" },
        new Blog{ Name = "CCC" },
        new Blog{ Name = "AAA" }
    };

    // Create a mock set and context
    var set = new Mock<DbSet<Blog>>()
        .SetupData(data);

    var context = new Mock<BloggingContext>();
    context.Setup(c => c.Blogs).Returns(set.Object);

    // Create a BlogsController and invoke the Index action
    var controller = new BlogsController(context.Object);
    var result = await controller.Index();

    // Check the results
    var blogs = (List<Blog>)result.Model;
    Assert.AreEqual(3, blogs.Count());
    Assert.AreEqual("AAA", blogs[0].Name);
    Assert.AreEqual("BBB", blogs[1].Name);
    Assert.AreEqual("CCC", blogs[2].Name);
}
```

### EntityFrameworkCore.Testing.Moq.Ninject [![NuGet Version](http://img.shields.io/nuget/v/EntityFrameworkTesting.Moq.Ninject.svg?style=flat)](https://www.nuget.org/packages/EntityFrameworkTesting.Moq.Ninject/) 

**EntityFrameworkCore.Testing.Moq.Ninject** provides a Ninject Module to auto mock `DbContext` and its `DbSet<>` properties using Ninject.MockingKernel.Moq.

```C#
[TestMethod]
public async Task Index_returns_blogs_ordered_by_name()
{
    using (var kernel = new MoqMockingKernel())
    {
        kernel.Load(new EntityFrameworkTestingMoqModule());

        // Create some test data
        var data = new List<Blog>
        {
            new Blog{ Name = "BBB" },
            new Blog{ Name = "CCC" },
            new Blog{ Name = "AAA" }
        };
        
        // Setup mock set
        kernel.GetMock<DbSet<Blog>>()
            .SetupData(data);

        // Get a BlogsController and invoke the Index action
        var controller = kernel.Get<BlogsController>();
        var result = await controller.Index();

        // Check the results
        var blogs = (List<Blog>)result.Model;
        Assert.AreEqual(3, blogs.Count());
        Assert.AreEqual("AAA", blogs[0].Name);
        Assert.AreEqual("BBB", blogs[1].Name);
        Assert.AreEqual("CCC", blogs[2].Name);
    }
}
``` 

### EntityFrameworkCore.Testing.NSubstitute [![NuGet Version](http://img.shields.io/nuget/v/EntityFrameworkTesting.NSubstitute.svg?style=flat)](https://www.nuget.org/packages/EntityFrameworkTesting.NSubstitute/) 

**EntityFrameworkCore.Testing.NSubstitute** provides a helpful extension method to mock EntityFramework's DbSets using [NSubstitute](http://nsubstitute.github.io/). 

You can write a unit test against a mock context as follows. `SetupData` extension method is part of EntityFrameworkCore.Testing.NSubstitute.

```C#
[TestMethod]
public async Task Index_returns_blogs_ordered_by_name()
{
    // Create some test data
    var data = new List<Blog>
    {
        new Blog{ Name = "BBB" },
        new Blog{ Name = "CCC" },
        new Blog{ Name = "AAA" }
    };

    // Create a DbSet substitute.
    var set = Substitute.For<DbSet<Blog>, IQueryable<Blog>, IDbAsyncEnumerable<Blog>>()
                        .SetupData(data);

    var context = Substitute.For<BloggingContext>();
    context.Blogs.Returns(set);

    // Create a BlogsController and invoke the Index action
    var controller = new BlogsController(context);
    var result = await controller.Index();

    // Check the results
    var blogs = (List<Blog>)result.Model;
    Assert.AreEqual(3, blogs.Count());
    Assert.AreEqual("AAA", blogs[0].Name);
    Assert.AreEqual("BBB", blogs[1].Name);
    Assert.AreEqual("CCC", blogs[2].Name);
}
```

### EntityFrameworkCore.Testing.NSubstitute.Ninject [![NuGet Version](http://img.shields.io/nuget/v/EntityFrameworkTesting.NSubstitute.Ninject.svg?style=flat)](https://www.nuget.org/packages/EntityFrameworkTesting.NSubstitute.Ninject/) 

**EntityFrameworkCore.Testing.NSubstitute.Ninject** provides a Ninject Module to auto mock `DbContext` and its `DbSet<>` properties using Ninject.MockingKernel.NSubstitute.

```C#
[TestMethod]
public async Task Index_returns_blogs_ordered_by_name()
{
    using (var kernel = new NSubstituteMockingKernel())
    {
        kernel.Load(new EntityFrameworkTestingNSubstituteModule());

        // Create some test data
        var data = new List<Blog>
        {
            new Blog{ Name = "BBB" },
            new Blog{ Name = "CCC" },
            new Blog{ Name = "AAA" }
        };
        
        // Setup mock set
        kernel.Get<DbSet<Blog>>()
            .SetupData(data);

        // Get a BlogsController and invoke the Index action
        var controller = kernel.Get<BlogsController>();
        var result = await controller.Index();

        // Check the results
        var blogs = (List<Blog>)result.Model;
        Assert.AreEqual(3, blogs.Count());
        Assert.AreEqual("AAA", blogs[0].Name);
        Assert.AreEqual("BBB", blogs[1].Name);
        Assert.AreEqual("CCC", blogs[2].Name);
    }
}
``` 

### EntityFrameworkCore.Testing.FakeItEasy [![NuGet Version](http://img.shields.io/nuget/v/EntityFrameworkTesting.FakeItEasy.svg?style=flat)](https://www.nuget.org/packages/EntityFrameworkTesting.FakeItEasy/) 

**EntityFrameworkCore.Testing.FakeItEasy** provides a helpful extension method to mock EntityFramework's DbSets using [FakeItEasy](http://fakeiteasy.github.io/). 

You can write a unit test against a mock context as follows. `SetupData` extension method is part of EntityFrameworkCore.Testing.FakeItEasy.

```C#
[TestMethod]
public async Task Index_returns_blogs_ordered_by_name()
{
    // Create some test data
    var data = new List<Blog>
    {
        new Blog{ Name = "BBB" },
        new Blog{ Name = "CCC" },
        new Blog{ Name = "AAA" }
    };

    // Create a DbSet substitute.
    var set = A.Fake<DbSet<Blog>>(o => o.Implements(typeof(IQueryable<Blog>)).Implements(typeof(IDbAsyncEnumerable<Blog>)))
                        .SetupData(data);

    var context = A.Fake<BloggingContext>();
    context.Blogs.Returns(set);

    // Create a BlogsController and invoke the Index action
    var controller = new BlogsController(context);
    var result = await controller.Index();

    // Check the results
    var blogs = (List<Blog>)result.Model;
    Assert.AreEqual(3, blogs.Count());
    Assert.AreEqual("AAA", blogs[0].Name);
    Assert.AreEqual("BBB", blogs[1].Name);
    Assert.AreEqual("CCC", blogs[2].Name);
}
```

### EntityFrameworkCore.Testing.FakeItEasy.Ninject [![NuGet Version](http://img.shields.io/nuget/v/EntityFrameworkTesting.FakeItEasy.Ninject.svg?style=flat)](https://www.nuget.org/packages/EntityFrameworkTesting.FakeItEasy.Ninject/) 

**EntityFrameworkCore.Testing.FakeItEasy.Ninject** provides a Ninject Module to auto mock `DbContext` and its `DbSet<>` properties using Ninject.MockingKernel.FakeItEasy.

```C#
[TestMethod]
public async Task Index_returns_blogs_ordered_by_name()
{
    using (var kernel = new FakeItEasyMockingKernel())
    {
        kernel.Load(new EntityFrameworkTestingFakeItEasyModule());

        // Create some test data
        var data = new List<Blog>
        {
            new Blog{ Name = "BBB" },
            new Blog{ Name = "CCC" },
            new Blog{ Name = "AAA" }
        };
        
        // Setup mock set
        kernel.Get<DbSet<Blog>>()
            .SetupData(data);

        // Get a BlogsController and invoke the Index action
        var controller = kernel.Get<BlogsController>();
        var result = await controller.Index();

        // Check the results
        var blogs = (List<Blog>)result.Model;
        Assert.AreEqual(3, blogs.Count());
        Assert.AreEqual("AAA", blogs[0].Name);
        Assert.AreEqual("BBB", blogs[1].Name);
        Assert.AreEqual("CCC", blogs[2].Name);
    }
}
``` 
