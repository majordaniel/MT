namespace MTMiddlewareBackgroundService.Interfaces
{
    public interface IInvestmentBookingJobService
    {
        Task ExecuteAsync(IJobCancellationToken token);
    }
}
