using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using CO.Servidor.Servicios.ContratoDatos.Tarifas;
using CO.Servidor.Tarifas.Comun;
using CO.Servidor.Tarifas.Datos.Modelo;
using Framework.Servidor.Comun;
using Framework.Servidor.Excepciones;

namespace CO.Servidor.Tarifas.Datos
{
  public partial class TARepositorio
  {
    #region Consultas

    /// <summary>
    /// Retorna los tipos de envíos con los servicios asociados
    /// </summary>
    /// <returns></returns>
    public IEnumerable<TATipoEnvio> ObtenerTiposDeEnvio()
    {
      using (EntidadesTarifas contexto = new EntidadesTarifas(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
      {
        return contexto.TipoEnvio_TAR.ToList()
          .ConvertAll(r => new TATipoEnvio
          {
             IdTipoEnvio = r.TEN_IdTipoEnvio,
             Nombre = r.TEN_Nombre, 
             PesoMaximo = r.TEN_PesoMaximo,
             PesoMinimo = r.TEN_PesoMinimo,
          });
      }
    }

    /// <summary>
    /// Obtener concepto de caja a partir del numero del servicio
    /// </summary>
    /// <param name="idServicio"></param>
    /// <returns></returns>
    public int ObtenerConceptoCaja(int idServicio)
    {
      using (EntidadesTarifas contexto = new EntidadesTarifas(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
      {
        Servicio_TAR servicio = contexto.Servicio_TAR.Where(ser => ser.SER_IdServicio == idServicio).FirstOrDefault();
        if (servicio == null)
        {
          throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.TARIFAS, TAEnumTipoErrorTarifas.EX_NO_EXISTE_CONCEPTO_SERVICIO.ToString(), TAMensajesTarifas.CargarMensaje(TAEnumTipoErrorTarifas.EX_NO_EXISTE_CONCEPTO_SERVICIO)));
        }
        else
        {
          return servicio.SER_IdConceptoCaja;
        }
      }
    }

    #endregion Consultas

    #region Tipos de Valor Adicional

    /// <summary>
    /// Obtiene los tipos de valor y sus campos para un servicio
    /// </summary>
    /// <param name="idServicio">ID del servicio</param>
    /// <returns>Lista con todos los tipos de valor  con sus campos</returns>
    public List<TAValorAdicional> ObtenerTipoValorAdicionalConCampos(int idServicio)
    {
      using (EntidadesTarifas contexto = new EntidadesTarifas(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
      {
        int idListaPrecios = ObtenerIdListaPrecioVigente();

        return contexto.ListaPrecioCampoValorAdd_VTAR.Where(lp => lp.LPS_IdListaPrecios == idListaPrecios && lp.LPS_IdServicio == idServicio).ToList()
          .GroupBy(valorAdd => valorAdd.CTA_IdTipoValorAdicional,
          (idtipo, campos) => new TAValorAdicional()
            {
              IdTipoValorAdicional = idtipo,
              Descripcion = campos.First().TVA_Descripcion,
              CamposTipoValorAdicionalDC = campos.ToList().ConvertAll<TACampoTipoValorAdicionalDC>
              (ctv => new TACampoTipoValorAdicionalDC()
              {
                Display = ctv.CTA_Display,
                IdCampo = ctv.CTA_IdCampo,
                IdTipoValorAdicional = ctv.CTA_IdTipoValorAdicional,
                TipoDato = ctv.CTA_TipoDato
              }),
              PrecioValorAdicional = campos.First().PVA_Valor,
              IdServicio = campos.First().LPS_IdServicio,
            }).ToList();
      }
    }

    #endregion Tipos de Valor Adicional
  }
}