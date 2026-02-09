using CO.Servidor.LogisticaInversa.Datos;
using CO.Servidor.Servicios.ContratoDatos.LogisticaInversa;
using Framework.Servidor.Comun;
using Framework.Servidor.Excepciones;
using Framework.Servidor.ParametrosFW;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;

namespace CO.Servidor.LogisticaInversa.PruebasEntrega.Descargue
{
    /// <summary>
    /// clase para el manejo de tapas de impresion
    /// </summary>
    public class LITapasLogisticaInversa : ControllerBase
    {
        #region Instancia

        private static readonly LITapasLogisticaInversa instancia = (LITapasLogisticaInversa)FabricaInterceptores.GetProxy(new LITapasLogisticaInversa(), COConstantesModulos.PRUEBAS_DE_ENTREGA);

        public static LITapasLogisticaInversa Instancia
        {
            get { return LITapasLogisticaInversa.instancia; }
        }

        public LITapasLogisticaInversa()
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo(PAAdministrador.Instancia.ConsultarParametrosFramework("Cultura"));
        }

        #endregion Instancia

        #region Métodos

        /// <summary>
        /// Método para adicionar una tapa de impresion
        /// </summary>
        /// <param name="tapaLogistica"></param>
        public void AdicionarTapaLogistica(LITapaLogisticaDC tapaLogistica) 
        {
            LIRepositorioPruebasEntrega.Instancia.AdicionarTapaLogistica(tapaLogistica);
        }

         /// <summary>
        /// Verifica si una guia tiene almenos una tapa por tipo tapa
        /// </summary>
        /// <param name="tapaLogistica"></param>
        /// <returns></returns>
        public bool VerificarGuiaConTapa(LITapaLogisticaDC tapaLogistica)
        {
           return LIRepositorioPruebasEntrega.Instancia.VerificarGuiaConTapa(tapaLogistica);
        }

        /// <summary>
        /// Verifica si un numero de guia esta en la tabla gestionAuditoria para imprimir la tapa
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <returns></returns>
        public bool VerificaGuiaConGestionAuditor(long numeroGuia)
        {
            return LIRepositorioPruebasEntrega.Instancia.VerificaGuiaConGestionAuditor(numeroGuia);
        }

        #endregion
    }
}
