using System.Data;
using System.IO;

namespace SSD.IO
{
    public static class DataFileParser
    {
        public static DataTable ExtractValues(Stream stream, char delimiter, int numOfColumns, int headerRow, int startRow, int endRow)
        {
            DataTable dt = new DataTable();
            stream.Position = 0;
            using (StreamReader reader = new StreamReader(stream))
            {
                string line;
                for (int i = 1; (line = reader.ReadLine()) != null; i++)
                {
                    if (i == headerRow)
                    {
                        CreateHeaderRow(delimiter, dt, line);
                    }
                    else if (i >= startRow)
                    {
                        if (i > endRow)
                        {
                            return dt;
                        }
                        else
                        {
                            CreateDataRow(delimiter, dt, line);
                        }
                    }
                }
            }
            return dt;
        }

        private static void CreateDataRow(char delimiter, DataTable dt, string line)
        {
            string[] values = line.Split(delimiter);
            DataRow row = dt.NewRow();
            if (values.Length < dt.Columns.Count)
            {
                row.RowError = "Incorrect number of columns.";
            }
            for (int i = 0; i < dt.Columns.Count && i < values.Length; i++)
            {
                row[i] = values[i];
            }
            dt.Rows.Add(row);
        }

        private static void CreateHeaderRow(char delimiter, DataTable dt, string line)
        {
            string[] values = line.Split(delimiter);
            foreach (var value in values)
            {
                dt.Columns.Add(value);
            }
        }
    }
}
