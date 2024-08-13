CREATE OR ALTER PROCEDURE [dbo].[spGetInvestmentStatusReport]
	@StartDate Date,
	@EndDate Date
AS
BEGIN

	-- SET NOCOUNT ON added to prevent extra result sets from interfering with SELECT statements.
	SET NOCOUNT ON;
	
	select a.Id, 
			'MTMiddleware' as InvestmentType,
			a.CustomerId,
			a.DigitalChannelId,
			a.EntryDate,
			a.InvestmentDate,
			a.IsLiquidated,
			a.IsPrematureLiquidation,
			b.AccountId,
			b.CashAccount,
			case when b.Type = 1 then b.FirstName + ' ' + ISNULL(b.OtherName, '') + b.Surname when b.Type = 2 then b.CompanyName else 'Unknown' end as CustomerName,
			a.Amount as InvestmentAmount,
			a.Tenor as Tenor,
			InvestmentDate as EffectiveDate,
			MaturityDate,
			InterestRate as InterestRate_AM,
			CAST(((a.Amount*a.Tenor*(a.BankBranchRate/100))/365) + ((a.Amount*a.Tenor*(a.CustomerRate/100))/365) AS decimal(18, 2)) as Accrued_Interest,
			BankBranchRate,
			CAST((a.Amount*a.Tenor*(a.BankBranchRate/100))/365 AS decimal(18, 2)) as Branch_AccruedInterest,
			CustomerRate,
			CAST((a.Amount*a.Tenor*(a.CustomerRate/100))/365 AS decimal(18, 2)) as Customer_AccruedInterest,
			(a.Amount) + CAST((a.Amount*a.Tenor*(a.CustomerRate/100))/365 AS decimal(18, 2)) as CustomerTotalPaidOutAmount
	from [dbo].[InvestmentBookingRequests] a
		inner join [dbo].[Customers] b
		on a.CustomerId = b.Id
		where a.InvestmentDate >= @StartDate and a.InvestmentDate <= @EndDate and a.IsActive = 1 and a.Archived = 0
END


--USE [fq.MTMiddleware.dev]
--GO
--/****** Object:  StoredProcedure [dbo].[spGetInvestmentStatusReport]    Script Date: 12/14/2022 12:34:57 PM ******/
--SET ANSI_NULLS ON
--GO
--SET QUOTED_IDENTIFIER ON
--GO
--ALTER   PROCEDURE [dbo].[spGetInvestmentStatusReport]
--AS
--BEGIN
--	-- SET NOCOUNT ON added to prevent extra result sets from interfering with SELECT statements.
--	SET NOCOUNT ON;
	
--	select a.Id, 
--			'MTMiddleware' as InvestmentType,
--			b.AccountId,
--			b.CashAccount,
--			b.Id as CustomerId,
--			case when b.Type = 1 then b.FirstName + ' ' + ISNULL(b.OtherName, '') + b.Surname when b.Type = 2 then b.CompanyName else 'Unknown' end as CustomerName,
--			a.EntryDate,
--			a.DigitalChannelId,
--			a.InvestmentDate,
--			a.IsLiquidated,
--			a.IsPrematureLiquidation,
--			a.Amount as InvestmentAmount,
--			a.Tenor as Tenor,
--			InvestmentDate as EffectiveDate,
--			MaturityDate,
--			InterestRate as InterestRate_AM,
--			CAST(((a.Amount*a.Tenor*(a.BankBranchRate/100))/365) + ((a.Amount*a.Tenor*(a.CustomerRate/100))/365) AS decimal(18, 2)) as Accrued_Interest,
--			BankBranchRate,
--			CAST((a.Amount*a.Tenor*(a.BankBranchRate/100))/365 AS decimal(18, 2)) as Branch_AccruedInterest,
--			CustomerRate,
--			CAST((a.Amount*a.Tenor*(a.CustomerRate/100))/365 AS decimal(18, 2)) as Customer_AccruedInterest,
--			(a.Amount) + CAST((a.Amount*a.Tenor*(a.CustomerRate/100))/365 AS decimal(18, 2)) as CustomerTotalPaidOutAmount
--	from [dbo].[InvestmentBookingRequests] a
--		inner join [dbo].[Customers] b
--		on a.CustomerId = b.Id
--		where a.IsActive = 1 and a.Archived = 0
--END