using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using Refit;
using SFA.DAS.QnA.Api.Types;
using SFA.DAS.QnA.Api.Types.Page;
using SFA.DAS.RoatpFinance.Web.ApplyTypes;
using SFA.DAS.RoatpFinance.Web.ApplyTypes.Apply;
using SFA.DAS.RoatpFinance.Web.ApplyTypes.Export;
using SFA.DAS.RoatpFinance.Web.Controllers;
using SFA.DAS.RoatpFinance.Web.Infrastructure.ApiClients;
using SFA.DAS.RoatpFinance.Web.Infrastructure.Models;
using SFA.DAS.RoatpFinance.Web.Services;
using SFA.DAS.RoatpFinance.Web.Validators;
using SFA.DAS.RoatpFinance.Web.Validators.Validation;
using SFA.DAS.RoatpFinance.Web.ViewModels;

namespace SFA.DAS.RoatpFinance.Web.Tests.Controllers.RoatpFinancial
{
    [TestFixture]
    public class RoatpFinancialControllerTests
    {
        private Mock<IRoatpApplicationApiClient> _applicationApplyApiClient;
        private Mock<IQnaApiClient> _qnaApiClient;
        private Mock<ISearchTermValidator> _searchTermValidator;
        private Mock<IRoatpFinancialClarificationViewModelValidator> _clarificationValidator;
        private Mock<IRoatpFinancialApplicationViewModelValidator> _applicationValidator;
        private Mock<ICsvExportService> _csvExportService;
        private RoatpFinancialController _controller;
        private readonly Guid _applicationId = Guid.NewGuid();
        private string _emailAddress = "Test@test.com";
        protected Mock<IHttpContextAccessor> MockHttpContextAccessor;
        private FinancialReviewDetails _financialReviewDetails;



        [SetUp]
        public void Before_each_test()
        {
            _applicationApplyApiClient = new Mock<IRoatpApplicationApiClient>();
            _searchTermValidator = new Mock<ISearchTermValidator>();
            _clarificationValidator = new Mock<IRoatpFinancialClarificationViewModelValidator>();
            _applicationValidator = new Mock<IRoatpFinancialApplicationViewModelValidator>();
            _qnaApiClient = new Mock<IQnaApiClient>();
            _csvExportService = new Mock<ICsvExportService>();


            _financialReviewDetails = new FinancialReviewDetails();
            MockHttpContextAccessor = SetupMockedHttpContextAccessor();

            _applicationValidator.Setup(x => x.Validate(It.IsAny<RoatpFinancialApplicationViewModel>()))
                .Returns(new ValidationResponse());

            _controller = new RoatpFinancialController(
                _applicationApplyApiClient.Object,
                _qnaApiClient.Object,
                _searchTermValidator.Object, _clarificationValidator.Object, _csvExportService.Object, _applicationValidator.Object)
            {
                ControllerContext = MockedControllerContext.Setup()
            };
        }

        private static Mock<IHttpContextAccessor> SetupMockedHttpContextAccessor()
        {

            var user = MockedUser.Setup();

            var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
            var context = new DefaultHttpContext { User = user };

            mockHttpContextAccessor.Setup(_ => _.HttpContext).Returns(context);
            return mockHttpContextAccessor;
        }

