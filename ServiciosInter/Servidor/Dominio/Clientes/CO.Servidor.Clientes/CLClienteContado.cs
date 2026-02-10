using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using CO.Servidor.Clientes.Datos;
using CO.Servidor.Servicios.ContratoDatos.Clientes;
using Framework.Servidor.Comun;
using Framework.Servidor.Comun.Util;
using Framework.Servidor.Excepciones;
using Framework.Servidor.ParametrosFW;

namespace CO.Servidor.Clientes
{
    /// <summary>
    /// Clase para administrar las opciones de un cliente contado.
    /// </summary>
    internal class CLClienteContado : ControllerBase
    {
        /// <summary>
        /// Instancia de la clase
        /// </summary>
        private static readonly CLClienteContado instancia = (CLClienteContado)FabricaInterceptores.GetProxy(new CLClienteContado(), COConstantesModulos.CLIENTES);

        /// <summary>
        /// Instancia de la clase
        /// </summary>
        public static CLClienteContado Instancia
        {
            get { return CLClienteContado.instancia; }
        }

        /// <summary>
        /// Consulta la informacion de un cliente Contado a partir de un tipo de documento y un numero de documento
        /// </summary>
        /// <param name="tipoDocumento">Tipo de documento del cliente a consultar</param>
        /// <param name="numeroDocumento">Numéro del documento del cliente a consultar </param>
        /// <param name="ConDestinatariosFrecuentes"> Indica si la consulta traera los destinatarios frecuentes del cliente contado</param>
        /// <returns>Cliente Contado</returns>
        public CLClienteContadoDC ConsultarClienteContado(string tipoDocumento, string numeroDocumento, bool conDestinatariosFrecuentes, string idMunicipioDestino)
        {
            CLClienteContadoDC clienteContado;

            if (PAAdministrador.Instancia.ValidarListaRestrictiva(numeroDocumento))
            {
                throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.CLIENTES, ETipoErrorFramework.EX_ALERTA_LISTAS_RESTRICTIVAS.ToString(), MensajesFramework.CargarMensaje(ETipoErrorFramework.EX_ALERTA_LISTAS_RESTRICTIVAS)));
            }

            clienteContado = CLRepositorio.Instancia.ConsultarClienteContado(tipoDocumento, numeroDocumento);

            if (clienteContado != null && conDestinatariosFrecuentes)
            {
              clienteContado.DestinatariosFrecuentes = ConsultarDestinatarioFrecuente(tipoDocumento, numeroDocumento, idMunicipioDestino);
            }
            else if (clienteContado == null)
            {
              clienteContado = new CLClienteContadoDC()
              {
                TipoId = tipoDocumento,
                Identificacion = numeroDocumento
              };
            }
            return clienteContado;
        }

        /// <summary>
        /// Enviar correo electronico cuando un cliente se encuentre en listas restrictivas
        /// </summary>
        /// <param name="identificacion"></param>
        /// <param name="idCentroServicios"></param>
        /// <param name="nombreCentroServicios"></param>
        public void EnviarCorreoListasRestrictivas(string identificacion, long idCentroServicios, string nombreCentroServicios)
        {
            Task.Factory.StartNew(() =>
            {
                try
                {
                    PAAdministrador.Instancia.EnviarCorreoListasRestrictivas(identificacion, idCentroServicios, nombreCentroServicios);
                }

                catch (Exception ex)
                {
                    AuditoriaTrace.EscribirAuditoria(ETipoAuditoria.Error, ex.ToString(), COConstantesModulos.GIROS);
                }
            }, TaskCreationOptions.PreferFairness);
        }

        /// <summary>
        /// Consulta los destinatarios frecuentes de un cliente
        /// </summary>
        /// <param name="tipoDocumento">Tipo de documento del cliente</param>
        /// <param name="numeroDocumento">Numero de documento del cliente</param>
        /// <returns>Lista con los ultimos 3 destinatarios frecuentes.</returns>
        internal IList<CLDestinatarioFrecuenteDC> ConsultarDestinatarioFrecuente(string tipoDocumento, string numeroDocumento, string idMunicipioDestino)
        {
          return CLRepositorio.Instancia.ConsultarDestinatarioFrecuente(tipoDocumento, numeroDocumento, idMunicipioDestino);
        }

        /// <summary>
        /// Adiciona el cliente remitente y destinatario, adiciona los destinatarios frecuentes
        /// y el acumulado a cada clilente
        /// </summary>
        /// <param name="clienteContado"></param>
        /// <param name="valorGiro"></param>
        public decimal AdmGuardarClienteContado(CLClienteContadoDC clienteContadoRemitente, CLClienteContadoDC clienteContadoDestinatario, decimal valorGiro, string descUltimoCentroServDestino, long idUltimoCentroServDestino)
        {
            return CLRepositorio.Instancia.AdmGuardarClienteContado(clienteContadoRemitente, clienteContadoDestinatario, valorGiro, descUltimoCentroServDestino, idUltimoCentroServDestino);
        }

        /// <summary>
        /// Almacenar a un cliente y acumular los valores
        /// </summary>
        /// <param name="clienteContadoRemitente"></param>
        /// <param name="?"></param>
        /// <returns></returns>
        public decimal GuardarClienteAcumularValores(CLClienteContadoDC clienteContado, decimal valorGiro)
        {
            return CLRepositorio.Instancia.GuardarClienteAcumularValores(clienteContado, valorGiro);
        }

        /// <summary>
        /// Adiciona el acumulado de pagos dinero al cliente
        /// </summary>
        /// <param name="clienteContado"></param>
        /// <returns>Valor acumulado de los pagos</returns>
        public decimal AcumuladoPagos(CLClienteContadoDC clienteContado, decimal valorPago)
        {
            return CLRepositorio.Instancia.AcumuladoPagos(clienteContado, valorPago);
        }

        /// <summary>
        /// Consultar la ultima cedula escaneada
        /// </summary>
        /// <returns> archivo del cliente</returns>
        public string ConsultarDocumentoCliente(string tipoId, string identificacion)
        {
            return CLRepositorio.Instancia.ConsultarDocumentoCliente(tipoId, identificacion);
        }

        /// <summary>
        /// Almacenar la cedula del cliente que reclama el giro
        /// </summary>
        ///<param name="pagosGiros">informacion del pago</param>
        public void AlmacenarCedulaCliente(CLClienteContadoDC clienteContado, string archivoCedulaClientePago)
        {
            CLRepositorio.Instancia.AlmacenarCedulaCliente(clienteContado, archivoCedulaClientePago);
        }

        /// <summary>
        /// Registra destinatarios frecuentes
        /// </summary>
        /// <param name="clienteContadoRemitente"></param>
        /// <param name="clienteContadoDestinatario"></param>
        /// <param name="descUltimoCentroServDestino"></param>
        /// <param name="idUltimoCentroServDestino"></param>
        internal void RegistrarClienteContado(CLClienteContadoDC clienteContadoRemitente, CLClienteContadoDC clienteContadoDestinatario, string descUltimoCentroServDestino, long idUltimoCentroServDestino, string usuarioCreacion)
        {
            CLRepositorio.Instancia.RegistrarClienteContado(clienteContadoRemitente, clienteContadoDestinatario, descUltimoCentroServDestino, idUltimoCentroServDestino, usuarioCreacion);
        }
    }
}