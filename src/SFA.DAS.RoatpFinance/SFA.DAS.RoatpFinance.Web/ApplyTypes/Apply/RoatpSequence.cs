namespace SFA.DAS.RoatpFinance.Web.ApplyTypes.Apply
{
    public class RoatpSequence
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public bool Sequential { get; set; }
        public List<string> ExcludeSections { get; set; }
        public List<string> Roles { get; set; }
    }
}
