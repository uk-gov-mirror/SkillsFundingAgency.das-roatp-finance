namespace SFA.DAS.RoatpFinance.Web.ApplyTypes.Apply
{
    public class RoatpApplyData
    {
        public List<RoatpApplySequence> Sequences { get; set; }
        public RoatpApplyDetails ApplyDetails { get; set; }
    }

    public class RoatpApplyDetails
    {
        public string ReferenceNumber { get; set; }
        public string UKPRN { get; set; }
        public string OrganisationName { get; set; }
        public string TradingName { get; set; }
        public int ProviderRoute { get; set; }
        public string ProviderRouteName { get; set; }
        public DateTime? ApplicationSubmittedOn { get; set; }
        public Guid? ApplicationSubmittedBy { get; set; }
        public DateTime? ApplicationWithdrawnOn { get; set; }
        public string ApplicationWithdrawnBy { get; set; }
        public DateTime? ApplicationRemovedOn { get; set; }
        public string ApplicationRemovedBy { get; set; }
    }
}
