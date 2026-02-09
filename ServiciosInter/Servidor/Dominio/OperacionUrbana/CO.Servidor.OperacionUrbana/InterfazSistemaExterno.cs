using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CO.Servidor.Dominio.Comun.OperacionUrbana;
using CO.Servidor.Servicios.ContratoDatos.OperacionUrbana;

namespace CO.Servidor.OperacionUrbana
{
  public class InterfazSistemaExterno : IInterfazSistemaExterno
  {
    #region Campos

    private static readonly InterfazSistemaExterno instancia = new InterfazSistemaExterno();

    #endregion Campos

    #region Propiedades

    public static InterfazSistemaExterno Instancia
    {
      get { return InterfazSistemaExterno.instancia; }
    }

    #endregion Propiedades

    /// <summary>
    /// Consulta la persona en la base de datos de novasoft
    /// </summary>
    /// <param name="documento"></param>
    /// <returns></returns>
    public OUPersonaInternaDC ConsultaMensajero(string documento, bool contratista)
    {
      return new OUPersonaInternaDC()
      {
        PrimerApellido = "Perez",
        SegundoApellido = "Gomez",
        Nombre = "Pepito",
        Identificacion = documento,
        Regional = 1003,
        Direccion = "Carrera 5 N° 61-13",
        Email = "pepito@prueba.com",
        IdCargo = 9,
        Cargo = "Mensajero",
        IdTipoIdentificacion = "CC",
        Telefono = "3235698",
        Municipio = "MEDELLIN",
        IdMunicipio = "05001",
        TipoContrato = "Termino Fijo",
        FechaInicioContrato = DateTime.Parse("2011-01-01 00:00:00.000"),
        FechaTerminacionContrato = DateTime.Parse("2011-12-01 00:00:00.000"),
        Comentarios = "Observaciones",
      };
    }

    /// <summary>
    /// Valida el tipo de vinculacion del mensajero, si es contratista o no
    /// </summary>
    /// <param name="documento"></param>
    /// <returns></returns>
    public bool ValidaVinculacionMensajero(string documento)
    {
      return false;
    }
  }
}