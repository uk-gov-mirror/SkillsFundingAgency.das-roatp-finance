using SFA.DAS.RoatpFinance.Web.Validators.Validation;

namespace SFA.DAS.RoatpFinance.Web.ViewModels
{
    public class RoatpFinancialClarificationViewModel : RoatpFinancialApplicationViewModel
    {
        public string Comments { get; set; }
        public IFormFileCollection FilesToUpload { get; set; }
        public string InternalComments { get; set; }
        public string ClarificationFile { get; set; }
        public List<ValidationErrorDetail> ErrorMessages { get; set; }
    }
}
