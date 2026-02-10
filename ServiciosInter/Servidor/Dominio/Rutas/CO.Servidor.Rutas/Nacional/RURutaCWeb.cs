using CO.Servidor.Rutas.Datos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CO.Servidor.Servicios.ContratoDatos.Rutas;
using Framework.Servidor.Comun;
using Framework.Servidor.Excepciones;

namespace CO.Servidor.Rutas.Nacional
{
    public class RURutaCWeb
    {
        private static readonly RURutaCWeb instancia = (RURutaCWeb)FabricaInterceptores.GetProxy(new RURutaCWeb(), COConstantesModulos.MODULO_RUTAS);

        public static RURutaCWeb Instancia
        {
            get { return RURutaCWeb.instancia; }
        }
        /// <summary>
        /// constructor
        /// </summary>
        private RURutaCWeb() { }

        /// <summary>
        /// Obtiene información de la ruta y Coordenadas de centros de servicio de la ruta
        /// </summary>
        /// <param name="IdRuta">
        /// <returns>Onbjeto RURutaICWeb</returns>
        public List<RURutaICWeb> ObtenerRutaDetalle()
        {
            return RURepositorioCWeb.Instancia.ObtenerRuta();
        }
        /// <summary>
        /// obtiene centros de servicios de la ruta indicada
        /// </summary>
        /// <param name="idRuta"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public List<RURutaCWebDetalleCentrosServicios> ObtenerRutaDetalleCentroServiciosRuta(int idRuta, int id)
        {
            return RURepositorioCWeb.Instancia.ObtenerRutaDetalleCentroServiciosRuta(idRuta, id);
        }
        //public RURutaICWebDetalle obtenerRutaDetalleCentroServiciosRuta(int IdRuta)
        //{
        //    return RURepositorioCWeb.Instancia.obtenerRutaDetalleCentroServiciosRuta(IdRuta);
        //}
        /// <summary>
        /// agrega un punto a la ruta indicada
        /// </summary>
        /// <param name="datosPunto"></param>
        public void AgregarPtoRuta(PtoRuta datosPunto)
        {
            RURepositorioCWeb.Instancia.AgregarPtoRuta(datosPunto);
        }
        //public void AgregarRuta(string origen, string destino, string nombre, int tipoRuta, int medioTransporte, int generaManifiesto)
        //{
        //    RURepositorioCWeb.Instancia.AgregarRuta(origen, destino, nombre, tipoRuta, medioTransporte, generaManifiesto);
        //}
        /// <summary>
        /// elimina punto de ruta indicada
        /// </summary>
        /// <param name="datosPunto"></param>
        public void EliminarPtoRuta(PtoRuta datosPunto)
        {
            RURepositorioCWeb.Instancia.EliminarPtoRuta(datosPunto);
        }
        /// <summary>
        /// crear punto
        /// </summary>
        /// <param name="datosPunto"></param>
        public void CrearPunto(PtoRuta datosPunto)
        {
            RURepositorioCWeb.Instancia.CrearPunto(datosPunto);
        }
        /// <summary>
        /// asigna posicion en ruta a punto indicado
        /// </summary>
        /// <param name="datosPunto"></param>
        public void OrganizarPtos(PtoRuta datosPunto)
        {
            RURepositorioCWeb.Instancia.OrganizarPtos(datosPunto);
        }
        /// <summary>
        /// obtiene todos los medios de transporte
        /// </summary>
        /// <returns></returns>
        public List<RUMedioTransporte> ObtenerMediosTransporte()
        {
            return RURepositorioCWeb.Instancia.ObtenerMediosTransporte();
        }
        /// <summary>
        /// obtiene todos lod tipos de vehiculos
        /// </summary>
        /// <returns></returns>
        public List<RUTipoVehiculo> ObtenerTiposVehiculos()
        {
            return RURepositorioCWeb.Instancia.ObtenerTiposVehiculos();
        }
        /// <summary>
        /// obtiene todos los tipos de ruta
        /// </summary>
        /// <returns></returns>
        public List<RUTipoRuta> ObtenerTiposRuta()
        {
            return RURepositorioCWeb.Instancia.ObtenerTiposRuta();
        }
        /// <summary>
        /// crea nueva ruta
        /// </summary>
        /// <param name="ruta"></param>
        /// <returns></returns>
        public int CrearRuta(RURutaICWeb ruta)
        {
            return RURepositorioCWeb.Instancia.CrearRuta(ruta);
        }
    }
}
