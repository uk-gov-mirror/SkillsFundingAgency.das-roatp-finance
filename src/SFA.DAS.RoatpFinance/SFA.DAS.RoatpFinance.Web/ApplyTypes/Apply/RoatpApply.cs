namespace SFA.DAS.RoatpFinance.Web.ApplyTypes.Apply
{
    public class RoatpApply
    {
        public Guid ApplicationId { get; set; }
        public Guid OrganisationId { get; set; }

        public string ApplicationStatus { get; set; }

        public RoatpApplyData ApplyData { get; set; }

        public string Comments { get; set; }
        public string ExternalComments { get; set; }

        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? DeletedAt { get; set; }
        public string DeletedBy { get; set; }
    }
}