        [Test]
        public async Task ViewApplication_creates_correct_view_model_with_email()
        {
            _applicationApplyApiClient.Setup(x => x.GetApplication(_applicationId)).ReturnsAsync(
                new ApiResponse<RoatpApply>(
                new HttpRequestMessage(HttpMethod.Get, ""),
                new HttpResponseMessage(HttpStatusCode.OK),
                new RoatpApply
                {
                    ApplicationId = _applicationId,
                    ApplyData = new RoatpApplyData
                    {
                        ApplyDetails = new RoatpApplyDetails
                        {
                            OrganisationName = "org name",
                            UKPRN = "12344321",
                            ReferenceNumber = "3443",
                            ProviderRouteName = "main",
                            ApplicationSubmittedOn = DateTime.Today
                        },
                        Sequences = new List<RoatpApplySequence>
                        {
                            new RoatpApplySequence
                            {
                                SequenceNo = 5,
                                NotRequired = true
                            }
                        }
                    }
                },
                new RefitSettings()
                ));

            _applicationApplyApiClient.Setup(x => x.GetFinancialReviewDetails(_applicationId)).ReturnsAsync(new ApiResponse<FinancialReviewDetails>(
                new HttpRequestMessage(HttpMethod.Get, ""),
                new HttpResponseMessage(HttpStatusCode.OK),
                _financialReviewDetails,
                new RefitSettings()
            ));

            _applicationApplyApiClient.Setup(x => x.GetRoatpSequences()).ReturnsAsync(new List<RoatpSequence>());
            _qnaApiClient
                .Setup(x => x.GetQuestionTag(_applicationId, RoatpQnaConstants.QnaQuestionTags.HasParentCompany))
                .ReturnsAsync("No");
            _applicationApplyApiClient.Setup(x => x.GetContactForApplication(_applicationId))
                .ReturnsAsync(new ApiResponse<RoatpContact>(
                    new HttpRequestMessage(HttpMethod.Get, ""),
                    new HttpResponseMessage(HttpStatusCode.OK),
                    new RoatpContact
                    {
                        Email = _emailAddress
                    },
                    new RefitSettings()
                ));

            _qnaApiClient.Setup(x => x.GetSectionBySectionNo(_applicationId,
                    RoatpQnaConstants.RoatpSequences.YourOrganisation,
                    RoatpQnaConstants.RoatpSections.YourOrganisation.OrganisationDetails))
                .ReturnsAsync(new Section { ApplicationId = _applicationId, QnAData = new QnAData() });
            _qnaApiClient.Setup(x => x.GetSectionBySectionNo(_applicationId,
                    RoatpQnaConstants.RoatpSequences.YourOrganisation,
                    RoatpQnaConstants.RoatpSections.YourOrganisation.DescribeYourOrganisation))
                .ReturnsAsync(new Section { ApplicationId = _applicationId, QnAData = new QnAData() });

            var result = await _controller.ViewApplication(_applicationId);
            result.Should().BeAssignableTo<ViewResult>();

            var viewResult = result as ViewResult;
            var viewModel = viewResult.Model as RoatpFinancialApplicationViewModel;

            viewModel.ApplicantEmailAddress.Should().Be(_emailAddress);
        }

        [TestCase(ApplicationStatus.GatewayAssessed, FinancialReviewStatus.New, "Application.cshtml")]
        [TestCase(ApplicationStatus.GatewayAssessed, FinancialReviewStatus.InProgress, "Application.cshtml")]
        [TestCase(ApplicationStatus.GatewayAssessed, FinancialReviewStatus.ClarificationSent, "Application_Clarification.cshtml")]
        [TestCase(ApplicationStatus.GatewayAssessed, FinancialReviewStatus.Pass, "Application_ReadOnly.cshtml")]
        [TestCase(ApplicationStatus.GatewayAssessed, FinancialReviewStatus.Fail, "Application_ReadOnly.cshtml")]
        [TestCase(ApplicationStatus.Withdrawn, null, "Application_Closed.cshtml")]
        [TestCase(ApplicationStatus.Removed, null, "Application_Closed.cshtml")]
        public async Task ViewApplication_shows_expected_view_based_on_status(string applicationStatus, string financialReviewStatus, string expectedView)
        {
            _applicationApplyApiClient.Setup(x => x.GetApplication(_applicationId)).ReturnsAsync(
                new ApiResponse<RoatpApply>(
                new HttpRequestMessage(HttpMethod.Get, ""),
                new HttpResponseMessage(HttpStatusCode.OK),
                new RoatpApply
                {
                    ApplicationId = _applicationId,
                    ApplicationStatus = applicationStatus,
                    ApplyData = new RoatpApplyData
                    {
                        ApplyDetails = new RoatpApplyDetails(),
                        Sequences = new List<RoatpApplySequence>()
                    }
                },
                new RefitSettings()
            ));

            _applicationApplyApiClient.Setup(x => x.GetFinancialReviewDetails(_applicationId)).ReturnsAsync(
                new ApiResponse<FinancialReviewDetails>(
                new HttpRequestMessage(HttpMethod.Get, ""),
                new HttpResponseMessage(HttpStatusCode.OK),
                new FinancialReviewDetails
                {
                    ApplicationId = _applicationId,
                    Status = financialReviewStatus
                },
                new RefitSettings()
            ));

            _applicationApplyApiClient.Setup(x => x.GetRoatpSequences()).ReturnsAsync(new List<RoatpSequence>());

            _qnaApiClient.Setup(x => x.GetQuestionTag(_applicationId, RoatpQnaConstants.QnaQuestionTags.HasParentCompany))
                .ReturnsAsync("No");

            _applicationApplyApiClient.Setup(x => x.GetContactForApplication(_applicationId))
                .ReturnsAsync(new ApiResponse<RoatpContact>(
                    new HttpRequestMessage(HttpMethod.Get, ""),
                    new HttpResponseMessage(HttpStatusCode.OK),
                    new RoatpContact
                    {
                        Email = _emailAddress
                    },
                    new RefitSettings()
                ));

            _qnaApiClient.Setup(x => x.GetSectionBySectionNo(_applicationId,
                    RoatpQnaConstants.RoatpSequences.YourOrganisation,
                    RoatpQnaConstants.RoatpSections.YourOrganisation.OrganisationDetails))
                .ReturnsAsync(new Section { ApplicationId = _applicationId, QnAData = new QnAData() });

            _qnaApiClient.Setup(x => x.GetSectionBySectionNo(_applicationId,
                    RoatpQnaConstants.RoatpSequences.YourOrganisation,
                    RoatpQnaConstants.RoatpSections.YourOrganisation.DescribeYourOrganisation))
                .ReturnsAsync(new Section { ApplicationId = _applicationId, QnAData = new QnAData() });

            var result = await _controller.ViewApplication(_applicationId);
            var viewResult = result as ViewResult;

            Assert.That(viewResult.ViewName.EndsWith(expectedView), Is.True);
        }


