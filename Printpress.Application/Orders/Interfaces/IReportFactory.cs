using System.Collections.Specialized;
using QuestPDF.Infrastructure;

namespace Printpress.Application
{
    public interface IReportFactory
    {
        string ReportName { get; }
        Task<IDocument> GenerateReport(NameValueCollection queryParams);
    }
}
