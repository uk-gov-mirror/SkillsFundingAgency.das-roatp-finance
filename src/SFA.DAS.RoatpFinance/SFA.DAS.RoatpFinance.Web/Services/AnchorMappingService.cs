namespace SFA.DAS.RoatpFinance.Web.Services;

public static class AnchorMappingService
{
    public static string Map(string fieldToMap)
    {
        var mapped = MappingItems().FirstOrDefault(x => x.FieldToMap == fieldToMap)!.ReturnValue;

        return mapped == string.Empty ? fieldToMap : mapped;
    }

    private record MappingItem(string FieldToMap, string ReturnValue);

    private static List<MappingItem> MappingItems() =>
    [
        new("FinancialReviewDetails.SelectedGrade", "outstanding"),
        new("InadequateComments", string.Empty),
        new("InadequateExternalComments", string.Empty),
        new("ClarificationComments", string.Empty),
        new("OutstandingFinancialDueDate", "OutstandingFinancialDueDate.Day"),
        new("GoodFinancialDueDate", "GoodFinancialDueDate.Day"),
        new("SatisfactoryFinancialDueDate", "SatisfactoryFinancialDueDate.Day")
    ];

}
