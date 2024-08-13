
using MTMiddleware.Core.Services.Interfaces;
using UtilityLibrary.Enumerations;
using UtilityLibrary.Extensions;

namespace MTMiddlewareBackgroundService.Services
{
    public class RolloverJobService : IRolloverJobService
    {
        private readonly ILogger<RolloverJobService> _logger;

        private readonly IBookingRollOverInstructionService _bookingRollOverInstructionService;

        public RolloverJobService(ILogger<RolloverJobService> logger, IBookingRollOverInstructionService bookingRollOverInstructionService)
        {
            _logger = logger;

            _bookingRollOverInstructionService = bookingRollOverInstructionService;
        }

        public async Task ExecuteAsync(IJobCancellationToken token)
        {
            var allPendingProcessing = await _bookingRollOverInstructionService.GetAllPendingRolloverInstructionsAsync();
            if (allPendingProcessing == null || allPendingProcessing.Data == null)
            {
                _logger.LogInformation("No pending Rollover instructions to process");

                return;
            }

            var list = allPendingProcessing.Data;

            int count = list.Count;

            if (count <= 0)
            {
                _logger.LogInformation($"No pending Rollover instructions to process");

                return;
            }

            _logger.LogInformation($"There are {count} rollover instruction(s) pending processing");

            foreach (var item in list)
            {
                _logger.LogInformation($"Beginning to rollover instruction with Id {item.Id}");
                var rolloverResult = await _bookingRollOverInstructionService.RolloverAsync(item.Id);

                if (rolloverResult != null && rolloverResult.Data != null && rolloverResult.Code == ResponseEnum.OperationCompletedSuccesfully.ResponseCode())
                {
                    _logger.LogInformation($"Rollover instruction with Id {item.Id} rolled over successfully");
                }
                else
                {
                    _logger.LogInformation($"Rollover instruction with Id {item.Id} failed during rollover. See same log (above) for details of failure");
                }
            }
        }
    }
}
