namespace ApplicationLayer.DTOs.User;

public class UserInfoDto
{
    public int UserAccountId { get; set; }

    public string DisplayName { get; set; }

    public string FirstName { get; set; }

    public string LastName { get; set; }

    public bool ConfirmPhoneNumber { get; set; }

    public bool HasCompletedProfile => FirstName is not null && LastName is not null && DisplayName is not null;
}