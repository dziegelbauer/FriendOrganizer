using System.Threading.Tasks;

namespace FriendOrganizer.UI.ViewModel;

public interface IDetailViewModel
{
  bool HasChanges { get; }
  int Id { get; }
  Task LoadAsync(int id);
}