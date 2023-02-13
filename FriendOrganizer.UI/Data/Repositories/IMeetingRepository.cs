using System.Collections.Generic;
using System.Threading.Tasks;
using FriendOrganizer.Model;

namespace FriendOrganizer.UI.Data.Repositories;

public interface IMeetingRepository : IGenericRepository<Meeting>
{
  public Task<List<Friend>> GetAllFriendsAsync();
  Task ReloadFriendAsync(int id);
}