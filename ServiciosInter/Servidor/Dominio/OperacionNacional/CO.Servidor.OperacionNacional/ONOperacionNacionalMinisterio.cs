using Framework.Servidor.Comun;
using Framework.Servidor.Excepciones;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CO.Servidor.OperacionNacional
{
    public class ONOperacionNacionalMinisterio : ControllerBase
    {
        private static readonly ONOperacionNacionalMinisterio instancia = (ONOperacionNacionalMinisterio)FabricaInterceptores.GetProxy(new ONOperacionNacionalMinisterio(), COConstantesModulos.MODULO_OPERACION_NACIONAL);

        public static ONOperacionNacionalMinisterio Instancia
        {
            get { return ONOperacionNacionalMinisterio.instancia; }
        }

    }
}
