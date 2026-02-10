using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Framework.Servidor.Comun;
using Framework.Servidor.Excepciones;
using CO.Servidor.Servicios.ContratoDatos.Produccion;
using CO.Servidor.Produccion.Datos;
using System.Transactions;
using CO.Servidor.Servicios.ContratoDatos.Admisiones.Mensajeria;
using CO.Servidor.Servicios.ContratoDatos.Area;
using CO.Servidor.Servicios.ContratoDatos.CentroServicios;
using CO.Servidor.Dominio.Comun.Middleware;
using CO.Servidor.Dominio.Comun.CentroServicios;
using Framework.Servidor.Servicios.ContratoDatos.Parametros;
using CO.Servidor.Dominio.Comun.Admisiones;

namespace CO.Servidor.Produccion
{
    public class PRProduccion : ControllerBase
    {
        #region creacion Instancia

        private static readonly PRProduccion instancia = (PRProduccion)FabricaInterceptores.GetProxy(new PRProduccion(), COConstantesModulos.MODULO_PRODUCCION);

        /// <summary>
        /// Retorna una instancia de administracion de produccion
        /// /// </summary>
        public static PRProduccion Instancia
        {
            get { return PRProduccion.instancia; }
        }

        #endregion Creacion Instancia

        #region Métodos
        public void GuardarNovedad()
        {

        }

        public void EliminarNovedad()
        {

        }

        #region Administración de motivos de novedades
        public void GuardarMotivoNovedad(PRMotivoNovedadDC motivoNovedad)
        {
            PRRepositorio.Instancia.GuardarMotivoNovedad(motivoNovedad);
        }

        public List<PRMotivoNovedadDC> ConsultarMotivosNovedad()
        {
            return PRRepositorio.Instancia.ConsultarMotivosNovedad();
        }

        public void BorrarMotivoNovedad(PRMotivoNovedadDC motivoNovedad)
        {
            PRRepositorio.Instancia.BorrarMotivoNovedad(motivoNovedad);
        }
        #endregion


        #region Administración de retenciones
        public void GuardarValoresRetencion(PRRetencionProduccionDC retencion)
        {
            PRRepositorio.Instancia.GuardarValoresRetencion(retencion);
        }

        public List<PRRetencionProduccionDC> ConsultarValoresRetenciones()
        {
            return PRRepositorio.Instancia.ConsultarValoresRetenciones();
        }

        public void BorrarRetencion(PRRetencionProduccionDC retencion)
        {
            PRRepositorio.Instancia.BorrarRetencion(retencion);
        }

        public void GuardarRetencionXCiudad(PRRetencionXCiudadDC retencionXCiudad)
        {
            PRRepositorio.Instancia.GuardarRetencionXCiudad(retencionXCiudad);
        }

        public void BorrarRetencionXCiudad(PRRetencionXCiudadDC retencionXCiudad)
        {
            PRRepositorio.Instancia.BorrarRetencionXCiudad(retencionXCiudad);
        }

        public List<PRRetencionXCiudadDC> ConsultarRetencionesXCiudad()
        {
            return PRRepositorio.Instancia.ConsultarRetencionesXCiudad();
        }

        public List<PRRetencionDC> ConsultarTiposRetencion()
        {
            return PRRepositorio.Instancia.ConsultarTiposRetencion();
        }
        #endregion

        #region Administrar novedades
        public void GuardarNovedadesProduccion(List<PRNovedadProduccionDC> novedadesProduccion)
        {
            PRRepositorio.Instancia.GuardarNovedadesProduccion(novedadesProduccion);
        }

        public List<PRNovedadProduccionDC> ConsultarNovedadesNoCargadas(int ano, int mes, long idCentroServicios)
        {
            return PRRepositorio.Instancia.ConsultarNovedadesNoCargadas(ano, mes, idCentroServicios);
        }

        public void EliminarNovedad(long Idnovedad)
        {
            PRRepositorio.Instancia.EliminarNovedad(Idnovedad);
        }
        #endregion

        #region Liquidaciones

        public void GenerarLiquidacion(long idCentroServicio, int mes, int ano)
        {
            if (idCentroServicio == 0)
            {
                PRRepositorio.Instancia.GenerarLiquidacionTodos(mes, ano);
            }
            else
            {
                PRRepositorio.Instancia.GenerarLiquidacionCentroServicio(idCentroServicio, mes, ano);
            }
        }

