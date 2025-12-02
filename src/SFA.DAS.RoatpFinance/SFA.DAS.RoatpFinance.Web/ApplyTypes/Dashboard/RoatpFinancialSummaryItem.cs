namespace SFA.DAS.RoatpFinance.Web.ApplyTypes.Dashboard
{
    public class RoatpFinancialSummaryItem : RoatpApplicationSummaryItem
    {
        public string DeclaredInApplication { get; set; }

        public DateTime GatewayOutcomeDate { get; set; }

        public string SelectedGrade { get; set; }
    }
}
