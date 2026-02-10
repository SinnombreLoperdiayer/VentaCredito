using CO.Servidor.Dominio.Comun.Middleware;
using CO.Servidor.Dominio.Comun.OperacionUrbana;
using CO.Servidor.Servicios.ContratoDatos.OperacionUrbana;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Framework.Servidor.ServicioNotificador
{
    public class COServicioNotificador
    {
        private static readonly COServicioNotificador instancia = new COServicioNotificador();

        public static COServicioNotificador Instancia
        {
            get { return instancia; }
        }

        private COServicioNotificador()
        {

        }

        public OURecogidasDC ObtenerRecogida(string idRecogida)
        {
            var fachada = COFabricaDominio.Instancia.CrearInstancia<IOUFachadaOperacionUrbana>();
            var recogida  = fachada.ObtenerRecogidaPeaton(long.Parse(idRecogida));
            return recogida;
        }
       

    }
}
