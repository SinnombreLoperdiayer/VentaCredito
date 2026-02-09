using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Transactions;
using CO.Servidor.ControlCuentas.Comun;
using CO.Servidor.Dominio.Comun.AdmEstadosGuia;
using CO.Servidor.Dominio.Comun.Admisiones;
using CO.Servidor.Dominio.Comun.Cajas;
using CO.Servidor.Dominio.Comun.Comisiones;
using CO.Servidor.Dominio.Comun.Facturacion;
using CO.Servidor.Dominio.Comun.Middleware;
using CO.Servidor.Dominio.Comun.OperacionUrbana;
using CO.Servidor.Dominio.Comun.Suministros;
using CO.Servidor.Dominio.Comun.Tarifas;
using CO.Servidor.Servicios.ContratoDatos.Admisiones.Mensajeria;
using CO.Servidor.Servicios.ContratoDatos.Cajas;
using CO.Servidor.Servicios.ContratoDatos.CentroServicios;
using CO.Servidor.Servicios.ContratoDatos.ControlCuentas;
using CO.Servidor.Servicios.ContratoDatos.OperacionUrbana;
using CO.Servidor.Servicios.ContratoDatos.Tarifas;
using CO.Servidor.Servicios.ContratoDatos.Tarifas.Precios;
using CO.Servidor.Servicios.ContratoDatos.Suministros;
using Framework.Servidor.Comun;
using Framework.Servidor.Excepciones;

namespace CO.Servidor.ControlCuentas
{
    public partial class CCControlCuentas
    {
        /// <summary>
        /// Adiciona una guía anulada. Se usa para la parte de anulación de una guía. Se espera uqe se pase el id del centro de servicio de origen y el número de la guía.
        /// </summary>
        /// <param name="guia"></param>
        public long AdicionarAdmisionAnulada(CCAnulacionGuiaMensajeriaDC anulacion)
        {
            long numGuia = anulacion.NumeroGuia;


            // TODO ID: Se debe Crear un Objeto ADGuia con (IdCentroServicioOrigen) y (NombreCentroServicioOrigen)
            SUPropietarioGuia PropietarioGuia = COFabricaDominio.Instancia.CrearInstancia<ISUFachadaSuministros>().ObtenerPropietarioGuia(anulacion.NumeroGuia);

            anulacion.Guia= new ADGuia();
            anulacion.Guia.NumeroGuia = anulacion.NumeroGuia;
            anulacion.Guia.IdCentroServicioOrigen = PropietarioGuia.CentroServicios.IdCentroServicio;
            anulacion.Guia.NombreCentroServicioOrigen = PropietarioGuia.CentroServicios.Nombre;


            using (TransactionScope tx = new TransactionScope())
            {
                long idAdmision = COFabricaDominio.Instancia.CrearInstancia<IADFachadaAdmisionesMensajeria>().AdicionarAdmisionAnulada(anulacion.Guia);

                // Crear Novedad
                // Grabar registro de novedad
                Dictionary<CCEnumNovedadRealizada, string> datosAdicionales = new Dictionary<CCEnumNovedadRealizada, string>();
                anulacion.Guia.IdAdmision = idAdmision;
                anulacion.TipoNovedad = CCEnumTipoNovedadGuia.AnulacionGuia;
                anulacion.IdModulo = COConstantesModulos.MODULO_CONTROL_CUENTAS;
                anulacion.QuienSolicita = string.Empty;
                anulacion.ResponsableNovedad = new CCResponsableCambioDC() { DescripcionResponsable = string.Empty };
                datosAdicionales.Add(CCEnumNovedadRealizada.AnulacionGuia, anulacion.MotivoAnulacion.IdMotivoAnulacion.ToString());
                COFabricaDominio.Instancia.CrearInstancia<IADFachadaAdmisionesMensajeria>().AdicionarNovedad(anulacion, datosAdicionales);
                tx.Complete();
            }
            return numGuia;
        }
    }
}