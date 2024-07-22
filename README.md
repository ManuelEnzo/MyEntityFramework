# CommonAPI NuGet Package

The CommonAPI package provides a generic implementation for managing Data Transfer Objects (DTO) with Entity Framework. This package simplifies the registration and usage of common API services for your DTOs.

## Installation
You can install the package via the **.NET CLI**:

```bash
dotnet add package MyEntityFramework --version 3.0.0
```
## Usage
1) **Configure Your DTOs**

Ensure that your DTOs are defined in a namespace that ends with **"DTO"**. 
For example:

```csharp
namespace MyProject.DTO
{
    public class MyDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
```

2) Configure the **DbContext**

Define your DbContext with Entity Framework:

```csharp
using Microsoft.EntityFrameworkCore;
public class MyEntityFrameworkDbContext : DbContext
{
    public DbSet<MyDto> MyDtos { get; set; }
    
    public MyEntityFrameworkDbContext(DbContextOptions<MyEntityFrameworkDbContext> options)
        : base(options)
    {
    }
}
```

3) **Register the Services**

In the ConfigureServices method of your Startup class or in the Program.cs file, you can now register the common API services with a single line of code:

```csharp
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using MyProject.DTO;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        services.AddDbContext<MyEntityFrameworkDbContext>(options =>
            options.UseSqlServer(context.Configuration.GetConnectionString("DefaultConnection")));

        // Register the common API services
        services.AddCommonAPIServices<YourDbContext>("NamespaceDtos");

        // Other services...
    })
    .Build();

await host.RunAsync();

```
4. **Use the Services**

Now you can inject and use the **ICommonAPI<DTO>** services in your controllers or other components:


```csharp
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class MyDtoController : ControllerBase
{
    private readonly ICommonAPI<MyDto> _commonApi;

    public MyDtoController(ICommonAPI<MyDto> commonApi)
    {
        _commonApi = commonApi;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var dtos = await _commonApi.GetAllAsync();
        return Ok(dtos);
    }

    // Other endpoints...
}
```

# How AddCommonAPIServices Works

**Method AddCommonAPIServices**

The AddCommonAPIServices method extends IServiceCollection and adds common API services for each DTO type defined in a specified namespace across all loaded assemblies. Here's a step-by-step explanation of how it works:

1) **Method Signature:**

```csharp
public static IServiceCollection AddCommonAPIServices<TContext>(this IServiceCollection services, string namespaceLocationDtos)
    where TContext : DbContext
```

**Parameters:**

__services__: The service collection to which we want to add the new services. It is passed as this to indicate that it is an extension method.
__namespaceLocationDtos__: The namespace containing the DTO types. This parameter is used to find and register the services for all DTOs defined in that namespace.

2) **Retrieve DTO Types:**

```csharp
var assemblies = AppDomain.CurrentDomain.GetAssemblies();

var dtoTypes = assemblies.SelectMany(assembly => assembly.GetTypes())
                         .Where(t => t.Namespace != null &&
                                     string.Equals(t.Namespace, namespaceLocationDtos, StringComparison.InvariantCultureIgnoreCase) &&
                                     !t.IsAbstract && 
                                     !t.IsInterface);
```
Uses reflection to get all types defined in the specified assembly.
Filters the types to get only those that:
 - Are not **abstract types** (abstract).
 - Are not **interfaces** (interface).

3) **Register Services:**

```csharp
foreach (var dtoType in dtoTypes)
{
    var commonApiType = typeof(ICommonAPI<>).MakeGenericType(dtoType);
    var commonApiImplementationType = typeof(CommonAPI<>).MakeGenericType(dtoType);

    services.AddScoped(commonApiType, serviceProvider =>
    {
        var dbContext = serviceProvider.GetRequiredService<TContext>();
        return Activator.CreateInstance(commonApiImplementationType, dbContext);
    });
}
```

For each DTO type found:
 - **Creates a generic type** for ICommonAPI<DTO> using the current DTO type (dtoType).
 - **Creates a generic type** for CommonAPI<DTO> using the same DTO type.
 - **Registers the ICommonAPI<DTO> service in the service collection (services) as scoped** (i.e., a new instance is created for each request).
The registration uses a factory method that:
 - **Retrieves an instance of TContext (your DbContext) from the service provider.
 - **Creates an instance of CommonAPI<DTO>** passing the DbContext.

4) **Return the Modified Service Collection:**

```csharp
return services;
```
Returns the modified service collection, allowing for further configurations downstream.

# Contributing
Contributions are welcome! Please contact me.

# License
This project is licensed under the MIT License. See the LICENSE file for details.


