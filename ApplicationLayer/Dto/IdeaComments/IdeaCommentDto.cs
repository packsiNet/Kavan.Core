namespace ApplicationLayer.Dto.IdeaComments;

public class IdeaCommentDto
{
    public int Id { get; set; }
    public int IdeaId { get; set; }
    public int UserId { get; set; }
    public string UserAvatar { get; set; }
    public string UserName { get; set; }
    public string Comment { get; set; }
    public DateTime CreatedAt { get; set; }
}
