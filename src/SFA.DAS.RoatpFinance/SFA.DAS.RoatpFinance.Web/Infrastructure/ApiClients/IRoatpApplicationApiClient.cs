using Refit;
using SFA.DAS.RoatpFinance.Web.ApplyTypes.Apply;
using SFA.DAS.RoatpFinance.Web.ApplyTypes.Dashboard;
using SFA.DAS.RoatpFinance.Web.ApplyTypes.Export;
using SFA.DAS.RoatpFinance.Web.Infrastructure.Models;

namespace SFA.DAS.RoatpFinance.Web.Infrastructure.ApiClients
{
    public interface IRoatpApplicationApiClient
    {
        [Get("/application/{applicationId}")]
        Task<ApiResponse<RoatpApply>> GetApplication(Guid applicationId);

        [Get("/application/{applicationId}/contact")]
        Task<ApiResponse<RoatpContact>> GetContactForApplication(Guid applicationId);

        [Get("/application/{applicationId}/financialreviewdetails")]
        Task<ApiResponse<FinancialReviewDetails>> GetFinancialReviewDetails(Guid applicationId);

        [Get("/roatp-sequences")]
        Task<List<RoatpSequence>> GetRoatpSequences();

        [Get("/financial/closedapplications?searchTerm={searchTerm}&sortColumn={sortColumn}&sortOrder={sortOrder}")]
        Task<List<RoatpFinancialSummaryItem>> GetClosedFinancialApplications(string searchTerm, string sortColumn, string sortOrder);

        [Get("/financial/clarificationapplications?searchTerm={searchTerm}&sortColumn={sortColumn}&sortOrder={sortOrder}")]
        Task<List<RoatpFinancialSummaryItem>> GetClarificationFinancialApplications(string searchTerm, string sortColumn, string sortOrder);

        [Get("/financial/openapplications?searchTerm={searchTerm}&sortColumn={sortColumn}&sortOrder={sortOrder}")]
        Task<List<RoatpFinancialSummaryItem>> GetOpenFinancialApplications(string searchTerm, string sortColumn, string sortOrder);

        [Get("/financial/openapplicationsfordownload")]
        Task<List<RoatpFinancialSummaryDownloadItem>> GetOpenFinancialApplicationsForDownload();

        [Get("/financial/statuscounts?searchTerm={searchTerm}")]
        Task<RoatpFinancialApplicationsStatusCounts> GetFinancialApplicationsStatusCounts(string searchTerm);

        [Post("/financial/{applicationId}/startreview")]
        Task StartFinancialReview(Guid applicationId, [Body] StartFinancialReviewCommandModel model);

        [Post("/financial/{applicationId}/grade")]
        Task ReturnFinancialReview(Guid applicationId, [Body] FinancialReviewDetails financialReviewDetails);

        [Post("/clarification/applications/{applicationId}/upload")]
        Task<ApiResponse<string>> UploadClarificationFile(Guid applicationId, [Body] MultipartFormDataContent content);

        [Post("/clarification/applications/{applicationId}/remove")]
        Task<ApiResponse<string>> RemoveClarificationFile(Guid applicationId, [Body] RemoveClarificationFileCommandModel model);

        [Get("/clarification/applications/{applicationId}/download/{filename}")]
        Task<HttpResponseMessage> DownloadClarificationFile(Guid applicationId, string filename);
    }
}
