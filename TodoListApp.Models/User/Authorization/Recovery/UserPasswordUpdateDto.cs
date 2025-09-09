using System.ComponentModel.DataAnnotations;

namespace TodoListApp.Models.User.Authorization.Recovery;

public class UserPasswordUpdateDto
{
    public long UserId { get; set; }

    [Required, MinLength(2, ErrorMessage = "Minmum length is 2")]
    public string Login { get; set; } = string.Empty;

    [DataType(DataType.Password), Required, MinLength(4, ErrorMessage = "Minmum length is 4")]
    public string OldPassword { get; set; } = string.Empty;

    [DataType(DataType.Password), Required, MinLength(4, ErrorMessage = "Minmum length is 4")]
    public string NewPassword { get; set; } = string.Empty;
}
