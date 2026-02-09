using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CO.Servidor.Dominio.Comun.AdmEstadosGuia
{
  internal class EGEstadoGuia
  {
    public short Id { get; set; }

    public string Nombre { get; set; }

    public List<EGEstadoGuia> Precedesores { get; set; }
  }
}