        [Test]
        public void SubmitClarification_redirects_when_no_application()
        {
            _applicationApplyApiClient.Setup(x => x.GetApplication(_applicationId)).ReturnsAsync(new ApiResponse<RoatpApply>(
                new HttpRequestMessage(HttpMethod.Get, ""),
                new HttpResponseMessage(HttpStatusCode.OK),
                null,
                new RefitSettings()
            ));

            var result = _controller.SubmitClarification(_applicationId, new RoatpFinancialClarificationViewModel() { ApplicationId = _applicationId }).Result as RedirectToActionResult;
            Assert.That("OpenApplications", Is.EqualTo(result.ActionName));
        }

        [TestCase(FinancialApplicationSelectedGrade.Outstanding)]
        [TestCase(FinancialApplicationSelectedGrade.Satisfactory)]
        [TestCase(FinancialApplicationSelectedGrade.Good)]
        [TestCase(FinancialApplicationSelectedGrade.Inadequate)]
        [TestCase(FinancialApplicationSelectedGrade.Exempt)]
        public void SubmitClarification_valid_submission(string grade)
        {
            _clarificationValidator.Setup(x =>
                    x.Validate(It.IsAny<RoatpFinancialClarificationViewModel>(), It.IsAny<bool>(), It.IsAny<bool>()))
                .Returns(new ValidationResponse { });

            _applicationApplyApiClient.Setup(x => x.GetApplication(It.IsAny<Guid>())).ReturnsAsync(
                new ApiResponse<RoatpApply>(
                new HttpRequestMessage(HttpMethod.Get, ""),
                new HttpResponseMessage(HttpStatusCode.OK),
                new RoatpApply
                {
                    ApplicationId = _applicationId,
                    ApplyData = new RoatpApplyData
                    {
                        ApplyDetails = new RoatpApplyDetails
                        {
                            OrganisationName = "org name",
                            UKPRN = "12344321",
                            ReferenceNumber = "3443",
                            ProviderRouteName = "main",
                            ApplicationSubmittedOn = DateTime.Today
                        },
                        Sequences = new List<RoatpApplySequence>
                        {
                            new RoatpApplySequence
                            {
                                SequenceNo = 5,
                                NotRequired = true
                            }
                        }
                    }
                },
                new RefitSettings()
            ));
            _financialReviewDetails = new FinancialReviewDetails
            {
                GradedBy = MockHttpContextAccessor.Name,
                GradedOn = DateTime.UtcNow,
                SelectedGrade = grade,
                FinancialDueDate = DateTime.Today.AddDays(5),
                Comments = "comments",
                ExternalComments = grade == FinancialApplicationSelectedGrade.Inadequate ? "external comments" : null,
                ClarificationResponse = "clarification response",
                ClarificationRequestedOn = DateTime.UtcNow
            };

            _applicationApplyApiClient.Setup(x => x.GetFinancialReviewDetails(_applicationId)).ReturnsAsync(new ApiResponse<FinancialReviewDetails>(
                new HttpRequestMessage(HttpMethod.Get, ""),
                new HttpResponseMessage(HttpStatusCode.OK),
                _financialReviewDetails,
                new RefitSettings()
            ));

            var vm = new RoatpFinancialClarificationViewModel
            {
                ApplicationId = _applicationId,
                FinancialReviewDetails = _financialReviewDetails,
                OutstandingFinancialDueDate = new FinancialDueDate
                {
                    Day = "1",
                    Month = "1",
                    Year = (DateTime.Now.Year + 1).ToString()
                },
                ClarificationResponse = "clarification response",
                ClarificationComments = "clarification comments",
                FilesToUpload = null
            };
            var result = _controller.SubmitClarification(_applicationId, vm).Result as RedirectToActionResult;
            _applicationApplyApiClient.Verify(x => x.ReturnFinancialReview(_applicationId, It.IsAny<FinancialReviewDetails>()), Times.Once);
            Assert.That("Graded", Is.EqualTo(result.ActionName));
        }

