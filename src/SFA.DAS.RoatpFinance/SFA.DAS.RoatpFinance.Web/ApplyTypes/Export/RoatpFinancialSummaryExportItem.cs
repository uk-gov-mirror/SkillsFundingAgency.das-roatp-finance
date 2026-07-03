namespace SFA.DAS.RoatpFinance.Web.ApplyTypes.Export
{
    public class RoatpFinancialSummaryExportItem
    {
        public Guid ApplicationId { get; set; }
        public string ApplicationReference { get; set; }
        public string Route { get; set; }
        public DateTime SubmissionDate { get; set; }
        public DateTime GatewayCompletionDate { get; set; }
        public string ProviderName { get; set; }
        public string Ukprn { get; set; }
        public string CompanyNo { get; set; }
        public string CharityNo { get; set; }
        public long? TurnOver { get; set; }
        public long? Depreciation { get; set; }
        public long? ProfitLoss { get; set; }
        public long? Dividends { get; set; }
        public long? IntangibleAssets { get; set; }
        public long? Assets { get; set; }
        public long? Liabilities { get; set; }
        public long? ShareholderFunds { get; set; }
        public long? Borrowings { get; set; }
        public DateTime? AccountingReferenceDate { get; set; }
        public byte? AccountingPeriod { get; set; }
        public long? AverageNumberofFTEEmployees { get; set; }

        public static implicit operator RoatpFinancialSummaryExportItem(RoatpFinancialSummaryDownloadItem source)
        {
            if (source == null)
            {
                return null;
            }

            return new RoatpFinancialSummaryExportItem
            {
                ApplicationId = source.ApplicationId,
                ApplicationReference = source.ApplicationReferenceNumber,
                Route = source.ApplicationRoute,
                ProviderName = source.OrganisationName,
                Ukprn = source.Ukprn,
                SubmissionDate = source.SubmittedDate ?? default,
                GatewayCompletionDate = source.GatewayOutcomeDate,
                CharityNo = source.CharityNumber,
                CompanyNo = source.CompanyNumber,
                TurnOver = source.FinancialData?.TurnOver,
                Depreciation = source.FinancialData?.Depreciation,
                ProfitLoss = source.FinancialData?.ProfitLoss,
                Dividends = source.FinancialData?.Dividends,
                IntangibleAssets = source.FinancialData?.IntangibleAssets,
                Assets = source.FinancialData?.Assets,
                Liabilities = source.FinancialData?.Liabilities,
                ShareholderFunds = source.FinancialData?.ShareholderFunds,
                Borrowings = source.FinancialData?.Borrowings,
                AccountingReferenceDate = source.FinancialData?.AccountingReferenceDate,
                AccountingPeriod = source.FinancialData?.AccountingPeriod,
                AverageNumberofFTEEmployees = source.FinancialData?.AverageNumberofFTEEmployees
            };
        }
    }
}
