using System;
using NUnit.Framework;
using SFA.DAS.RoatpFinance.Web.ApplyTypes.Export;

namespace SFA.DAS.RoatpFinance.Web.Tests.ApplyTypes.Export;

public class RoatpFinancialSummaryExportItemTests
{
    [Test]
    public void ImplicitConversion_WhenSourceHasValues_ThenMapsAllFields()
    {
        // Arrange
        var source = new RoatpFinancialSummaryDownloadItem
        {
            ApplicationId = Guid.NewGuid(),
            ApplicationReferenceNumber = "Test",
            ApplicationRoute = "Test route",
            OrganisationName = "Test Provider",
            Ukprn = "12345678",
            SubmittedDate = DateTime.Today,
            GatewayOutcomeDate = DateTime.Today,
            CompanyNumber = "12345",
            CharityNumber = "67890",
            FinancialData = new FinancialData
            {
                TurnOver = 1000,
                Depreciation = 200,
                ProfitLoss = 300,
                Dividends = 400,
                IntangibleAssets = 500,
                Assets = 600,
                Liabilities = 700,
                ShareholderFunds = 800,
                Borrowings = 900,
                AccountingReferenceDate = DateTime.Today,
                AccountingPeriod = 12,
                AverageNumberofFTEEmployees = 25
            }
        };

        // Act
        RoatpFinancialSummaryExportItem result = source;

        // Assert
        Assert.NotNull(result);
        Assert.That(source.ApplicationId, Is.EqualTo(result.ApplicationId));
        Assert.That(source.ApplicationReferenceNumber, Is.EqualTo(result.ApplicationReference));
        Assert.That(source.ApplicationRoute, Is.EqualTo(result.Route));
        Assert.That(source.OrganisationName, Is.EqualTo(result.ProviderName));
        Assert.That(source.Ukprn, Is.EqualTo(result.Ukprn));
        Assert.That(source.SubmittedDate.Value, Is.EqualTo(result.SubmissionDate));
        Assert.That(source.GatewayOutcomeDate, Is.EqualTo(result.GatewayCompletionDate));
        Assert.That(source.CompanyNumber, Is.EqualTo(result.CompanyNo));
        Assert.That(source.CharityNumber, Is.EqualTo(result.CharityNo));

        Assert.That(1000, Is.EqualTo(result.TurnOver));
        Assert.That(200, Is.EqualTo(result.Depreciation));
        Assert.That(300, Is.EqualTo(result.ProfitLoss));
        Assert.That(400, Is.EqualTo(result.Dividends));
        Assert.That(500, Is.EqualTo(result.IntangibleAssets));
        Assert.That(600, Is.EqualTo(result.Assets));
        Assert.That(700, Is.EqualTo(result.Liabilities));
        Assert.That(800, Is.EqualTo(result.ShareholderFunds));
        Assert.That(900, Is.EqualTo(result.Borrowings));
        Assert.That(DateTime.Today, Is.EqualTo(result.AccountingReferenceDate));
        Assert.That(12, Is.EqualTo(result.AccountingPeriod));
        Assert.That(25, Is.EqualTo(result.AverageNumberofFTEEmployees));
    }

    [Test]
    public void ImplicitConversion_HandlesNullSourceProperties()
    {
        // Arrange
        var source = new RoatpFinancialSummaryDownloadItem
        {
            ApplicationId = Guid.NewGuid(),
            ApplicationReferenceNumber = "Test",
            ApplicationRoute = "Test route",
            OrganisationName = "Test Provider",
            Ukprn = "87654321",
            SubmittedDate = null,
            GatewayOutcomeDate = DateTime.Today,
            CompanyNumber = "12345",
            CharityNumber = "67890",
            FinancialData = null
        };

        // Act
        RoatpFinancialSummaryExportItem result = source;

        // Assert
        Assert.AreEqual(default(DateTime), result.SubmissionDate);
        Assert.Null(result.TurnOver);
        Assert.Null(result.Depreciation);
        Assert.Null(result.ProfitLoss);
        Assert.Null(result.Dividends);
        Assert.Null(result.IntangibleAssets);
        Assert.Null(result.Assets);
        Assert.Null(result.Liabilities);
        Assert.Null(result.ShareholderFunds);
        Assert.Null(result.Borrowings);
        Assert.Null(result.AccountingReferenceDate);
        Assert.Null(result.AccountingPeriod);
        Assert.Null(result.AverageNumberofFTEEmployees);
    }

    [Test]
    public void ImplicitConversion_WhenSourceIsNull_ThenReturnsNull()
    {
        // Arrange
        RoatpFinancialSummaryDownloadItem source = null;

        // Act
        RoatpFinancialSummaryExportItem result = source;

        // Assert
        Assert.Null(result);
    }
}
