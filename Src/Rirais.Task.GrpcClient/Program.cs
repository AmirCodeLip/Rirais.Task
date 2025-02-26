using Grpc.Core;
using Microsoft.Extensions.DependencyInjection;
using Rirais.Task.GrpcSdk;
using Rirais.Task.GrpcSdk.Services;

/// Step 1: Define Service Collection.
var serviceCollection = new ServiceCollection();

/// Step 2: Register Sdk Requirements.
ServiceExtensions.ConfigureClientServices(serviceCollection);

/// Step 3: Build the Services for grpc and resolve.
var serviceProvider = serviceCollection.BuildServiceProvider();
var greeterGrpcService = serviceProvider.GetRequiredService<IPersonGrpcService>();

Task.Run(async () =>
{
    using CancellationTokenSource cts = new(); // Create CancellationTokenSource
    CancellationToken processToken = cts.Token; // Get the token
    /// Wait for the server to load.
    Task.Delay(TimeSpan.FromMilliseconds(1000)).Wait();
    /// create some users.
    var user1 = await greeterGrpcService.CreateAsync("mahnaz", "mahnazian", "3101723507", "2000/10/13", processToken);
    var user2 = await greeterGrpcService.CreateAsync("amir", "momeni", "9667627780", "2000/01/01", processToken);
    var fakeUser = await greeterGrpcService.CreateAsync("fake user", "fake", "1524619841", "2000/01/01", processToken);
    Console.WriteLine($"user with name {user1.Name} and id {user1.Id} is successfully registered.");
    Console.WriteLine($"user with name {user2.Name} and id {user2.Id} is successfully registered.");
    try
    {
        await greeterGrpcService.CreateAsync("mahnaz", "mahnazian", "1536", "2000/10/13", processToken);
    }
    catch (RpcException ex)
    {
        /// it'll be return error because valid national code is not provided.
        Console.WriteLine($"Client Error: {ex.Status.Detail}");
    }
    try
    {
        await greeterGrpcService.CreateAsync("mahnaz", "mahnazian", "9667627780", "2000/10/13", processToken);
    }
    catch (RpcException ex)
    {
        /// it'll be return error because valid national code is not provided.
        Console.WriteLine($"Client Error: {ex.Status.Detail}");
    }
    /// update user 1.
    var updatedUser2 = await greeterGrpcService.UpdateAsync(user2.Id, "amir2", "momeni", "9667627780", "2000/01/01", processToken);
    Console.WriteLine($"user with name {user2.Name} and id {updatedUser2.Id} is successfully renamed to {updatedUser2.Name}.");
    /// read list of pepole.
    var pepole = await greeterGrpcService.ReadAsync(processToken);
    Console.WriteLine("list of the pepole are registered " + string.Join(',', pepole.Select(x => x.Name)));
    /// delete one user so fake user is not exist in database anymore.
    await greeterGrpcService.DeleteAsync(fakeUser.Id, processToken);
    pepole = await greeterGrpcService.ReadAsync(processToken);
    Console.WriteLine("list of the pepole are registered " + string.Join(',', pepole.Select(x => x.Name)));
    cts.Cancel();
}).Wait();

Console.ReadLine();
