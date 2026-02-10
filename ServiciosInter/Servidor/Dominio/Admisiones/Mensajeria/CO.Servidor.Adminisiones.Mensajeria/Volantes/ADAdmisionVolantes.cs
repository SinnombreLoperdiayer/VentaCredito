using System;
using System.Collections.Generic;
using CO.Servidor.Adminisiones.Mensajeria.Datos;
using CO.Servidor.Adminisiones.Mensajeria.Servicios;
using CO.Servidor.Servicios.ContratoDatos.Admisiones.Mensajeria;
using CO.Servidor.Servicios.ContratoDatos.LogisticaInversa;
using CO.Servidor.Servicios.ContratoDatos.Tarifas;
using Framework.Servidor.Comun;
using Framework.Servidor.Excepciones;

namespace CO.Servidor.Adminisiones.Mensajeria.Volantes
{
  internal class ADAdmisionVolantes : ADAdmisionServicio
  {
    private static readonly ADAdmisionVolantes instancia = (ADAdmisionVolantes)FabricaInterceptores.GetProxy(new ADAdmisionVolantes(), COConstantesModulos.MENSAJERIA);

    /// <summary>
    /// Retorna una instancia de Consultas de volantes admision
    /// /// </summary>
    public static ADAdmisionVolantes Instancia
    {
      get { return ADAdmisionVolantes.instancia; }
    }

    #region Adicionar

    /// <summary>
    /// Adiciona archivo de un volante
    /// </summary>
    /// <param name="archivo">objeto de tipo archivo</param>
    public void AdicionarArchivo(LIArchivosDC archivo)
    {
       ADRepositorio.Instancia.AdicionarArchivo(archivo);
    }

    

    /// <summary>
    /// Metodo para guardar un volante de devolución
    /// </summary>
    /// <param name="volante"></param>
    /// <returns></returns>
    public long AdicionarEvidenciaDevolucion(LIEvidenciaDevolucionDC evidenciaDevolucion)
    {
      return ADRepositorio.Instancia.AdicionarEvidenciaDevolucion(evidenciaDevolucion);
    }

    /// <summary>
    /// Edita un archivo de evidencia de devolución
    /// </summary>
    /// <param name="imagen">Objeto imágen</param>
    public void EditarArchivoEvidencia(LIArchivosDC imagen)
    {
      ADRepositorio.Instancia.EditarArchivoEvidencia(imagen);
    }

    #endregion Adicionar
  }
}