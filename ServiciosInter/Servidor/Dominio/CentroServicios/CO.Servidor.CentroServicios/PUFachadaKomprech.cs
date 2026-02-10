using System.Linq;
using System.Collections.Generic;
using CO.Servidor.Servicios.ContratoDatos.CentroServicios;
using Framework.Servidor.Seguridad;
using Framework.Servidor.Servicios.ContratoDatos.Seguridad;

namespace CO.Servidor.CentroServicios
{
  public class PUFachadaKomprech
  {
    /// <summary>
    /// Instancia Singleton
    /// </summary>
    private static readonly PUFachadaKomprech instancia = new PUFachadaKomprech();

    /// <summary>
    /// Retorna una instancia de la fabrica de Dominio
    /// </summary>
    public static PUFachadaKomprech Instancia
    {
      get { return PUFachadaKomprech.instancia; }
    }

    /// <summary>
    /// Método que devuelve una lista con todos los centros de servicio
    /// </summary>
    /// <returns></returns>
    public List<PUCentroServiciosDC> ObtenerAgenciasPuntosActivosPorLocalidad(string idMunicipio)
    {
      return PUCentroServicios.Instancia.ObtenerAgenciasPuntosActivosKomprechPorLocalidad(idMunicipio);
    }

    /// <summary>
    /// Método que autentica usuario
    /// </summary>
    /// <param name="nombreUsuario"></param>
    /// <param name="contrasena"></param>
    /// <returns></returns>
    public ResultadoAutenticacion AutenticarUsuario(string nombreUsuario, string contrasena)
    {
      SECredencialUsuario autenticacion = SEProveedor.Instancia.AutenticarUsuario(new Framework.Servidor.Servicios.ContratoDatos.Seguridad.SECredencialUsuario()
                                     {
                                       Usuario = nombreUsuario,
                                       Password = contrasena
                                     });

      ResultadoAutenticacion resultado = new ResultadoAutenticacion()
      {
        NombreCompleto = string.Join(" ", autenticacion.Nombre, autenticacion.Apellido1, autenticacion.Apellido2)
      };

      foreach (var ubi in autenticacion.LocacionesAutorizadas.Where(ubi => ubi.TipoLocacion == TipoLocacionAutorizada.CentroServicios && ubi.TipoCentroServicio != "RAC"))
      {
        long idCs = 0;
        if (long.TryParse(ubi.IdLocacion, out idCs))
        {
          bool servicioAsignado = PUCentroServicios.Instancia.CentroServicioTieneServicioKomprechAsociado(idCs);
          if (servicioAsignado)
          {
            resultado.IdCentroServicio = idCs;
            resultado.IdCentroCostos = ubi.IdCentroCostos != null ? ubi.IdCentroCostos : string.Empty;
            resultado.NombreCentroServicio = ubi.DescripcionLocacion;
            break;
          }
        }
      }
      
      return resultado;
    }
  }

  public class ResultadoAutenticacion
  {
    public string NombreCompleto { get; set; }

    public string NombreCentroServicio { get; set; }

    public long IdCentroServicio { get; set; }

    public string IdCentroCostos { get; set; }
  }
}
