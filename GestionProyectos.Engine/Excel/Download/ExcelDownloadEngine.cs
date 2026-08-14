using System.ComponentModel;
using System.Reflection;
using GestionProyectos.Engine.Excel.Download.Request;
using GestionProyectos.Engine.Excel.Download.Response;
using GestionProyectos.Shared.Message;
using GestionProyectos.Shared.Utility;
using OfficeOpenXml;
using static OfficeOpenXml.ExcelErrorValue;

namespace GestionProyectos.Engine.Excel.Download
{
    public class ExcelDownloadEngine : IExcelDownloadEngine
    {
        public OperationResult<ExcelDownloadResponse> Download(ExcelDownloadRequest request)
        {
            try
            {
                // TIPO DE ARCHIVO
                string xlsxContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

                // SE CONSTRUYE EL ARCHIVO EXCEL
                OfficeOpenXml.ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
                var package = new ExcelPackage();

                var worksheet = package.Workbook.Worksheets.Add(request.WorksheetName);

                // OBTIENE LAS PROPIEDADES DE LA ENTIDAD
                var firstRow = request.Rows.FirstOrDefault();
                if (firstRow == null)
                    return OperationResult<ExcelDownloadResponse>.CreateFailureResult("No hay filas para exportar.");

                var properties = firstRow.GetType().GetProperties();

                // AÑADIR CABECERAS DEL ARCHIVO
                for (int i = 0; i < properties.Length; i++)
                {
                    var displayName = properties[i].GetCustomAttribute<DisplayNameAttribute>()?.DisplayName;
                    worksheet.Cells[1, i + 1].Value = displayName ?? properties[i].Name;
                }

                int rowNumber = 2;
                foreach (var row in request.Rows)
                {
                    int columnNumber = 1;
                    foreach (var property in properties)
                    {
                        var dbValue = Utils.GetFormatedValueForDb(property.GetValue(row).GetType(), property.GetValue(row));
                        string value = Convert.ToString(dbValue);
                        if (value.Contains('\n')) 
                        {
                             List<string> valuesList = value.Split("\n").ToList();

                            var cell = worksheet.Cells[rowNumber, columnNumber];

                            cell.Value = value.Replace("'", string.Empty);
                            cell.Style.WrapText = true;
                            cell.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Top;
                            //}
                        }
                        else
                        {
                            worksheet.Cells[rowNumber, columnNumber].Value = dbValue.ToString().Replace("'", string.Empty);
                        }
                        columnNumber++;
                    }
                    rowNumber++;
                }

                var numberFormat = "#.##0";
                var dataCellStyleName = "TableNumber";
                var numStyle = package.Workbook.Styles.CreateNamedStyle(dataCellStyleName);
                numStyle.Style.Numberformat.Format = numberFormat;

                var tbl = worksheet.Tables.Add(new ExcelAddressBase(fromRow: 1, fromCol: 1, toRow: request.Rows.Count + 1, toColumn: properties.Length), "Data");

                tbl.ShowHeader = true;
                tbl.TableStyle = OfficeOpenXml.Table.TableStyles.Custom;
                tbl.Columns[1].DataCellStyleName = dataCellStyleName;

                worksheet.Cells[1, 1, request.Rows.Count, properties.Length].AutoFitColumns();

                // OBTENER EL ARCHIVO EN BASE 64
                string reportBytes;
                reportBytes = Convert.ToBase64String(package.GetAsByteArray());

                // RESPUESTA DEL SERVICIO
                ExcelDownloadResponse response = new()
                {
                    FileBase64 = reportBytes,
                    FileType = xlsxContentType
                };

                return OperationResult<ExcelDownloadResponse>.CreateSuccessResult(response);
            }
            catch (Exception ex)
            {
                return OperationResult<ExcelDownloadResponse>.CreateFailureResult(ex);
            }
        }
    }
}
