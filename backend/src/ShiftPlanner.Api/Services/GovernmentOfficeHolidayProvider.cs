using System.Globalization;
using Microsoft.VisualBasic.FileIO;

namespace ShiftPlanner.Api.Services;

public sealed class GovernmentOfficeHolidayProvider : IHolidayProvider
{
    private readonly Lazy<IReadOnlyDictionary<DateOnly, HolidayInfo>> holidays;

    public GovernmentOfficeHolidayProvider(IWebHostEnvironment environment)
    {
        var calendarFilePath = Path.Combine(
            environment.ContentRootPath,
            "Data",
            "Holidays",
            "government-office-calendar.csv");

        holidays = new Lazy<IReadOnlyDictionary<DateOnly, HolidayInfo>>(
            () => LoadHolidays(calendarFilePath));
    }

    public HolidayInfo? GetHoliday(DateOnly date)
    {
        return holidays.Value.GetValueOrDefault(date);
    }

    private static IReadOnlyDictionary<DateOnly, HolidayInfo> LoadHolidays(string calendarFilePath)
    {
        var holidays = new Dictionary<DateOnly, HolidayInfo>();

        using var parser = new TextFieldParser(calendarFilePath);
        parser.TextFieldType = FieldType.Delimited;
        parser.SetDelimiters(",");
        parser.HasFieldsEnclosedInQuotes = true;

        var headers = parser.ReadFields() ?? throw new InvalidOperationException("國定假日資料缺少欄位標題。");
        var columnIndexes = headers
            .Select((header, index) => new { Header = header.Trim(), Index = index })
            .ToDictionary(column => column.Header, column => column.Index, StringComparer.OrdinalIgnoreCase);

        while (!parser.EndOfData)
        {
            var fields = parser.ReadFields();

            if (fields is null
                || !TryGetValue(fields, columnIndexes, "date", out var dateValue)
                || !TryGetValue(fields, columnIndexes, "name", out var name)
                || !TryGetValue(fields, columnIndexes, "isholiday", out var isHoliday)
                || !TryGetValue(fields, columnIndexes, "holidaycategory", out var category)
                || isHoliday != "是"
                || category == "星期六、星期日"
                || !DateOnly.TryParseExact(dateValue, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var date))
            {
                continue;
            }

            var holidayName = string.IsNullOrWhiteSpace(name) ? category : name;
            holidays[date] = new HolidayInfo(holidayName.Trim());
        }

        return holidays;
    }

    private static bool TryGetValue(
        IReadOnlyList<string> fields,
        IReadOnlyDictionary<string, int> columnIndexes,
        string columnName,
        out string value)
    {
        value = string.Empty;

        return columnIndexes.TryGetValue(columnName, out var index)
            && index < fields.Count
            && (value = fields[index].Trim()) is not null;
    }
}