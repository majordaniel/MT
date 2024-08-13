namespace MTMiddlewareBackgroundService.Interfaces
{
    public interface IInvestmentLiquidationJobService
    {
        Task ExecuteAsync(IJobCancellationToken token);
    }
}