        [Test]
        public void When_clarification_file_is_uploaded_and_page_is_refreshed_with_filename_included_in_model()
        {
            var buttonPressed = "submitClarificationFiles";
            _applicationApplyApiClient.Setup(x => x.GetRoatpSequences()).ReturnsAsync(new List<RoatpSequence>());
            _qnaApiClient.Setup(x => x.GetSectionBySectionNo(_applicationId,
                    RoatpQnaConstants.RoatpSequences.YourOrganisation,
                    RoatpQnaConstants.RoatpSections.YourOrganisation.OrganisationDetails))
                .ReturnsAsync(new Section { ApplicationId = _applicationId, QnAData = new QnAData() });
            _qnaApiClient.Setup(x => x.GetSectionBySectionNo(_applicationId,
                    RoatpQnaConstants.RoatpSequences.YourOrganisation,
                    RoatpQnaConstants.RoatpSections.YourOrganisation.DescribeYourOrganisation))
                .ReturnsAsync(new Section { ApplicationId = _applicationId, QnAData = new QnAData() });
            _controller = new RoatpFinancialController(
                _applicationApplyApiClient.Object,
                _qnaApiClient.Object,
                _searchTermValidator.Object, _clarificationValidator.Object, Mock.Of<ICsvExportService>(), _applicationValidator.Object)
            {
                ControllerContext = MockedControllerContext.Setup(buttonPressed)
            };

            _clarificationValidator.Setup(x =>
                    x.Validate(It.IsAny<RoatpFinancialClarificationViewModel>(), It.IsAny<bool>(), It.IsAny<bool>()))
                .Returns(new ValidationResponse { });

            _applicationApplyApiClient.Setup(x => x.GetApplication(It.IsAny<Guid>())).ReturnsAsync(
                new ApiResponse<RoatpApply>(
                new HttpRequestMessage(HttpMethod.Get, ""),
                new HttpResponseMessage(HttpStatusCode.OK),
                new RoatpApply
                {
                    ApplicationId = _applicationId,
                    ApplyData = new RoatpApplyData
                    {
                        ApplyDetails = new RoatpApplyDetails
                        {
                            OrganisationName = "org name",
                            UKPRN = "12344321",
                            ReferenceNumber = "3443",
                            ProviderRouteName = "main",
                            ApplicationSubmittedOn = DateTime.Today
                        },
                        Sequences = new List<RoatpApplySequence>
                        {
                            new RoatpApplySequence
                            {
                                SequenceNo = 5,
                                NotRequired = true
                            }
                        }
                    }
                },
                new RefitSettings()
            ));

            _applicationApplyApiClient.Setup(x =>
                    x.UploadClarificationFile(_applicationId, It.IsAny<MultipartFormDataContent>()))
                .ReturnsAsync(new ApiResponse<string>(new HttpResponseMessage(HttpStatusCode.OK), string.Empty, new RefitSettings()));


            _financialReviewDetails = new FinancialReviewDetails
            {
                GradedBy = MockHttpContextAccessor.Name,
                GradedOn = DateTime.UtcNow,
                SelectedGrade = FinancialApplicationSelectedGrade.Good,
                FinancialDueDate = DateTime.Today.AddDays(5),
                Comments = "comments",
                ClarificationResponse = "clarification response",
                ClarificationRequestedOn = DateTime.UtcNow
            };
            _applicationApplyApiClient.Setup(x => x.GetFinancialReviewDetails(_applicationId)).ReturnsAsync(new ApiResponse<FinancialReviewDetails>(
                new HttpRequestMessage(HttpMethod.Get, ""),
                new HttpResponseMessage(HttpStatusCode.OK),
                _financialReviewDetails,
                new RefitSettings()
            ));

            var vm = new RoatpFinancialClarificationViewModel
            {
                ApplicationId = _applicationId,
                FinancialReviewDetails = _financialReviewDetails,
                OutstandingFinancialDueDate = new FinancialDueDate
                {
                    Day = "1",
                    Month = "1",
                    Year = (DateTime.Now.Year + 1).ToString()
                },
                ClarificationResponse = "clarification response",
                ClarificationComments = "clarification comments",
                FilesToUpload = null
            };
            var result = _controller.SubmitClarification(_applicationId, vm).Result as ViewResult;

            Assert.That(result.ViewName.Contains("Application_Clarification.cshtml"), Is.True);
            var resultModel = result.Model as RoatpFinancialClarificationViewModel;

            Assert.That(resultModel.FinancialReviewDetails.ClarificationFiles[0].Filename == "file.pdf", Is.True);
        }

