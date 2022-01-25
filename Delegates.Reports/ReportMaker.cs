using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Delegates.Reports
{
    public class ReportMaker
    {
        public ReportMaker
        (
            string caption,
            Func<string, string> makeCaption,
            Func<string> beginList,
            Func<string> endList,
            Func<string, string, string> makeItem,
            Func<IEnumerable<double>, object> makeStatistics
        )
        {
            Caption = caption;
            MakeCaption = makeCaption;
            BeginList = beginList;
            EndList = endList;
            MakeItem = makeItem;
            MakeStatistics = makeStatistics;
        }
		
        public string Caption { get; }
        public Func<string, string> MakeCaption { get; }
        public Func<string> BeginList { get; }
        public Func<string> EndList { get; }
        public Func<string, string, string> MakeItem { get; }
        public Func<IEnumerable<double>, object> MakeStatistics { get; }
        public string MakeReport(IEnumerable<Measurement> measurements)
        {
            var data = measurements.ToList();
            var result = new StringBuilder();
            result.Append(MakeCaption(Caption));
            result.Append(BeginList());
            result.Append(MakeItem("Temperature", MakeStatistics(data.Select(z => z.Temperature)).ToString()));
            result.Append(MakeItem("Humidity", MakeStatistics(data.Select(z => z.Humidity)).ToString()));
            result.Append(EndList());
            return result.ToString();
        }
    }
	
    public static class ReportMakerHelper
	{
		public static string MeanAndStdHtmlReport(IEnumerable<Measurement> measurements)
		{
			return new ReportMaker
                (
                    "Mean and Std",
                    (caption) => $"<h1>{caption}</h1>",
                    () => "<ul>",
                    () => "</ul>",
                    (valueType, entry) => "<li><b>" + valueType + "</b>: " + entry,
                    (data) =>
                    {
                        var list = data.ToList();
                        var mean = list.Average();
                        var std = Math.Sqrt(list.Select(z => Math.Pow(z - mean, 2)).Sum() / (list.Count - 1));
                        return new MeanAndStd
                        {
                            Mean = mean,
                            Std = std
                        };
                    }
                ).MakeReport(measurements);
		}
		
		public static string MedianMarkdownReport(IEnumerable<Measurement> measurements)
		{
            return new ReportMaker
                (
                    "Median",
                    (caption) => $"## {caption}\n\n",
                    () => "",
                    () => "",
                    (valueType, entry) => $" * **{valueType}**: {entry}\n\n",
                    (data) =>
                    {
                        var list = data.OrderBy(z => z).ToList();
                        if (list.Count % 2 == 0)
                            return (list[list.Count / 2] + list[list.Count / 2 - 1]) / 2;
                        return list[list.Count / 2];
                    }
                ).MakeReport(measurements);
        }
		
		public static string MeanAndStdMarkdownReport(IEnumerable<Measurement> measurements)
		{
            return new ReportMaker
                (
                    "Mean and Std",
                    (caption) => $"## {caption}\n\n",
                    () => "",
                    () => "",
                    (valueType, entry) => $" * **{valueType}**: {entry}\n\n",
                    (data) =>
                    {
                        var list = data.ToList();
                        var mean = list.Average();
                        var std = Math.Sqrt(list.Select(z => Math.Pow(z - mean, 2)).Sum() / (list.Count - 1));
                        return new MeanAndStd
                        {
                            Mean = mean,
                            Std = std
                        };
                    }
                ).MakeReport(measurements);
        }
		
		public static string MedianHtmlReport(IEnumerable<Measurement> measurements)
		{
            return new ReportMaker
                (
                    "Median",
                    (caption) => $"<h1>{caption}</h1>",
                    () => "<ul>",
                    () => "</ul>",
                    (valueType, entry) => "<li><b>" + valueType + "</b>: " + entry,
                    (data) =>
                    {
                        var list = data.OrderBy(z => z).ToList();
                        if (list.Count % 2 == 0)
                            return (list[list.Count / 2] + list[list.Count / 2 - 1]) / 2;
                        return list[list.Count / 2];
                    }
                ).MakeReport(measurements);
        }
	}
}
