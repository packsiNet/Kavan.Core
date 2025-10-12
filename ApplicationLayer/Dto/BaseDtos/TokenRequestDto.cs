namespace ApplicationLayer.Dto.BaseDtos;

public class TokenRequestDto
{
    public string AccessTokens { get; set; }

    public string RefreshToken { get; set; }
}