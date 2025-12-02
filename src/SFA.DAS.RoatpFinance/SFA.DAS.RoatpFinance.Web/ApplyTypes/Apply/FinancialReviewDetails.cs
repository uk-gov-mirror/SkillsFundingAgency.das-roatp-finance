namespace SFA.DAS.RoatpFinance.Web.ApplyTypes.Apply
{
    public class FinancialReviewDetails
    {
        public Guid ApplicationId { get; set; }
        public string Status { get; set; }
        public string SelectedGrade { get; set; }
        public DateTime? FinancialDueDate { get; set; }
        public string GradedBy { get; set; }
        public DateTime? GradedOn { get; set; }
        public string Comments { get; set; }
        public string ExternalComments { get; set; }
        public List<FinancialEvidence> FinancialEvidences { get; set; }
        public List<ClarificationFile> ClarificationFiles { get; set; }
        public DateTime? ClarificationRequestedOn { get; set; }
        public string ClarificationRequestedBy { get; set; }
        public string ClarificationResponse { get; set; }

        public string Outcome => FinancialApplicationSelectedGrade.PassingGrades.Contains(SelectedGrade)
                ? FinancialApplicationOutcome.Passed
                : FinancialApplicationOutcome.Failed;

        public string OutcomeCssClass => Outcome == FinancialApplicationOutcome.Passed ? "govuk-tag govuk-tag--pass" : "govuk-tag govuk-tag--fail";
    }
    public class FinancialEvidence
    {
        public string Filename { get; set; }
    }

    public class ClarificationFile
    {
        public string Filename { get; set; }
    }
}
