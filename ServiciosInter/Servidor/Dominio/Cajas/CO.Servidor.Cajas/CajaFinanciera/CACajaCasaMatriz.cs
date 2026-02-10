using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CO.Servidor.Cajas.Datos;
using CO.Servidor.Cajas.Datos.Modelo;
using CO.Servidor.Servicios.ContratoDatos.GestionCajas;
using Framework.Servidor.Comun;
using Framework.Servidor.Excepciones;
using System.Data.SqlClient;

namespace CO.Servidor.Cajas.CajaFinanciera
{
  /// <summary>
  /// Administra operaciones sobre caja de casa matriz
  /// </summary>
  internal class CACajaCasaMatriz:ControllerBase
  {
      private static readonly CACajaCasaMatriz instancia = (CACajaCasaMatriz)FabricaInterceptores.GetProxy(new CACajaCasaMatriz(), COConstantesModulos.MENSAJERIA);

      /// <summary>
      /// Retorna una instancia de Consultas de admision guía interna
      /// /// </summary>
      public static CACajaCasaMatriz Instancia
      {
          get { return CACajaCasaMatriz.instancia; }
      }

    /// <summary>
    /// Registrar operación en la tabla de caja Casa Matriz
    /// </summary>
    /// <param name="idApertura">Identificador de la apertura de caja</param>
    /// <param name="infoTransaccion">Información de la operación</param>
    /// <returns>Identificador con el cual queda guardado el registro de la operación</returns>
    public long RegistrarOperacion(long idApertura, CACajaCasaMatrizDC infoTransaccion)
    {
      OperacionCajaCasaMatriz_CAJ operacion = new OperacionCajaCasaMatriz_CAJ
      {
        CAE_ConceptoEsIngreso = infoTransaccion.ConceptoCaja.EsIngreso,
        CAE_CreadoPor = infoTransaccion.CreadoPor,
        CAE_Descripcion = infoTransaccion.Descripcion ?? String.Empty,
        CAE_FechaGrabacion = DateTime.Now,
        CAE_FechaMovimiento = infoTransaccion.FechaMov,
        CAE_IdAperturaCaja = idApertura,
        CAE_IdConceptoCaja = infoTransaccion.ConceptoCaja.IdConceptoCaja,
        CAE_NombreConceptoCaja = infoTransaccion.ConceptoCaja.Nombre,
        CAE_MovHechoPor = infoTransaccion.MovHechoPor,
        CAE_NumeroDocumento = infoTransaccion.NumeroDocumento,
        CAE_Observacion = infoTransaccion.Observacion ?? String.Empty,
        CAE_Valor = infoTransaccion.Valor
      };

      if (operacion.CAE_FechaMovimiento.Equals(DateTime.MinValue))
      {
        operacion.CAE_FechaMovimiento = operacion.CAE_FechaGrabacion;
      }

      return CARepositorioGestionCajas.Instancia.AdicionarOperacionCajaCasaMatriz(operacion);
    }

    /// <summary>
    /// Registrar operación en la tabla de caja Casa Matriz
    /// </summary>
    /// <param name="idApertura">Identificador de la apertura de caja</param>
    /// <param name="infoTransaccion">Información de la operación</param>
    /// <returns>Identificador con el cual queda guardado el registro de la operación</returns>
    public long RegistrarOperacion(long idApertura, CACajaCasaMatrizDC infoTransaccion,SqlConnection conexion, SqlTransaction transaccion)
    {
        OperacionCajaCasaMatriz_CAJ operacion = new OperacionCajaCasaMatriz_CAJ
        {
            CAE_ConceptoEsIngreso = infoTransaccion.ConceptoCaja.EsIngreso,
            CAE_CreadoPor = infoTransaccion.CreadoPor,
            CAE_Descripcion = infoTransaccion.Descripcion ?? String.Empty,
            CAE_FechaGrabacion = DateTime.Now,
            CAE_FechaMovimiento = infoTransaccion.FechaMov,
            CAE_IdAperturaCaja = idApertura,
            CAE_IdConceptoCaja = infoTransaccion.ConceptoCaja.IdConceptoCaja,
            CAE_NombreConceptoCaja = infoTransaccion.ConceptoCaja.Nombre,
            CAE_MovHechoPor = infoTransaccion.MovHechoPor,
            CAE_NumeroDocumento = infoTransaccion.NumeroDocumento,
            CAE_Observacion = infoTransaccion.Observacion ?? String.Empty,
            CAE_Valor = infoTransaccion.Valor
        };

        if (operacion.CAE_FechaMovimiento.Equals(DateTime.MinValue))
        {
            operacion.CAE_FechaMovimiento = operacion.CAE_FechaGrabacion;
        }

        return CARepositorioGestionCajas.Instancia.AdicionarOperacionCajaCasaMatriz(operacion,conexion,transaccion);
    }
  }
}