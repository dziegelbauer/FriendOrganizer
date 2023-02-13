using System.Linq;
using System.Threading.Tasks;
using FriendOrganizer.DataAccess;
using FriendOrganizer.Model;
using Microsoft.EntityFrameworkCore;

namespace FriendOrganizer.UI.Data.Repositories;

public class FriendRepository : GenericRepository<Friend, FriendOrganizerDbContext>, IFriendRepository
{
  public FriendRepository(FriendOrganizerDbContext friendOrganizerDb) : base(friendOrganizerDb)
  {
  }

  public override async Task<Friend> GetByIdAsync(int id)
  {
    return await _dbContext.Friends
      .Include(f => f.PhoneNumbers)
      .SingleAsync(f => f.Id == id);
  }

  public void RemovePhoneNumber(FriendPhoneNumber model)
  {
    _dbContext.FriendPhoneNumbers.Remove(model);
  }

  public async Task<bool> HasMeetingsAsync(int id)
  {
    return await _dbContext.Meetings.AsNoTracking()
      .Include(m => m.Friends)
      .AnyAsync(m => m.Friends.Any(f => f.Id == id));
  }
}