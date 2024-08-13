namespace MTMiddlewareBackgroundService.Interfaces
{
    public interface IRolloverJobService
    {
        Task ExecuteAsync(IJobCancellationToken token);
    }
}
