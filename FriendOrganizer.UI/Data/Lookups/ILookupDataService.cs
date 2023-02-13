using System.Collections.Generic;
using System.Threading.Tasks;
using FriendOrganizer.Model;

namespace FriendOrganizer.UI.Data.Lookups;

public interface ILookupDataService
{
  Task<IEnumerable<LookupItem>> GetFriendLookupAsync();
  Task<IEnumerable<LookupItem>> GetMeetingLookupAsync();
}