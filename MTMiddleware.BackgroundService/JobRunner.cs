namespace MTMiddlewareBackgroundService
{
    public class JobRunner
    {
        [DisableConcurrentExecution(5)]
        [AutomaticRetry(Attempts = 0, LogEvents = true, OnAttemptsExceeded = AttemptsExceededAction.Delete)]
        public static void Schedule()
        {
            //RecurringJob.RemoveIfExists(nameof(RolloverJobService));
            //RecurringJob.AddOrUpdate<RolloverJobService>(nameof(RolloverJobService), job => job.ExecuteAsync(JobCancellationToken.Null), Cron.Minutely); //"*/15 * * * * *"   Cron.Minutely

            //RecurringJob.RemoveIfExists(nameof(InvestmentBookingJobService));
            //RecurringJob.AddOrUpdate<InvestmentBookingJobService>(nameof(InvestmentBookingJobService), job => job.ExecuteAsync(JobCancellationToken.Null), Cron.Minutely); //"*/15 * * * * *"   Cron.Minutely

            //RecurringJob.RemoveIfExists(nameof(InvestmentLiquidationJobService));
            //RecurringJob.AddOrUpdate<InvestmentLiquidationJobService>(nameof(InvestmentLiquidationJobService), job => job.ExecuteAsync(JobCancellationToken.Null), Cron.Minutely); //"*/15 * * * * *"   Cron.Minutely
        
        
        }
    }
}