        [Test]
        public void When_clarification_file_is_removed_and_page_is_refreshed_with_filename_removed_from_model()
        {
            var buttonPressed = "removeClarificationFile";
            _applicationApplyApiClient.Setup(x => x.GetRoatpSequences()).ReturnsAsync(new List<RoatpSequence>());
            _qnaApiClient.Setup(x => x.GetSectionBySectionNo(_applicationId,
                    RoatpQnaConstants.RoatpSequences.YourOrganisation,
                    RoatpQnaConstants.RoatpSections.YourOrganisation.OrganisationDetails))
                .ReturnsAsync(new Section { ApplicationId = _applicationId, QnAData = new QnAData() });
            _qnaApiClient.Setup(x => x.GetSectionBySectionNo(_applicationId,
                    RoatpQnaConstants.RoatpSequences.YourOrganisation,
                    RoatpQnaConstants.RoatpSections.YourOrganisation.DescribeYourOrganisation))
                .ReturnsAsync(new Section { ApplicationId = _applicationId, QnAData = new QnAData() });
            _controller = new RoatpFinancialController(
                _applicationApplyApiClient.Object,
                _qnaApiClient.Object,
                _searchTermValidator.Object, _clarificationValidator.Object, Mock.Of<ICsvExportService>(), _applicationValidator.Object)
            {
                ControllerContext = MockedControllerContext.Setup(buttonPressed)
            };

            _clarificationValidator.Setup(x =>
                    x.Validate(It.IsAny<RoatpFinancialClarificationViewModel>(), It.IsAny<bool>(), It.IsAny<bool>()))
                .Returns(new ValidationResponse { });
            var fileToBeRemoved = "file.pdf";
            _financialReviewDetails = new FinancialReviewDetails
            {
                GradedBy = MockHttpContextAccessor.Name,
                GradedOn = DateTime.UtcNow,
                SelectedGrade = FinancialApplicationSelectedGrade.Good,
                FinancialDueDate = DateTime.Today.AddDays(5),
                Comments = "comments",
                ClarificationResponse = "clarification response",
                ClarificationRequestedOn = DateTime.UtcNow,
                ClarificationFiles = new List<ClarificationFile> { new ClarificationFile { Filename = fileToBeRemoved } }
            };

            _applicationApplyApiClient.Setup(x => x.GetApplication(It.IsAny<Guid>())).ReturnsAsync(
                new ApiResponse<RoatpApply>(
                new HttpRequestMessage(HttpMethod.Get, ""),
                new HttpResponseMessage(HttpStatusCode.OK),
                new RoatpApply
                {
                    ApplicationId = _applicationId,
                    ApplyData = new RoatpApplyData
                    {
                        ApplyDetails = new RoatpApplyDetails
                        {
                            OrganisationName = "org name",
                            UKPRN = "12344321",
                            ReferenceNumber = "3443",
                            ProviderRouteName = "main",
                            ApplicationSubmittedOn = DateTime.Today
                        },
                        Sequences = new List<RoatpApplySequence>
                        {
                            new RoatpApplySequence
                            {
                                SequenceNo = 5,
                                NotRequired = true
                            }
                        }
                    }
                },
                new RefitSettings()
            ));

            var model = new RemoveClarificationFileCommandModel { UserId = "", FileName = fileToBeRemoved };
            _applicationApplyApiClient.Setup(x =>
                    x.RemoveClarificationFile(It.IsAny<Guid>(), It.IsAny<RemoveClarificationFileCommandModel>()))
                .ReturnsAsync(new ApiResponse<string>(new HttpResponseMessage(HttpStatusCode.OK), string.Empty, new RefitSettings()));

            _applicationApplyApiClient.Setup(x => x.GetFinancialReviewDetails(_applicationId)).ReturnsAsync(new ApiResponse<FinancialReviewDetails>(
                new HttpRequestMessage(HttpMethod.Get, ""),
                new HttpResponseMessage(HttpStatusCode.OK),
                new FinancialReviewDetails(),
                new RefitSettings()
            ));

            var vm = new RoatpFinancialClarificationViewModel
            {
                ApplicationId = _applicationId,
                FinancialReviewDetails = _financialReviewDetails,
                OutstandingFinancialDueDate = new FinancialDueDate
                {
                    Day = "1",
                    Month = "1",
                    Year = (DateTime.Now.Year + 1).ToString()
                },
                ClarificationResponse = "clarification response",
                ClarificationComments = "clarification comments",
                FilesToUpload = null
            };
            var result = _controller.SubmitClarification(_applicationId, vm).Result as ViewResult;

            Assert.That(result.ViewName.Contains("Application_Clarification.cshtml"), Is.True);
            var resultModel = result.Model as RoatpFinancialClarificationViewModel;

            Assert.That(resultModel.FinancialReviewDetails.ClarificationFiles, Is.Null);
        }

