using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Servicio.Entidades.Admisiones.Mensajeria;

namespace VentaCredito.Datos.Repositorio
{
    public class MensajeriaConvenioConvenioRepositorio
    {
        private static MensajeriaConvenioConvenioRepositorio instancia = new MensajeriaConvenioConvenioRepositorio();

        public static MensajeriaConvenioConvenioRepositorio Instancia { get { return instancia; } }

        /// <summary>
        /// Inserta la información de convenio - convenio
        /// </summary>
        /// <param name="convenio"></param>
        /// <param name="usuario"></param>
        public void AdicionarConvenioConvenio(long idAdmisionesMensajeria, ADMensajeriaTipoCliente convenio, string usuario, int idCliente)
        {
            //using (EntidadesAdmisionesMensajeria contexto = new EntidadesAdmisionesMensajeria(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            //{
            //    contexto.paCrearAdmConvenioConvenio_MEN(idAdmisionesMensajeria, convenio.FacturaRemitente, idCliente, convenio.ConvenioRemitente.Nit,
            //      convenio.ConvenioRemitente.RazonSocial, convenio.IdContratoConvenioRemitente, convenio.ConvenioDestinatario.Id, convenio.ConvenioDestinatario.Nit, convenio.ConvenioDestinatario.RazonSocial,
            //      convenio.ConvenioDestinatario.Telefono, convenio.ConvenioDestinatario.Direccion, convenio.ConvenioDestinatario.EMail, convenio.ConvenioRemitente.IdSucursalRecogida, usuario);
            //}
        }

        /// <summary>
        /// Inserta la información de un envío convenio - peatón
        /// </summary>
        /// <param name="idAdmisionesMensajeria"></param>
        /// <param name="convenioPeaton"></param>
        /// <param name="usuario"></param>
        public void AdicionarConvenioPeaton(long idAdmisionesMensajeria, ADMensajeriaTipoCliente convenioPeaton, string usuario, int idCliente)
        {
            //using (EntidadesAdmisionesMensajeria contexto = new EntidadesAdmisionesMensajeria(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            //{
            //    contexto.paCrearAdmiConvenioPeaton_MEN(idAdmisionesMensajeria, idCliente, convenioPeaton.ConvenioRemitente.Nit, convenioPeaton.ConvenioRemitente.RazonSocial,
            //      convenioPeaton.IdContratoConvenioRemitente, convenioPeaton.PeatonDestinatario.TipoIdentificacion, convenioPeaton.PeatonDestinatario.Identificacion,
            //      convenioPeaton.PeatonDestinatario.Nombre, convenioPeaton.PeatonDestinatario.Apellido1, convenioPeaton.PeatonDestinatario.Apellido2, convenioPeaton.PeatonDestinatario.Telefono,
            //      convenioPeaton.PeatonDestinatario.Direccion, convenioPeaton.PeatonDestinatario.Email, convenioPeaton.ConvenioRemitente.IdSucursalRecogida, usuario);
            //}
        }

        /// <summary>
        /// Inserta la información de un envío peatón - convenio
        /// </summary>
        /// <param name="idAdmisionesMensajeria"></param>
        /// <param name="peatonConvenio"></param>
        /// <param name="usuario"></param>
        public void AdicionarPeatonConvenio(int idCliente, long idAdmisionesMensajeria, ADMensajeriaTipoCliente peatonConvenio, string usuario)
        {
            //using (EntidadesAdmisionesMensajeria contexto = new EntidadesAdmisionesMensajeria(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            //{
            //    contexto.paCrearAdmiPeatonConvenio_MEN(idAdmisionesMensajeria, idCliente, peatonConvenio.ConvenioDestinatario.Nit, peatonConvenio.ConvenioDestinatario.RazonSocial,
            //      peatonConvenio.PeatonRemitente.TipoIdentificacion, peatonConvenio.PeatonRemitente.Identificacion, peatonConvenio.PeatonRemitente.Nombre, peatonConvenio.PeatonRemitente.Apellido1,
            //      peatonConvenio.PeatonRemitente.Apellido2, peatonConvenio.PeatonRemitente.Telefono, peatonConvenio.PeatonRemitente.Direccion, peatonConvenio.ConvenioDestinatario.IdSucursalRecogida,
            //      peatonConvenio.ConvenioDestinatario.Contrato, peatonConvenio.PeatonRemitente.Email, usuario);
            //}
        }

        /// <summary>
        /// Inserta la información sde peatón - peatón
        /// </summary>
        /// <param name="peatonPeaton"></param>
        /// <param name="usuario"></param>
        public void AdicionarPeatonPeaton(long idAdmisionesMensajeria, ADMensajeriaTipoCliente peatonPeaton, string usuario, long idCentroServicioOrigen, string nombreCentroServicioOrigen)
        {
            //using (EntidadesAdmisionesMensajeria contexto = new EntidadesAdmisionesMensajeria(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            //{
            //    contexto.paCrearAdmiPeatonPeaton_MEN(idAdmisionesMensajeria, idCentroServicioOrigen, nombreCentroServicioOrigen, peatonPeaton.PeatonRemitente.TipoIdentificacion,
            //      peatonPeaton.PeatonRemitente.TipoIdentificacion, peatonPeaton.PeatonRemitente.Nombre, peatonPeaton.PeatonRemitente.Apellido1, peatonPeaton.PeatonRemitente.Apellido2,
            //      peatonPeaton.PeatonRemitente.Telefono, peatonPeaton.PeatonRemitente.Direccion, peatonPeaton.PeatonRemitente.Email, peatonPeaton.PeatonDestinatario.TipoIdentificacion,
            //      peatonPeaton.PeatonDestinatario.Identificacion, peatonPeaton.PeatonDestinatario.Nombre, peatonPeaton.PeatonDestinatario.Apellido1, peatonPeaton.PeatonDestinatario.Apellido2,
            //      peatonPeaton.PeatonDestinatario.Telefono, peatonPeaton.PeatonDestinatario.Direccion, peatonPeaton.PeatonDestinatario.Email, usuario);
            //}
        }
    }
}
