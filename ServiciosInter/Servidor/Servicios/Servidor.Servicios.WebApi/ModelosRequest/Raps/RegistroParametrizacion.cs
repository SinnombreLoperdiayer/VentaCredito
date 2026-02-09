using CO.Servidor.Servicios.ContratoDatos.Raps.Configuracion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CO.Servidor.Servicios.WebApi.ModelosRequest.Raps
{
    public class RegistroParametrizacion
    {     
         public RAParametrizacionRapsDC ParametrizacionRaps { get; set; }
         public List<RACargoDC> ListarCargos { get; set; }     
    }
}