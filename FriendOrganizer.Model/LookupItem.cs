using System.ComponentModel.DataAnnotations;

namespace FriendOrganizer.Model;

public class LookupItem
{
  [Key] public int Id { get; set; }

  public string DisplayMember { get; set; }
}