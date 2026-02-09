using CO.Controller.Servidor.Integraciones.AccesoDatos.DALBelCorp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CO.Controller.Servidor.Integraciones.BelCorp
{
    public class IntegracionBelCorp
    {
        private static readonly IntegracionBelCorp instancia = new IntegracionBelCorp();

        public static IntegracionBelCorp Instancia
        {
            get
            {
                return instancia;
            }
        }

        private IntegracionBelCorp()
        {

        }


        /// <summary>
        /// Valida que la transaccion esté confirmada
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public bool VerificarTransaccionInventarioDevolucion(Guid token)
        {
            return RepositorioBelcorp.Instancia.VerificarTransaccionInventarioDevolucion(token);
        }
        /// <summary>
        /// Asocia el token con el numero de guia generado para cerrar la transaccion
        /// </summary>
        /// <param name="token"></param>
        /// <param name="numeroGuia"></param>
        public void ActualizarTransaccionInventario(Guid token, long numeroGuia)
        {
            RepositorioBelcorp.Instancia.ActualizarTransaccionInventario(token, numeroGuia);
        }


        }
}
