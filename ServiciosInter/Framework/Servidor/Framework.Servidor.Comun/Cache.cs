using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Framework.Servidor.Comun
{
  public class Cache : IDictionary<string, object>
  {
    #region Atributos

    /// <summary>
    /// Elementos a almacenar en el diccionario
    /// </summary>
    Dictionary<string, object> elementos;

    /// <summary>
    /// Vigencia de los elementos almacenados en el diccionario
    /// </summary>
    Dictionary<string, DateTime> vigencia;

    /// <summary>
    /// Vigencia por defecto en minutos de los elementos en caché
    /// </summary>
    int vigenciaPorDefecto = 120;

    /// <summary>
    /// lista de festivos
    /// </summary>
    List<DateTime> listaFestivos;




    /// <summary>
    /// Instancia de la clase
    /// </summary>
    private static readonly Cache instancia = new Cache();

    #endregion Atributos

    #region Propiedades

    /// <summary>
    /// Instancia de la clase
    /// </summary>
    public static Cache Instancia
    {
      get
      {
        if (instancia.elementos == null)
        {
          instancia.Inicializar();
        }
        return instancia;
      }
    }

    #endregion Propiedades

    #region Métodos

    /// <summary>
    /// Inicialización del diccionario
    /// </summary>
    private void Inicializar()
    {
      elementos = new Dictionary<string, object>();
      vigencia = new Dictionary<string, DateTime>();
      listaFestivos = new List<DateTime>();
    }

    /// <summary>
    /// Agrega elemento a caché
    /// </summary>
    /// <param name="key">Llave con la cual se identifica la lista</param>
    /// <param name="value">Lista de elemntos almacenados en el caché</param>
    public void Add(string key, object value)
    {
      elementos.Add(key, value);
      vigencia.Add(key, DateTime.Now.AddMinutes(vigenciaPorDefecto));
    }

    /// <summary>
    /// Agrega elemento a caché
    /// </summary>
    /// <param name="key">Llave con la cual se identifica la lista</param>
    /// <param name="value">Lista de elemntos almacenados en el caché</param>
    /// <param name="vigenciaEnMinutos">Vigencia en minutos de la lista</param>
    public void Add(string key, object value, int vigenciaEnMinutos)
    {
      elementos.Add(key, value);
      vigencia.Add(key, DateTime.Now.AddMinutes(vigenciaEnMinutos));
    }

    /// <summary>
    /// Indica si ha sido grabado en caché un elemento de este tipo
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public bool ContainsKey(string key)
    {
      if (elementos.ContainsKey(key))
      {
        bool vigente = DateTime.Now.CompareTo(vigencia[key]) < 0;
        if (!vigente)
        {
          elementos.Remove(key);
          vigencia.Remove(key);
          return false;
        }
        return vigente;
      }
      return false;
    }

      /// <summary>
      /// Método para validar que los festivos esten creados en cache
      /// </summary>
      /// <returns></returns>
    public bool ContainsFestivos()
    {
        if (listaFestivos.Any())
            return true;
        else
            return false;
    }

    /// <summary>
    /// Método para adicionar los festivos
    /// </summary>
    /// <returns></returns>
    public void AddFestivos(List<DateTime> listaFestivos)
    {
        this.listaFestivos = listaFestivos;
    }

    /// <summary>
    /// Método para validar que los festivos esten creados en cache
    /// </summary>
    /// <returns></returns>
    public List<DateTime> GetFestivos()
    {
        return listaFestivos;
    }



    public ICollection<string> Keys
    {
      get { throw new NotImplementedException(); }
    }

    public bool Remove(string key)
    {
      return elementos.Remove(key);
    }

    public bool TryGetValue(string key, out object value)
    {
      return elementos.TryGetValue(key, out value);
    }

    public ICollection<object> Values
    {
      get { throw new NotImplementedException(); }
    }

    public object this[string key]
    {
      get
      {
        return elementos[key];
      }
      set
      {
        elementos[key] = value;
      }
    }

    public void Add(KeyValuePair<string, object> item)
    {
      elementos.Add(item.Key, item.Value);
    }

    public void Clear()
    {
      elementos.Clear();
    }

    public bool Contains(KeyValuePair<string, object> item)
    {
      throw new NotImplementedException();
    }

    public void CopyTo(KeyValuePair<string, object>[] array, int arrayIndex)
    {
      throw new NotImplementedException();
    }

    public int Count
    {
      get { return elementos.Count; }
    }

    public bool IsReadOnly
    {
      get { throw new NotImplementedException(); }
    }

    public bool Remove(KeyValuePair<string, object> item)
    {
      return elementos.Remove(item.Key);
    }

    public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
    {
      return elementos.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return elementos.GetEnumerator();
    }

    #endregion Métodos
  }
}