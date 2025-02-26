namespace Rirais.Task.GrpcServer.Models;

public class PersonEntity
{
    public PersonEntity()
    {
        IsDeleted = false;
    }
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? LastName { get; set; }
    public string? NationalCode { get; set; }
    public DateTime? BirthDate { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public bool IsDeleted { get; set; }
}