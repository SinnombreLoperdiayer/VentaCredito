using Servicio.Entidades.Clientes;
using Servicio.Entidades.Comisiones;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VentaCredito.Clientes.Datos.Repositorio;
//using Servicio.Entidades.Clientes;
//using VentaCredito.Clientes.Datos.Repositorio;
//using Servicio.Entidades.Comisiones;

namespace VentaCredito.Clientes
{
    public class ClienteContado
    {
        private static ClienteContado instancia = new ClienteContado();

        public static ClienteContado Instancia
        {
            get
            {
                return instancia;
            }
        }

        /// <summary>
        /// Registra destinatarios frecuentes
        /// </summary>
        /// <param name="clienteContadoRemitente"></param>
        /// <param name="clienteContadoDestinatario"></param>
        /// <param name="descUltimoCentroServDestino"></param>
        /// <param name="idUltimoCentroServDestino"></param>
        public void RegistrarClienteContado(CLClienteContadoDC clienteContadoRemitente, CLClienteContadoDC clienteContadoDestinatario, string descUltimoCentroServDestino, long idUltimoCentroServDestino, string usuarioCreacion)
        {
            if (clienteContadoRemitente.Identificacion != null)
            {
                long? idClienteRemitente = CLClienteContadoRepositorio.Instancia.AdicionarClienteContado(clienteContadoRemitente, usuarioCreacion);

                if (idClienteRemitente != null)
                    clienteContadoRemitente.IdClienteContado = idClienteRemitente.GetValueOrDefault();
            }

            if (!string.IsNullOrWhiteSpace(clienteContadoDestinatario.Identificacion) && clienteContadoDestinatario.Identificacion != "0")
            {
                long? idClienteDestinatario = CLClienteContadoRepositorio.Instancia.AdicionarClienteContado(clienteContadoDestinatario, usuarioCreacion);

                if (idClienteDestinatario != null)
                    clienteContadoDestinatario.IdClienteContado = idClienteDestinatario.GetValueOrDefault();
            }

            if (!string.IsNullOrWhiteSpace(clienteContadoDestinatario.Identificacion) && !string.IsNullOrWhiteSpace(clienteContadoRemitente.Identificacion) && clienteContadoDestinatario.Identificacion != "0")
            {
                CLClienteContadoRepositorio.Instancia.AdicionarDestinatariosFrecuentes(clienteContadoRemitente, clienteContadoDestinatario, descUltimoCentroServDestino, idUltimoCentroServDestino, usuarioCreacion);
            }
        }


        /// <summary>
        /// Calcula las comisiones por ventas
        /// de un punto, su responsable y de una Agencia.
        /// </summary>
        /// <param name="consulta">Parametros de entrada de idCentroServicio-TipoServicio.</param>
        /// <returns>Las Comisiones del Punto y su responsable - Agencia </returns>
        public CMComisionXVentaCalculadaDC CalcularComisionesxVentas(CMConsultaComisionVenta consulta)
        {
            //decimal TotalCentroServiciosComision;
            //decimal TotalResponsableCentroServiciosComision;
            //decimal porcentajeCentroServicio;
            //decimal porcentajeCentroServicioResponsable;

            CMComisionXVentaCalculadaDC ComisionVenta = new CMComisionXVentaCalculadaDC();
            ComisionVenta.BaseComision = consulta.ValorBaseComision;
            ComisionVenta.NumeroOperacion = consulta.NumeroOperacion;

            return ComisionVenta;
        }


    }
}
