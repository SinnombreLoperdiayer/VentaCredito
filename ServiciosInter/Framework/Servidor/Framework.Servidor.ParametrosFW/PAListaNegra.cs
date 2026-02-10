///////////////////////////////////////////////////////////
//  PAAListaRestrictiva.cs
//  Implementation of the Class PAAdministradorListaRestrictiva
//  Created on:      29-nov-2011 11:56:29 a.m.
//  Original author: diego.toro
///////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Framework.Servidor.Comun;
using Framework.Servidor.Excepciones;
using Framework.Servidor.ParametrosFW.Datos;
using Framework.Servidor.Servicios.ContratoDatos.Parametros;

namespace Framework.Servidor.ParametrosFW
{
  /// <summary>
  /// Clase para manejo de la lista restrictiva
  /// </summary>
  public class PAListaNegra : ControllerBase
  {
    private static readonly PAListaNegra instancia = (PAListaNegra)FabricaInterceptores.GetProxy(new PAListaNegra(), ConstantesFramework.PARAMETROS_FRAMEWORK);

    public static PAListaNegra Instancia
    {
      get { return instancia; }
    }

    /// <summary>
    /// Aplicar cambios a registro de lista negra, permite insertar, actualizar o
    /// borrar un registro
    /// </summary>
    /// <param name="registroListaNegra">Registro de la lista negra a aplicar los
    /// cambios</param>
    /// <param name="idTipoLista">Identificador del tipo de lista negra</param>
    public static void Post(/*PAListaNegraDC registroListaNegra, */ short idTipoLista)
    {
    }

    /// <summary>
    /// Validar si la identificación se encuentra incluida en una de las listas
    /// restrictivas
    /// </summary>
    /// <param name="identificacion">Identificación a validar en la lista
    /// restrictiva</param>
    public bool ValidarIdentificacion(string identificacion)
    {
      return PARepositorio.Instancia.ValidarListaRestrictiva(identificacion);
    }

    /// <summary>
    /// Obtiene los datos de la lista restrictiva
    /// </summary>
    /// <param name="filtro">Filtro</param>
    /// <param name="campoOrdenamiento">Campo de Ordenamiento</param>
    /// <param name="indicePagina">Indice de página</param>
    /// <param name="registrosPorPagina">Registro por página</param>
    /// <param name="ordenamientoAscendente">Ordenamiento</param>
    /// <param name="totalRegistros">Total de Registros</param>
    /// <returns>Colección lista restrictiva</returns>
    public IEnumerable<PAListaRestrictivaDC> ObtenerListaRestrictiva(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros)
    {
      return PARepositorio.Instancia.ObtenerListaRestrictiva(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros);
    }

    /// <summary>
    /// Adiciona, edita o elimina una lista restrictiva
    /// </summary>
    /// <param name="listaRestricitva">Objeto lista restrictiva</param>
    public void ActualizarListaRestrictiva(PAListaRestrictivaDC listaRestricitva)
    {
      if (listaRestricitva.EstadoRegistro == EnumEstadoRegistro.ADICIONADO)
        PARepositorio.Instancia.AdicionarListaRestrictiva(listaRestricitva);
      else if (listaRestricitva.EstadoRegistro == EnumEstadoRegistro.MODIFICADO)
        PARepositorio.Instancia.EditarListaRestrictiva(listaRestricitva);
      else if (listaRestricitva.EstadoRegistro == EnumEstadoRegistro.BORRADO)
        PARepositorio.Instancia.EliminarListaRestrictiva(listaRestricitva);
    }

    /// <summary>
    /// Obtiene los tipos de lista restrictiva
    /// </summary>
    /// <returns>Colección tipo de lista restrictiva</returns>
    public IEnumerable<PATipoListaRestrictivaDC> ObtenerTiposListaRestrictiva()
    {
      return PARepositorio.Instancia.ObtenerTiposListaRestrictiva();
    }
  }
}