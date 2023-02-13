using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FriendOrganizer.DataAccess;
using FriendOrganizer.Model;
using Microsoft.EntityFrameworkCore;

namespace FriendOrganizer.UI.Data.Lookups;

public class LookupDataService : ILookupDataService, IProgrammingLanguageLookupDataService
{
  private readonly FriendOrganizerDbContext _friendOrganizerDb;

  public LookupDataService(FriendOrganizerDbContext friendOrganizerDb)
  {
    _friendOrganizerDb = friendOrganizerDb;
  }

  public async Task<IEnumerable<LookupItem>> GetFriendLookupAsync()
  {
    return await _friendOrganizerDb.Friends.AsNoTracking().Select(f => new LookupItem
    {
      Id = f.Id,
      DisplayMember = $"{f.FirstName} {f.LastName}"
    }).ToListAsync();
  }

  public async Task<IEnumerable<LookupItem>> GetMeetingLookupAsync()
  {
    return await _friendOrganizerDb.Meetings.AsNoTracking().Select(m => new LookupItem
    {
      Id = m.Id,
      DisplayMember = m.Title
    }).ToListAsync();
  }

  public async Task<IEnumerable<LookupItem>> GetProgrammingLanguageLookupAsync()
  {
    return await _friendOrganizerDb.Languages.AsNoTracking().Select(l => new LookupItem
    {
      Id = l.Id,
      DisplayMember = l.Name
    }).ToListAsync();
  }
}