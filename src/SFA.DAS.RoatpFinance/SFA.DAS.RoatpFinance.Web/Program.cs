using Microsoft.AspNetCore;
using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.RoatpFinance.Web
{
    [ExcludeFromCodeCoverage]
    public class Program
    {
        public static void Main(string[] args)
        {
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .Build()
                .Run();
        }
    }
}
