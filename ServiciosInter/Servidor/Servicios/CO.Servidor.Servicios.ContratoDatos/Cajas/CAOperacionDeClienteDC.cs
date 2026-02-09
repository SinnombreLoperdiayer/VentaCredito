using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace CO.Servidor.Servicios.ContratoDatos.Cajas
{
  public class CAOperacionDeClienteDC
  {
    #region Primitive Properties

    [DataMember]
    public long IdMovimiento
    {
      get;
      set;
    }

    [DataMember]
    public int IdServicio
    {
      get;
      set;
    }

    [DataMember]
    public int IdCliente
    {
      get;
      set;
    }

    [DataMember]
    public decimal ValorOperacion
    {
      get;
      set;
    }

    [DataMember]
    public string NombreServicio
    {
      get;
      set;
    }

    [DataMember]
    public DateTime FechaOperacion
    {
      get;
      set;
    }

    #endregion Primitive Properties
  }
}