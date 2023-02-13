using System.Threading.Tasks;
using FriendOrganizer.DataAccess;
using FriendOrganizer.Model;
using Microsoft.EntityFrameworkCore;

namespace FriendOrganizer.UI.Data.Repositories;

public class ProgrammingLanguageRepository
  : GenericRepository<ProgrammingLanguage, FriendOrganizerDbContext>, IProgrammingLanguageRepository
{
  public ProgrammingLanguageRepository(FriendOrganizerDbContext dbContext) : base(dbContext)
  {
  }

  public async Task<bool> IsReferencedByFriendAsync(int id)
  {
    return await _dbContext.Friends.AsNoTracking().AnyAsync(f => f.FavoriteLanguageId == id);
  }
}