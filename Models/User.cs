using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace krist_server.Models
{
  [Table("users")]
  public class User : BaseModel
  {
    [PrimaryKey("user_id", false)]
    public string UserId { get; set; } = string.Empty;

    [Column("u_id")]
    public string UId { get; set; } = string.Empty;

    [Column("email")]
    public string Email { get; set; } = string.Empty;

    [Column("first_name")]
    public string FirstName { get; set; } = string.Empty;

    [Column("last_name")]
    public string LastName { get; set; } = string.Empty;

    [Column("password")]
    public string? Password { get; set; } = string.Empty;

    [Column("profile_image_url")]
    public string? ProfileImageUrl { get; set; } = string.Empty;
  }

}