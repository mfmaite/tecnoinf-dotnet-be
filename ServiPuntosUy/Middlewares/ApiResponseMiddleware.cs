using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using ServiPuntosUY.Controllers.Response;

namespace ServiPuntosUy.Middlewares
{
    /// <summary>
    /// Middleware para envolver todas las respuestas en la estructura ApiResponse
    /// </summary>
    public class ApiResponseMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ApiResponseMiddleware> _logger;
        private readonly List<string> _excludedPaths;

        public ApiResponseMiddleware(RequestDelegate next, ILogger<ApiResponseMiddleware> logger)
        {
            _next = next;
            _logger = logger;
            
            // Rutas excluidas del procesamiento de ApiResponse
            _excludedPaths = new List<string>
            {
                "/",
                "/home",
                "/swagger",
                "/css",
                "/js",
                "/lib",
                "/favicon.ico"
            };
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Verificar si la ruta está excluida del procesamiento
            string path = context.Request.Path.Value.ToLower();
            
            if (_excludedPaths.Any(p => path.StartsWith(p)))
            {
                // Ruta excluida, continuar con el pipeline sin procesar
                await _next(context);
                return;
            }

            // Verificar si es una solicitud de API (basado en el Accept header o la ruta)
            bool isApiRequest = IsApiRequest(context);
            
            if (!isApiRequest)
            {
                // No es una solicitud de API, continuar con el pipeline sin procesar
                await _next(context);
                return;
            }

            // Guardar la referencia al stream de respuesta original
            var originalBodyStream = context.Response.Body;

            try
            {
                // Crear un nuevo stream en memoria para capturar la respuesta
                using var responseBody = new MemoryStream();
                context.Response.Body = responseBody;

                // Continuar con el pipeline y esperar a que se complete
                await _next(context);

                // Verificar si la respuesta es JSON
                if (!IsJsonResponse(context))
                {
                    // No es una respuesta JSON, copiar el stream sin procesar
                    responseBody.Seek(0, SeekOrigin.Begin);
                    await responseBody.CopyToAsync(originalBodyStream);
                    return;
                }

                // Leer la respuesta
                responseBody.Seek(0, SeekOrigin.Begin);
                var responseContent = await new StreamReader(responseBody).ReadToEndAsync();
                
                // Verificar si la respuesta ya está en formato ApiResponse
                bool isAlreadyApiResponse = IsApiResponseFormat(responseContent);

                // Preparar el nuevo contenido de la respuesta
                string newResponseContent;
                
                if (isAlreadyApiResponse)
                {
                    // Si ya es un ApiResponse, mantenerlo como está
                    newResponseContent = responseContent;
                }
                else
                {
                    // Si no es un ApiResponse, envolverlo en uno
                    newResponseContent = WrapInApiResponse(responseContent, context.Response.StatusCode);
                }

                // Configurar la respuesta con el nuevo contenido
                context.Response.ContentType = "application/json; charset=utf-8";
                responseBody.Seek(0, SeekOrigin.Begin);
                responseBody.SetLength(0);
                
                await new StreamWriter(responseBody).WriteAsync(newResponseContent);
                await responseBody.FlushAsync();

                // Copiar el stream modificado de vuelta al body original
                responseBody.Seek(0, SeekOrigin.Begin);
                await responseBody.CopyToAsync(originalBodyStream);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en ApiResponseMiddleware");
                
                // En caso de error en el middleware, asegurarse de restaurar el body original
                context.Response.Body = originalBodyStream;
                
                // Crear una respuesta de error
                context.Response.ContentType = "application/json; charset=utf-8";
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                
                var errorResponse = new ApiResponse<object>
                {
                    Error = true,
                    Message = "Error interno del servidor: " + ex.Message
                };
                
                await context.Response.WriteAsJsonAsync(errorResponse);
            }
        }

