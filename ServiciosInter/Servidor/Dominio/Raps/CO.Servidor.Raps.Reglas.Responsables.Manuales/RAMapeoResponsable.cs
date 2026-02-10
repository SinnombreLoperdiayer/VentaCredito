using CO.Servidor.Servicios.ContratoDatos.Raps.Configuracion;
using CO.Servidor.Servicios.ContratoDatos.Raps.Solicitudes;
using Framework.Servidor.Comun;
using Framework.Servidor.Excepciones;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CO.Servidor.RAPS.Reglas.ResponsablesManuales
{
    public class RAMapeoResponsable
    {

        public static RAResponsableDC MapeoResponsable(int idResponsableFalla, long IdCentroServicios, string Identificacion, string NombreCentroServicio)
        {
            RAResponsableDC responsable = new RAResponsableDC()
            {
                IdResponsableFalla = idResponsableFalla,
                Id = IdCentroServicios,
                IdentificacionResponsable = Identificacion,
                Nombre = NombreCentroServicio,
            };


            return responsable;
        }
    }
}
