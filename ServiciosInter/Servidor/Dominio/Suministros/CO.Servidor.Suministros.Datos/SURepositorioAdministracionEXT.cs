using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CO.Servidor.Servicios.ContratoDatos.Admisiones.Mensajeria;
using CO.Servidor.Servicios.ContratoDatos.Area;
using CO.Servidor.Servicios.ContratoDatos.CentroServicios;
using CO.Servidor.Servicios.ContratoDatos.Clientes;
using CO.Servidor.Servicios.ContratoDatos.Suministros;
using CO.Servidor.Suministros.Datos.Modelo;
using Framework.Servidor.Comun;
using Framework.Servidor.Excepciones;
using Framework.Servidor.Servicios.ContratoDatos.Parametros;

namespace CO.Servidor.Suministros.Datos
{
  public partial class SURepositorioAdministracion
  {
    #region Suministros  Centro de Servicio

    /// <summary>
    /// Guardar los suministros que posee un centro de servicio
    /// o los actualiza si se cambiado la autorizacion
    /// </summary>
    /// <param name="suministroCentroServicio"></param>
    public void GuardarSuministroCentroServicio(SUSuministroCentroServicioDC suministroCentroServicio)
    {
      using (ModeloSuministros contexto = new ModeloSuministros(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
      {
        SuministrosCentroServicios_SUM suministroBD;
        bool suministroAutorizado;

        suministroCentroServicio.Suministro.ToList().ForEach(
          sum =>
          {
            suministroBD = contexto.SuministrosCentroServicios_SUM.FirstOrDefault(suministro => suministro.SCS_IdCentroServicios == suministroCentroServicio.IdCentroServicio && suministro.SCS_IdSuministro == sum.Id);

            if (suministroBD == null)
            {
              //Adicionar nuevo suministro
              contexto.SuministrosCentroServicios_SUM.Add(
                new SuministrosCentroServicios_SUM
                {
                  SCS_IdSuministro = sum.Id,
                  SCS_IdCentroServicios = suministroCentroServicio.IdCentroServicio,
                  SCS_Estado = sum.SuministroAutorizado ? ConstantesFramework.ESTADO_ACTIVO : ConstantesFramework.ESTADO_INACTIVO,
                  SCS_StockMinimo = sum.StockMinimo,
                  SCS_CantidadInicialAutorizada = sum.CantidadInicialAutorizada,
                  SCS_CreadoPor = ControllerContext.Current.Usuario,
                  SCS_FechaGrabacion = DateTime.Now
                });
            }
            else
            {
              suministroAutorizado = suministroBD.SCS_Estado == ConstantesFramework.ESTADO_ACTIVO ? true : false;
              if (suministroAutorizado != sum.SuministroAutorizado)  // Modificar suministro
              {
                suministroBD.SCS_Estado = sum.SuministroAutorizado ? ConstantesFramework.ESTADO_ACTIVO : ConstantesFramework.ESTADO_INACTIVO;
              }
            }
          });
        contexto.SaveChanges();
      }
    }

    /// <summary>
    /// Obtener suministros de grupo de un centro de servicio
    /// </summary>
    /// <param name="idGrupo">age -PUA -rac</param>
    /// <returns>Lista de suministros</returns>
    public List<SUSuministro> ObtenerSuministrosGrupoCentroServicio(string idGrupo, long idCentroServicio)
    {
      using (ModeloSuministros contexto = new ModeloSuministros(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
      {
        return contexto.paObtenerSumAsignadoPorGrupoYCentroServicio_SUM(idCentroServicio, idGrupo)
          .ToList()
          .ConvertAll(conv => new SUSuministro()
            {
              Id = conv.scs_idsuministro,
              CantidadInicialAutorizada = conv.SCS_CantidadInicialAutorizada,
              StockMinimo = conv.SCS_StockMinimo,
              CodigoERP = conv.SUM_CodigoERP,
              Descripcion = conv.SUM_Descripcion,
              CodigoAlterno = conv.SUM_CodigoAlterno,
              SuministroAutorizado = conv.asignado == ConstantesFramework.ESTADO_ACTIVO ? true : false,
              UnidadMedida = new PAUnidadMedidaDC()
              {
                Descripcion = conv.UNM_Descripcion,
                IdUnidadMedida = conv.SUM_IdUnidadMedida
              },
              EstaActivo = conv.SUM_Estado == ConstantesFramework.ESTADO_ACTIVO ? true : false
            });
      }
    }

    /// <summary>
    /// Obtiene los suministros que no estan en el grupo seleccionado ni asignados en el centro de servicio
    /// </summary>
    /// <param name="filtro">codigo erp(novasoft) y descripcion del suministro</param>
    /// <param name="idGrupo">id del grupo</param>
    /// <param name="idCentroServicio">id del centro de servicio</param>
    /// <returns>Lista de suministro</returns>
    public List<SUSuministro> ObtenerSuministrosCentroServicioNoIncluidosEnGrupo(IDictionary<string, string> filtro, string idGrupo, long idCentroServicio)
    {
      using (ModeloSuministros contexto = new ModeloSuministros(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
      {
        string codigoERP;
        string descripcion;

        filtro.TryGetValue("SUM_CodigoERP", out codigoERP);
        filtro.TryGetValue("SUM_Descripcion", out descripcion);

        return contexto.paObtenerSumCentroServicioNoIncluidosEnGrupo_SUM(idCentroServicio, idGrupo, codigoERP, descripcion).ToList()
           .ConvertAll(r => new SUSuministro()
           {
             Id = r.SUM_IdSuministro,
             Descripcion = r.SUM_Descripcion,
             CodigoERP = r.SUM_CodigoERP,
             CodigoAlterno = r.SUM_CodigoAlterno,
             UnidadMedida = new PAUnidadMedidaDC()
             {
               IdUnidadMedida = r.SUM_IdUnidadMedida,
               Descripcion = r.UNM_Descripcion
             },
             EstaActivo = r.SUM_Estado == ConstantesFramework.ESTADO_ACTIVO ? true : false
           });
      }
    }

    #endregion Suministros  Centro de Servicio

    #region Impresion Suministros Por Centro de Servicio

    /// <summary>
    /// Obtiene informacion de las provisiones de un suministro en una sucursal
    /// con la informacion del remitente y destinatario si el origen o el destino es cerrado
    /// </summary>
    /// <param name="fechaInicial">fecha inicial de la consulta por defecto deja la actual menos 3 dias</param>
    /// <param name="fechaFinal">fecha final de la consulta por defecto deja la actual</param>
    /// <param name="idCentroServicio">id del centros de servicio</param>
    /// <param name="idSuministro">id del suministro</param>
    /// <param name="pageIndex">indice de la pagina</param>
    /// <param name="pageSize">tamaño de la paginacion</param>
    /// <returns>Lista con la informacion para impresion los suministro de una sucursal</returns>
    public List<SUImpresionSumSucursalDC> ObtenerProvisionesSuministroSucursal(DateTime? fechaInicial, DateTime? fechaFinal, long idCentroServicio, int idSuministro, int pageIndex, int pageSize)
    {
      using (ModeloSuministros contexto = new ModeloSuministros(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
      {
        fechaInicial = fechaInicial ?? DateTime.Now.AddDays(-3);
        fechaFinal = fechaFinal ?? DateTime.Now;
        return contexto.paObtenerProvisionesSumSucursal_SUM(fechaInicial, fechaFinal, idCentroServicio, idSuministro).ToList()
          .ConvertAll(sum => new SUImpresionSumSucursalDC()
          {
            RemisionSuministro = new SURemisionSuministroDC()
            {
              IdCasaMatriz = sum.RES_IdCasaMatrizElabora,
              NumeroGuiaDespacho = sum.RES_NumeroGuiaInternaDespacho,
              Destinatario = sum.RES_NombreDestinatario,
              GuiaInterna = new ADGuiaInternaDC()
              {
                IdAdmisionGuia = sum.AGI_IdAdmisionMensajeria,
                NombreRemitente = sum.AGI_NombreRemitente,
                NombreDestinatario = sum.AGI_NombreDestinatario,
                DireccionDestinatario = sum.AGI_DireccionDestinatario,
                DireccionRemitente = sum.AGI_DireccionRemitente,
                TelefonoDestinatario = sum.AGI_TelefonoDestinatario,
                TelefonoRemitente = sum.AGI_TelefonoRemitente,
                GestionOrigen = new ARGestionDC() { Descripcion = sum.AGI_DescripcionGestionDest },
                GestionDestino = new ARGestionDC() { Descripcion = sum.AGI_DescripcionGestionOrig }
              }
            },
            Rango = new SURango()
            {
              Inicio = sum.SSS_Inicio,
              Fin = sum.SSS_Fin
            },

            ReferenciaUsoGuia = new CLReferenciaUsoGuiaDC()
            {
              CiudadDestino = sum.NombreCiudad,
              CiudadOrigen = sum.ReferenciaCiudadOrigen,
              CodigoPostalDestino = sum.RUG_CodigoPostalDestino,
              CodigoPostalOrigen = sum.RUG_CodigoPostalOrigen,
              DireccionDestino = sum.RUG_DireccionDestino,
              DireccionOrigen = sum.RUG_DireccionOrigen,
              EsDestionoAbierto = sum.RUG_EsDestionoAbierto == null ? true : sum.RUG_EsDestionoAbierto.Value,
              EsOrigenAbierto = sum.EsOrigenAbierto == null ? true : sum.EsOrigenAbierto.Value,
              IdentificacionDestino = sum.RUG_IdentificacionDestino,
              IdentificacionOrigen = sum.RUG_IdentificacionOrigen,
              NombreDestino = sum.RUG_NombreOrigen,
              NombreOrigen = sum.RUG_NombreOrigen,
              PaisDestino = sum.NombrePais,
              PaisOrigen = sum.ReferenciaPaisOrigen,
              TelefonoDestino = sum.RUG_TelefonoDestino,
              TelefonoOrigen = sum.RUG_TelefonoOrigen,
              TipoIdentificacionDestino = sum.RUG_TipoIdentificacionDestino,
              TipoIdentificacionOrigen = sum.ReferenciaTipoIdOrigen
            },
            Sucursal = new CLSucursalDC()
            {
              IdSucursal = sum.ReferenciaIdSucursal != null ? sum.ReferenciaIdSucursal.Value : 0
            }
          });
      }
    }

    /// <summary>
    /// Obtiene informacion de las provisiones de un suministro en una sucursal por ciudad de destino
    /// con la informacion del remitente y destinatario si el origen o el destino es cerrado
    /// </summary>
    /// <param name="fechaInicial">fecha inicial de la consulta por defecto deja la actual menos 3 dias</param>
    /// <param name="fechaFinal">fecha final de la consulta por defecto deja la actual</param>
    /// <param name="idCentroServicio">id del centros de servicio</param>
    /// <param name="idSuministro">id del suministro</param>
    /// <param name="pageIndex">indice de la pagina</param>
    /// <param name="pageSize">tamaño de la paginacion</param>
    /// <returns>Lista con la informacion para impresion los suministro de una sucursal por ciudad de destino</returns>
    public List<SUImpresionSumSucursalDC> ObtenerProvisionesSuministroSucursalCiudadDestino(DateTime? fechaInicial, DateTime? fechaFinal, string idCiudadDestino, int idSuministro, int pageIndex, int pageSize)
    {
      using (ModeloSuministros contexto = new ModeloSuministros(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
      {
        fechaInicial = fechaInicial ?? DateTime.Now.AddDays(-3);
        fechaFinal = fechaFinal ?? DateTime.Now;
        return contexto.paObtenerProvisionesSumSucursalCiudadDestino_SUM(fechaInicial, fechaFinal, idCiudadDestino, idSuministro).ToList()
          .ConvertAll(sum => new SUImpresionSumSucursalDC()
          {
            RemisionSuministro = new SURemisionSuministroDC()
            {
              IdCasaMatriz = sum.RES_IdCasaMatrizElabora,
              NumeroGuiaDespacho = sum.RES_NumeroGuiaInternaDespacho,
              Destinatario = sum.RES_NombreDestinatario,
              GuiaInterna = new ADGuiaInternaDC()
              {
                IdAdmisionGuia = sum.AGI_IdAdmisionMensajeria,
                NombreRemitente = sum.AGI_NombreRemitente,
                NombreDestinatario = sum.AGI_NombreDestinatario,
                DireccionDestinatario = sum.AGI_DireccionDestinatario,
                DireccionRemitente = sum.AGI_DireccionRemitente,
                TelefonoDestinatario = sum.AGI_TelefonoDestinatario,
                TelefonoRemitente = sum.AGI_TelefonoRemitente,
                GestionOrigen = new ARGestionDC() { Descripcion = sum.AGI_DescripcionGestionDest },
                GestionDestino = new ARGestionDC() { Descripcion = sum.AGI_DescripcionGestionOrig }
              }
            },
            Rango = new SURango()
            {
              Inicio = sum.SSS_Inicio,
              Fin = sum.SSS_Fin
            },

            ReferenciaUsoGuia = new CLReferenciaUsoGuiaDC()
            {
              CiudadDestino = sum.NombreCiudad,
              CiudadOrigen = sum.ReferenciaCiudadOrigen,
              CodigoPostalDestino = sum.RUG_CodigoPostalDestino,
              CodigoPostalOrigen = sum.RUG_CodigoPostalOrigen,
              DireccionDestino = sum.RUG_DireccionDestino,
              DireccionOrigen = sum.RUG_DireccionOrigen,
              EsDestionoAbierto = sum.RUG_EsDestionoAbierto == null ? true : sum.RUG_EsDestionoAbierto.Value,
              EsOrigenAbierto = sum.EsOrigenAbierto == null ? true : sum.EsOrigenAbierto.Value,
              IdentificacionDestino = sum.RUG_IdentificacionDestino,
              IdentificacionOrigen = sum.RUG_IdentificacionOrigen,
              NombreDestino = sum.RUG_NombreOrigen,
              NombreOrigen = sum.RUG_NombreOrigen,
              PaisDestino = sum.NombrePais,
              PaisOrigen = sum.ReferenciaPaisOrigen,
              TelefonoDestino = sum.RUG_TelefonoDestino,
              TelefonoOrigen = sum.RUG_TelefonoOrigen,
              TipoIdentificacionDestino = sum.RUG_TipoIdentificacionDestino,
              TipoIdentificacionOrigen = sum.ReferenciaTipoIdOrigen
            },
            Sucursal = new CLSucursalDC()
            {
              IdSucursal = sum.ReferenciaIdSucursal != null ? sum.ReferenciaIdSucursal.Value : 0
            }
          });
      }
    }


    /// <summary>
    /// Obtiene informacion de las provisiones de un suministro en un Mensajero
    /// </summary>
    /// <param name="fechaInicial">fecha inicial de la consulta por defecto deja la actual menos 3 dias</param>
    /// <param name="fechaFinal">fecha final de la consulta por defecto deja la actual</param>
    /// <param name="idAgencia">id de la agencia de los mensajeros</param>
    /// <param name="idSuministro">id del suministro</param>
    /// <param name="pageIndex">indice de la pagina</param>
    /// <param name="pageSize">tamaño de la paginacion</param>
    /// <returns>Lista con la informacion para impresion los suministro de un Mensajero</returns>
    public List<SUImpresionSumMensajeroDC> ObtenerProvisionesSuministroMensajero(DateTime? fechaInicial, DateTime? fechaFinal, long idAgencia, int idSuministro, int pageIndex, int pageSize)
    {
      using (ModeloSuministros contexto = new ModeloSuministros(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
      {
        fechaInicial = fechaInicial ?? DateTime.Now.AddDays(-3);
        fechaFinal = fechaFinal ?? DateTime.Now;

        return contexto.paObtenerProvisionesSumMensajero_SUM(fechaInicial, fechaFinal, idAgencia, idSuministro).ToList()
          .ConvertAll(sum => new SUImpresionSumMensajeroDC()
          {
            RemisionSuministro = new SURemisionSuministroDC()
            {
              IdCasaMatriz = sum.RES_IdCasaMatrizElabora,
              NumeroGuiaDespacho = sum.RES_NumeroGuiaInternaDespacho,
              Destinatario = sum.RES_NombreDestinatario,
              GuiaInterna = new ADGuiaInternaDC()
              {
                IdAdmisionGuia = sum.AGI_IdAdmisionMensajeria,
                NombreRemitente = sum.AGI_NombreRemitente,
                NombreDestinatario = sum.AGI_NombreDestinatario,
                DireccionDestinatario = sum.AGI_DireccionDestinatario,
                DireccionRemitente = sum.AGI_DireccionRemitente,
                TelefonoDestinatario = sum.AGI_TelefonoDestinatario,
                TelefonoRemitente = sum.AGI_TelefonoRemitente,
                GestionOrigen = new ARGestionDC() { Descripcion = sum.AGI_DescripcionGestionDest },
                GestionDestino = new ARGestionDC() { Descripcion = sum.AGI_DescripcionGestionOrig }
              }
            },
            CentroServicio = new PUCentroServiciosDC()
             {
               IdCentroServicio = sum.CES_IdCentroServicios,
               CiudadUbicacion = new PALocalidadDC() { Nombre = sum.LOC_Nombre, CodigoPostal = sum.LOC_CodigoPostal },
               PaisCiudad = new PALocalidadDC() { Nombre = sum.NombrePais }
             },
            Rango = new SURango()
            {
              Inicio = sum.MSS_Inicio,
              Fin = sum.MSS_Fin
            },
            InformacionMensajero = new PAPersonaInternaDC()
            {
              Identificacion = sum.PEI_Identificacion
            }
          });
      }
    }

    /// <summary>
    /// Obtiene informacion de las provisiones de un suministro en un Mensajero
    /// </summary>
    /// <param name="fechaInicial">fecha inicial de la consulta por defecto deja la actual menos 3 dias</param>
    /// <param name="fechaFinal">fecha final de la consulta por defecto deja la actual</param>
    /// <param name="idAgencia">id de la agencia de los mensajeros</param>
    /// <param name="idSuministro">id del suministro</param>
    /// <param name="pageIndex">indice de la pagina</param>
    /// <param name="pageSize">tamaño de la paginacion</param>
    /// <returns>Lista con la informacion para impresion los suministro de un Mensajero</returns>
    public List<SUImpresionSumMensajeroDC> ObtenerProvisionesSuministroMensajeroCiudadDestino(DateTime? fechaInicial, DateTime? fechaFinal, string idCiudadDestino, int idSuministro, int pageIndex, int pageSize)
    {
      using (ModeloSuministros contexto = new ModeloSuministros(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
      {
        fechaInicial = fechaInicial ?? DateTime.Now.AddDays(-3);
        fechaFinal = fechaFinal ?? DateTime.Now;

        return contexto.paObtenerProvisionesSumMensajeroCiudadDestino_SUM(fechaInicial, fechaFinal, idCiudadDestino, idSuministro).ToList()
          .ConvertAll(sum => new SUImpresionSumMensajeroDC()
          {
            RemisionSuministro = new SURemisionSuministroDC()
            {
              IdCasaMatriz = sum.RES_IdCasaMatrizElabora,
              NumeroGuiaDespacho = sum.RES_NumeroGuiaInternaDespacho,
              Destinatario = sum.RES_NombreDestinatario,
              GuiaInterna = new ADGuiaInternaDC()
              {
                IdAdmisionGuia = sum.AGI_IdAdmisionMensajeria,
                NombreRemitente = sum.AGI_NombreRemitente,
                NombreDestinatario = sum.AGI_NombreDestinatario,
                DireccionDestinatario = sum.AGI_DireccionDestinatario,
                DireccionRemitente = sum.AGI_DireccionRemitente,
                TelefonoDestinatario = sum.AGI_TelefonoDestinatario,
                TelefonoRemitente = sum.AGI_TelefonoRemitente,
                GestionOrigen = new ARGestionDC() { Descripcion = sum.AGI_DescripcionGestionDest },
                GestionDestino = new ARGestionDC() { Descripcion = sum.AGI_DescripcionGestionOrig }
              },
             FechaRemision = sum.MSS_FechaGrabacion,
             RangoFechaInicial = sum.MSS_FechaInicial,
             RangoFechaFinal = sum.MSS_FechaFinal,
            },
            CentroServicio = new PUCentroServiciosDC()
            {
              IdCentroServicio = sum.CES_IdCentroServicios,
              CiudadUbicacion = new PALocalidadDC() { Nombre = sum.LOC_Nombre, CodigoPostal = sum.LOC_CodigoPostal },
              PaisCiudad = new PALocalidadDC() { Nombre = sum.NombrePais }
            },
            Rango = new SURango()
            {
              Inicio = sum.MSS_Inicio,
              Fin = sum.MSS_Fin
            },
            InformacionMensajero = new PAPersonaInternaDC()
            {
              Identificacion = sum.PEI_Identificacion
            }
          });
      }
    }


    /// <summary>
    /// Obtiene informacion de las provisiones de un suministro en un centro de servicio
    /// en un rango de fechas
    /// </summary>
    /// <param name="fechaInicial">fecha inicial de la consulta por defecto deja la actual menos 3 dias</param>
    /// <param name="fechaFinal">fecha final de la consulta por defecto deja la actual</param>
    /// <param name="idCentroServicio">id del centros de servicio</param>
    /// <param name="idSuministro">id del suministro</param>
    /// <param name="pageIndex">indice de la pagina</param>
    /// <param name="pageSize">tamaño de la paginacion</param>
    /// <returns>Lista con la informacion para impresion los suministro de un Centro Servicio</returns>
    public List<SUImpresionSumCentroServicioDC> ObtenerProvisionesSuministroCentroServicio(DateTime? fechaInicial, DateTime? fechaFinal, long idCentroServicio, int idSuministro, int pageIndex, int pageSize)
    {
      using (ModeloSuministros contexto = new ModeloSuministros(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
      {
        fechaInicial = fechaInicial ?? DateTime.Now.AddDays(-3);
        fechaFinal = fechaFinal ?? DateTime.Now;

        return contexto.paObtenerProvisionesSumCentroServicio_SUM(fechaInicial, fechaFinal, idCentroServicio, idSuministro).ToList()
           .ConvertAll(sum => new SUImpresionSumCentroServicioDC()
           {
             RemisionSuministro = new SURemisionSuministroDC()
             {
               IdCasaMatriz = sum.RES_IdCasaMatrizElabora,
               NumeroGuiaDespacho = sum.RES_NumeroGuiaInternaDespacho,
               Destinatario = sum.RES_NombreDestinatario,
               GuiaInterna = new ADGuiaInternaDC()
               {
                 IdAdmisionGuia = sum.AGI_IdAdmisionMensajeria,
                 NombreRemitente = sum.AGI_NombreRemitente,
                 NombreDestinatario = sum.AGI_NombreDestinatario,
                 DireccionDestinatario = sum.AGI_DireccionDestinatario,
                 DireccionRemitente = sum.AGI_DireccionRemitente,
                 TelefonoDestinatario = sum.AGI_TelefonoDestinatario,
                 TelefonoRemitente = sum.AGI_TelefonoRemitente,
                 GestionOrigen = new ARGestionDC() { Descripcion = sum.AGI_DescripcionGestionDest },
                 GestionDestino = new ARGestionDC() { Descripcion = sum.AGI_DescripcionGestionOrig }
               }
             },
             CentroServicio = new PUCentroServiciosDC()
             {
               IdCentroServicio = sum.CES_IdCentroServicios,
               CiudadUbicacion = new PALocalidadDC() { Nombre = sum.NombreCiudad, CodigoPostal = sum.LOC_CodigoPostal },
               PaisCiudad = new PALocalidadDC() { Nombre = sum.NombrePais }
             },
             Rango = new SURango()
             {
               Inicio = sum.PCS_Inicio,
               Fin = sum.PCS_Fin
             }
           });
      }
    }


    /// <summary>
    /// Obtiene informacion de las provisiones de un suministro por ciudad de destino
    /// en un rango de fechas
    /// </summary>
    /// <param name="fechaInicial">fecha inicial de la consulta por defecto deja la actual menos 3 dias</param>
    /// <param name="fechaFinal">fecha final de la consulta por defecto deja la actual</param>
    /// <param name="idCentroServicio">id del centros de servicio</param>
    /// <param name="idSuministro">id del suministro</param>
    /// <param name="pageIndex">indice de la pagina</param>
    /// <param name="pageSize">tamaño de la paginacion</param>
    /// <returns>Lista con la informacion para impresion los suministro de un Centro Servicio</returns>
    public List<SUImpresionSumCentroServicioDC> ObtenerProvisionesSuministroCentroServicioCiudadDestino(DateTime? fechaInicial, DateTime? fechaFinal, string idCiudadDestino, int idSuministro, int pageIndex, int pageSize)
    {
      using (ModeloSuministros contexto = new ModeloSuministros(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
      {
        fechaInicial = fechaInicial ?? DateTime.Now.AddDays(-3);
        fechaFinal = fechaFinal ?? DateTime.Now;

        return contexto.paObtenerProvisionesSumCiudadDestino_SUM(fechaInicial, fechaFinal, idCiudadDestino, idSuministro).ToList()
           .ConvertAll(sum => new SUImpresionSumCentroServicioDC()
           {
             RemisionSuministro = new SURemisionSuministroDC()
             {
               IdCasaMatriz = sum.RES_IdCasaMatrizElabora,
               NumeroGuiaDespacho = sum.RES_NumeroGuiaInternaDespacho,
               Destinatario = sum.RES_NombreDestinatario,
               GuiaInterna = new ADGuiaInternaDC()
               {
                 IdAdmisionGuia = sum.AGI_IdAdmisionMensajeria,
                 NombreRemitente = sum.AGI_NombreRemitente,
                 NombreDestinatario = sum.AGI_NombreDestinatario,
                 DireccionDestinatario = sum.AGI_DireccionDestinatario,
                 DireccionRemitente = sum.AGI_DireccionRemitente,
                 TelefonoDestinatario = sum.AGI_TelefonoDestinatario,
                 TelefonoRemitente = sum.AGI_TelefonoRemitente,
                 GestionOrigen = new ARGestionDC() { Descripcion = sum.AGI_DescripcionGestionDest },
                 GestionDestino = new ARGestionDC() { Descripcion = sum.AGI_DescripcionGestionOrig }
               }
             },
             CentroServicio = new PUCentroServiciosDC()
             {
               IdCentroServicio = sum.CES_IdCentroServicios,
               CiudadUbicacion = new PALocalidadDC() { Nombre = sum.NombreCiudad, CodigoPostal = sum.LOC_CodigoPostal, IdLocalidad=sum.CES_IdMunicipio },
               PaisCiudad = new PALocalidadDC() { Nombre = sum.NombrePais },
                Direccion= sum.AGI_DireccionRemitente,
                 Nombre = sum.CES_Nombre,
                  Telefono1 = sum.CES_Telefono1

             },
             Rango = new SURango()
             {
               Inicio = sum.PCS_Inicio,
               Fin = sum.PCS_Fin
             }
           });
      }
    }


    #endregion Impresion Suministros Por Centro de Servicio

    #region Consultar Suministros por rango

    /// <summary>
    /// Obtiene las agencias que tienen suministros en los rangos ingresados por parametros
    /// en un rango de fechas
    /// </summary>
    /// <param name="fechaInicial">fecha inicial de la consulta por defecto deja la actual menos 3 dias</param>
    /// <param name="fechaFinal">fecha final de la consulta por defecto deja la actual</param>
    /// <param name="rangoInicial">rango inicial</param>
    /// <param name="rangoFinal">rango final</param>
    /// <param name="pageIndex">indice de la pagina</param>
    /// <param name="pageSize">tamaño de la paginacion</param>
    /// <returns>Lista con la informacion para impresion los suministro de un Centro Servicio</returns>
    public List<SUImpresionSumCentroServicioDC> ObtenerProvisionSuministroCentroServicioPorRango(SUFiltroSuministroPorRangoDC filtroPorRango)
    {
      using (ModeloSuministros contexto = new ModeloSuministros(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
      {
        filtroPorRango.FechaInicial = filtroPorRango.FechaInicial ?? DateTime.Now.AddYears(-1);
        filtroPorRango.FechaFinal = filtroPorRango.FechaFinal ?? DateTime.Now;

        return contexto.paObtenerProvisionSumCentroServicioPorRango_SUM(filtroPorRango.FechaInicial, filtroPorRango.FechaFinal, filtroPorRango.RangoInicial, filtroPorRango.RangoFinal, filtroPorRango.IdSuministro).ToList()
           .ConvertAll(sum => new SUImpresionSumCentroServicioDC()
           {
             RemisionSuministro = new SURemisionSuministroDC()
             {
               IdCasaMatriz = sum.RES_IdCasaMatrizElabora,
               NumeroGuiaDespacho = sum.RES_NumeroGuiaInternaDespacho,
               Destinatario = sum.RES_NombreDestinatario,
               GuiaInterna = new ADGuiaInternaDC()
               {
                IdAdmisionGuia = sum.AGI_IdAdmisionMensajeria ?? 0 ,
                 NombreRemitente = sum.AGI_NombreRemitente ?? string.Empty,
                 NombreDestinatario = sum.AGI_NombreDestinatario ?? string.Empty,
                 DireccionDestinatario = sum.AGI_DireccionDestinatario ?? string.Empty,
                 DireccionRemitente = sum.AGI_DireccionRemitente ?? string.Empty,
                 DiceContener = string.Empty,
                 TelefonoDestinatario = sum.AGI_TelefonoDestinatario ?? string.Empty,
                 TelefonoRemitente = sum.AGI_TelefonoRemitente ?? string.Empty,
                 GestionOrigen = new ARGestionDC() { IdGestion = 0, Descripcion = sum.AGI_DescripcionGestionDest ?? string.Empty },
                 GestionDestino = new ARGestionDC() { IdGestion = 0, Descripcion = sum.AGI_DescripcionGestionOrig ?? string.Empty },
                 NombreCentroServicioDestino = string.Empty,
                 NombreCentroServicioOrigen = string.Empty,
                 LocalidadOrigen = new PALocalidadDC { IdLocalidad = string.Empty, Nombre = string.Empty },
                 LocalidadDestino = new PALocalidadDC { IdLocalidad = string.Empty, Nombre = string.Empty },
               }
             },
             CentroServicio = new PUCentroServiciosDC()
             {
               IdCentroServicio = sum.CES_IdCentroServicios,
               Nombre = sum.CES_Nombre,
               Telefono1 = sum.CES_Telefono1,
               Direccion = sum.CES_Direccion,
               CiudadUbicacion = new PALocalidadDC() { Nombre = sum.NombreCiudad, CodigoPostal = sum.LOC_CodigoPostal },
               PaisCiudad = new PALocalidadDC() { Nombre = sum.NombrePais }
             },
             Rango = new SURango()
             {
               Inicio = sum.PCS_Inicio,
               Fin = sum.PCS_Fin
             }
           });
      }
    }

    /// <summary>
    /// Obtiene las sucursales que tienen suministros en los rangos ingresados por parametros y en
    /// en un rango de fechas
    /// </summary>
    /// <param name="fechaInicial">fecha inicial de la consulta por defecto deja la actual menos 3 dias</param>
    /// <param name="fechaFinal">fecha final de la consulta por defecto deja la actual</param>
    /// <param name="rangoInicial">rango inicial</param>
    /// <param name="rangoFinal">rango final</param>
    /// <param name="pageIndex">indice de la pagina</param>
    /// <param name="pageSize">tamaño de la paginacion</param>
    /// <returns>Lista con la informacion para impresion los suministro de una sucursal</returns>
    public List<SUImpresionSumSucursalDC> ObtenerProvisionSumSucursalPorRango(SUFiltroSuministroPorRangoDC filtroPorRango)
    {
      using (ModeloSuministros contexto = new ModeloSuministros(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
      {
        if (filtroPorRango.FechaInicial == ConstantesFramework.MinDateTimeController)
          filtroPorRango.FechaFinal = DateTime.Now;
        return contexto.paObtenerProvisionSumSucursalPorRango_SUM(filtroPorRango.FechaInicial, filtroPorRango.FechaFinal, filtroPorRango.RangoInicial, filtroPorRango.RangoFinal, filtroPorRango.IdSuministro).ToList()
          .ConvertAll(sum => new SUImpresionSumSucursalDC()
          {
            RemisionSuministro = new SURemisionSuministroDC()
            {
              IdCasaMatriz = sum.RES_IdCasaMatrizElabora,
              NumeroGuiaDespacho = sum.RES_NumeroGuiaInternaDespacho,
              Destinatario = sum.RES_NombreDestinatario,
              GuiaInterna = new ADGuiaInternaDC()
              {
                IdAdmisionGuia = sum.AGI_IdAdmisionMensajeria ?? 0,
                NombreRemitente = sum.AGI_NombreRemitente ,
                NombreDestinatario = sum.AGI_NombreDestinatario,
                DireccionDestinatario = sum.AGI_DireccionDestinatario,
                DireccionRemitente = sum.AGI_DireccionRemitente,
                TelefonoDestinatario = sum.AGI_TelefonoDestinatario,
                TelefonoRemitente = sum.AGI_TelefonoRemitente,
                GestionOrigen = new ARGestionDC() { Descripcion = sum.AGI_DescripcionGestionDest },
                GestionDestino = new ARGestionDC() { Descripcion = sum.AGI_DescripcionGestionOrig }
              }
            },
            Rango = new SURango()
            {
              Inicio = sum.SSS_Inicio,
              Fin = sum.SSS_Fin
            },

            ReferenciaUsoGuia = new CLReferenciaUsoGuiaDC()
            {
              CiudadDestino = sum.NombreCiudadDestino,
              CiudadOrigen = sum.NombreCiudadOrigen,
              CodigoPostalDestino = sum.CodigoPostalCiudadDestino,
              CodigoPostalOrigen = sum.CodigoPostalCiudadOrigen,
              DireccionDestino = sum.RUG_DireccionDestino,
              DireccionOrigen = sum.RUG_DireccionOrigen,
              EsDestionoAbierto = sum.RUG_EsDestionoAbierto !=null ?sum.RUG_EsDestionoAbierto.Value:true,
              EsOrigenAbierto = sum.RUG_EsOrigenAbierto !=null?sum.RUG_EsOrigenAbierto.Value:true,
              IdentificacionDestino = sum.RUG_IdentificacionDestino,
              IdentificacionOrigen = sum.RUG_IdentificacionOrigen,
              NombreDestino = sum.RUG_NombreDestino,
              NombreOrigen = sum.RUG_NombreOrigen,
              PaisDestino = sum.NombrePaisCiudadDestino,
              PaisOrigen = sum.NombrePaisCiudadOrigen,
              TelefonoDestino = sum.RUG_TelefonoDestino,
              TelefonoOrigen = sum.RUG_TelefonoOrigen,
              TipoIdentificacionDestino = sum.RUG_TipoIdentificacionDestino,
              TipoIdentificacionOrigen = sum.RUG_TipoIdentificacionOrigen
            },
            Sucursal = new CLSucursalDC()
            {
              IdSucursal = sum.SUC_IdSucursal
            }
          });
      }
    }

    /// <summary>
    /// Obtiene informacion de las provisiones de un suministro en un Mensajero en los rangos ingresados por parametros y en
    /// en un rango de fechas
    /// </summary>
    /// <param name="fechaInicial">fecha inicial de la consulta por defecto deja la actual menos 3 dias</param>
    /// <param name="fechaFinal">fecha final de la consulta por defecto deja la actual</param>
    /// <param name="rangoInicial">rango inicial</param>
    /// <param name="rangoFinal">rango final</param>
    /// <param name="pageIndex">indice de la pagina</param>
    /// <param name="pageSize">tamaño de la paginacion</param>
    /// <returns>Lista con la informacion para impresion los suministro de un mensajero</returns>
    public List<SUImpresionSumMensajeroDC> ObtenerProvisionSumMensajeroPorRango(SUFiltroSuministroPorRangoDC filtroPorRango)
    {
      using (ModeloSuministros contexto = new ModeloSuministros(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
      {
        filtroPorRango.FechaInicial = filtroPorRango.FechaInicial ?? DateTime.Now.AddDays(-3);
        filtroPorRango.FechaFinal = filtroPorRango.FechaFinal ?? DateTime.Now;

        return contexto.paObtenerProvisionSumMensajeroPorRango_SUM(filtroPorRango.FechaInicial, filtroPorRango.FechaFinal, filtroPorRango.RangoInicial, filtroPorRango.RangoFinal, filtroPorRango.IdSuministro).ToList()
           .ConvertAll(sum => new SUImpresionSumMensajeroDC()
           {
             RemisionSuministro = new SURemisionSuministroDC()
             {
               IdCasaMatriz = sum.RES_IdCasaMatrizElabora,
               NumeroGuiaDespacho = sum.RES_NumeroGuiaInternaDespacho,
               Destinatario = sum.RES_NombreDestinatario,
               GuiaInterna = new ADGuiaInternaDC()
               {
                 IdAdmisionGuia = sum.AGI_IdAdmisionMensajeria,
                 NombreRemitente = sum.AGI_NombreRemitente,
                 NombreDestinatario = sum.AGI_NombreDestinatario,
                 DireccionDestinatario = sum.AGI_DireccionDestinatario,
                 DireccionRemitente = sum.AGI_DireccionRemitente,
                 TelefonoDestinatario = sum.AGI_TelefonoDestinatario,
                 TelefonoRemitente = sum.AGI_TelefonoRemitente,
                 GestionOrigen = new ARGestionDC() { Descripcion = sum.AGI_DescripcionGestionDest },
                 GestionDestino = new ARGestionDC() { Descripcion = sum.AGI_DescripcionGestionOrig }
               }
             },
             CentroServicio = new PUCentroServiciosDC()
             {
               IdCentroServicio = sum.CES_IdCentroServicios,
               CiudadUbicacion = new PALocalidadDC() { Nombre = sum.NombreCiudad, CodigoPostal = sum.LOC_CodigoPostal },
               PaisCiudad = new PALocalidadDC() { Nombre = sum.NombrePais }
             },
             Rango = new SURango()
             {
               Prefijo = sum.MSS_Prefijo,
               Inicio = sum.MSS_Inicio,
               Fin = sum.MSS_Fin
             },
             InformacionMensajero = new PAPersonaInternaDC()
             {
               Nombre = sum.PEI_Nombre,
               PrimerApellido = sum.PEI_PrimerApellido,
               SegundoApellido = sum.PEI_SegundoApellido,
               Identificacion = sum.PEI_Identificacion,
               IdTipoIdentificacion = sum.PEI_IdTipoIdentificacion,
               Telefono = sum.MEN_Telefono2
             }
           });
      }
    }

    /// <summary>
    /// Obtiene informacion de las provisiones de las gestiones en los rangos ingresados por parametros y en
    /// en un rango de fechas
    /// </summary>
    /// <param name="fechaInicial">fecha inicial de la consulta por defecto deja la actual menos 3 dias</param>
    /// <param name="fechaFinal">fecha final de la consulta por defecto deja la actual</param>
    /// <param name="rangoInicial">rango inicial</param>
    /// <param name="rangoFinal">rango final</param>
    /// <param name="pageIndex">indice de la pagina</param>
    /// <param name="pageSize">tamaño de la paginacion</param>
    /// <returns>Lista con la informacion para impresion los suministro de un mensajero</returns>
    public List<SUImpresionSumGestionDC> ObtenerProvisionSumGestionPorRango(SUFiltroSuministroPorRangoDC filtroPorRango)
    {
      using (ModeloSuministros contexto = new ModeloSuministros(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
      {
        filtroPorRango.FechaInicial = filtroPorRango.FechaInicial ?? DateTime.Now.AddDays(-3);
        filtroPorRango.FechaFinal = filtroPorRango.FechaFinal ?? DateTime.Now;

        return contexto.paObtenerProvisionSumGestionPorRango_SUM(filtroPorRango.FechaInicial, filtroPorRango.FechaFinal, filtroPorRango.RangoInicial, filtroPorRango.RangoFinal, filtroPorRango.IdSuministro).ToList()
           .ConvertAll(sum => new SUImpresionSumGestionDC()
           {
             RemisionSuministro = new SURemisionSuministroDC()
             {
               IdRemision = sum.RES_IdRemisionSuministros,
               IdCasaMatriz = sum.RES_IdCasaMatrizElabora,
               NumeroGuiaDespacho = sum.RES_NumeroGuiaInternaDespacho,
               Destinatario = sum.RES_NombreDestinatario,
               GuiaInterna = new ADGuiaInternaDC()
               {
                 IdAdmisionGuia = sum.AGI_IdAdmisionMensajeria,
                 NombreRemitente = sum.AGI_NombreRemitente,
                 NombreDestinatario = sum.AGI_NombreDestinatario,
                 NombreCentroServicioDestino = sum.NombreCiudadDestino,
                 DireccionDestinatario = sum.AGI_DireccionDestinatario,
                 DireccionRemitente = sum.AGI_DireccionRemitente,
                 TelefonoDestinatario = sum.AGI_TelefonoDestinatario,
                 TelefonoRemitente = sum.AGI_TelefonoRemitente,
                 GestionOrigen = new ARGestionDC() { Descripcion = sum.AGI_DescripcionGestionDest },
                 GestionDestino = new ARGestionDC() { Descripcion = sum.AGI_DescripcionGestionOrig },
               }
             },
             Rango = new SURango()
             {
               Prefijo = sum.PPS_Prefijo,
               Inicio = sum.PPS_Inicio,
               Fin = sum.PPS_Fin
             },
             CasaMatriz = new ARCasaMatrizDC()
             {
               Estado = sum.CAM_Estado,
               CodigoMinisterio = sum.CAM_CodigoMinisterio,
               CentroCostos = sum.CAM_CentroCostos,
               IdLocalidad = sum.CAM_IdLocalidad,
               Telefono = sum.CAM_Telefono,
               IdCasaMatriz = sum.CAM_IdCasaMatriz,
               Nombre = sum.CAM_Nombre,
               Nit = sum.CAM_Nit,
               DigitoVerificacion = sum.CAM_DigitoVerificacion,
               CodigoSucursalERP = sum.CAM_CodigoSucursalERP,
               Sigla = sum.CAM_Sigla,
               Direccion = sum.CAM_Direccion,
               NombreLocalidad = sum.NombreCiudadCasaMatriz
             }
           });
      }
    }

    #endregion Consultar Suministros por rango

    #region Consultar Suministros por Usuario

    /// <summary>
    /// Obtiene las agencias que tienen suministros creados por un usuario
    /// en un rango de fechas
    /// </summary>
    /// <param name="fechaInicial">fecha inicial de la consulta por defecto deja la actual menos 3 dias</param>
    /// <param name="fechaFinal">fecha final de la consulta por defecto deja la actual</param>
    /// <param name="rangoInicial">rango inicial</param>
    /// <param name="rangoFinal">rango final</param>
    /// <param name="pageIndex">indice de la pagina</param>
    /// <param name="pageSize">tamaño de la paginacion</param>
    /// <returns>Lista con la informacion para impresion los suministro de un Centro Servicio</returns>
    public List<SUImpresionSumCentroServicioDC> ObtenerProvisionSuministroCentroServicioPorUsuario(SUFiltroSuministroPorUsuarioDC filtroPorUsuario)
    {
      using (ModeloSuministros contexto = new ModeloSuministros(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
      {
        filtroPorUsuario.FechaInicial = filtroPorUsuario.FechaInicial ?? DateTime.Now.AddYears(-1);
        filtroPorUsuario.FechaFinal = filtroPorUsuario.FechaFinal ?? DateTime.Now;

        return contexto.paObtenerProvisionesSumCentroServicioPorUsuario_SUM(filtroPorUsuario.FechaInicial, filtroPorUsuario.FechaFinal, filtroPorUsuario.Usuario, filtroPorUsuario.IdSuministro).ToList()
           .ConvertAll(sum => new SUImpresionSumCentroServicioDC()
           {
             RemisionSuministro = new SURemisionSuministroDC()
             {
               IdCasaMatriz = sum.RES_IdCasaMatrizElabora,
               NumeroGuiaDespacho = sum.RES_NumeroGuiaInternaDespacho,
               Destinatario = sum.RES_NombreDestinatario,
               GuiaInterna = new ADGuiaInternaDC()
               {
                 IdAdmisionGuia = sum.AGI_IdAdmisionMensajeria,
                 NombreRemitente = sum.AGI_NombreRemitente ?? string.Empty,
                 NombreDestinatario = sum.AGI_NombreDestinatario ?? string.Empty,
                 DireccionDestinatario = sum.AGI_DireccionDestinatario ?? string.Empty,
                 DireccionRemitente = sum.AGI_DireccionRemitente ?? string.Empty,
                 DiceContener = string.Empty,
                 TelefonoDestinatario = sum.AGI_TelefonoDestinatario ?? string.Empty,
                 TelefonoRemitente = sum.AGI_TelefonoRemitente ?? string.Empty,
                 GestionOrigen = new ARGestionDC() { IdGestion = 0, Descripcion = sum.AGI_DescripcionGestionDest ?? string.Empty },
                 GestionDestino = new ARGestionDC() { IdGestion = 0, Descripcion = sum.AGI_DescripcionGestionOrig ?? string.Empty },
                 NombreCentroServicioDestino = string.Empty,
                 NombreCentroServicioOrigen = string.Empty,
                 LocalidadOrigen = new PALocalidadDC { IdLocalidad = string.Empty, Nombre = string.Empty },
                 LocalidadDestino = new PALocalidadDC { IdLocalidad = string.Empty, Nombre = string.Empty },
               }
             },
             CentroServicio = new PUCentroServiciosDC()
             {
               IdCentroServicio = sum.CES_IdCentroServicios,
               Nombre = sum.CES_Nombre,
               Telefono1 = sum.CES_Telefono1,
               Direccion = sum.CES_Direccion,
               CiudadUbicacion = new PALocalidadDC() { Nombre = sum.NombreCiudad, CodigoPostal = sum.LOC_CodigoPostal },
               PaisCiudad = new PALocalidadDC() { Nombre = sum.NombrePais }
             },
             Rango = new SURango()
             {
               Inicio = sum.PCS_Inicio,
               Fin = sum.PCS_Fin
             }
           });
      }
    }

    /// <summary>
    /// Obtiene las sucursales que tienen suministros por un usuario y en
    /// en un rango de fechas
    /// </summary>
    /// <param name="fechaInicial">fecha inicial de la consulta por defecto deja la actual menos 3 dias</param>
    /// <param name="fechaFinal">fecha final de la consulta por defecto deja la actual</param>
    /// <param name="rangoInicial">rango inicial</param>
    /// <param name="rangoFinal">rango final</param>
    /// <param name="pageIndex">indice de la pagina</param>
    /// <param name="pageSize">tamaño de la paginacion</param>
    /// <returns>Lista con la informacion para impresion los suministro de una sucursal</returns>
    public List<SUImpresionSumSucursalDC> ObtenerProvisionSumSucursalPorUsuario(SUFiltroSuministroPorUsuarioDC filtroPorUsuario)
    {
      using (ModeloSuministros contexto = new ModeloSuministros(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
      {
        if (filtroPorUsuario.FechaInicial == ConstantesFramework.MinDateTimeController)
          filtroPorUsuario.FechaFinal = DateTime.Now;

        return contexto.paObtenerProvisionSumSucursalPorUsuario_SUM(filtroPorUsuario.FechaInicial, filtroPorUsuario.FechaFinal, filtroPorUsuario.IdSuministro, filtroPorUsuario.Usuario).ToList()
          .ConvertAll(sum => new SUImpresionSumSucursalDC()
          {
            RemisionSuministro = new SURemisionSuministroDC()
            {
              IdCasaMatriz = sum.RES_IdCasaMatrizElabora,
              NumeroGuiaDespacho = sum.RES_NumeroGuiaInternaDespacho,
              Destinatario = sum.RES_NombreDestinatario,
              GuiaInterna = new ADGuiaInternaDC()
              {
                IdAdmisionGuia = sum.AGI_IdAdmisionMensajeria,
                NombreRemitente = sum.AGI_NombreRemitente,
                NombreDestinatario = sum.AGI_NombreDestinatario,
                DireccionDestinatario = sum.AGI_DireccionDestinatario,
                DireccionRemitente = sum.AGI_DireccionRemitente,
                TelefonoDestinatario = sum.AGI_TelefonoDestinatario,
                TelefonoRemitente = sum.AGI_TelefonoRemitente,
                GestionOrigen = new ARGestionDC() { Descripcion = sum.AGI_DescripcionGestionDest },
                GestionDestino = new ARGestionDC() { Descripcion = sum.AGI_DescripcionGestionOrig }
              }
            },
            Rango = new SURango()
            {
              Inicio = sum.SSS_Inicio,
              Fin = sum.SSS_Fin
            },

            ReferenciaUsoGuia = new CLReferenciaUsoGuiaDC()
            {
              CiudadDestino = sum.NombreCiudadDestino,
              CiudadOrigen = sum.NombreCiudadOrigen,
              CodigoPostalDestino = sum.CodigoPostalCiudadDestino,
              CodigoPostalOrigen = sum.CodigoPostalCiudadOrigen,
              DireccionDestino = sum.RUG_DireccionDestino,
              DireccionOrigen = sum.RUG_DireccionOrigen,
              EsDestionoAbierto = sum.RUG_EsDestionoAbierto!=null? sum.RUG_EsDestionoAbierto.Value:true,
              EsOrigenAbierto = sum.RUG_EsOrigenAbierto!=null?sum.RUG_EsOrigenAbierto.Value:true,
              IdentificacionDestino = sum.RUG_IdentificacionDestino,
              IdentificacionOrigen = sum.RUG_IdentificacionOrigen,
              NombreDestino = sum.RUG_NombreDestino,
              NombreOrigen = sum.RUG_NombreOrigen,
              PaisDestino = sum.NombrePaisCiudadDestino,
              PaisOrigen = sum.NombrePaisCiudadOrigen,
              TelefonoDestino = sum.RUG_TelefonoDestino,
              TelefonoOrigen = sum.RUG_TelefonoOrigen,
              TipoIdentificacionDestino = sum.RUG_TipoIdentificacionDestino,
              TipoIdentificacionOrigen = sum.RUG_TipoIdentificacionOrigen
            },
            Sucursal = new CLSucursalDC()
            {
              IdSucursal = sum.SUC_IdSucursal
            }
          });
      }
    }

    /// <summary>
    /// Obtiene informacion de las provisiones de un suministro en un Mensajero por el usuario de creacion y en
    /// en un rango de fechas
    /// </summary>
    /// <param name="fechaInicial">fecha inicial de la consulta por defecto deja la actual menos 3 dias</param>
    /// <param name="fechaFinal">fecha final de la consulta por defecto deja la actual</param>
    /// <param name="rangoInicial">rango inicial</param>
    /// <param name="rangoFinal">rango final</param>
    /// <param name="pageIndex">indice de la pagina</param>
    /// <param name="pageSize">tamaño de la paginacion</param>
    /// <returns>Lista con la informacion para impresion los suministro de un mensajero</returns>
    public List<SUImpresionSumMensajeroDC> ObtenerProvisionSumMensajeroPorUsuario(SUFiltroSuministroPorUsuarioDC filtroPorUsuario)
    {
      using (ModeloSuministros contexto = new ModeloSuministros(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
      {
        filtroPorUsuario.FechaInicial = filtroPorUsuario.FechaInicial ?? DateTime.Now.AddDays(-3);
        filtroPorUsuario.FechaFinal = filtroPorUsuario.FechaFinal ?? DateTime.Now;

        return contexto.paObtenerProvisionSumMensajeroPorUsuario_SUM(filtroPorUsuario.FechaInicial, filtroPorUsuario.FechaFinal, filtroPorUsuario.IdSuministro, filtroPorUsuario.Usuario).ToList()
           .ConvertAll(sum => new SUImpresionSumMensajeroDC()
           {
             RemisionSuministro = new SURemisionSuministroDC()
             {
               IdCasaMatriz = sum.RES_IdCasaMatrizElabora,
               NumeroGuiaDespacho = sum.RES_NumeroGuiaInternaDespacho,
               Destinatario = sum.RES_NombreDestinatario,
               GuiaInterna = new ADGuiaInternaDC()
               {
                 IdAdmisionGuia = sum.AGI_IdAdmisionMensajeria,
                 NombreRemitente = sum.AGI_NombreRemitente,
                 NombreDestinatario = sum.AGI_NombreDestinatario,
                 DireccionDestinatario = sum.AGI_DireccionDestinatario,
                 DireccionRemitente = sum.AGI_DireccionRemitente,
                 TelefonoDestinatario = sum.AGI_TelefonoDestinatario,
                 TelefonoRemitente = sum.AGI_TelefonoRemitente,
                 GestionOrigen = new ARGestionDC() { Descripcion = sum.AGI_DescripcionGestionDest },
                 GestionDestino = new ARGestionDC() { Descripcion = sum.AGI_DescripcionGestionOrig }
               }
             },
             CentroServicio = new PUCentroServiciosDC()
             {
               IdCentroServicio = sum.CES_IdCentroServicios,
               CiudadUbicacion = new PALocalidadDC() { Nombre = sum.NombreCiudad, CodigoPostal = sum.LOC_CodigoPostal },
               PaisCiudad = new PALocalidadDC() { Nombre = sum.NombrePais }
             },
             Rango = new SURango()
             {
               Prefijo = sum.MSS_Prefijo,
               Inicio = sum.MSS_Inicio,
               Fin = sum.MSS_Fin
             },
             InformacionMensajero = new PAPersonaInternaDC()
             {
               Nombre = sum.PEI_Nombre,
               PrimerApellido = sum.PEI_PrimerApellido,
               SegundoApellido = sum.PEI_SegundoApellido,
               Identificacion = sum.PEI_Identificacion,
               IdTipoIdentificacion = sum.PEI_IdTipoIdentificacion,
               Telefono = sum.MEN_Telefono2
             }
           });
      }
    }

    /// <summary>
    /// Obtiene informacion de las provisiones de las gestiones en los rangos ingresados por parametros y en
    /// en un rango de fechas
    /// </summary>
    /// <param name="fechaInicial">fecha inicial de la consulta por defecto deja la actual menos 3 dias</param>
    /// <param name="fechaFinal">fecha final de la consulta por defecto deja la actual</param>
    /// <param name="rangoInicial">rango inicial</param>
    /// <param name="rangoFinal">rango final</param>
    /// <param name="pageIndex">indice de la pagina</param>
    /// <param name="pageSize">tamaño de la paginacion</param>
    /// <returns>Lista con la informacion para impresion los suministro de un mensajero</returns>
    public List<SUImpresionSumGestionDC> ObtenerProvisionSumGestionPorUsuario(SUFiltroSuministroPorUsuarioDC filtroPorRango)
    {
      using (ModeloSuministros contexto = new ModeloSuministros(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
      {
        filtroPorRango.FechaInicial = filtroPorRango.FechaInicial ?? DateTime.Now.AddDays(-3);
        filtroPorRango.FechaFinal = filtroPorRango.FechaFinal ?? DateTime.Now;

        return contexto.paObtenerProvisionSumGestionPorUsuario_SUM(filtroPorRango.FechaInicial, filtroPorRango.FechaFinal, filtroPorRango.IdSuministro, filtroPorRango.Usuario).ToList()
           .ConvertAll(sum => new SUImpresionSumGestionDC()
           {
             RemisionSuministro = new SURemisionSuministroDC()
             {
               IdRemision = sum.RES_IdRemisionSuministros,
               IdCasaMatriz = sum.RES_IdCasaMatrizElabora,
               NumeroGuiaDespacho = sum.RES_NumeroGuiaInternaDespacho,
               Destinatario = sum.RES_NombreDestinatario,
               GuiaInterna = new ADGuiaInternaDC()
               {
                 IdAdmisionGuia = sum.AGI_IdAdmisionMensajeria,
                 NombreRemitente = sum.AGI_NombreRemitente,
                 NombreDestinatario = sum.AGI_NombreDestinatario,
                 NombreCentroServicioDestino = sum.NombreCiudadDestino,
                 DireccionDestinatario = sum.AGI_DireccionDestinatario,
                 DireccionRemitente = sum.AGI_DireccionRemitente,
                 TelefonoDestinatario = sum.AGI_TelefonoDestinatario,
                 TelefonoRemitente = sum.AGI_TelefonoRemitente,
                 GestionOrigen = new ARGestionDC() { Descripcion = sum.AGI_DescripcionGestionDest },
                 GestionDestino = new ARGestionDC() { Descripcion = sum.AGI_DescripcionGestionOrig },
               }
             },
             Rango = new SURango()
             {
               Prefijo = sum.PPS_Prefijo,
               Inicio = sum.PPS_Inicio,
               Fin = sum.PPS_Fin
             },
             CasaMatriz = new ARCasaMatrizDC()
             {
               Estado = sum.CAM_Estado,
               CodigoMinisterio = sum.CAM_CodigoMinisterio,
               CentroCostos = sum.CAM_CentroCostos,
               IdLocalidad = sum.CAM_IdLocalidad,
               Telefono = sum.CAM_Telefono,
               IdCasaMatriz = sum.CAM_IdCasaMatriz,
               Nombre = sum.CAM_Nombre,
               Nit = sum.CAM_Nit,
               DigitoVerificacion = sum.CAM_DigitoVerificacion,
               CodigoSucursalERP = sum.CAM_CodigoSucursalERP,
               Sigla = sum.CAM_Sigla,
               Direccion = sum.CAM_Direccion,
               NombreLocalidad = sum.NombreCiudadCasaMatriz
             }
           });
      }
    }

    #endregion Consultar Suministros por Usuario

    #region Consultar Suministros Por Remision

    /// <summary>
    /// Obtiene informacion de las provisiones de un suministro en un centro de servicio por numero de remision
    /// </summary>
    /// <param name="fechaInicial">fecha inicial de la consulta por defecto deja la actual menos 3 dias</param>
    /// <param name="fechaFinal">fecha final de la consulta por defecto deja la actual</param>
    /// <param name="usuario">Usuario que realizo la remision</param>
    /// <param name="remisionInicial">numero de la remision inicial</param>
    /// <param name="remisionFinal">numero de la remision final</param>
    /// <param name="pageIndex">indice de la pagina</param>
    /// <param name="pageSize">tamaño de la paginacion</param>
    /// <returns>Lista con la informacion para impresion los suministro de un Centro Servicio</returns>
    public List<SUImpresionSumCentroServicioDC> ObtenerProvisionesSumCentroServicioPorRemision(SUFiltroSuministroPorRemisionDC filtroPorRemision)
    {
      using (ModeloSuministros contexto = new ModeloSuministros(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
      {
        filtroPorRemision.FechaInicial = filtroPorRemision.FechaInicial ?? DateTime.Now.AddDays(-3);
        filtroPorRemision.FechaFinal = filtroPorRemision.FechaFinal ?? DateTime.Now;

        return contexto.paObtenerProvisionesSumCentroServicioPorRemision_SUM(filtroPorRemision.FechaInicial, filtroPorRemision.FechaFinal, filtroPorRemision.Usuario, filtroPorRemision.RemisionInicial, filtroPorRemision.RemisionFinal, filtroPorRemision.IdSuministro, filtroPorRemision.IdCiudad).ToList()
           .ConvertAll(sum => new SUImpresionSumCentroServicioDC()
           {
             RemisionSuministro = new SURemisionSuministroDC()
             {
               IdCasaMatriz = sum.RES_IdCasaMatrizElabora,
               NumeroGuiaDespacho = sum.RES_NumeroGuiaInternaDespacho,
               Destinatario = sum.RES_NombreDestinatario,
               GuiaInterna = new ADGuiaInternaDC()
               {
                 IdAdmisionGuia = sum.AGI_IdAdmisionMensajeria,
                 NombreRemitente = sum.AGI_NombreRemitente,
                 NombreDestinatario = sum.AGI_NombreDestinatario,
                 DireccionDestinatario = sum.AGI_DireccionDestinatario,
                 DireccionRemitente = sum.AGI_DireccionRemitente,
                 TelefonoDestinatario = sum.AGI_TelefonoDestinatario,
                 TelefonoRemitente = sum.AGI_TelefonoRemitente,
                 GestionOrigen = new ARGestionDC() { Descripcion = sum.AGI_DescripcionGestionDest },
                 GestionDestino = new ARGestionDC() { Descripcion = sum.AGI_DescripcionGestionOrig }
               }
             },
             CentroServicio = new PUCentroServiciosDC()
             {
               IdCentroServicio = sum.CES_IdCentroServicios,
               Nombre = sum.CES_Nombre,
               Telefono1 = sum.CES_Telefono1,
               Direccion = sum.CES_Direccion,
               CiudadUbicacion = new PALocalidadDC() { Nombre = sum.NombreCiudad, CodigoPostal = sum.LOC_CodigoPostal },
               PaisCiudad = new PALocalidadDC() { Nombre = sum.NombrePais }
             },
             Rango = new SURango()
             {
               Inicio = sum.PCS_Inicio,
               Fin = sum.PCS_Fin
             }
           });
      }
    }

    /// <summary>
    /// Obtiene informacion de las provisiones de un suministro en las sucursales por numero de remision
    /// </summary>
    /// <param name="fechaInicial">fecha inicial de la consulta por defecto deja la actual menos 3 dias</param>
    /// <param name="fechaFinal">fecha final de la consulta por defecto deja la actual</param>
    /// <param name="usuario">Usuario que realizo la remision</param>
    /// <param name="remisionInicial">numero de la remision inicial</param>
    /// <param name="remisionFinal">numero de la remision final</param>
    /// <param name="pageIndex">indice de la pagina</param>
    /// <param name="pageSize">tamaño de la paginacion</param>
    /// <returns>Lista con la informacion para impresion los suministro de un Sucursal</returns>
    public List<SUImpresionSumSucursalDC> ObtenerProvisionesSumSucursalesPorRemision(SUFiltroSuministroPorRemisionDC filtroPorRemision)
    {
      using (ModeloSuministros contexto = new ModeloSuministros(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
      {
        filtroPorRemision.FechaInicial = filtroPorRemision.FechaInicial ?? DateTime.Now.AddDays(-3);
        filtroPorRemision.FechaFinal = filtroPorRemision.FechaFinal ?? DateTime.Now;

        return contexto.paObtenerProvisionesSumSucursalesPorRemision_SUM(filtroPorRemision.FechaInicial, filtroPorRemision.FechaFinal, filtroPorRemision.Usuario, filtroPorRemision.RemisionInicial, filtroPorRemision.RemisionFinal, filtroPorRemision.IdSuministro, filtroPorRemision.IdCiudad).ToList()
             .ConvertAll(sum => new SUImpresionSumSucursalDC()
             {
               RemisionSuministro = new SURemisionSuministroDC()
               {
                 IdCasaMatriz = sum.RES_IdCasaMatrizElabora,
                 NumeroGuiaDespacho = sum.RES_NumeroGuiaInternaDespacho,
                 Destinatario = sum.RES_NombreDestinatario,
                 GuiaInterna = new ADGuiaInternaDC()
                 {
                   IdAdmisionGuia = sum.AGI_IdAdmisionMensajeria,
                   NombreRemitente = sum.AGI_NombreRemitente,
                   NombreDestinatario = sum.AGI_NombreDestinatario,
                   DireccionDestinatario = sum.AGI_DireccionDestinatario,
                   DireccionRemitente = sum.AGI_DireccionRemitente,
                   TelefonoDestinatario = sum.AGI_TelefonoDestinatario,
                   TelefonoRemitente = sum.AGI_TelefonoRemitente,
                   GestionOrigen = new ARGestionDC() { Descripcion = sum.AGI_DescripcionGestionDest },
                   GestionDestino = new ARGestionDC() { Descripcion = sum.AGI_DescripcionGestionOrig },
                 }
               },
               Rango = new SURango()
               {
                 Inicio = sum.SSS_Inicio,
                 Fin = sum.SSS_Fin
               },

               ReferenciaUsoGuia = new CLReferenciaUsoGuiaDC()
               {
                 CiudadDestino = sum.NombreCiudad,
                 CiudadOrigen = sum.RUG_CiudadOrigen,
                 CodigoPostalDestino = sum.RUG_CodigoPostalDestino,
                 CodigoPostalOrigen = sum.RUG_CodigoPostalOrigen,
                 DireccionDestino = sum.RUG_DireccionDestino,
                 DireccionOrigen = sum.RUG_DireccionOrigen,
                 EsDestionoAbierto = sum.RUG_EsDestionoAbierto!=null?sum.RUG_EsDestionoAbierto.Value:true,
                 EsOrigenAbierto = sum.RUG_EsOrigenAbierto != null?sum.RUG_EsOrigenAbierto.Value:true,
                 IdentificacionDestino = sum.RUG_IdentificacionDestino,
                 IdentificacionOrigen = sum.RUG_IdentificacionOrigen,
                 NombreDestino = sum.RUG_NombreOrigen,
                 NombreOrigen = sum.RUG_NombreOrigen,
                 PaisDestino = sum.NombrePais,
                 PaisOrigen = sum.RUG_PaisOrigen,
                 TelefonoDestino = sum.RUG_TelefonoDestino,
                 TelefonoOrigen = sum.RUG_TelefonoOrigen,
                 TipoIdentificacionDestino = sum.RUG_TipoIdentificacionDestino,
                 TipoIdentificacionOrigen = sum.RUG_TipoIdentificacionOrigen
               },
               Sucursal = new CLSucursalDC()
               {
                 IdSucursal = sum.SUC_IdSucursal
               }
             });
      }
    }

    /// <summary>
    /// Obtiene informacion de las provisiones de un suministro de los Mensajeros por numero de remision
    /// </summary>
    /// <param name="fechaInicial">fecha inicial de la consulta por defecto deja la actual menos 3 dias</param>
    /// <param name="fechaFinal">fecha final de la consulta por defecto deja la actual</param>
    /// <param name="usuario">Usuario que realizo la remision</param>
    /// <param name="remisionInicial">numero de la remision inicial</param>
    /// <param name="remisionFinal">numero de la remision final</param>
    /// <param name="pageIndex">indice de la pagina</param>
    /// <param name="pageSize">tamaño de la paginacion</param>
    /// <returns>Lista con la informacion para impresion los suministro de los mensajeros</returns>
    public List<SUImpresionSumMensajeroDC> ObtenerProvisionesSumMensajeroPorRemision(SUFiltroSuministroPorRemisionDC filtroPorRemision)
    {
      using (ModeloSuministros contexto = new ModeloSuministros(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
      {
        filtroPorRemision.FechaInicial = filtroPorRemision.FechaInicial ?? DateTime.Now.AddDays(-3);
        filtroPorRemision.FechaFinal = filtroPorRemision.FechaFinal ?? DateTime.Now;

        return contexto.paObtenerProvisionesSumMensajeroPorRemision_SUM(filtroPorRemision.FechaInicial, filtroPorRemision.FechaFinal, filtroPorRemision.Usuario, filtroPorRemision.RemisionInicial, filtroPorRemision.RemisionFinal, filtroPorRemision.IdSuministro, filtroPorRemision.IdCiudad).ToList()
               .ConvertAll(sum => new SUImpresionSumMensajeroDC()
               {
                 RemisionSuministro = new SURemisionSuministroDC()
                 {
                   IdCasaMatriz = sum.RES_IdCasaMatrizElabora,
                   NumeroGuiaDespacho = sum.RES_NumeroGuiaInternaDespacho,
                   Destinatario = sum.RES_NombreDestinatario,
                   GuiaInterna = new ADGuiaInternaDC()
                   {
                     IdAdmisionGuia = sum.AGI_IdAdmisionMensajeria,
                     NombreRemitente = sum.AGI_NombreRemitente,
                     NombreDestinatario = sum.AGI_NombreDestinatario,
                     DireccionDestinatario = sum.AGI_DireccionDestinatario,
                     DireccionRemitente = sum.AGI_DireccionRemitente,
                     TelefonoDestinatario = sum.AGI_TelefonoDestinatario,
                     TelefonoRemitente = sum.AGI_TelefonoRemitente,
                     GestionOrigen = new ARGestionDC() { Descripcion = sum.AGI_DescripcionGestionDest },
                     GestionDestino = new ARGestionDC() { Descripcion = sum.AGI_DescripcionGestionOrig }
                   }
                 },
                 CentroServicio = new PUCentroServiciosDC()
                 {
                   IdCentroServicio = sum.CES_IdCentroServicios,
                   CiudadUbicacion = new PALocalidadDC() { Nombre = sum.NombreCiudad, CodigoPostal = sum.LOC_CodigoPostal },
                   PaisCiudad = new PALocalidadDC() { Nombre = sum.NombrePais }
                 },
                 Rango = new SURango()
                 {
                   Prefijo = sum.MSS_Prefijo,
                   Inicio = sum.MSS_Inicio,
                   Fin = sum.MSS_Fin
                 },
                 InformacionMensajero = new PAPersonaInternaDC()
                 {
                   Nombre = sum.PEI_Nombre,
                   PrimerApellido = sum.PEI_PrimerApellido,
                   SegundoApellido = sum.PEI_SegundoApellido,
                   Identificacion = sum.PEI_Identificacion,
                   IdTipoIdentificacion = sum.PEI_IdTipoIdentificacion,
                   Telefono = sum.MEN_Telefono2
                 }
               });
      }
    }

    /// <summary>
    /// Obtiene informacion de las provisiones de un suministro de las Gestiones por numero de remision
    /// </summary>
    /// <param name="fechaInicial">fecha inicial de la consulta por defecto deja la actual menos 3 dias</param>
    /// <param name="fechaFinal">fecha final de la consulta por defecto deja la actual</param>
    /// <param name="usuario">Usuario que realizo la remision</param>
    /// <param name="remisionInicial">numero de la remision inicial</param>
    /// <param name="remisionFinal">numero de la remision final</param>
    /// <param name="pageIndex">indice de la pagina</param>
    /// <param name="pageSize">tamaño de la paginacion</param>
    /// <returns>Lista con la informacion para impresion los suministro de las Gestiones</returns>
    public List<SUImpresionSumGestionDC> ObtenerProvisionesSumGestionPorRemision(SUFiltroSuministroPorRemisionDC filtroPorRemision)
    {
      using (ModeloSuministros contexto = new ModeloSuministros(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
      {
        filtroPorRemision.FechaInicial = filtroPorRemision.FechaInicial ?? DateTime.Now.AddDays(-3);
        filtroPorRemision.FechaFinal = filtroPorRemision.FechaFinal ?? DateTime.Now;

        return contexto.paObtenerProvisionesSumGestionPorRemision_SUM(filtroPorRemision.FechaInicial, filtroPorRemision.FechaFinal, filtroPorRemision.Usuario, filtroPorRemision.RemisionInicial, filtroPorRemision.RemisionFinal, filtroPorRemision.IdSuministro, filtroPorRemision.IdCiudad).ToList()
                .ConvertAll(sum => new SUImpresionSumGestionDC()
                {
                  RemisionSuministro = new SURemisionSuministroDC()
                  {
                    IdCasaMatriz = sum.RES_IdCasaMatrizElabora,
                    NumeroGuiaDespacho = sum.RES_NumeroGuiaInternaDespacho,
                    Destinatario = sum.RES_NombreDestinatario,
                    GuiaInterna = new ADGuiaInternaDC()
                    {
                      IdAdmisionGuia = sum.AGI_IdAdmisionMensajeria,
                      NombreRemitente = sum.AGI_NombreRemitente,
                      NombreDestinatario = sum.AGI_NombreDestinatario,
                      NombreCentroServicioDestino = sum.NombreCiudadDestino,
                      DireccionDestinatario = sum.AGI_DireccionDestinatario,
                      DireccionRemitente = sum.AGI_DireccionRemitente,
                      TelefonoDestinatario = sum.AGI_TelefonoDestinatario,
                      TelefonoRemitente = sum.AGI_TelefonoRemitente,
                      GestionOrigen = new ARGestionDC() { Descripcion = sum.AGI_DescripcionGestionDest },
                      GestionDestino = new ARGestionDC() { Descripcion = sum.AGI_DescripcionGestionOrig }
                    }
                  },
                  Rango = new SURango()
                  {
                    Prefijo = sum.PPS_Prefijo,
                    Inicio = sum.PPS_Inicio,
                    Fin = sum.PPS_Fin
                  },
                  CasaMatriz = new ARCasaMatrizDC()
                  {
                    Estado = sum.CAM_Estado,
                    CodigoMinisterio = sum.CAM_CodigoMinisterio,
                    CentroCostos = sum.CAM_CentroCostos,
                    IdLocalidad = sum.CAM_IdLocalidad,
                    Telefono = sum.CAM_Telefono,
                    IdCasaMatriz = sum.CAM_IdCasaMatriz,
                    Nombre = sum.CAM_Nombre,
                    Nit = sum.CAM_Nit,
                    DigitoVerificacion = sum.CAM_DigitoVerificacion,
                    CodigoSucursalERP = sum.CAM_CodigoSucursalERP,
                    Sigla = sum.CAM_Sigla,
                    Direccion = sum.CAM_Direccion,
                    NombreLocalidad = sum.NombreCiudadCasaMatriz
                  }
                });
      }
    }

    #endregion Consultar Suministros Por Remision

    /// <summary>
    /// Obtiene la cantidad de suministros en un rango que fueron aprovisionados.
    /// </summary>
    /// <param name="rangoInicial">rango inicial </param>
    /// <param name="rangoFinal">rango final</param>
    /// <returns>cantidad de suministros entre el rango inicial y el rango final</returns>
    public int ObtenerCantidadSuministroProvisionReferencia(long rangoInicial, long rangoFinal)
    {
      using (ModeloSuministros contexto = new ModeloSuministros(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
      {
        return contexto.paObtenerCantidadSuministroProvisionReferencia_SUM(rangoInicial, rangoFinal).FirstOrDefault() ?? 0;
      }
    }

    /// <summary>
    /// Consulta los parametros de suministros
    /// </summary>
    /// <param name="IdParametro"></param>
    /// <returns></returns>
    public string ObtenerParametrosSuministro(string idParametro)
    {
      using (ModeloSuministros contexto = new ModeloSuministros(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
      {
        ParametrosSuministros_SUM parametro = contexto.ParametrosSuministros_SUM.Where(para => para.PSU_IdParametro == idParametro).FirstOrDefault();
        if (parametro != null)
        {
          return parametro.PSU_ValorParametro;
        }
        else
        {
          return string.Empty;
        }
      }
    }
  }
}