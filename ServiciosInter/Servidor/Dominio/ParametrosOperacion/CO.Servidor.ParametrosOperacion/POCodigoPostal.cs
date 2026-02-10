using Framework.Servidor.Comun;
using Framework.Servidor.Excepciones;
using Framework.Servidor.ParametrosFW;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;

namespace CO.Servidor.ParametrosOperacion
{
   public class POCodigoPostal: ControllerBase
    {
        private static readonly POCodigoPostal instancia = (POCodigoPostal)FabricaInterceptores.GetProxy(new POCodigoPostal(), COConstantesModulos.PARAMETROS_OPERATIVOS);

        public static POCodigoPostal Instancia
        {
            get { return POCodigoPostal.instancia; }
        }

        /// <summary>
        /// constructor
        /// </summary>
        private POCodigoPostal()
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo(PAAdministrador.Instancia.ConsultarParametrosFramework("Cultura"));
        }
    }
}