        /// <summary>
        /// Verifica si el contenido ya tiene formato de ApiResponse
        /// </summary>
        private bool IsApiResponseFormat(string content)
        {
            if (string.IsNullOrWhiteSpace(content))
                return false;

            try
            {
                // Intentar deserializar como un objeto JSON
                using var document = JsonDocument.Parse(content);
                var root = document.RootElement;

                // Verificar si tiene las propiedades de ApiResponse
                return root.TryGetProperty("error", out _) &&
                       root.TryGetProperty("data", out _) &&
                       root.TryGetProperty("message", out _);
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Envuelve el contenido en un ApiResponse según el código de estado
        /// </summary>
        private string WrapInApiResponse(string content, int statusCode)
        {
            try
            {
                // Si el contenido está vacío o no es JSON válido
                if (string.IsNullOrWhiteSpace(content) || !IsValidJson(content))
                {
                    var emptyResponse = new ApiResponse<object>
                    {
                        Error = statusCode >= 400,
                        Message = statusCode >= 400 ? GetDefaultErrorMessage(statusCode) : "Operación completada con éxito"
                    };
                    
                    return JsonSerializer.Serialize(emptyResponse);
                }

                // Si es un código de éxito (2xx)
                if (statusCode >= 200 && statusCode < 300)
                {
                    // Deserializar el contenido original
                    using var document = JsonDocument.Parse(content);
                    
                    // Crear un ApiResponse con el contenido como Data
                    var successResponse = new
                    {
                        error = false,
                        data = document.RootElement,
                        message = "Operación completada con éxito"
                    };
                    
                    return JsonSerializer.Serialize(successResponse);
                }
                else // Si es un código de error (4xx, 5xx)
                {
                    // Intentar extraer un mensaje de error del contenido
                    string errorMessage = ExtractErrorMessage(content);
                    
                    if (string.IsNullOrWhiteSpace(errorMessage))
                    {
                        errorMessage = GetDefaultErrorMessage(statusCode);
                    }
                    
                    var errorResponse = new ApiResponse<object>
                    {
                        Error = true,
                        Message = errorMessage
                    };
                    
                    return JsonSerializer.Serialize(errorResponse);
                }
            }
            catch (Exception ex)
            {
                // En caso de error al procesar, devolver un ApiResponse de error genérico
                var fallbackResponse = new ApiResponse<object>
                {
                    Error = true,
                    Message = "Error al procesar la respuesta: " + ex.Message
                };
                
                return JsonSerializer.Serialize(fallbackResponse);
            }
        }

        /// <summary>
        /// Verifica si una cadena es JSON válido
        /// </summary>
        private bool IsValidJson(string content)
        {
            try
            {
                JsonDocument.Parse(content);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Intenta extraer un mensaje de error del contenido JSON
        /// </summary>
        private string ExtractErrorMessage(string content)
        {
            try
            {
                using var document = JsonDocument.Parse(content);
                var root = document.RootElement;

                // Buscar propiedades comunes que podrían contener mensajes de error
                if (root.TryGetProperty("message", out var messageElement))
                {
                    return messageElement.GetString();
                }
                else if (root.TryGetProperty("error", out var errorElement) && errorElement.ValueKind == JsonValueKind.String)
                {
                    return errorElement.GetString();
                }

                return null;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Obtiene un mensaje de error predeterminado según el código de estado HTTP
        /// </summary>
        private string GetDefaultErrorMessage(int statusCode)
        {
            return statusCode switch
            {
                400 => "Solicitud incorrecta",
                401 => "No autorizado",
                403 => "Prohibido",
                404 => "Recurso no encontrado",
                500 => "Error interno del servidor",
                _ => $"Error con código de estado {statusCode}"
            };
        }

        /// <summary>
        /// Determina si la solicitud es una solicitud de API
        /// </summary>
        private bool IsApiRequest(HttpContext context)
        {
            // Verificar si la ruta comienza con /api
            if (context.Request.Path.Value.StartsWith("/api", StringComparison.OrdinalIgnoreCase))
                return true;

            // Verificar si el Accept header incluye application/json
            var acceptHeader = context.Request.Headers["Accept"].ToString();
            if (acceptHeader.Contains("application/json"))
                return true;

            // Verificar si el Content-Type header es application/json
            var contentTypeHeader = context.Request.Headers["Content-Type"].ToString();
            if (contentTypeHeader.Contains("application/json"))
                return true;

            return false;
        }

        /// <summary>
        /// Determina si la respuesta es JSON
        /// </summary>
        private bool IsJsonResponse(HttpContext context)
        {
            var contentType = context.Response.ContentType;
            return contentType != null && contentType.Contains("application/json");
        }
    }
}
