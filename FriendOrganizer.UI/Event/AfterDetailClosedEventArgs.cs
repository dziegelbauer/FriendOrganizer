namespace FriendOrganizer.UI.Event;

public class AfterDetailClosedEventArgs
{
  public int Id { get; set; }
  public string ViewModelName { get; set; } = string.Empty;
}