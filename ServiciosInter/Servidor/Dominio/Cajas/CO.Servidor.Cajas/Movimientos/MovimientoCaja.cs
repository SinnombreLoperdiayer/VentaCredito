using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CO.Servidor.Cajas.Datos;
using CO.Servidor.Servicios.ContratoDatos.GestionCajas;

namespace CO.Servidor.Cajas.Movimientos
{
  internal class MovimientoCaja
  {
    /// <summary>
    /// Adiciona  el Movimiento entre un centro Servicio y la caja del Banco.
    /// </summary>
    /// <param name="movimiento">Movimiento entre Banco y Centro de Servicios.</param>
    public void RegistrarMovCajaBancoCentroServicios(CAMovBancoCentroSvcDC movimiento)
    {
      CARepositorioCaja.Instancia.AdicionarMovCentroSrvCajaBanco(movimiento);
    }
  }
}