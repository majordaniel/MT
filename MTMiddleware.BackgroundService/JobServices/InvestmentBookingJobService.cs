using MTMiddleware.Data.ViewModels;
using UtilityLibrary.Enumerations;
using UtilityLibrary.Extensions;

namespace MTMiddlewareBackgroundService.Services
{
    public class InvestmentBookingJobService : IInvestmentBookingJobService
    {
        private readonly ILogger<InvestmentBookingJobService> _logger;

        private readonly IInvestmentBookingRequestService _investmentBookingRequestService;
        //private readonly InvestmentLiquidationRequestService _investmentLiquidationRequestService;

        public InvestmentBookingJobService(ILogger<InvestmentBookingJobService> logger, IInvestmentBookingRequestService investmentBookingRequestService)
        {
            _logger = logger;

            _investmentBookingRequestService = investmentBookingRequestService;
        }

        public async Task ExecuteAsync(IJobCancellationToken token)
        {
            var allPendingProcessing = await _investmentBookingRequestService.GetNotProcessedMaturedInvestmentsToRolloverAsync();
            if (allPendingProcessing == null || allPendingProcessing.Data == null)
            {
                _logger.LogInformation("No matured investment to rebook");

                return;
            }

            var list = allPendingProcessing.Data;

            int count = list.Count;

            if (count <= 0)
            {
                _logger.LogInformation("No matured investment to rebook");

                return;
            }

            _logger.LogInformation($"There are {count} matured investments waiting to be rebooked");

            foreach (var item in list)
            {
                if (item.RolloverOnMaturity)
                {
                    _logger.LogInformation($"Beginning to rebook matured investment with Id '{item.Id}'");

                    var bookingResult = await _investmentBookingRequestService.RebookAsync(item);

                    if (bookingResult == null)
                    {
                        _logger.LogError($"Error occured while rebooking investment with Id '{item.Id}'");
                    }
                    else if (bookingResult.Code == ResponseEnum.OperationCompletedSuccesfully.ResponseCode())
                    {
                        _logger.LogError($"Investment with Id '{item.Id}' was rebooked successfully");
                    }
                    else if (bookingResult.Data == null)
                    {
                        _logger.LogError($"Error occured while rebooking investment with Id '{item.Id}'");
                    }
                    else if (bookingResult.Code != ResponseEnum.OperationCompletedSuccesfully.ResponseCode())
                    {
                        _logger.LogError($"Investment with Id '{item.Id}' could not be rebooked: {ResponseEnum.OperationCompletedSuccesfully.Description()}");
                    }
                    else
                    {
                        _logger.LogError($"Investment with Id '{item.Id}' could not be rebooked");
                    }
                }


                //else
                //{
                //    _logger.LogInformation($"Beginning to liquidate matured investment with Id '{item.Id}' and no rollover instruction");

                //    DateTime currentDateTime = DateTime.Now;

                //    InvestmentLiquidationRequestSubmitViewModel model = new InvestmentLiquidationRequestSubmitViewModel()
                //    {
                //        InvestmentBookingRequestId = item.Id,
                //        LiquidationDate = currentDateTime,
                //        LiquidationAmount = item.Amount,
                //        Narration = "Auto liquidation of investment"
                //    };
                //    var liquidationResult = await _investmentLiquidationRequestService.SubmitAsync(model, ApplicationConstants.SYSTEM_USER, currentDateTime);

                //    if (liquidationResult == null)
                //    {
                //        _logger.LogError($"Error occured while liquidating investment with Id '{item.Id}'");
                //    }
                //    else if (liquidationResult.Code == ResponseEnum.OperationCompletedSuccesfully.ResponseCode())
                //    {
                //        _logger.LogError($"Investment with Id '{item.Id}' was liquidated successfully");
                //    }
                //    else if (liquidationResult.Data == null)
                //    {
                //        _logger.LogError($"Error occured while liquidating investment with Id '{item.Id}'");
                //    }
                //    else if (liquidationResult.Code != ResponseEnum.OperationCompletedSuccesfully.ResponseCode())
                //    {
                //        _logger.LogError($"Investment with Id '{item.Id}' could not be liquidated: {ResponseEnum.OperationCompletedSuccesfully.Description()}");
                //    }
                //    else
                //    {
                //        _logger.LogError($"Investment with Id '{item.Id}' could not be liquidated");
                //    }
                //}
            }
        }
    }
}
