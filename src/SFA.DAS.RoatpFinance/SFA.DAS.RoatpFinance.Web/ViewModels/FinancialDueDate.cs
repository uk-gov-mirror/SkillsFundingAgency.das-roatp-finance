namespace SFA.DAS.RoatpFinance.Web.ViewModels
{
    public class FinancialDueDate
    {
        public string Day { get; set; }
        public string Month { get; set; }
        public string Year { get; set; }

        public DateTime ToDateTime()
        {
            return new DateTime(int.Parse(Year), int.Parse(Month), int.Parse(Day));
        }
    }
}
