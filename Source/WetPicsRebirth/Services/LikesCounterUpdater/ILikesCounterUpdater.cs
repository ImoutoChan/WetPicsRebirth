namespace WetPicsRebirth.Services.LikesCounterUpdater;

public interface ILikesCounterUpdater
{
    Task Update(MessageToUpdateCounter message);
}

public record MessageToUpdateCounter(long ChatId, int MessageId);
