using Microsoft.AspNetCore.Mvc;
using ServiPuntosUy.Controllers.Base;
using ServiPuntosUy.Controllers.Requests;
using ServiPuntosUY.Controllers.Response;
using ServiPuntosUy.DataServices;
using ServiPuntosUy.DataServices.Services;
using ServiPuntosUy.DTO;
using ServiPuntosUy.Enums;
using System.ComponentModel.DataAnnotations;

namespace ServiPuntosUy.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ServiceController : BaseController
    {
        private readonly IServiceManager _serviceManager;

        public ServiceController(IServiceFactory serviceFactory) : base(serviceFactory)
        {
            _serviceManager = serviceFactory.GetService<IServiceManager>();
        }

        [HttpGet]
        [Route("branch/{branchId}")]
        public async Task<IActionResult> GetBranchServices(int branchId)
        {
            try
            {
                // Obtener los servicios con sus disponibilidades
                var services = await _serviceManager.GetBranchServicesAsync(branchId);
                return Ok(new ApiResponse<ServiceDTO[]>
                {
                    Error = false,
                    Data = services,
                    Message = "Servicios obtenidos correctamente"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Error = true,
                    Message = ex.Message
                });
            }
        }

        [HttpGet]
        [Route("{serviceId}")]
        public async Task<IActionResult> GetServiceById(int serviceId)
        {
            try
            {
                // Obtener un servicio específico con sus disponibilidades
                var service = await _serviceManager.GetServiceByIdAsync(serviceId);
                return Ok(new ApiResponse<ServiceDTO>
                {
                    Error = false,
                    Data = service,
                    Message = "Servicio obtenido correctamente"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Error = true,
                    Message = ex.Message
                });
            }
        }

        [HttpPost]
        [Route("create")]
        public async Task<IActionResult> CreateService([FromBody] CreateServiceRequest request)
        {
            // Verificar que el usuario sea administrador de branch
            if (UserType != UserType.Branch)
                return Unauthorized(new ApiResponse<object>
                {
                    Error = true,
                    Message = "Solo los administradores de branch pueden crear servicios"
                });

            try
            {
                var service = await _serviceManager.CreateServiceAsync(
                    BranchId.Value,
                    request.Name,
                    request.Description,
                    request.Price,
                    request.AgeRestricted,
                    request.StartTime,
                    request.EndTime);

                return Ok(new ApiResponse<ServiceDTO>
                {
                    Error = false,
                    Data = service,
                    Message = "Servicio creado correctamente con su disponibilidad"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Error = true,
                    Message = ex.Message
                });
            }
        }

        [HttpPut]
        [Route("{serviceId}")]
        public async Task<IActionResult> UpdateService(int serviceId, [FromBody] UpdateServiceRequest request)
        {
            // Verificar que el usuario sea administrador de branch
            if (UserType != UserType.Branch)
                return Unauthorized(new ApiResponse<object>
                {
                    Error = true,
                    Message = "Solo los administradores de branch pueden actualizar servicios"
                });

            try
            {
                var service = await _serviceManager.UpdateServiceAsync(
                    serviceId,
                    request.Name,
                    request.Description,
                    request.Price,
                    request.AgeRestricted,
                    request.StartTime,
                    request.EndTime);

                return Ok(new ApiResponse<ServiceDTO>
                {
                    Error = false,
                    Data = service,
                    Message = "Servicio actualizado correctamente"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Error = true,
                    Message = ex.Message
                });
            }
        }

        [HttpDelete]
        [Route("{serviceId}")]
        public async Task<IActionResult> DeleteService(int serviceId)
        {
            // Verificar que el usuario sea administrador de branch
            if (UserType != UserType.Branch)
                return Unauthorized(new ApiResponse<object>
                {
                    Error = true,
                    Message = "Solo los administradores de branch pueden eliminar servicios"
                });

            try
            {
                var result = await _serviceManager.DeleteServiceAsync(serviceId);
                return Ok(new ApiResponse<bool>
                {
                    Error = false,
                    Data = result,
                    Message = "Servicio eliminado correctamente"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Error = true,
                    Message = ex.Message
                });
            }
        }

    }
}
