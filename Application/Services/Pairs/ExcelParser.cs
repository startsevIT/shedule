using System.Data;
using System.Text;
using ExcelDataReader;

namespace Application.Services.Pairs;

public static class ExcelParser
{
    public static List<PairParseModel> Parse(string filePath)
    {
        var configuration = new ExcelReaderConfiguration { FallbackEncoding = Encoding.GetEncoding(1251) };

        using var stream = File.Open(filePath, FileMode.Open, FileAccess.Read);
        using var reader = ExcelReaderFactory.CreateReader(stream, configuration);

        var dataSet = reader.AsDataSet(new ExcelDataSetConfiguration()
        {
            ConfigureDataTable = _ => new ExcelDataTableConfiguration
            {
                FilterRow = rowReader =>
                {
                    for (var i = 0; i < rowReader.FieldCount; i++)
                        if (rowReader[i] != null && !string.IsNullOrEmpty(rowReader[i].ToString()))
                            return true;
                    return false;
                },
                FilterColumn = (rowReader, colIndex) =>
                {
                    rowReader.Reset();
                    rowReader.Read();

                    while (rowReader.Read())
                    {
                        if (rowReader[colIndex] != null && !string.IsNullOrEmpty(rowReader[colIndex].ToString()))
                        {
                            rowReader.Reset();
                            rowReader.Read();
                            return true;
                        }
                    }

                    rowReader.Reset();
                    rowReader.Read();
                    return false;
                }
            }
        });

        return new TableParser(dataSet.Tables[0]).Parse();
    }

    private class TableParser(DataTable data)
    {
        public List<PairParseModel> Parse()
        {
            var result = new List<PairParseModel>();

            for (var y = 4; y < data.Columns.Count; y += 2)
            for (int x = 5, pairNumber = 1; x < data.Rows.Count; x += 2, pairNumber++)
            {
                if (pairNumber % 7 == 0)
                {
                    x += 1;
                    pairNumber = 1;
                }

                var pair = new PairParser(data, rowIndex: x, columnIndex: y).ParsePair();
                if (pair != null)
                    result.Add(pair);
            }

            return result;
        }

        private class PairParser(DataTable data, int rowIndex, int columnIndex)
        {
            private string DisciplineName => data.Rows[rowIndex][data.Columns[columnIndex]].ToString()!;
            private string Cabinet => data.Rows[rowIndex][data.Columns[columnIndex + 1]].ToString()!;
            private string TeacherInitials => data.Rows[rowIndex + 1][data.Columns[columnIndex]].ToString()!;
            private string GroupNumber => data.Rows[4][data.Columns[columnIndex]].ToString()!;
            private DateTime DateTime
            {
                get
                {
                    var timeStr = data.Rows[rowIndex][data.Columns[1]].ToString()!;
                    var dateStr = data.Rows[rowIndex / 13 * 13 + 5][data.Columns[0]].ToString()!;

                    var startTime = TimeOnly.Parse(timeStr.Split('-')[0]);
                    var date = DateOnly.Parse(dateStr.Split(' ')[0]);

                    return new DateTime(date, startTime);
                }
            }

            public PairParseModel? ParsePair()
            {
                return DisciplineName is "" or "Самостоятельная подготовка к экзамену"
                    ? null
                    : new PairParseModel(DisciplineName, TeacherInitials, Cabinet, DateTime, GroupNumber);
            }
        }
    }
}