        public void AprobarLiquidaciones(long idRacol, string idCiudad, long idCentroServicio, int mes, int ano, long idLiqDesde, long idLiqHasta)
        {

            PRRepositorio.Instancia.AprobarLiquidaciones(idRacol, idCiudad, idCentroServicio, mes, ano, idLiqDesde, idLiqHasta);
        }

        public void EliminarLiquidacionProduccion(long idLiqProduccion)
        {
            PRRepositorio.Instancia.EliminarLiquidacionProduccion(idLiqProduccion);
        }

        public void CargarLiquidacionEnCaja(int mes, int ano)
        {
            PRRepositorio.Instancia.CargarLiquidacionEnCaja(mes, ano);
        }

        /// <summary>
        /// Método para generar las guías internas
        /// </summary>
        /// <param name="filtro"></param>
        /// <returns></returns>
        public List<PRLiquidacionProduccionDC> GenerarGuiasLiquidaciones(long idRacol, string idCiudad, long idCentroServicio, int mes, int ano, long idLiqDesde, long idLiqHasta)
        {
            List<PRLiquidacionProduccionDC> listaLiquidaciones;

            listaLiquidaciones = PRRepositorio.Instancia.ConsultarLiquidacionProduccion(idRacol, idCiudad, idCentroServicio, mes, ano, idLiqDesde, idLiqHasta);

            foreach (PRLiquidacionProduccionDC liq in listaLiquidaciones)
            {
                if (liq.NumeroGuiaInterna == 0 && liq.IdEstadoLiquidacionProduccion == "APR")
                {
                    PUCentroServiciosDC CSDestino = COFabricaDominio.Instancia.CrearInstancia<IPUFachadaCentroServicios>().ObtenerCentroServicio(liq.IdCentroServicios);

                    using (TransactionScope transaccion = new TransactionScope())
                    {
                        if (liq.NumeroGuiaInterna == 0)
                        {
                            ADGuiaInternaDC GuiaInterna = new ADGuiaInternaDC
                            {
                                EsOrigenGestion = true,
                                GestionDestino = new ARGestionDC { IdGestion = 0, Descripcion = string.Empty },
                                GestionOrigen = new ARGestionDC { IdGestion = 3, Descripcion = "GESTION FINANCIERA", IdCasaMatriz = 1 },
                                DiceContener = "Liquidación de Producción No. " + liq.IdLiquidacionProduccion,
                                DireccionDestinatario = CSDestino.Direccion,
                                EsManual = false,
                                IdAdmisionGuia = 0,
                                LocalidadDestino = CSDestino.CiudadUbicacion,
                                NombreDestinatario = CSDestino.IdCentroServicio.ToString() + "-" + CSDestino.Nombre,
                                NumeroGuia = 0,
                                PaisDefault = new PALocalidadDC { IdLocalidad = ConstantesFramework.ID_LOCALIDAD_COLOMBIA, Nombre = ConstantesFramework.DESC_LOCALIDAD_COLOMBIA },
                                TelefonoDestinatario = CSDestino.Telefono1,
                            };
                            IADFachadaAdmisionesMensajeria fachadaAdmision = COFabricaDominio.Instancia.CrearInstancia<IADFachadaAdmisionesMensajeria>();
                            GuiaInterna = fachadaAdmision.AdicionarGuiaInterna(GuiaInterna);
                            liq.NumeroGuiaInterna = GuiaInterna.NumeroGuia;
                            PRRepositorio.Instancia.ActualizarNumeroGuiaEnLiquidacion(liq.IdLiquidacionProduccion, GuiaInterna.NumeroGuia);
                        }
                        transaccion.Complete();
                    }
                }
            }
            return listaLiquidaciones;
        }

        public List<PRLiquidacionProduccionDC> ConsultarLiquidacionProduccion(long idRacol, string idCiudad, long idCentroServicio, int mes, int ano, long idLiqDesde, long idLiqHasta)
        {
            return PRRepositorio.Instancia.ConsultarLiquidacionProduccion(idRacol, idCiudad, idCentroServicio, mes, ano,idLiqDesde,idLiqHasta);
        }
        #endregion
        #endregion
    }
}