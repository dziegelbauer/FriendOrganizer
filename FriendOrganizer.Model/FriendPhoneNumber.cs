using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FriendOrganizer.Model;

public class FriendPhoneNumber
{
  public int Id { get; set; }

  [Phone] [Required] public string Number { get; set; }

  public int? FriendId { get; set; }

  [ForeignKey("FriendId")] public Friend? Friend { get; set; }
}