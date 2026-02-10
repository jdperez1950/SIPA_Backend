namespace Pavis.Application.DTOs.Common;

/// <summary>
/// Estructura estándar de respuesta de la API
/// </summary>
/// <typeparam name="T">Tipo de datos de la respuesta</typeparam>
public class ApiResponse<T>
{
    /// <summary>
    /// Indica si la operación fue exitosa
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Mensaje descriptivo de la operación
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Datos de la respuesta
    /// </summary>
    public T? Data { get; set; }

    /// <summary>
    /// Lista de errores en caso de fallo
    /// </summary>
    public List<string> Errors { get; set; } = new();

    /// <summary>
    /// Crea una respuesta exitosa
    /// </summary>
    public static ApiResponse<T> Ok(T data, string message = "Operación exitosa")
    {
        return new ApiResponse<T> { Success = true, Message = message, Data = data };
    }

    /// <summary>
    /// Crea una respuesta de error
    /// </summary>
    public static ApiResponse<T> Fail(string message, List<string>? errors = null)
    {
        return new ApiResponse<T> { Success = false, Message = message, Errors = errors ?? new List<string>() };
    }
}
