using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using Rirais.Task.GrpcServer.Data;
using Rirais.Task.GrpcServer.Models;
using System.Globalization;

namespace Rirais.Task.Grpc.Services;

public class PersonService(AppDbContext dbContext, ILogger<PersonService> logger) : Person.PersonBase
{

    public override async Task<CreateResponse> Create(CreateRequest request, ServerCallContext context)
    {
        /// validate for check invalid arguments.
        if (string.IsNullOrWhiteSpace(request.Name) || string.IsNullOrWhiteSpace(request.LastName) || string.IsNullOrWhiteSpace(request.BirthDate) || string.IsNullOrWhiteSpace(request.NationalCode))
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument, $"please fill required fields."));
        }
        else if (!ValidationExtensions.IsValidNationalCode(request.NationalCode))
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument, $"{nameof(request.NationalCode)} is not valid."));
        }
        /// check to see provided BirthDate is an valid time.
        if (!DateTime.TryParseExact(request.BirthDate, "yyyy/MM/dd", null, DateTimeStyles.None, out var birthDate))
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument, $"{nameof(request.BirthDate)} is invalid date."));
        }
        /// same user with national code or same name and last name already is registerd.
        if (dbContext.Persons.Any(x => (x.Name == request.Name && x.LastName == request.LastName) || x.NationalCode == request.NationalCode))
        {
            throw new RpcException(new Status(StatusCode.AlreadyExists, $"user with same data is already exist."));
        }
        try
        {
            /// create new user data.
            var newUserId = Guid.NewGuid();
            /// save object in database.
            dbContext.Persons.Add(new PersonEntity
            {
                Id = newUserId,
                Name = request.Name,
                LastName = request.LastName,
                NationalCode = request.NationalCode,
                BirthDate = birthDate,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                IsDeleted = false
            });
            await dbContext.SaveChangesAsync();
            logger.Log(LogLevel.Information, $"user with name {request.Name} and id {newUserId} created successfully");
            var users = dbContext.Persons.ToList();
            return (new CreateResponse
            {
                Id = newUserId.ToString(),
                Name = request.Name
            });
        }
        catch (Exception ex)
        {
            logger.LogError(ex.Message.ToString());
            throw new RpcException(new Status(StatusCode.Internal, $"server not responding."));
        }
    }

    public override async Task<UpdateResponse> Update(UpdateRequest request, ServerCallContext context)
    {
        /// parse user id.
        var userId = Guid.Empty;
        if (!Guid.TryParse(request.Id, out userId) || !dbContext.Persons.Any(x => x.Id == userId && !x.IsDeleted))
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument, $"invalid user id is provided."));
        }
        /// validate for check invalid arguments.
        if (string.IsNullOrWhiteSpace(request.Name) || string.IsNullOrWhiteSpace(request.LastName) || string.IsNullOrWhiteSpace(request.BirthDate) || string.IsNullOrWhiteSpace(request.NationalCode))
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument, $"please fill required fields."));
        }
        else if (!ValidationExtensions.IsValidNationalCode(request.NationalCode))
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument, $"{nameof(request.NationalCode)} is not valid."));
        }
        /// check to see provided BirthDate is an valid time.
        if (!DateTime.TryParseExact(request.BirthDate, "yyyy/MM/dd", null, DateTimeStyles.None, out var birthDate))
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument, $"{nameof(request.BirthDate)} is invalid date."));
        }
        /// same user with national code or same name and last name already is registerd.
        if (dbContext.Persons.Any(x => x.Id != userId && ((x.Name == request.Name && x.LastName == request.LastName) || x.NationalCode == request.NationalCode)))
        {
            throw new RpcException(new Status(StatusCode.AlreadyExists, $"user with same data is already exist."));
        }
        try
        {
            /// find person.
            var person = dbContext.Persons.Find(userId)!;
            /// update person object in database.
            person.UpdatedAt = DateTime.Now;
            person.Name = request.Name;
            person.LastName = request.LastName;
            person.NationalCode = request.NationalCode;
            person.BirthDate = birthDate;
            dbContext.Update(person);
            await dbContext.SaveChangesAsync();
            logger.Log(LogLevel.Information, $"user with name {request.Name} and id {userId} updated successfully");
            var users = dbContext.Persons.ToList();
            return (new UpdateResponse
            {
                Id = userId.ToString(),
                Name = request.Name
            });
        }
        catch (Exception ex)
        {
            logger.LogError(ex.Message.ToString());
            throw new RpcException(new Status(StatusCode.Internal, $"server not responding."));
        }
    }

    public override async Task<PeopleList> Read(Empty request, ServerCallContext context)
    {
        var response = new PeopleList();
        response.People.AddRange(await dbContext.Persons.Where(x => !x.IsDeleted).Select(x => new PersonModel
        {
            Id = x.Id.ToString(),
            Name = x.Name,
            LastName = x.LastName,
            NationalCode = x.NationalCode,
            BirthDate = x.BirthDate!.Value.ToString("yyyy/MM/dd")
        }).ToListAsync());
        return response;
    }

    public override async Task<Empty> Delete(DeleteRequest request, ServerCallContext context)
    {
        /// parse user id.
        var userId = Guid.Empty;
        if (!Guid.TryParse(request.Id, out userId) || !dbContext.Persons.Any(x => x.Id == userId && !x.IsDeleted))
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument, $"invalid user id is provided."));
        }
        var person = await dbContext.Persons.FindAsync(userId)!;
        person!.IsDeleted = true;
        dbContext.Update(person);
        await dbContext.SaveChangesAsync();
        return new();
    }

}