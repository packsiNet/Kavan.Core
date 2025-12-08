namespace ApplicationLayer.DTOs.ChannelPosts;

public class PostDto
{
    public int Id { get; set; }
    public int ChannelId { get; set; }
    public string Type { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string Image { get; set; }
    public DateTime CreatedAt { get; set; }
    public string TimeAgo { get; set; } // Added
    public SignalDetailDto SignalDetail { get; set; }
    public NewsDetailDto NewsDetail { get; set; }
    public int Likes { get; set; }
    public int Dislikes { get; set; }
    public int CommentsCount { get; set; } // Added
    public List<CommentDto> Comments { get; set; } = new(); // Added
}

public class SignalDetailDto
{
    public string Symbol { get; set; }
    public string Timeframe { get; set; }
    public string TradeType { get; set; }
    public List<decimal> EntryPoints { get; set; } = [];
    public List<decimal> Tps { get; set; } = [];
    public decimal StopLoss { get; set; }
}

public class NewsDetailDto
{
    public string Url { get; set; }
}

public class CommentDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string UserName { get; set; } // Needs User join
    public string Comment { get; set; }
    public string TimeAgo { get; set; }
    public DateTime CreatedAt { get; set; }
}
