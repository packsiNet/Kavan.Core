using ApplicationLayer.Dto.BaseDtos;
using ApplicationLayer.Dto.IdeaComments;

namespace ApplicationLayer.Interfaces.IdeaComments;

public interface IIdeaCommentService
{
    Task<Result<IdeaCommentDto>> AddCommentAsync(CreateIdeaCommentDto dto);
    Task<Result<List<IdeaCommentDto>>> GetCommentsAsync(int ideaId);
    Task<Result> DeleteCommentAsync(int id);
}
