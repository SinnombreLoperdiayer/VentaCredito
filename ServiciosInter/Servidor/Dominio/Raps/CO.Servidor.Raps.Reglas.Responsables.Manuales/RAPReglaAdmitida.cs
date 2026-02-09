
using CO.Servidor.Dominio.Comun.CentroServicios;
using CO.Servidor.Dominio.Comun.Middleware;
using CO.Servidor.Dominio.Comun.OperacionNacional;
using CO.Servidor.Dominio.Comun.OperacionUrbana;
using CO.Servidor.Dominio.Comun.Suministros;
using CO.Servidor.Raps.Comun;
using CO.Servidor.Servicios.ContratoDatos.Admisiones.Mensajeria;
using CO.Servidor.Servicios.ContratoDatos.CentroServicios;
using CO.Servidor.Servicios.ContratoDatos.OperacionUrbana;
using CO.Servidor.Servicios.ContratoDatos.Raps.Configuracion;
using CO.Servidor.Servicios.ContratoDatos.Suministros;
using Framework.Servidor.Comun;
using Framework.Servidor.Comun.Reglas;
using Framework.Servidor.Excepciones;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace CO.Servidor.RAPS.Reglas.ResponsablesManuales
{
    public class RAPReglaAdmitida : IReglaIntegraciones
    {
        private IOUFachadaOperacionUrbana fachadaOperacionUrbana = COFabricaDominio.Instancia.CrearInstancia<IOUFachadaOperacionUrbana>();
        private IONFachadaOperacionNacional fachadaOperacionNacional = COFabricaDominio.Instancia.CrearInstancia<IONFachadaOperacionNacional>();
        private IPUFachadaCentroServicios fachadaCentroServicio = COFabricaDominio.Instancia.CrearInstancia<IPUFachadaCentroServicios>();
        private ISUFachadaSuministros fachadaSuministros = COFabricaDominio.Instancia.CrearInstancia<ISUFachadaSuministros>();

        public RAResponsableDC ObtenerResponsableNovedadRaps(IDictionary<string, object> parametrosRegla)
        {
            ADGuia datosGuia = new ADGuia();
            RAResponsableDC responsable = null;


            if (parametrosRegla.ContainsKey("guia"))
            {

                datosGuia = (ADGuia)(parametrosRegla["guia"]);


                if (datosGuia.EsAutomatico)
                {
                    /********************* ES AUTOMATICA *************************/

                    if (datosGuia.IdMensajero != 0)
                    {
                        /****************************** EMITIDA DESDE APP ********************************/

                        OUDatosMensajeroDC datosMensajero = fachadaOperacionUrbana.ObtenerDatosMensajero(datosGuia.IdMensajero);

                        if (datosMensajero == null)
                        {
                            throw new FaultException<ControllerException>(new ControllerException(Framework.Servidor.Comun.COConstantesModulos.MODULO_RAPS, RAEnumTipoErrorClientes.EX_CONSULTA_SOLICITUD.ToString(), RAMensajesRaps.CargarMensaje(RAEnumTipoErrorClientes.EX_CONSULTA_DATOS_MENSAJERO)));
                        }

                        responsable = RAMapeoResponsable.MapeoResponsable((int)RAEnumOrigenRaps.Mensajero.GetHashCode(), datosMensajero.IdCentroServicios, datosMensajero.Identificacion, string.Format("{0} {1}", datosMensajero.NombreMensajero, datosMensajero.PrimerApellido));
                    }

                    else
                    {
                        /****************************** EMITIDA DESDE OTROS SISTEMAS ********************************/

                        PUCentroServiciosDC centroServicio = fachadaCentroServicio.ObtenerTipoYResponsableCentroServicio(datosGuia.IdCentroServicioOrigen);

                        if (centroServicio == null)
                        {
                            throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.MODULO_RAPS, RAEnumTipoErrorClientes.EX_CONSULTA_SOLICITUD.ToString(), RAMensajesRaps.CargarMensaje(RAEnumTipoErrorClientes.EX_CONSULTA_RESPONSABLE_CENTRO_SERVICIO)));
                        }

                        if (centroServicio.Tipo == PUEnumTipoCentroServicioDC.AGE.ToString())
                        {
                            responsable = RAMapeoResponsable.MapeoResponsable((int)RAEnumOrigenRaps.Agencias.GetHashCode(), centroServicio.IdCentroServicio, centroServicio.infoResponsable.IdResponsable.ToString(), centroServicio.Nombre);
                        }
                        else if (centroServicio.Tipo == PUEnumTipoCentroServicioDC.PTO.ToString())
                        {
                            responsable = RAMapeoResponsable.MapeoResponsable((int)RAEnumOrigenRaps.Puntos.GetHashCode(), centroServicio.IdCentroServicio, centroServicio.infoResponsable.IdResponsable.ToString(), centroServicio.Nombre);
                        }
                        else
                        {
                            throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.MODULO_RAPS, RAEnumTipoErrorClientes.EX_CONSULTA_SOLICITUD.ToString(), RAMensajesRaps.CargarMensaje(RAEnumTipoErrorClientes.EX_RESPONSABLE_DIFERENTE_AGE_PTO)));
                        }

                    }
                }
                else
                {
                    /********************* ES MANUAL *************************/

                    SUDatosResponsableSuministroDC datosSuministros = fachadaSuministros.ObtenerResponsableSuministro(datosGuia.NumeroGuia);

                    if (datosSuministros == null)
                    {
                        throw new FaultException<ControllerException>(new ControllerException(Framework.Servidor.Comun.COConstantesModulos.MODULO_RAPS, RAEnumTipoErrorClientes.EX_CONSULTA_SOLICITUD.ToString(), RAMensajesRaps.CargarMensaje(RAEnumTipoErrorClientes.EX_CONSULTA_RESPONSABLE_CENTRO_SERVICIO)));
                    }

                    if (datosSuministros.TipoCentroServicios == PUEnumTipoCentroServicioDC.AGE.ToString())
                    {
                        responsable = RAMapeoResponsable.MapeoResponsable((int)RAEnumOrigenRaps.Agencias.GetHashCode(), datosSuministros.IdCentroServicios, datosSuministros.Identificacion.ToString(), string.Format("{0} {0}", datosSuministros.NombreResponsable, datosSuministros.PrimerApellido));
                    }
                    else if (datosSuministros.TipoCentroServicios == PUEnumTipoCentroServicioDC.PTO.ToString())
                    {
                        responsable = RAMapeoResponsable.MapeoResponsable((int)RAEnumOrigenRaps.Puntos.GetHashCode(), datosSuministros.IdCentroServicios, datosSuministros.Identificacion.ToString(), string.Format("{0} {0}", datosSuministros.NombreResponsable, datosSuministros.PrimerApellido));
                    }
                    else if (String.IsNullOrEmpty(datosSuministros.TipoCentroServicios))
                    {
                        responsable = RAMapeoResponsable.MapeoResponsable((int)RAEnumOrigenRaps.Mensajero.GetHashCode(), datosSuministros.IdCentroServicios, datosSuministros.Identificacion.ToString(), string.Format("{0} {0}", datosSuministros.NombreResponsable, datosSuministros.PrimerApellido));
                    }
                }
            }
            return responsable;
        }


    }
}
