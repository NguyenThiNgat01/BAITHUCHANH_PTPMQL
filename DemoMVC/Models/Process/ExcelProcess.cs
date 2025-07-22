using OfficeOpenXml;
using System.Data;
using System.IO;

namespace DemoMVC.Models.Process // ✅ Thêm namespace chính xác
{
    public class ExcelProcess // ✅ Phải là public
    {
        public DataTable ExcelToDataTable(string fileLocation)
        {
            var dataTable = new DataTable();

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (var package = new ExcelPackage(new FileInfo(fileLocation)))
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                int colCount = worksheet.Dimension.End.Column;
                int rowCount = worksheet.Dimension.End.Row;

                for (int col = 1; col <= colCount; col++)
                {
                    dataTable.Columns.Add(worksheet.Cells[1, col].Text);
                }

                for (int row = 2; row <= rowCount; row++)
                {
                    DataRow dr = dataTable.NewRow();
                    for (int col = 1; col <= colCount; col++)
                    {
                        dr[col - 1] = worksheet.Cells[row, col].Text;
                    }
                    dataTable.Rows.Add(dr);
                }
            }

            return dataTable;
        }
    }
}
