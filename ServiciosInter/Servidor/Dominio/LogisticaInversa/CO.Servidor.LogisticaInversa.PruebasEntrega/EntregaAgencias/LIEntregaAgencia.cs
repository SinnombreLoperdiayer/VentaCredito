using Framework.Servidor.Comun;
using Framework.Servidor.Excepciones;
using Framework.Servidor.ParametrosFW;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using CO.Servidor.Servicios.ContratoDatos.OperacionUrbana;

namespace CO.Servidor.LogisticaInversa.EntregaAgencias.DescargueAgencias
{
    public class LIEntregaAgencia :ControllerBase
    {
        #region Instancia

        private static readonly LIEntregaAgencia instancia = (LIEntregaAgencia)FabricaInterceptores.GetProxy(new LIEntregaAgencia(), COConstantesModulos.PRUEBAS_DE_ENTREGA);

        public static LIEntregaAgencia Instancia
        {
            get { return LIEntregaAgencia.instancia; }
        }

        public LIEntregaAgencia()
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo(PAAdministrador.Instancia.ConsultarParametrosFramework("Cultura"));
        }

        public void DescargueEntregaAgencia(OUGuiaIngresadaDC guia)
        {
            throw new NotImplementedException();
        }

        #endregion Instancia


    }
}
