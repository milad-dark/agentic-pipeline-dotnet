using System.Xml.Linq;
using AgenticPipeline.Domain.Models;

namespace AgenticPipeline.QAAgent.Services;

public sealed class TestResultParser
{
    public QAResult ParseTrx(string trxPath)
    {
        var doc = XDocument.Load(trxPath);
        XNamespace ns = "http://microsoft.com/schemas/VisualStudio/TeamTest/2010";
        var counters = doc.Root?.Element(ns + "ResultSummary")?.Element(ns + "Counters");

        int total = ParseInt(counters?.Attribute("total")?.Value);
        int passed = ParseInt(counters?.Attribute("passed")?.Value);
        int failed = ParseInt(counters?.Attribute("failed")?.Value);

        return new QAResult
        {
            Passed = failed == 0,
            TestsRun = total,
            TestsPassed = passed,
            TestsFailed = failed,
            FailureReason = failed == 0 ? null : "Some tests failed"
        };
    }

    private static int ParseInt(string? value) => int.TryParse(value, out var i) ? i : 0;
}
