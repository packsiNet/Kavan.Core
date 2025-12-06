namespace ApplicationLayer.DTOs.Profiles.Public;

public class PersonalPublicProfileDto
{
    public int UserAccountId { get; set; }
    public string UserName { get; set; }
    public string DisplayName { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string AvatarUrl { get; set; }
}

public class PublicProfileDto
{
    public PersonalPublicProfileDto Personal { get; set; }
    public ApplicationLayer.DTOs.Profiles.Organizations.OrganizationProfileDto Organization { get; set; }
}
