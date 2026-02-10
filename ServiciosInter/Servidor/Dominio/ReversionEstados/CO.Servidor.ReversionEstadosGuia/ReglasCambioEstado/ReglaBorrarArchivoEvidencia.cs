using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CO.Servidor.Servicios.ContratoDatos.ReversionEstados;
using CO.Servidor.ReversionEstadosGuia.Datos;

namespace CO.Servidor.ReversionEstadosGuia.ReglasCambioEstado
{
    public class ReglaBorrarArchivoEvidencia : IReglasCambioEstado
    {
        public void EjecucionRegla(ReversionEstado reversionEstado)
        {
            ADReversionEstadosRepositorio.Instancia.EliminarArchivoEvidencia(reversionEstado);
        }
    }
}
