using System.Globalization;
using SFA.DAS.RoatpFinance.Web.ApplyTypes.Apply;
using SFA.DAS.RoatpFinance.Web.Validators.Validation;
using SFA.DAS.RoatpFinance.Web.ViewModels;

namespace SFA.DAS.RoatpFinance.Web.Validators
{
    public class RoatpFinancialApplicationViewModelValidator : IRoatpFinancialApplicationViewModelValidator
    {
        public ValidationResponse Validate(RoatpFinancialApplicationViewModel vm)
        {
            var validationResponse = new ValidationResponse
            {
                Errors = new List<ValidationErrorDetail>()
            };

            if (vm?.FinancialReviewDetails is null || string.IsNullOrWhiteSpace(vm.FinancialReviewDetails.SelectedGrade))
            {
                validationResponse.Errors.Add(new ValidationErrorDetail("FinancialReviewDetails.SelectedGrade",
                    "Select the outcome of this financial health assessment"));
            }
            else if (vm.FinancialReviewDetails.SelectedGrade == FinancialApplicationSelectedGrade.Inadequate)
            {
                if (string.IsNullOrWhiteSpace(vm.InadequateComments))
                {
                    validationResponse.Errors.Add(new ValidationErrorDetail("InadequateComments", "Enter internal comments"));
                }
                else if (HasExceededWordCount(vm.InadequateComments))
                {
                    validationResponse.Errors.Add(new ValidationErrorDetail("InadequateComments", "Your internal comments must be 500 words or less"));
                }

                if (string.IsNullOrWhiteSpace(vm.InadequateExternalComments))
                {
                    validationResponse.Errors.Add(new ValidationErrorDetail("InadequateExternalComments", "Enter external comments"));
                }
                else if (HasExceededWordCount(vm.InadequateExternalComments))
                {
                    validationResponse.Errors.Add(new ValidationErrorDetail("InadequateExternalComments", "Your external comments must be 500 words or less"));
                }
            }
            else if (vm.FinancialReviewDetails.SelectedGrade == FinancialApplicationSelectedGrade.Clarification)
            {
                if (string.IsNullOrWhiteSpace(vm.ClarificationComments))
                {
                    validationResponse.Errors.Add(new ValidationErrorDetail("ClarificationComments", "Enter internal comments"));
                }
                else if (HasExceededWordCount(vm.ClarificationComments))
                {
                    validationResponse.Errors.Add(new ValidationErrorDetail("ClarificationComments", "Your comments must be 500 words or less"));
                }

            }
            else if (vm.FinancialReviewDetails.SelectedGrade == FinancialApplicationSelectedGrade.Outstanding
                     || vm.FinancialReviewDetails.SelectedGrade == FinancialApplicationSelectedGrade.Good
                     || vm.FinancialReviewDetails.SelectedGrade == FinancialApplicationSelectedGrade.Satisfactory)
            {
                switch (vm.FinancialReviewDetails.SelectedGrade)
                {
                    case FinancialApplicationSelectedGrade.Outstanding:
                        ProcessDate(vm.OutstandingFinancialDueDate, "OutstandingFinancialDueDate", validationResponse);
                        break;
                    case FinancialApplicationSelectedGrade.Good:
                        ProcessDate(vm.GoodFinancialDueDate, "GoodFinancialDueDate", validationResponse);
                        break;
                    case FinancialApplicationSelectedGrade.Satisfactory:
                        ProcessDate(vm.SatisfactoryFinancialDueDate, "SatisfactoryFinancialDueDate", validationResponse);
                        break;
                }
            }

            return validationResponse;
        }

        private static void ProcessDate(FinancialDueDate dueDate, string propertyName, ValidationResponse validationResponse)
        {
            if (string.IsNullOrWhiteSpace(dueDate.Day) || string.IsNullOrWhiteSpace(dueDate.Month) || string.IsNullOrWhiteSpace(dueDate.Year))
            {
                validationResponse.Errors.Add(new ValidationErrorDetail(propertyName, "Enter the financial due date"));
                return;
            }

            if (!int.TryParse(dueDate.Day, out int _) || !int.TryParse(dueDate.Month, out int _) || !int.TryParse(dueDate.Year, out int _))
            {
                validationResponse.Errors.Add(new ValidationErrorDetail(propertyName, "Enter a correct financial due date"));
                return;
            }

            var day = dueDate.Day;
            var month = dueDate.Month;
            var year = dueDate.Year;

            var isValidDate = DateTime.TryParseExact($"{day}/{month}/{year}", "d/M/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsedDate);

            if (!isValidDate)
            {
                validationResponse.Errors.Add(new ValidationErrorDetail(propertyName, "Enter a correct financial due date"));
                return;
            }

            if (parsedDate < DateTime.Today)
            {
                validationResponse.Errors.Add(new ValidationErrorDetail(propertyName, "Financial due date must be a future date"));
            }
        }

        private static bool HasExceededWordCount(string input, int maxWordcount = 500)
        {
            bool hasExceeded = false;

            var text = input?.Trim();

            if (!string.IsNullOrEmpty(text))
            {
                var wordCount = text.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries).Length;

                hasExceeded = (wordCount > maxWordcount);
            }

            return hasExceeded;
        }
    }

    public interface IRoatpFinancialApplicationViewModelValidator
    {
        public ValidationResponse Validate(RoatpFinancialApplicationViewModel vm);
    }
}
