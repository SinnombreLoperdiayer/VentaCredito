using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CO.Servidor.LogisticaInversa.Reglas.Modelo;
using CO.Servidor.Servicios.ContratoDatos.LogisticaInversa;
using Framework.Servidor.Comun;
using Framework.Servidor.Comun.Reglas;

namespace CO.Servidor.LogisticaInversa.Reglas
{
  internal class LOIReglasRepositorio
  {
    #region Campos

    private static readonly LOIReglasRepositorio instancia = new LOIReglasRepositorio();
    private const string NombreModelo = "ModeloReglasLogisticaInversa";

    #endregion Campos

    #region Propiedades

    /// <summary>
    /// Retorna la instancia de la clase
    /// </summary>
    public static LOIReglasRepositorio Instancia
    {
      get { return LOIReglasRepositorio.instancia; }
    }

    #endregion Propiedades

    #region Consultas

    /// <summary>
    /// Metodo para obtener la cantidad de gestiones a partir de un resultado
    /// </summary>
    /// <param name="gestion"></param>
    /// <returns></returns>
    public int ObtenerGestiones(LIGestionesDC gestion)
    {
      using (EntidadesReglasLogisticaInversa contexto = new EntidadesReglasLogisticaInversa(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
      {
        List<GestionGuiaTelemercadeo_LOI> listaGestiones = contexto.GestionGuiaTelemercadeo_LOI
          .Where(s => s.GGT_IdAdminisionMensajeria == gestion.idAdmisionGuia
            && s.GGT_IdResultadoTelemercadeo == gestion.Resultado.IdResultado)
            .ToList();
        return listaGestiones.Count();
      }
    }

      /// <summary>
    /// Metodo para buscar una direccion en admisiones
    /// </summary>
    /// <param name="gestion"></param>
    /// <returns></returns>
    public long ObtenerDireccion(string direccion, bool esDestinatario, string localidad)
    {
        using (EntidadesReglasLogisticaInversa contexto = new EntidadesReglasLogisticaInversa(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
        {
            long? resultado = contexto.paBuscarDireccionMensajeria_MEN(direccion, esDestinatario, localidad).FirstOrDefault().Value;
            return resultado == null? 0 : resultado.Value;
        }
    }


    #endregion Consultas
  }
}