
using AutoMapper;
using MTMiddleware.Data.Entities;
using MTMiddleware.Data.ViewModels;

namespace MTMiddleware.Data;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        //CreateMap<BankInterestRateCreateViewModel, BankInterestRate>().ReverseMap();
        //CreateMap<BankInterestRateViewModel, BankInterestRate>().ReverseMap();

        //CreateMap<BankInterestRateViewModel, BankInterestRateCreateViewModel>().ReverseMap();
        //CreateMap<BankInterestRate, BankInterestWeekRateViewModel>().ReverseMap();
        
        //CreateMap<InvestmentBookingRequestSubmitViewModel, InvestmentBookingRequest>().ReverseMap();
        //CreateMap<InvestmentBookingRequestViewModel, InvestmentBookingRequest>().ReverseMap();
        //CreateMap<InvestmentBookingRequestStatusCompactViewModel, InvestmentBookingRequest>().ReverseMap()
        //    .AfterMap((source, destination) => {
        //        destination.InvestmentId = source.Id;
        //});

        //CreateMap<InvestmentLiquidationRequestSubmitViewModel, InvestmentLiquidationRequest>().ReverseMap();
        //CreateMap<InvestmentLiquidationRequestViewModel, InvestmentLiquidationRequest>().ReverseMap();
        //CreateMap<InvestmentLiquidationReportViewModel, InvestmentLiquidationRequest>().ReverseMap();

        //CreateMap<InvestmentLiquidationReportViewModel, InvestmentLiquidationRequest>().ReverseMap();
        //CreateMap<InvestmentLiquidationRequest, InvestmentLiquidationRequestSearchViewModel>().ReverseMap();

        //CreateMap<RetailCustomerAddViewModel, Customer>().ReverseMap();
        //CreateMap<RetailCustomerViewModel, Customer>().ReverseMap();
        
        //CreateMap<CorporateCustomerAddViewModel, Customer>().ReverseMap();
        //CreateMap<CorporateCustomerViewModel, Customer>().ReverseMap();

        //CreateMap<CustomerAccountSignitoryAddViewModel, CustomerAccountSignitory>().ReverseMap();
        //CreateMap<CompanyPromoterAddViewModel, CompanyPromoter>().ReverseMap();

        //CreateMap<CustomerAccountSignitoryViewModel, CustomerAccountSignitory>().ReverseMap();
        //CreateMap<CompanyPromoterViewModel, CompanyPromoter>().ReverseMap();

        //CreateMap<RetailCustomerSearchViewModel, Customer>().ReverseMap();
        //CreateMap<CorporateCustomerSearchViewModel, Customer>().ReverseMap();
        //CreateMap<CustomerReportViewModel, Customer>().ReverseMap();

        //CreateMap<DigitalChannelViewModel, DigitalChannel>().ReverseMap();
        //CreateMap<TitleViewModel, Title>().ReverseMap();

        //CreateMap<AnnualSalaryRangeViewModel, AnnualSalaryRange>().ReverseMap();
        //CreateMap<AnnualTurnoverViewModel, AnnualTurnover>().ReverseMap();
        //CreateMap<CountryViewModel, Country>().ReverseMap();
        //CreateMap<CountryStateViewModel, CountryState>().ReverseMap();
        //CreateMap<CountryStateLGAViewModel, CountryStateLGA>().ReverseMap();
        //CreateMap<EmploymentStatusViewModel, EmploymentStatus>().ReverseMap();
        //CreateMap<FamilyRelationshipViewModel, FamilyRelationship>().ReverseMap();
        //CreateMap<GenderViewModel, Gender>().ReverseMap();
        //CreateMap<MaritalStatusViewModel, MaritalStatus>().ReverseMap();
        //CreateMap<MeansOfIdentificationViewModel, MeansOfIdentification>().ReverseMap();
        //CreateMap<ResidencyStatusViewModel, ResidencyStatus>().ReverseMap();
        //CreateMap<SignatoryClassViewModel, SignatoryClass>().ReverseMap();
        //CreateMap<TenorViewModel, Tenor>().ReverseMap();
        //CreateMap<TitleViewModel, Title>().ReverseMap();

        //CreateMap<BookingRollOverInstructionSubmitViewModel, BookingRollOverInstruction>().ReverseMap();
        //CreateMap<BookingRollOverInstructionViewModel, BookingRollOverInstruction>().ReverseMap();

        CreateMap<RegisterUserViewModel, ApplicationUser>().ReverseMap();
        CreateMap<RegisterCustomerRequestViewModel, ApplicationUser>().ReverseMap();
        CreateMap<InviteUserViewModel, ApplicationUser>().ReverseMap();
        CreateMap<SignInViewModel, ApplicationUser>().ReverseMap();
        CreateMap<ApplicationUserViewModel, ApplicationUser>().ReverseMap();
   
        CreateMap<ApplicationRoleViewModel, ApplicationRole>().ReverseMap();

        //CreateMap<PrincipalBandCreateViewModel, PrincipalBand>().ReverseMap();
        //CreateMap<PrincipalBandUpdateViewModel, PrincipalBand>().ReverseMap();
        //CreateMap<PrincipalBandViewModel, PrincipalBand>().ReverseMap();

        //CreateMap<InvestmentBookingRequestSendNotificationViewModel, InvestmentBookingRequestViewModel>().ReverseMap();
        //CreateMap<InvestmentLiquidationRequestSendNotificationViewModel, InvestmentLiquidationRequestViewModel>().ReverseMap();
        //CreateMap<InvestmentBookingRequest, InvestmentBookingRequestStatusViewModel>().ReverseMap();
        //CreateMap<InvestmentBookingRequest, InvestmentBookingRequestSearchViewModel>().ReverseMap();

        //CreateMap<Holliday, HollidayCreateViewModel>().ReverseMap();
        //CreateMap<Holliday, HollidayUpdateViewModel>().ReverseMap();
        //CreateMap<Holliday, HollidayViewModel>().ReverseMap();
    }
}