using Microsoft.Extensions.DependencyInjection;
using Rirais.Task.Grpc;
using Rirais.Task.GrpcSdk.Services;

namespace Rirais.Task.GrpcSdk;

public static class ServiceExtensions
{

    public static void ConfigureClientServices(this IServiceCollection services)
    {

        services.AddGrpcClient<Person.PersonClient>(client =>
        {
            client.Address = new Uri("http://localhost:5076");
        });

        services.AddScoped<IPersonGrpcService, PersonGrpcService>();

    }

}
