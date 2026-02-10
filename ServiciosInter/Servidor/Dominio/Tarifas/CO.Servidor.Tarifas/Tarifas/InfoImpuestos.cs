using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CO.Servidor.Tarifas
{
  public class InfoImpuestos
  {
    private string descripcionImpuesto;
    private double valor;

    public string DescripcionImpuesto
    {
      get { return descripcionImpuesto; }
      set { descripcionImpuesto = value; }
    }

    public double Valor
    {
      get { return valor; }
      set { valor = value; }
    }

    public InfoImpuestos(string descripcionImpuesto, double valor)
    {
      this.descripcionImpuesto = descripcionImpuesto;
      this.valor = valor;
    }
  }
}