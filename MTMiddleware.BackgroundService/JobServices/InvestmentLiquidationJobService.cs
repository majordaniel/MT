using MTMiddleware.Data.ViewModels;
using UtilityLibrary.Enumerations;
using UtilityLibrary.Extensions;

namespace MTMiddlewareBackgroundService.Services
{
    public class InvestmentLiquidationJobService : IInvestmentLiquidationJobService
    {
        private readonly ILogger<InvestmentLiquidationJobService> _logger;

        private readonly IInvestmentBookingRequestService _investmentBookingRequestService;
        private readonly IInvestmentLiquidationRequestService _investmentLiquidationRequestService;

        public InvestmentLiquidationJobService(ILogger<InvestmentLiquidationJobService> logger, IInvestmentBookingRequestService investmentBookingRequestService, IInvestmentLiquidationRequestService investmentLiquidationRequestService)
        {
            _logger = logger;

            _investmentBookingRequestService = investmentBookingRequestService;
            _investmentLiquidationRequestService = investmentLiquidationRequestService;
        }

        public async Task ExecuteAsync(IJobCancellationToken token)
        {
            var allPendingProcessing = await _investmentBookingRequestService.GetNotCompletedMaturedInvestmentsToLiquidateAsync();
            if (allPendingProcessing == null || allPendingProcessing.Data == null)
            {
                _logger.LogInformation("No matured investment to liquidated");

                return;
            }

            var list = allPendingProcessing.Data;

            int count = list.Count;

            if (count <= 0)
            {
                _logger.LogInformation("No matured investment to liquidated");

                return;
            }

            _logger.LogInformation($"There are {count} matured investments waiting to be liquidated");

            foreach (var item in list)
            {
                _logger.LogInformation($"Beginning to liquidate matured investment with Id '{item.Id}' and no rollover instruction");

                DateTime currentDateTime = DateTime.Now;

                InvestmentLiquidationRequestSubmitViewModel model = new InvestmentLiquidationRequestSubmitViewModel()
                {
                    InvestmentBookingRequestId = item.Id,
                    LiquidationDate = currentDateTime,
                    LiquidationAmount = item.Amount,
                    Narration = $"Auto liquidation of investment with investment Id '{item.Id}'"
                };
                var liquidationResult = await _investmentLiquidationRequestService.SubmitAsync(model, ApplicationConstants.SYSTEM_USER, currentDateTime);

                if (liquidationResult == null)
                {
                    _logger.LogError($"Error occured while liquidating investment with Id '{item.Id}'");
                }
                else if (liquidationResult.Code == ResponseEnum.OperationCompletedSuccesfully.ResponseCode())
                {
                    _logger.LogError($"Investment with Id '{item.Id}' was liquidated successfully");
                }
                else if (liquidationResult.Data == null)
                {
                    _logger.LogError($"Error occured while liquidating investment with Id '{item.Id}'");
                }
                else if (liquidationResult.Code != ResponseEnum.OperationCompletedSuccesfully.ResponseCode())
                {
                    _logger.LogError($"Investment with Id '{item.Id}' could not be liquidated: {ResponseEnum.OperationCompletedSuccesfully.Description()}");
                }
                else
                {
                    _logger.LogError($"Investment with Id '{item.Id}' could not be liquidated");
                }
            }
        }
    }
}
