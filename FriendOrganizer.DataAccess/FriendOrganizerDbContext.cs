using FriendOrganizer.Model;
using Microsoft.EntityFrameworkCore;

namespace FriendOrganizer.DataAccess;

public class FriendOrganizerDbContext : DbContext
{
  public DbSet<Friend> Friends { get; set; }
  public DbSet<ProgrammingLanguage> Languages { get; set; }
  public DbSet<FriendPhoneNumber> FriendPhoneNumbers { get; set; }
  public DbSet<Meeting> Meetings { get; set; }

  protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
  {
    optionsBuilder.UseSqlServer(
      "Server=db;User Id=sa;Password=!!NAelen0328;Database=FriendOrganizer;Trusted_Connection=True;MultipleActiveResultSets=True;TrustServerCertificate=true;Integrated Security=False");
    base.OnConfiguring(optionsBuilder);
  }

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    base.OnModelCreating(modelBuilder);

    modelBuilder.Entity<Friend>().HasData(
      new Friend
      {
        Id = 1,
        FirstName = "Thomas",
        LastName = "Huber"
      },
      new Friend
      {
        Id = 2,
        FirstName = "Andreas",
        LastName = "Boehler"
      },
      new Friend
      {
        Id = 3,
        FirstName = "Julia",
        LastName = "Huber"
      },
      new Friend
      {
        Id = 4,
        FirstName = "Chrissi",
        LastName = "Egin"
      });

    modelBuilder.Entity<ProgrammingLanguage>().HasData(
      new ProgrammingLanguage
      {
        Id = 1,
        Name = "C#"
      },
      new ProgrammingLanguage
      {
        Id = 2,
        Name = "TypeScript"
      },
      new ProgrammingLanguage
      {
        Id = 3,
        Name = "F#"
      },
      new ProgrammingLanguage
      {
        Id = 4,
        Name = "Swift"
      },
      new ProgrammingLanguage
      {
        Id = 5,
        Name = "Java"
      });

    modelBuilder.Entity<FriendPhoneNumber>().HasData(
      new FriendPhoneNumber
      {
        Id = 1,
        Number = "8453253975",
        FriendId = 1
      });
  }
}