namespace SFA.DAS.RoatpFinance.Web.ApplyTypes.Apply
{
    public class RoatpContact
    {
        public Guid Id { get; set; }
        public string Email { get; set; }
        public string GivenNames { get; set; }
        public string FamilyName { get; set; }
        public Guid? SigninId { get; set; }
        public string SigninType { get; set; }
        public Guid? ApplyOrganisationId { get; set; }
    }
}
