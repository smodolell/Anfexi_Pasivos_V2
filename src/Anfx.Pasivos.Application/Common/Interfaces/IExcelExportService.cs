namespace Anfx.Pasivos.Application.Common.Interfaces;

public interface IExcelExportService
{
    /// <summary>
    /// Exporta una colección de objetos a un archivo Excel
    /// </summary>
    /// <typeparam name="T">Tipo de objeto a exportar</typeparam>
    /// <param name="data">Colección de datos a exportar</param>
    /// <param name="sheetName">Nombre de la hoja de Excel</param>
    /// <param name="fileName">Nombre del archivo (sin extensión)</param>
    /// <returns>Array de bytes del archivo Excel</returns>
    byte[] ExportToExcel<T>(IEnumerable<T> data, string sheetName = "Datos", string fileName = "export") where T : class;
}