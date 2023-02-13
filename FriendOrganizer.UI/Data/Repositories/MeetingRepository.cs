using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FriendOrganizer.DataAccess;
using FriendOrganizer.Model;
using Microsoft.EntityFrameworkCore;

namespace FriendOrganizer.UI.Data.Repositories;

public class MeetingRepository : GenericRepository<Meeting, FriendOrganizerDbContext>, IMeetingRepository
{
  public MeetingRepository(FriendOrganizerDbContext dbContext) : base(dbContext)
  {
  }

  public override async Task<Meeting> GetByIdAsync(int id)
  {
    return await _dbContext.Meetings
      .Include(m => m.Friends)
      .SingleAsync(m => m.Id == id);
  }

  public async Task<List<Friend>> GetAllFriendsAsync()
  {
    return await _dbContext.Friends.ToListAsync();
  }

  public async Task ReloadFriendAsync(int id)
  {
    var entity = _dbContext.ChangeTracker.Entries<Friend>().SingleOrDefault(db => db.Entity.Id == id);

    if (entity is not null) await entity.ReloadAsync();
  }
}