using System.Threading.Tasks;

namespace ServiPuntosUy.DataServices.Services.CommonLogic
{
    /// <summary>
    /// Interfaz para el servicio de envío de emails
    /// </summary>
    public interface IEmailService
    {
        /// <summary>
        /// Envía un email
        /// </summary>
        /// <param name="to">Email del destinatario</param>
        /// <param name="subject">Asunto del email</param>
        /// <param name="body">Cuerpo del email (HTML)</param>
        /// <returns>True si el email se envió correctamente</returns>
        Task<bool> SendEmailAsync(string to, string subject, string body);
    }
}
