using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CO.Servidor.Cajas.Datos;
using CO.Servidor.Cajas.Datos.Modelo;
using CO.Servidor.Servicios.ContratoDatos.GestionCajas;
using Framework.Servidor.Comun;
using Framework.Servidor.Excepciones;

namespace CO.Servidor.Cajas.CajaFinanciera
{
  /// <summary>
  /// Clase para manejo de operaciones sobre la caja de Operación Nacional
  /// </summary>
  internal class CACajaOperacionNacional : ControllerBase
  {
    #region Atributos

    private static readonly CACajaOperacionNacional instancia = (CACajaOperacionNacional)FabricaInterceptores.GetProxy(new CACajaOperacionNacional(), COConstantesModulos.CAJA);

    #endregion Atributos

    #region Instancia

    public static CACajaOperacionNacional Instancia
    {
      get { return CACajaOperacionNacional.instancia; }
    }

    #endregion Instancia

    public CACajaOperacionNacional()
    {
    }

    /// <summary>
    /// Registrar operación sobre la caja de Operación Nacional
    /// </summary>
    /// <param name="idApertura">Identificador de la apertura</param>
    /// <param name="infoTransaccion">Inforamción de la transacción de caja de operación nacional</param>
    /// <returns>Identificador con el cual se guarda la operación</returns>
    public long RegistrarOperacion(long idApertura, CARegistroOperacionOpnDC infoTransaccion)
    {
      OperacionCajaOperacionNacional_CAJ operacion = new OperacionCajaOperacionNacional_CAJ
      {
        CON_ConceptoEsIngreso = infoTransaccion.EsIngreso,
        CON_CreadoPor = ControllerContext.Current.Usuario,
        CON_Descripcion = infoTransaccion.Descripcion,
        CON_FechaGrabacion = DateTime.Now,
        CON_FechaMovimiento = infoTransaccion.FechaMovimiento,
        CON_IdAperturaCaja = idApertura,
        CON_IdConceptoCaja = infoTransaccion.IdConceptoCaja,
        CON_NombreConceptoCaja = infoTransaccion.NombreConceptoCaja,
        CON_MovHechoPor = infoTransaccion.MovHechoPor,
        CON_NumeroDocumento = infoTransaccion.NumeroDocumento,
        CON_Observacion = infoTransaccion.Observacion,
        CON_Valor = infoTransaccion.Valor
      };

      if (operacion.CON_FechaMovimiento.Equals(DateTime.MinValue))
      {
        operacion.CON_FechaMovimiento = operacion.CON_FechaGrabacion;
      }

      return CARepositorioGestionCajas.Instancia.AdicionarOperacionCajaOpn(operacion);
    }

    /// <summary>
    /// Obtener las operaciones de caja de Operación Nacional en una fecha
    /// </summary>
    /// <param name="idCasaMatriz">Identificador único de la casa matriz</param>
    /// <param name="fecha">Fecha en la cual se hace la consulta</param>
    /// <returns>Collección con la información de las operaciones</returns>
    public IList<CACajaCasaMatrizDC> ObtenerOperaciones(short idCasaMatriz, DateTime fecha)
    {
      return CARepositorioCaja.Instancia.ObtenerOperacionesOpn(idCasaMatriz, fecha);
    }

    /// <summary>
    /// Obtiene la info la base de caja de Operacion Nacional
    /// de acuerdo con su Casa matriz
    /// </summary>
    /// <param name="idCasaMatriz"></param>
    /// <returns>info de base de caja de OpeNal</returns>
    public CABaseGestionCajasDC ObtenerBaseCajaOperacionNacional(int idCasaMatriz)
    {
      return CARepositorioGestionCajas.Instancia.ObtenerBaseCajaOperacionNacional(idCasaMatriz);
    }

    /// <summary>
    /// Obtiene la info la base de caja de Operacion Nacional
    /// de acuerdo con su Casa matriz
    /// </summary>
    /// <param name="idCasaMatriz"></param>
    /// <returns>lista de Bases de Caja de las Casas Matriz y Operacion Nal</returns>
    public List<CABaseGestionCajasDC> ObtenerBasesDeCajasOperacionNacional()
    {
      return CARepositorioGestionCajas.Instancia.ObtenerBasesDeCajasOperacionNacional();
    }

    /// <summary>
    /// Actualiza, Modifica, Borra la Base de la caja
    /// de Operacional Nacional por su Casa Matriz
    /// </summary>
    /// <param name="infoBaseCajaOpeNal">data de Entrada</param>
    public void GestionBaseCajaOperacionNacional(CABaseGestionCajasDC infoBaseCajaOpeNal)
    {
      switch (infoBaseCajaOpeNal.EstadoRegistro)
      {
        case EnumEstadoRegistro.ADICIONADO:
          CARepositorioGestionCajas.Instancia.AdicionarBaseCajaOperacionNacional(infoBaseCajaOpeNal);
          break;

        case EnumEstadoRegistro.MODIFICADO:
          CARepositorioGestionCajas.Instancia.ActualizarBaseCajaOperacionNacional(infoBaseCajaOpeNal);
          break;

        case EnumEstadoRegistro.BORRADO:
          CARepositorioGestionCajas.Instancia.BorrarBaseCajaOperacionNacional(infoBaseCajaOpeNal);
          break;

        default:
          break;
      };
    }
  }
}