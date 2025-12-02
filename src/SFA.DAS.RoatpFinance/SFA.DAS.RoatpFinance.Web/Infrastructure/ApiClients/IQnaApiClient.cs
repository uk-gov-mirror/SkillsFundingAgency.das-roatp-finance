using SFA.DAS.QnA.Api.Types;
using RestEase;

namespace SFA.DAS.RoatpFinance.Web.Infrastructure.ApiClients
{
    public interface IQnaApiClient
    {
        [Get("applications/{applicationId}/applicationData/{questionTag}")]
        Task<string> GetQuestionTag([Path] Guid applicationId, [Path] string questionTag);

        [Get("applications/{applicationId}/sequences")]
        Task<IEnumerable<Sequence>> GetSequences([Path] Guid applicationId);

        [Get("applications/{applicationId}/sequences/{sequenceId}")]
        Task<Sequence> GetSequence([Path] Guid applicationId, [Path] Guid sequenceId);

        [Get("applications/{applicationId}/sequences/{sequenceNo}")]
        Task<Sequence> GetSequenceBySequenceNo([Path] Guid applicationId, [Path] int sequenceNo);

        [Get("applications/{applicationId}/sequences/{sequenceId}/sections")]
        Task<IEnumerable<Section>> GetSections([Path] Guid applicationId, [Path] Guid sequenceId);

        [Get("applications/{applicationId}/sections/{sectionId}")]
        Task<Section> GetSection([Path] Guid applicationId, [Path] Guid sectionId);

        [Get("applications/{applicationId}/sequences/{sequenceNo}/sections/{sectionNo}")]
        Task<Section> GetSectionBySectionNo([Path] Guid applicationId, [Path] int sequenceNo, [Path] int sectionNo);

        [Get("/applications/{applicationId}/sections/{sectionId}/pages/{pageId}/questions/{questionId}/download/{fileName}")]
        Task<HttpResponseMessage> DownloadFile([Path] Guid applicationId, [Path] Guid sectionId, [Path] string pageId, [Path] string questionId, [Path] string fileName);
    }
}
