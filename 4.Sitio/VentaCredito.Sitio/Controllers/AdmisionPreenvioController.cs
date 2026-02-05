using System.Web.Http;
using VentaCredito.Transversal.Entidades.AdmisionPreenvio;
using CustomException;
using VentaCredito.Sitio.Seguridad;
using System.Collections.Generic;

namespace VentaCredito.Sitio.Controllers
{
    [RoutePrefix("api/Admision")]
    public class AdmisionPreenvioController : ApiController
    {
        private readonly AdmisionPreenvio.AdmisionPreenvioNegocio instanciaPreenvioNegocio = AdmisionPreenvio.AdmisionPreenvioNegocio.Instancia;

        [HttpPost]
        [AdministradorSeguridad]
        [Route("InsertarAdmision")]
        [LogExceptionFilter]
        public ResponsePreAdmisionWrapper InsertarAdmision(RequestPreAdmisionWrapperCV admision)
        {
            return instanciaPreenvioNegocio.InsertarAdmision(admision);
        }

        /// <summary>
        /// Inserta un preenvio realizado desde el portal de clientes Express
        /// </summary>
        /// <param name="admisionPortalCli"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("InsertarAdmisionPortalCli")]
        [LogExceptionFilter]
        public IHttpActionResult InsertarAdmisionPortalCli(RequestPreAdmisionPortalCli admisionPortalCli)
        {
            return Ok(instanciaPreenvioNegocio.InsertarAdmisionPortalCli(admisionPortalCli));
        }

        [HttpGet]
        //[Authorize]
        [AdministradorSeguridad]
        [Route("ObtenerBase64PdfPreGuia/{numeroGuia}")]
        [LogExceptionFilter]
        public IHttpActionResult ObtenerBase64PdfPreGuia(long numeroGuia)
        {
            return Ok(instanciaPreenvioNegocio.ObtenerPdfGuia(numeroGuia));
        }

        [HttpGet]
        //[Authorize]
        [AdministradorSeguridad]
        [Route("ObtenerBase64PdfPreGuiaFormatoPeq/{numeroGuia}")]
        [LogExceptionFilter]
        public IHttpActionResult ObtenerBase64PdfPreGuiaFormatoPeq(long numeroGuia)
        {
            return Ok(instanciaPreenvioNegocio.ObtenerPdfGuiaFormatoPeq(numeroGuia));
        }

        /// <summary>
        /// Obtiene el arreglo de bytes de los formatos de guía de una lista de preenvios
        /// </summary>
        /// <param name="requestImpresionPreguia"></param>
        /// <returns></returns>
        [HttpPost]
        [AdministradorSeguridad]
        [Route("ObtenerBase64PdfPreguias")]
        [LogExceptionFilter]
        public IHttpActionResult ObtenerBase64PdfPreguias(RequestImpresionPreguia requestImpresionPreguias)
        {
            return Ok(instanciaPreenvioNegocio.ObtenerByArrGuias(requestImpresionPreguias));
        }


        /// <summary>
        /// Obtiene todos los estados que puede tener una guia
        /// Hevelin Dayana Diaz - 05/10/2021
        /// </summary>
        /// <returns>Lista de estados mensajeria</returns>
        [HttpGet]
        [Route("ObtenerEstadosGuias")]
        [LogExceptionFilter]
        public List<EstadoGuia_MEN> ObtenerEstadosLogisticosGuias()
        {
            return instanciaPreenvioNegocio.ObtenerEstadosLogisticosGuias();
        }

        /// <summary>
        /// Meotodo que obtiene base64 del formato de un preenvio para cliente express
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("ObtenerBase64PdfPreGuiaCLExpress/{numeroGuia}")]
        [LogExceptionFilter]
        public IHttpActionResult ObtenerBase64PdfPreGuiaCLExpress(long numeroGuia)
        {
            return Ok(instanciaPreenvioNegocio.ObtenerPdfGuia(numeroGuia));
        }
    }
}
