using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Objects;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Transactions;
using CO.Servidor.OperacionNacional.Comun;
using CO.Servidor.OperacionNacional.Datos.Modelo;
using CO.Servidor.ParametrosOperacion.Comun;
using CO.Servidor.Servicios.ContratoDatos.OperacionNacional;
using CO.Servidor.Servicios.ContratoDatos.ParametrosOperacion;
using CO.Servidor.Servicios.ContratoDatos.Rutas;
using Framework.Servidor.Comun;
using Framework.Servidor.Excepciones;
using Framework.Servidor.Servicios.ContratoDatos.Parametros;

namespace CO.Servidor.OperacionNacional.Datos
{
  public partial class ONRepositorio
  {
    #region Ingreso y Salida de Transportador

    /// <summary>
    /// Obtiene el ingreso o salida del transportador por los parametros de entrada
    /// </summary>
    /// <param name="placa"></param>
    /// <param name="idRuta"></param>
    /// <param name="fechaInicio"></param>
    /// <param name="fechaFin"></param>
    /// <param name="identificadorConductor"></param>
    /// <param name="nombreConductor"></param>
    /// <param name="incluyeFecha"></param>
    /// <param name="pageIndex"></param>
    /// <param name="pageSize"></param>
    /// <returns></returns>
    public IEnumerable<ONIngresoSalidaTransportadorDC> ObtenerIngresoSalidaTransportador(ONFiltroTransportadorDC filtro, int indicePagina, int registrosPorPagina, bool incluyeFecha)
    {
      using (ModeloOperacionNacional contexto = new ModeloOperacionNacional(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
      {
        if (!string.IsNullOrEmpty(filtro.Placa))
        {
          filtro.Placa = filtro.Placa.Replace(" ", string.Empty);
        }
        else
          filtro.Placa = null;

        if (!filtro.IncluyeFecha)
        {
          filtro.FechaInicio = DateTime.Now.AddDays(-360);
          filtro.FechaFin = DateTime.Now;
        }


        return contexto.paObtenerIngSalidaTranspor_OPN(indicePagina, registrosPorPagina, filtro.Ruta.IdRuta, filtro.Placa, filtro.FechaInicio, filtro.FechaFin, filtro.IdentificadorConductor,filtro.IdAgenciaIngreso).ToList().ConvertAll(
          transportador => new ONIngresoSalidaTransportadorDC()
          {
            EsIngreso = transportador.IST_EsIngreso,
            FechaIngresoSalida = transportador.IST_FechaIngresoSalida,
            IdAgenciaIngresoSalida = transportador.IST_IdAgenciaIngresoSalida,
            IdConductor = transportador.IST_IdConductor,
            IdentificacionConductor = transportador.IST_IdentificacionConductor,
            IdIngrsoSalidaTransportado = transportador.IST_IdIngrsoSalidaTransportado,
            IdLocalidadIngresoSalida = transportador.IST_IdLocalidadIngresoSalida,
            IdNovedadOperativo = transportador.IST_IdNovedadOperativo,
            IdRuta = transportador.IST_IdRuta,
            IdVehiculo = transportador.IST_IdVehiculo,
            NombreConductor = transportador.IST_NombreConductor,
            NumeroPrecinto = transportador.IST_NumeroPrecinto,
            Observacion = transportador.IST_Observacion,
            Placa = transportador.IST_Placa
          });
      }
    }

    /// <summary>
    /// Obtiene la informacion de ingreso y salida del transportador por ID
    /// </summary>
    /// <param name="idIngrsoSalidaTransportado"></param>
    /// <returns></returns>
    public ONIngresoSalidaTransportadorDC ObtenerIngresoSalidaTransportadorPorId(long idIngrsoSalidaTransportado)
    {
      using (ModeloOperacionNacional contexto = new ModeloOperacionNacional(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
      {
        ONpaObtenerIngSalTransporDet ingresoSalidaTransportador = contexto.paObtenerIngSalTransporDet_OPN(idIngrsoSalidaTransportado).FirstOrDefault();

        return new ONIngresoSalidaTransportadorDC()
        {
          EsIngreso = ingresoSalidaTransportador.IST_EsIngreso,
          FechaIngresoSalida = ingresoSalidaTransportador.IST_FechaIngresoSalida,
          IdentificacionConductor = ingresoSalidaTransportador.IST_IdentificacionConductor,
          NombreConductor = ingresoSalidaTransportador.IST_NombreConductor,
          NumeroPrecinto = ingresoSalidaTransportador.IST_NumeroPrecinto,
          Observacion = ingresoSalidaTransportador.IST_Observacion,
          Placa = ingresoSalidaTransportador.IST_Placa,
          NovedadDescripcion = ingresoSalidaTransportador.IST_DescripcionNovedad,
          RutaDescripcion = ingresoSalidaTransportador.RUT_Nombre,
          LocalidadDescripcion = ingresoSalidaTransportador.LOC_Nombre
        };
      }
    }

    /// <summary>
    /// Obtiene todas las novedades operativas
    /// </summary>
    public IList<ONNovedadOperativoDC> ObtenerNovedadOperativo()
    {
      using (ModeloOperacionNacional contexto = new ModeloOperacionNacional(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
      {
        return contexto.NovedadOperativo_OPN.Where(nov => nov.NOO_Estado == ConstantesFramework.ESTADO_ACTIVO).ToList()
          .ConvertAll(conv => new ONNovedadOperativoDC()
          {
            IdNovedadOperativo = conv.NOO_IdNovedadOperativo,
            Descripcion = conv.NOO_Descripcion
          }).OrderBy(n => n.Descripcion).ToList();
      }
    }

    /// <summary>
    /// Ingreso en la tabla de ingreso salida transportador
    /// </summary>
    /// <param name="ingresoSalidaTrans"></param>
    public void IngresarIngresosSalidasTrasnportador(ONIngresoSalidaTransportadorDC ingresoSalidaTrans)
    {
      using (ModeloOperacionNacional contexto = new ModeloOperacionNacional(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
      {
        contexto.paInsertarIngSalidaTranspo_OPN(
          ingresoSalidaTrans.IdLocalidadIngresoSalida,
          ingresoSalidaTrans.IdNovedadOperativo,
          ingresoSalidaTrans.Novedad.Descripcion,
          ingresoSalidaTrans.IdAgenciaIngresoSalida,
          ingresoSalidaTrans.IdVehiculo,
          ingresoSalidaTrans.Placa.Replace(" ", string.Empty).ToUpper(),
          ingresoSalidaTrans.IdConductor,
          ingresoSalidaTrans.IdentificacionConductor,
          ingresoSalidaTrans.NombreConductor,
          ingresoSalidaTrans.EsIngreso,
          ingresoSalidaTrans.IdRuta,
          ingresoSalidaTrans.NumeroPrecinto,
          ingresoSalidaTrans.FechaIngresoSalida,
          ingresoSalidaTrans.Observacion,
          DateTime.Now,
          ControllerContext.Current.Usuario);
      }
    }

    /// <summary>
    /// Consultar la última ruta de un vehículo que ha sido manifestado,
    /// el vehículo se consulta por su placa
    /// </summary>
    public ONRutaConductorDC ObtenerUltimaRutaConduPlaca(string placa)
    {
      using (ModeloOperacionNacional contexto = new ModeloOperacionNacional(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
      {
        ONpaObtenerUltRutaConduPlaca rutaConductor = contexto.paObtenerUltRutaConduPlaca_OPN(placa.Replace(" ", string.Empty)).FirstOrDefault();
        if (rutaConductor == null)
        {
          return null;
        }

        return new ONRutaConductorDC()
        {
          Ruta = new RURutaDC()
          {
            IdRuta = rutaConductor.RUT_IdRuta,
            NombreRuta = rutaConductor.RUT_Nombre
          },
          Conductor = new POConductores()
          {
            IdConductor = rutaConductor.MOT_IdConductor,
            Identificacion = rutaConductor.MOT_IdentificacionConductor,
            NombreCompleto = rutaConductor.MOT_NombreConductor
          },
          IdVehiculo = rutaConductor.MOT_IdVehiculo
        };
      }
    }

    /// <summary>
    /// Obtener los consolidados a partir de la guia interna
    /// </summary>
    /// <param name="idGuiaInterna"></param>
    public ONConsolidado ObtenerConsolidadoPorIdGuia(long idGuiaInterna)
    {
      using (ModeloOperacionNacional contexto = new ModeloOperacionNacional(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
      {
        ONpaObtenerConsolOpNacIdGui_OPN consolidado = contexto.paObtenerConsolOpNacIdGui_OPN(idGuiaInterna).FirstOrDefault();

        if (consolidado == null)
        {
          throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.MODULO_OPERACION_NACIONAL, EnumTipoErrorOperacionNacional.EX_NO_EXISTE_CONSOLIDADO_NUM_GUIA.ToString(), MensajesOperacionNacional.CargarMensaje(EnumTipoErrorOperacionNacional.EX_NO_EXISTE_CONSOLIDADO_NUM_GUIA)));
        }
        return new ONConsolidado()
        {
          ConsecutivoConsolidado = consolidado.MOC_ConsecutivoConsolidado,
          NumeroContenedorTula = consolidado.MOC_NumeroContenedorTula,
          IdTipoConsolidado = consolidado.MOC_IdTipoConsolidado,
          NombreTipoConsolidado = consolidado.TIC_Descripcion,
          IdGuiaInterna = consolidado.MOC_IdGuiaInterna,
          NumeroPrecintoRetorno = consolidado.MOC_NumeroPrecintoRetorno,
          NumeroPrecintoSalida = consolidado.MOC_NumeroPrecintoSalida,
          DescripcionConsolidadoDetalle = consolidado.MOC_DescpConsolidadoDetalle,
          IdManfiestoConsolidado = consolidado.MOC_IdManfiestoOperaNacioConso,
          LocalidadManifestada = new PALocalidadDC() { IdLocalidad = consolidado.MOC_IdLocalidadManifestada }
        };
      }
    }

    #endregion Ingreso y Salida de Transportador

    #region Operativo Ciudad

    /// <summary>
    /// Guarda el Ingreso de un envio no registrado en el sistema
    /// </summary>
    /// <param name="ingreso"></param>
    public void GuardarIngresoEnvioNoRegistrado(ONIngresoOperativoCiudadDC ingreso)
    {
      using (ModeloOperacionNacional contexto = new ModeloOperacionNacional(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
      {
        contexto.paCrearIngOpeAgenEnviNoReg_OPN(ingreso.IdIngresoOperativo
          , ingreso.EnvioIngreso.NumeroGuia
          , ingreso.EnvioIngreso.EstadoEmpaque.IdEstadoEmpaque
          , ingreso.EnvioIngreso.PesoGuiaIngreso
          , ControllerContext.Current.Usuario);
      }
    }

    /// <summary>
    /// Guarda el ingreso de un envio registrado en el sistema
    /// </summary>
    /// <param name="ingreso"></param>
    public void GuardarIngresoEnvioRegistrado(ONIngresoOperativoCiudadDC ingreso)
    {
      using (ModeloOperacionNacional contexto = new ModeloOperacionNacional(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
      {
        contexto.paCrearIngreOperaAgenEnvio_OPN(ingreso.IdIngresoOperativo
          , ingreso.EnvioIngreso.IdAdmisionMensajeria
          , ingreso.EnvioIngreso.NumeroGuia
          , ingreso.EnvioIngreso.EstadoEmpaque.IdEstadoEmpaque
          , ingreso.EnvioIngreso.PesoGuiaIngreso
          , ControllerContext.Current.Usuario);
      }
    }

    /// <summary>
    /// Guarda el ingreso del operativo por ciudad
    /// </summary>
    /// <param name="ingresoOperativo"></param>
    /// <param name="tipoOperativo"></param>
    /// <returns></returns>
    public long GuardarIngresoOperativoAgencia(ONIngresoOperativoCiudadDC ingresoOperativo, int tipoOperativo)
    {
      using (ModeloOperacionNacional contexto = new ModeloOperacionNacional(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
      {
        long idIngresoOperativo;
        var idIngreso = contexto.paCrearIngreOperaAgencRuta_OPN((short)tipoOperativo
          , null
          , string.Empty
          , ingresoOperativo.Vehiculo.IdVehiculo
          , ingresoOperativo.Vehiculo.Placa
          , ingresoOperativo.Conductores.IdConductor
          , ingresoOperativo.Conductores.NombreCompleto
          , ingresoOperativo.CiudadIngreso.IdLocalidad
          , ingresoOperativo.CiudadIngreso.Nombre
          , ingresoOperativo.IdAgencia
          , false
          , ControllerContext.Current.Usuario,null).FirstOrDefault();

        if (long.TryParse(idIngreso.ToString(), out idIngresoOperativo))
          return idIngresoOperativo;
        else
          throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.MODULO_OPERACION_NACIONAL, EnumTipoErrorOperacionNacional.EX_ERROR_INGRESO_OPERATIVO.ToString(), MensajesOperacionNacional.CargarMensaje(EnumTipoErrorOperacionNacional.EX_ERROR_INGRESO_OPERATIVO))); ;
      }
    }

    /// <summary>
    /// Valida que la ciudad destino del envio no tenga una ruta configurada
    /// </summary>
    /// <param name="idAdmision"></param>
    public void ValidaCiudadDestinoEnvio(long idAdmision)
    {
      using (ModeloOperacionNacional contexto = new ModeloOperacionNacional(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
      {
        string idCiudad = contexto.paObtenerEnvioSueltosCiuda_MEN(idAdmision).FirstOrDefault();
        if (!string.IsNullOrEmpty(idCiudad))
          throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.MODULO_OPERACION_NACIONAL, EnumTipoErrorOperacionNacional.EX_CIUDAD_DESTINO_ENVIO_TIENE_RUTA.ToString(), MensajesOperacionNacional.CargarMensaje(EnumTipoErrorOperacionNacional.EX_CIUDAD_DESTINO_ENVIO_TIENE_RUTA)));
      }
    }

    #endregion Operativo Ciudad
  }
}