        [Test]
        public void when_validation_errors_occur_page_refreshes_with_validation_messages()
        {
            var buttonPressed = "submitClarificationFiles";
            _applicationApplyApiClient.Setup(x => x.GetRoatpSequences()).ReturnsAsync(new List<RoatpSequence>());
            _qnaApiClient.Setup(x => x.GetSectionBySectionNo(_applicationId,
                    RoatpQnaConstants.RoatpSequences.YourOrganisation,
                    RoatpQnaConstants.RoatpSections.YourOrganisation.OrganisationDetails))
                .ReturnsAsync(new Section { ApplicationId = _applicationId, QnAData = new QnAData() });
            _qnaApiClient.Setup(x => x.GetSectionBySectionNo(_applicationId,
                    RoatpQnaConstants.RoatpSequences.YourOrganisation,
                    RoatpQnaConstants.RoatpSections.YourOrganisation.DescribeYourOrganisation))
                .ReturnsAsync(new Section { ApplicationId = _applicationId, QnAData = new QnAData() });
            _controller = new RoatpFinancialController(
                _applicationApplyApiClient.Object,
                _qnaApiClient.Object,
                _searchTermValidator.Object, _clarificationValidator.Object, Mock.Of<ICsvExportService>(), _applicationValidator.Object)
            {
                ControllerContext = MockedControllerContext.Setup(buttonPressed)
            };

            _clarificationValidator.Setup(x =>
                    x.Validate(It.IsAny<RoatpFinancialClarificationViewModel>(), It.IsAny<bool>(), It.IsAny<bool>()))
                .Returns(new ValidationResponse { Errors = new List<ValidationErrorDetail> { new ValidationErrorDetail { ErrorMessage = "error message", Field = "errorField" } } });

            _applicationApplyApiClient.Setup(x => x.GetApplication(It.IsAny<Guid>())).ReturnsAsync(
                new ApiResponse<RoatpApply>(
                new HttpRequestMessage(HttpMethod.Get, ""),
                new HttpResponseMessage(HttpStatusCode.OK),
                new RoatpApply
                {
                    ApplicationId = _applicationId,
                    ApplyData = new RoatpApplyData
                    {
                        ApplyDetails = new RoatpApplyDetails
                        {
                            OrganisationName = "org name",
                            UKPRN = "12344321",
                            ReferenceNumber = "3443",
                            ProviderRouteName = "main",
                            ApplicationSubmittedOn = DateTime.Today
                        },
                        Sequences = new List<RoatpApplySequence>
                        {
                            new RoatpApplySequence
                            {
                                SequenceNo = 5,
                                NotRequired = true
                            }
                        }
                    }
                },
                new RefitSettings()
            ));

            _applicationApplyApiClient.Setup(x =>
                    x.UploadClarificationFile(_applicationId, It.IsAny<MultipartFormDataContent>()))
                .ReturnsAsync(new ApiResponse<string>(new HttpResponseMessage(HttpStatusCode.OK), string.Empty, new RefitSettings()));


            _financialReviewDetails = new FinancialReviewDetails
            {
                GradedBy = MockHttpContextAccessor.Name,
                GradedOn = DateTime.UtcNow,
                SelectedGrade = FinancialApplicationSelectedGrade.Good,
                FinancialDueDate = DateTime.Today.AddDays(5),
                Comments = "comments",
                ClarificationResponse = "clarification response",
                ClarificationRequestedOn = DateTime.UtcNow
            };

            _applicationApplyApiClient.Setup(x => x.GetFinancialReviewDetails(_applicationId)).ReturnsAsync(new ApiResponse<FinancialReviewDetails>(
                new HttpRequestMessage(HttpMethod.Get, ""),
                new HttpResponseMessage(HttpStatusCode.OK),
                _financialReviewDetails,
                new RefitSettings()
            ));

            var vm = new RoatpFinancialClarificationViewModel
            {
                ApplicationId = _applicationId,
                FinancialReviewDetails = _financialReviewDetails,
                OutstandingFinancialDueDate = new FinancialDueDate
                {
                    Day = "1",
                    Month = "1",
                    Year = (DateTime.Now.Year + 1).ToString()
                },
                ClarificationResponse = "clarification response",
                ClarificationComments = "clarification comments",
                FilesToUpload = null
            };
            var result = _controller.SubmitClarification(_applicationId, vm).Result as ViewResult;

            Assert.That(result.ViewName.Contains("Application_Clarification.cshtml"), Is.True);
            var resultModel = result.Model as RoatpFinancialClarificationViewModel;
            Assert.That(1, Is.EqualTo(resultModel.ErrorMessages.Count));
        }

