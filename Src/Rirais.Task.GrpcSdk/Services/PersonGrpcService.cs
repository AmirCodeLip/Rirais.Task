using Rirais.Task.Grpc;
using System.Xml.Linq;

namespace Rirais.Task.GrpcSdk.Services;

internal class PersonGrpcService(Person.PersonClient grpcClient) : IPersonGrpcService
{

    public async Task<CreateResponse> CreateAsync(string name, string lastName, string nationalCode, string birthDate, CancellationToken cancellationToken)
    {
        var result = await grpcClient.CreateAsync(new CreateRequest
        {
            Name = name,
            LastName = lastName,
            NationalCode = nationalCode,
            BirthDate = birthDate,
        }, cancellationToken: cancellationToken);
        return result;
    }

    public async Task<UpdateResponse> UpdateAsync(string userId, string name, string lastName, string nationalCode, string birthDate, CancellationToken cancellationToken)
    {
        var result = await grpcClient.UpdateAsync(new UpdateRequest
        {
            Id = userId,
            Name = name,
            LastName = lastName,
            NationalCode = nationalCode,
            BirthDate = birthDate,
        }, cancellationToken: cancellationToken);
        return result;
    }

    public async Task<IEnumerable<PersonModel>> ReadAsync(CancellationToken cancellationToken)
    {
        return (await grpcClient.ReadAsync(new())).People;
    }

    public async System.Threading.Tasks.Task DeleteAsync(string id, CancellationToken cancellationToken)
    {
        await grpcClient.DeleteAsync(new DeleteRequest
        {
            Id = id
        });
    }



}
