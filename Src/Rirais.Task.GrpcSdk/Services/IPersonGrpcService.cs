using Rirais.Task.Grpc;

namespace Rirais.Task.GrpcSdk.Services;

/// <summary>
///     interface for comunication between client and grpc service.
/// </summary>
public interface IPersonGrpcService
{
    /// <param name="birthDate">with format YYYY/MM/DD.</param>
    Task<CreateResponse> CreateAsync(string name, string lastName, string nationalCode, string birthDate, CancellationToken cancellationToken);

    /// <param name="birthDate">with format YYYY/MM/DD.</param>
    Task<UpdateResponse> UpdateAsync(string userId, string name, string lastName, string nationalCode, string birthDate, CancellationToken cancellationToken);
    
    Task<IEnumerable<PersonModel>> ReadAsync(CancellationToken cancellationToken);

    System.Threading.Tasks.Task DeleteAsync(string id, CancellationToken cancellationToken);
}
