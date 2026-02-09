using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Threading;
using System.Transactions;
using CO.Servidor.Dominio.Comun.AdmEstadosGuia;
using CO.Servidor.Dominio.Comun.Admisiones;
using CO.Servidor.Dominio.Comun.Middleware;
using CO.Servidor.LogisticaInversa.Comun;
using CO.Servidor.LogisticaInversa.Datos;
using CO.Servidor.LogisticaInversa.Datos.Modelo;
using CO.Servidor.Servicios.ContratoDatos.Admisiones.Mensajeria;
using CO.Servidor.Servicios.ContratoDatos.Area;
using CO.Servidor.Servicios.ContratoDatos.LogisticaInversa;
using Framework.Servidor;
using Framework.Servidor.Comun;
using Framework.Servidor.Comun.Reglas;
using Framework.Servidor.Excepciones;
using Framework.Servidor.MotorReglas;
using Framework.Servidor.ParametrosFW;
using Framework.Servidor.Servicios.ContratoDatos.Parametros;
using CO.Servidor.Servicios.ContratoDatos.CentroServicios;
using CO.Servidor.Dominio.Comun.CentroServicios;

namespace CO.Servidor.LogisticaInversa.Telemercadeo
{
  internal class LIConfiguradorRapiradicados : ControllerBase
  {
    private static readonly LIConfiguradorRapiradicados instancia = (LIConfiguradorRapiradicados)FabricaInterceptores.GetProxy(new LIConfiguradorRapiradicados(), COConstantesModulos.TELEMERCADEO);

    public static LIConfiguradorRapiradicados Instancia
    {
      get { return LIConfiguradorRapiradicados.instancia; }
    }

    IADFachadaAdmisionesMensajeria fachadaMensajeria = COFabricaDominio.Instancia.CrearInstancia<IADFachadaAdmisionesMensajeria>();
    IPUFachadaCentroServicios fachadaCentroServicios = COFabricaDominio.Instancia.CrearInstancia<IPUFachadaCentroServicios>();
  
    #region Consultas

    /// <summary>
    /// Método  para obtener las guias en estado rapiradicado
    /// </summary>
    /// <param name="filtro"></param>
    /// <param name="indicePagina"></param>
    /// <param name="registrosPorPagina"></param>
    /// <returns></returns>
    public List<ADRapiRadicado> ObtenerGuiasRapiradicados(IDictionary<string, string> filtro)
    {
      return fachadaMensajeria.ObtenerGuiasRapiradicados(filtro);
    }

    /// <summary>
    /// Método para obtener información de los rapiradicados asociados a una admision
    /// </summary>
    /// <returns></returns>
    public List<ADRapiRadicado> ObtenerRapiradicadosGuia(long numeroGuia)
    {
      return fachadaMensajeria.ObtenerRapiradicadosGuia(numeroGuia);
    }

    #endregion Consultas

    #region Inserciones

    /// <summary>
    /// Método para generar una guía interna de un rapiradicado
    /// </summary>
    /// <returns></returns>
    public List<ADRapiRadicado> GenerarGuiasInternas(List<ADRapiRadicado> listaRadicados)
    {
      using (TransactionScope transaccion = new TransactionScope())
      {
        listaRadicados.ForEach(g =>
        {
            g.CentroServicioCreacion = fachadaCentroServicios.ObtenerCentroServicio(g.CentroServicioCreacion.IdCentroServicio);
            g.GuiaInterna = new ADGuiaInternaDC
            {
                  DiceContener = LOIConstantesLogisticaInversa.DEVOLUCION_GUIA_NO + g.GuiaAdmision.NumeroGuia ,
                

                  EsOrigenGestion = false,
                  GestionOrigen = new ARGestionDC { IdGestion = 0, Descripcion = string.Empty },
                  DireccionRemitente = g.CentroServicioCreacion.Direccion,
                  IdCentroServicioOrigen = g.CentroServicioCreacion.IdCentroServicio,
                  LocalidadOrigen =  g.CentroServicioCreacion.CiudadUbicacion,
                  NombreRemitente = g.CentroServicioCreacion.Nombre,
                  TelefonoRemitente = g.CentroServicioCreacion.Telefono1,
                  NombreCentroServicioOrigen = g.CentroServicioCreacion.Nombre,

                
                  EsManual = false,

                  EsDestinoGestion = false,
                  GestionDestino = new ARGestionDC { IdGestion = 0, Descripcion = string.Empty },
                  IdCentroServicioDestino = 0,
                  DireccionDestinatario = g.DireccionDestinatario,    
                  LocalidadDestino = g.CiudadDestino,
                  NombreCentroServicioDestino = string.Empty,
                  NombreDestinatario = g.NombreDestinatario,
                  TelefonoDestinatario = g.TelefonoDestinatario,
                  
                  NumeroGuia = 0 ,
                  IdAdmisionGuia = 0,
                 
                  PaisDefault = new PALocalidadDC { IdLocalidad = ConstantesFramework.ID_LOCALIDAD_COLOMBIA, Nombre = ConstantesFramework.DESC_LOCALIDAD_COLOMBIA },    
            };
          g.GuiaInterna = fachadaMensajeria.AdicionarGuiaInterna(g.GuiaInterna);
          g.NumeroGuiaInterna = g.GuiaInterna.NumeroGuia;
          fachadaMensajeria.ActualizarGuiaRapiradicado(g.IdRapiradicado, g.NumeroGuiaInterna);
        });

        transaccion.Complete();
      }
      return listaRadicados;
    }

    /// <summary>
    /// Genera una guía interna y la actualiza en los radicados asociados
    /// </summary>
    /// <param name="listaRadicados"></param>
    /// <returns></returns>
    public List<ADRapiRadicado> GenerarGuiasInternasConsolidado(List<ADRapiRadicado> listaRadicados)
    {
      using (TransactionScope transaccion = new TransactionScope())
      {
        ADGuiaInternaDC GuiaInterna = fachadaMensajeria.AdicionarGuiaInterna(listaRadicados.FirstOrDefault().GuiaInterna);

        listaRadicados.ForEach(g =>
        {
          g.NumeroGuiaInterna = GuiaInterna.NumeroGuia;
          fachadaMensajeria.ActualizarGuiaRapiradicado(g.IdRapiradicado, g.NumeroGuiaInterna);
        });

        transaccion.Complete();
      }
      return listaRadicados;
    }

    #endregion Inserciones
  }
}