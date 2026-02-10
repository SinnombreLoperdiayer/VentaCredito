using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Framework.Servidor.Archivo
{
  public class MergeWord
  {
    public Dictionary<string, string> Campos { get; set; }

    public DataTable TablaAdicional1 { get; set; }

    public DataTable TablaAdicional2 { get; set; }
  }
}
