namespace SFA.DAS.RoatpFinance.Web.ApplyTypes.Apply
{
    public static class FinancialApplicationSelectedGrade
    {
        public const string Outstanding = "Outstanding";
        public const string Good = "Good";
        public const string Satisfactory = "Satisfactory";
        public const string Clarification = "Clarification";
        public const string Inadequate = "Inadequate";
        public const string Exempt = "Exempt";

        public static IReadOnlyList<string> PassingGrades => new List<string>
        {
            Outstanding,
            Good,
            Satisfactory,
            Exempt
        };
    }
}
