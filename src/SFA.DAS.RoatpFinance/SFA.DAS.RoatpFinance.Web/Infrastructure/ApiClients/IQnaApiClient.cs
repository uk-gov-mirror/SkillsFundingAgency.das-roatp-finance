using Refit;
using SFA.DAS.QnA.Api.Types;

namespace SFA.DAS.RoatpFinance.Web.Infrastructure.ApiClients
{
    public interface IQnaApiClient
    {
        [Get("/applications/{applicationId}/applicationData/{questionTag}")]
        Task<string> GetQuestionTag(Guid applicationId, string questionTag);

        [Get("/applications/{applicationId}/sequences")]
        Task<IEnumerable<Sequence>> GetSequences(Guid applicationId);

        [Get("/applications/{applicationId}/sequences/{sequenceId}")]
        Task<Sequence> GetSequence(Guid applicationId, Guid sequenceId);

        [Get("/applications/{applicationId}/sequences/{sequenceNo}")]
        Task<Sequence> GetSequenceBySequenceNo(Guid applicationId, int sequenceNo);

        [Get("/applications/{applicationId}/sequences/{sequenceId}/sections")]
        Task<IEnumerable<Section>> GetSections(Guid applicationId, Guid sequenceId);

        [Get("/applications/{applicationId}/sections/{sectionId}")]
        Task<Section> GetSection(Guid applicationId, Guid sectionId);

        [Get("/applications/{applicationId}/sequences/{sequenceNo}/sections/{sectionNo}")]
        Task<Section> GetSectionBySectionNo(Guid applicationId, int sequenceNo, int sectionNo);

        [Get("/applications/{applicationId}/sections/{sectionId}/pages/{pageId}/questions/{questionId}/download/{fileName}")]
        Task<HttpResponseMessage> DownloadFile(Guid applicationId, Guid sectionId, string pageId, string questionId, string fileName);
    }
}