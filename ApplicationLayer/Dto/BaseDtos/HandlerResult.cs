#region Usings

using ApplicationLayer.Common;
using ApplicationLayer.Common.Enums;
using ApplicationLayer.Features.Validations;
using Microsoft.Extensions.Logging;

#endregion

namespace ApplicationLayer;

public class HandlerResult
{
    public ValidationResult ValidationResult { get; set; }

    public RequestStatus RequestStatus { get; set; }

    public string Message { get; set; }

    public object ObjectResult { get; set; }

    public NotificationType NotificationType => GetNotificationType(RequestStatus);

    private static NotificationType GetNotificationType(RequestStatus requestStatus)
    {
        return (int)requestStatus switch
        {
            var SuccessfulRow when SuccessfulRow.Equals(RequestStatus.Successful) => NotificationType.Success,
            var failedRow when failedRow.Equals(RequestStatus.Failed) => NotificationType.Error,
            _ => NotificationType.Warning,
        };
    }

    public HandlerResult Failed(ILogger logger, Exception exception, string title)
    {
        logger.LogError(exception, title);
        return new HandlerResult()
        {
            RequestStatus = RequestStatus.Failed,
            Message = CommonMessages.Failed
        };
    }
}