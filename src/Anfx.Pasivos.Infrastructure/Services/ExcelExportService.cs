using Anfx.Pasivos.Application.Common.Interfaces;
using ClosedXML.Excel;
using System.ComponentModel;
using System.Reflection;

namespace Anfx.Pasivos.Infrastructure.Services;


public class ExcelExportService : IExcelExportService
{
    public byte[] ExportToExcel<T>(IEnumerable<T> data, string sheetName = "Datos", string fileName = "export") where T : class
    {
        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add(sheetName);

        if (!data.Any())
        {
            worksheet.Cell(1, 1).Value = "No hay datos para mostrar";
            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }

        // Obtener las propiedades del tipo T
        var properties = typeof(T).GetProperties()
            .Where(p => p.CanRead && !IsComplexType(p.PropertyType))
            .ToList();

        // Crear encabezados
        for (int i = 0; i < properties.Count; i++)
        {
            var property = properties[i];
            var displayName = GetDisplayName(property);
            worksheet.Cell(1, i + 1).Value = displayName;

            // Formatear encabezados
            worksheet.Cell(1, i + 1).Style.Font.Bold = true;
            worksheet.Cell(1, i + 1).Style.Fill.BackgroundColor = XLColor.LightGray;
        }

        // Llenar datos
        int row = 2;
        foreach (var item in data)
        {
            for (int col = 0; col < properties.Count; col++)
            {
                var property = properties[col];
                var value = property.GetValue(item);

                // Formatear el valor según el tipo
                var cellValue = FormatCellValue(value, property.PropertyType);
                worksheet.Cell(row, col + 1).Value = cellValue;
            }
            row++;
        }

        // Ajustar ancho de columnas automáticamente
        worksheet.ColumnsUsed().AdjustToContents();

        // Agregar bordes a toda la tabla
        var range = worksheet.Range(1, 1, row - 1, properties.Count);
        range.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
        range.Style.Border.InsideBorder = XLBorderStyleValues.Thin;

        using var memoryStream = new MemoryStream();
        workbook.SaveAs(memoryStream);
        return memoryStream.ToArray();
    }

    private static bool IsComplexType(Type type)
    {
        return type != typeof(string) &&
               type != typeof(DateTime) &&
               type != typeof(DateTime?) &&
               type != typeof(decimal) &&
               type != typeof(decimal?) &&
               type != typeof(double) &&
               type != typeof(double?) &&
               type != typeof(float) &&
               type != typeof(float?) &&
               type != typeof(int) &&
               type != typeof(int?) &&
               type != typeof(long) &&
               type != typeof(long?) &&
               type != typeof(bool) &&
               type != typeof(bool?) &&
               type != typeof(Guid) &&
               type != typeof(Guid?) &&
               !type.IsPrimitive &&
               !type.IsEnum;
    }

    private static string GetDisplayName(PropertyInfo property)
    {
        // Buscar DisplayName attribute
        var displayNameAttr = property.GetCustomAttribute<DisplayNameAttribute>();
        if (displayNameAttr != null)
            return displayNameAttr.DisplayName;

        // Buscar Display attribute
        var displayAttr = property.GetCustomAttribute<System.ComponentModel.DataAnnotations.DisplayAttribute>();
        if (displayAttr?.Name != null)
            return displayAttr.Name;

        // Usar el nombre de la propiedad
        return property.Name;
    }

    private static XLCellValue FormatCellValue(object? value, Type propertyType)
    {
        if (value == null)
            return string.Empty;

        // Formatear fechas
        if (propertyType == typeof(DateTime) || propertyType == typeof(DateTime?))
        {
            if (value is DateTime dateTime)
                return dateTime.ToString("dd/MM/yyyy HH:mm:ss");
        }

        // Formatear decimales
        if (propertyType == typeof(decimal) || propertyType == typeof(decimal?) ||
            propertyType == typeof(double) || propertyType == typeof(double?) ||
            propertyType == typeof(float) || propertyType == typeof(float?))
        {
            return Convert.ToDouble(value);
        }

        // Formatear enteros
        if (propertyType == typeof(int) || propertyType == typeof(int?) ||
            propertyType == typeof(long) || propertyType == typeof(long?) ||
            propertyType == typeof(short) || propertyType == typeof(short?))
        {
            return Convert.ToInt64(value);
        }

        // Formatear booleanos
        if (propertyType == typeof(bool) || propertyType == typeof(bool?))
        {
            return Convert.ToBoolean(value);
        }

        // Para strings y otros tipos, convertir a string
        return value?.ToString() ?? string.Empty;
    }
}