        [Test]
        public void DownloadClarification_downloads_file()
        {
            var filename = "test.pdf";
            HttpContent content = new StringContent("4");
            var response = new HttpResponseMessage { StatusCode = HttpStatusCode.OK, Content = content };

            _applicationApplyApiClient.Setup(x => x.DownloadClarificationFile(_applicationId, filename)).ReturnsAsync(response);

            var result = _controller.DownloadClarificationFile(_applicationId, filename).Result as FileStreamResult;
            Assert.That(filename, Is.EqualTo(result.FileDownloadName));
        }

        [Test]
        public async Task DownloadOpenApplications_downloads_file()
        {
            // Need this otherwise AutoMapper will complain

            var apiResponse = new List<RoatpFinancialSummaryDownloadItem>();

            _applicationApplyApiClient.Setup(x => x.GetOpenFinancialApplicationsForDownload())
                .ReturnsAsync(() => apiResponse);

            var expectedFileContents = Encoding.ASCII.GetBytes("THIS IS A TEST");

            _csvExportService.Setup(x =>
                    x.WriteCsvToByteArray<RoatpFinancialSummaryExportItem, RoatpFinancialSummaryExportCsvMap>(new List<RoatpFinancialSummaryExportItem>()))
                .Returns(expectedFileContents);

            var result = await _controller.DownloadOpenApplications() as FileContentResult;

            Assert.That(expectedFileContents, Is.EqualTo(result.FileContents));
            Assert.That($"current_applications_{DateTime.UtcNow:ddMMyy}.csv", Is.EqualTo(result.FileDownloadName));
        }
    }
}
