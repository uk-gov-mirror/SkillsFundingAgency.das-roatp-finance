using SFA.DAS.RoatpFinance.Web.ApplyTypes.Apply;
using SFA.DAS.RoatpFinance.Web.ApplyTypes.Dashboard;
using SFA.DAS.RoatpFinance.Web.ApplyTypes.Export;
using RestEase;
using SFA.DAS.RoatpFinance.Web.Infrastructure.Models;

namespace SFA.DAS.RoatpFinance.Web.Infrastructure.ApiClients
{
    public interface IRoatpApplicationApiClient
    {
        [Get("/application/{applicationId}")]
        Task<RoatpApply> GetApplication([Path] Guid applicationId);

        [Get("/application/{applicationId}/contact")]
        Task<RoatpContact> GetContactForApplication([Path] Guid applicationId);

        [Get("/application/{applicationId}/financialreviewdetails")]
        Task<FinancialReviewDetails> GetFinancialReviewDetails([Path] Guid applicationId);

        [Get("/roatp-sequences")]
        Task<List<RoatpSequence>> GetRoatpSequences();

        [Get("/financial/closedapplications?searchTerm={searchTerm}&sortColumn={sortColumn}&sortOrder={sortOrder}")]
        Task<List<RoatpFinancialSummaryItem>> GetClosedFinancialApplications([Path] string searchTerm, [Path] string sortColumn, [Path] string sortOrder);

        [Get("/financial/clarificationapplications?searchTerm={searchTerm}&sortColumn={sortColumn}&sortOrder={sortOrder}")]
        Task<List<RoatpFinancialSummaryItem>> GetClarificationFinancialApplications([Path] string searchTerm, [Path] string sortColumn, [Path] string sortOrder);

        [Get("/financial/openapplications?searchTerm={searchTerm}&sortColumn={sortColumn}&sortOrder={sortOrder}")]
        Task<List<RoatpFinancialSummaryItem>> GetOpenFinancialApplications([Path] string searchTerm, [Path] string sortColumn, [Path] string sortOrder);

        [Get("/financial/openapplicationsfordownload")]
        Task<List<RoatpFinancialSummaryDownloadItem>> GetOpenFinancialApplicationsForDownload();

        [Get("/financial/statuscounts?searchTerm={searchTerm}")]
        Task<RoatpFinancialApplicationsStatusCounts> GetFinancialApplicationsStatusCounts([Path] string searchTerm);

        [Post("/financial/{applicationId}/startreview")]
        Task StartFinancialReview([Path] Guid applicationId, [Body] StartFinancialReviewCommandModel model);

        [Post("/financial/{applicationId}/grade")]
        Task ReturnFinancialReview([Path] Guid applicationId, [Body] FinancialReviewDetails financialReviewDetails);

        [Post("/clarification/applications/{applicationId}/upload")]
        [AllowAnyStatusCode]
        Task<Response<string>> UploadClarificationFile([Path]Guid applicationId, [Body] MultipartFormDataContent content);

        [Post("/clarification/applications/{applicationId}/remove")]
        Task<Response<string>> RemoveClarificationFile([Path] Guid applicationId, [Body] RemoveClarificationFileCommandModel model);

        [Get("/clarification/applications/{applicationId}/download/{filename}")]
        Task<HttpResponseMessage> DownloadClarificationFile([Path] Guid applicationId, [Path] string filename);
    }
}
