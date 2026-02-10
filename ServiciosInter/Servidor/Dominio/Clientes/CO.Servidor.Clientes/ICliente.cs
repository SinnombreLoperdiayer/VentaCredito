using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CO.Servidor.Clientes
{
  public interface ICliente
  {
    void ObtenerCliente(int idCliente, int idServicio);

    void ValidarCliente();
  }
}