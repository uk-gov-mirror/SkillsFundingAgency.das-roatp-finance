using CsvHelper.Configuration;


namespace SFA.DAS.RoatpFinance.Web.Services
{
    public interface ICsvExportService
    {
        byte[] WriteCsvToByteArray<T, TU>(IEnumerable<T> records) where TU : ClassMap<T>;
    }
}
