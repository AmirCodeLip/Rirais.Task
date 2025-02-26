using Microsoft.EntityFrameworkCore;
using Rirais.Task.GrpcServer.Models;
using System.Collections.Generic;

namespace Rirais.Task.GrpcServer.Data;

public class AppDbContext : DbContext
{
    public DbSet<PersonEntity> Persons { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
}
