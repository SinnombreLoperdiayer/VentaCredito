using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using CO.Servidor.Servicios.ContratoDatos.Tarifas;
using CO.Servidor.Tarifas.Datos.Modelo;
using Framework.Servidor.Comun;
using Framework.Servidor.Excepciones;
using Framework.Servidor.Servicios.ContratoDatos.Parametros;

namespace CO.Servidor.Tarifas.Datos
{
  /// <summary>
  /// Repositorio para Servicio Internacional
  /// </summary>
  public class TARepositorioInternacional
  {
    #region Campos

    private static readonly TARepositorioInternacional instancia = new TARepositorioInternacional();
    private const string NombreModelo = "ModeloTarifas";

    #endregion Campos

    #region Propiedades

    /// <summary>
    /// Retorna la instancia de la clase TARepositorioInternacional
    /// </summary>
    public static TARepositorioInternacional Instancia
    {
      get { return TARepositorioInternacional.instancia; }
    }

    #endregion Propiedades

    #region Servicio Internacional

    /// <summary>
    /// Obtiene precio internacional
    /// </summary>
    /// <param name="filtro">Filtro</param>
    /// <param name="campoOrdenamiento">Campo de Ordenamiento</param>
    /// <param name="indicePagina">Indice de página</param>
    /// <param name="registrosPorPagina">Registro por página</param>
    /// <param name="ordenamientoAscendente">Ordenamiento</param>
    /// <param name="totalRegistros">Total de Registros</param>
    /// <returns>Listado de precio internacional</returns>
    public IEnumerable<TAServicioInternacionalPrecioDC> ObtenerZonasPorOperadorPostal(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros, int idServicio, int idListaPrecio)
    {
      using (EntidadesTarifas contexto = new EntidadesTarifas(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
      {
        string idListaPrecioServicio = TARepositorio.Instancia.ObtenerIdentificadorListaPrecioServicio(idServicio, idListaPrecio);

        filtro.Add("PIN_IdListaPrecioServicio", idListaPrecioServicio);
        if (string.IsNullOrEmpty(campoOrdenamiento))
        {
          campoOrdenamiento = "OPO_Nombre,ZON_Descripcion,PIN_Peso";
        }
        return contexto.ConsultarEqualsServicioInternacionalZona_VTAR(filtro, campoOrdenamiento, out totalRegistros, indicePagina, registrosPorPagina, ordenamientoAscendente)
          .ToList()
          .ConvertAll(r => new TAServicioInternacionalPrecioDC
          {
            IdServicio = idServicio,
            IdPrecioInternacional = r.PIN_IdPrecioInternalcional,
            Peso = r.PIN_Peso,
            Valor = r.PIN_Valor,
            EstadoRegistro = EnumEstadoRegistro.SIN_CAMBIOS,
            DescripcionOperadorPostal = r.OPO_Nombre,
            Zona = new PAZonaDC()
            {
              IdZona = r.PIN_IdZona,
              Descripcion = r.ZON_Descripcion
            },
            TipoEmpaque = new TATipoEmpaque()
            {
              IdTipoEmpaque = r.PIN_IdTipoEmpaque,
              Descripcion = r.TEM_Descripcion
            },
            OperadorPostal = new TAOperadorPostalDC()
            {
              IdOperadorPostal = r.OPZ_IdOperadorPostal,
              Nombre = r.OPO_Nombre
            },
            ZonasDisponibles = contexto.OperadorPostalZona_PAR.Include("Zona_PAR")
            .Where(oz => oz.OPZ_IdOperadorPostal == r.OPZ_IdOperadorPostal)
            .ToList()
            .ConvertAll(oz => new PAZonaDC()
            {
              IdZona = oz.OPZ_IdZona,
              Descripcion = oz.Zona_PAR.ZON_Descripcion
            })
          });
      }
    }

    /// <summary>
    /// Adiciona un precio internacional
    /// </summary>
    /// <param name="tarifaInternacional">Objeto con el registro a adicionar</param>
    public void AdicionarPrecioInternacional(TAServicioInternacionalPrecioDC tarifaInternacional)
    {
      using (EntidadesTarifas contexto = new EntidadesTarifas(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
      {
        string idListaPrecioServicio = TARepositorio.Instancia.ObtenerIdentificadorListaPrecioServicio(tarifaInternacional.IdServicio, tarifaInternacional.IdListaPrecio);

        PrecioInternacional_TAR interEn = new PrecioInternacional_TAR()
        {
          PIN_IdListaPrecioServicio = int.Parse(idListaPrecioServicio),
          PIN_IdZona = tarifaInternacional.Zona.IdZona,
          PIN_IdTipoEmpaque = tarifaInternacional.TipoEmpaque.IdTipoEmpaque,
          PIN_Peso = tarifaInternacional.Peso,
          PIN_Valor = tarifaInternacional.Valor,
          PIN_FechaGrabacion = DateTime.Now,
          PIN_CreadoPor = ControllerContext.Current.Usuario
        };
        contexto.PrecioInternacional_TAR.Add(interEn);
        contexto.SaveChanges();
      }
    }

    /// <summary>
    /// Edita los cambios realizados a precio internacional
    /// </summary>
    /// <param name="tarifaInternacional">Objeto con los cambios</param>
    public void EditarPrecioInternacional(TAServicioInternacionalPrecioDC tarifaInternacional)
    {
      using (EntidadesTarifas contexto = new EntidadesTarifas(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
      {
        PrecioInternacional_TAR interEn = contexto.PrecioInternacional_TAR
          .Where(r => r.PIN_IdPrecioInternalcional == tarifaInternacional.IdPrecioInternacional)
          .SingleOrDefault();

        if (interEn == null)
        {
          ControllerException excepcion = new ControllerException(COConstantesModulos.TARIFAS, ETipoErrorFramework.EX_CONSULTA_DB_NULL.ToString(), MensajesFramework.CargarMensaje(ETipoErrorFramework.EX_CONSULTA_DB_NULL));
          throw new FaultException<ControllerException>(excepcion);
        }

        interEn.PIN_IdZona = tarifaInternacional.Zona.IdZona;
        interEn.PIN_IdTipoEmpaque = tarifaInternacional.TipoEmpaque.IdTipoEmpaque;
        interEn.PIN_Peso = tarifaInternacional.Peso;
        interEn.PIN_Valor = tarifaInternacional.Valor;
        TARepositorioAudit.MapeoAuditPrecioInternacional(contexto);
        contexto.SaveChanges();
      }
    }

    /// <summary>
    /// Elimina un precio internacional
    /// </summary>
    /// <param name="tarifaInternacional">Objeto a eliminar</param>
    public void EliminarPrecioInternacional(TAServicioInternacionalPrecioDC tarifaInternacional)
    {
      using (EntidadesTarifas contexto = new EntidadesTarifas(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
      {
        PrecioInternacional_TAR interEn = contexto.PrecioInternacional_TAR.Where(r => r.PIN_IdPrecioInternalcional == tarifaInternacional.IdPrecioInternacional).SingleOrDefault();
        contexto.PrecioInternacional_TAR.Remove(interEn);
        TARepositorioAudit.MapeoAuditPrecioInternacional(contexto);
        contexto.SaveChanges();
      }
    }

    #endregion Servicio Internacional
  }
}