using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Data.Objects;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Framework.Servidor.Comun;
using Framework.Servidor.Mensajeria.Datos.Modelo;
using Framework.Servidor.Servicios.ContratoDatos;
using Framework.Servidor.Servicios.ContratoDatos.Mensajeria;
using System.Data.SqlClient;
using System.Configuration;
using System.IO;

namespace Framework.Servidor.Mensajeria.Datos
{
  public class MERepositorio
  {
    #region Instancia Singleton de la clase

    /// <summary>
    /// Instancia de la clase ASRepositorio
    /// </summary>
    public static readonly MERepositorio Instancia = new MERepositorio();

    #endregion Instancia Singleton de la clase


    private MERepositorio()
    {
       
    }

    #region Atributos

    private const string nombreModelo = "ModeloMensajeria";

    private string RutaImagenes = "";

    #endregion Atributos

    #region Metodos publicos

    /// <summary>
    /// Consulta los mensajes dirigidos a una agencia
    /// </summary>
    /// <param name="agencia">Agencia a la cual se le quiere hacer la consulta</param>
    /// <returns></returns>
    public GenericoConsultasFramework<MEMensajeEnviado> ConsultarMensajesAgencia(long agencia, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool esAscendente)
    {
      int totalRegistros = 0;

      using (EntidadesMensajeria contexto = new EntidadesMensajeria(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(nombreModelo)))
      {
          var racol = contexto.CentrosServicosRacol_VPUA.Where(cs => cs.CES_IdCentroServicios == agencia).FirstOrDefault();
        //    contexto.Agencia_PUA.GroupJoin(contexto.PuntoServicio_PUA, (a) => a.AGE_IdAgencia, (p) => p.PUS_IdAgencia, (b, a) => new { AGENCIA = b, PUNTOS = a }).SelectMany(X => X.PUNTOS.DefaultIfEmpty(), (X, Y) => new { Agencia = X.AGENCIA, Punto = Y })
        //.Join(contexto.CentroLogistico_PUA, (a) => a.Agencia.AGE_IdCentroLogistico, (c) => c.CEL_IdCentroLogistico, (a, c) => new { MERacol = c.CEL_IdRegionalAdm, IdAgencia = (long?)a.Agencia.AGE_IdAgencia, IdPunto = (long?)a.Punto.PUS_IdPuntoServicio })
        //.Where(a => a.IdAgencia == agencia || a.IdPunto == agencia);

        //var racol = QueryRacol.FirstOrDefault();

        //No se utiliza la función ConsultarMensajesEnviados_MEN del contexto porque se necesita una operación OR en el filtrado
        IEnumerable<MensajesEnviados_MEN> mensajes = new List<MensajesEnviados_MEN>();// = contexto.ConsultarMensajesEnviados_MEN(filtro, campoOrdenamiento, out totalRegistros,indicePagina, registrosPorPagina,esAscendente);

        // Verificando parámetros para la consulta
        if (indicePagina < 0)
          throw new ArgumentException("Número de Página inválida");

        if (registrosPorPagina <= 0)
          throw new ArgumentException("Número de registros por página no válido");

        if (racol != null)
        {
          totalRegistros = contexto.MensajesEnviados_MEN.Where(m => m.MEN_IdPuntoAtencionDestino == agencia || m.MEN_IdPuntoAtencionDestino == racol.REA_IdRegionalAdm).Count();

          if (campoOrdenamiento.Trim() == string.Empty)
          {
            var objectSet = ((IObjectContextAdapter)contexto).ObjectContext.CreateObjectSet<MensajesEnviados_MEN>();

            string llavePrimaria = objectSet.EntitySet.ElementType.KeyMembers.First().Name;
            mensajes = contexto.MensajesEnviados_MEN.OrdenarPor(llavePrimaria)
                                                      .Where(m => m.MEN_IdPuntoAtencionDestino == agencia || m.MEN_IdPuntoAtencionDestino == racol.REA_IdRegionalAdm)
                                         .Skip(indicePagina * registrosPorPagina)
                                         .Take(registrosPorPagina).ToList();
          }
          else
          {
            mensajes = (esAscendente) ? contexto.MensajesEnviados_MEN.OrdenarUsandoExpresion(campoOrdenamiento + " ASC")
                                                    .Where(m => m.MEN_IdPuntoAtencionDestino == agencia || m.MEN_IdPuntoAtencionDestino == racol.REA_IdRegionalAdm)
                                                    .Skip(indicePagina * registrosPorPagina)
                                                    .Take(registrosPorPagina)
                                                    .ToList()
                                             :
                                    contexto.MensajesEnviados_MEN.OrdenarUsandoExpresion(campoOrdenamiento + " DESC")
                                                    .Where(m => m.MEN_IdPuntoAtencionDestino == agencia || m.MEN_IdPuntoAtencionDestino == racol.REA_IdRegionalAdm)
                                                    .Skip(indicePagina * registrosPorPagina)
                                                    .Take(registrosPorPagina)
                                                    .ToList();
          }
        }
        if (mensajes.Count() > 0)
        {
          return new GenericoConsultasFramework<MEMensajeEnviado>()
          {
            Lista = mensajes.ToList().ConvertAll(mensaje =>
            {
              return CargarArbolMensajeEnviado(mensaje, contexto);
            }),
            TotalRegistros = totalRegistros
          };
        }
        else
          return new GenericoConsultasFramework<MEMensajeEnviado>() { TotalRegistros = 0 };
      }
    }

    /// <summary>
    /// Consulta los mensajes creados x un usuario
    /// </summary>
    /// <param name="usuario">Usuario al cual se le quiere hacer la consulta</param>
    /// <returns></returns>
    public GenericoConsultasFramework<MEMensajeEnviado> ConsultarMensajesEnviadosXUsuario(string usuario, IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool esAscendente)
    {
      int totalRegistros = 0;
      using (EntidadesMensajeria contexto = new EntidadesMensajeria(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(nombreModelo)))
      {
        Dictionary<LambdaExpression, OperadorLogico> where = new Dictionary<LambdaExpression, OperadorLogico>();
        LambdaExpression lamda = contexto.CrearExpresionLambda<MensajesEnviados_MEN>("MEN_UsuarioOrigen", usuario, OperadorComparacion.Equal);
        where.Add(lamda, OperadorLogico.And);
        //lamda = contexto.CrearExpresionLambda<MensajesEnviados_MEN>("MEN_Asunto", "dfdg", OperadorComparacion.Contains);
        //where.Add(lamda,OperadorLogico.And);
        //lamda = contexto.CrearExpresionLambda<MensajesEnviados_MEN>("MEN_FechaCreacion", System.DateTime.Today.ToShortDateString(), OperadorComparacion.Between, System.DateTime.Today.AddDays(2).ToShortDateString());
        //where.Add(lamda,OperadorLogico.And);

        IEnumerable<MensajesEnviados_MEN> mensajes = contexto.ConsultarMensajesEnviados_MEN(filtro, where, campoOrdenamiento, out totalRegistros, indicePagina, registrosPorPagina, esAscendente);

        if (mensajes.Count() > 0)
        {
          return new GenericoConsultasFramework<MEMensajeEnviado>()
          {
            Lista = mensajes.ToList().ConvertAll(mensaje =>
            {
              return CargarArbolMensajeEnviado(mensaje, contexto);
            }),
            TotalRegistros = totalRegistros
          };
        }
        else
          return new GenericoConsultasFramework<MEMensajeEnviado>() { TotalRegistros = 0 };
      }
    }

    /// <summary>
    /// Consulta el numero de mensajes que la agencia no ha leido
    /// </summary>
    /// <param name="idCentroServicio">Centro de servicio</param>
    /// <returns>Numero de mensajes sin leer</returns>
    public int ConsultarMensajesSinLeer(string idCentroServicio)
    {
      using (EntidadesMensajeria contexto = new EntidadesMensajeria(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(nombreModelo)))
      {
        return contexto.MensajesEnviados_MEN.Where(m => m.MEN_EstadoNotificacion == "ENV").Count();
      }
    }

    /// <summary>
    /// Crea un mensaje nuevo dirigido a una agencia
    /// </summary>
    /// <param name="mensaje">Datos del mensaje que se está creando</param>
    /// <param name="usuario">Usuario que está creando el mensaje</param>
    public void CrearMensajeNuevo(MEMensajeEnviado mensaje, string usuario)
    {
      using (EntidadesMensajeria contexto = new EntidadesMensajeria(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(nombreModelo)))
      {
        MensajesEnviados_MEN mensajeDB = new MensajesEnviados_MEN()
              {
                MEN_Asunto = mensaje.Asunto,
                MEN_CategoriaMensaje = mensaje.Categoria,
                MEN_EstadoNotificacion = "ENV",
                MEN_FechaCreacion = System.DateTime.Now,
                MEN_IdMensajeOriginal = 0,
                MEN_IdPuntoAtencionDestino = mensaje.CentroServicioDestino.HasValue ? mensaje.CentroServicioDestino.Value : 0,
                MEN_TextoMensaje = mensaje.Texto,
                MEN_UsuarioOrigen = usuario,
                MEN_AceptaRespuestas = mensaje.AceptaRespuestas
              };
        contexto.MensajesEnviados_MEN.Add(mensajeDB);
        contexto.SaveChanges();
      }
    }

    /// <summary>
    /// Crea un mensaje nuevo dirigido a una agencia
    /// </summary>
    /// <param name="mensaje">Datos del mensaje que se está creando</param>
    /// <param name="usuario">Usuario que está creando el mensaje</param>
    public void CrearMensajeNuevoMasivo(MEMensajeEnviado mensaje, string usuario,long[] centrosServicioDestinatario)
    {
        using (EntidadesMensajeria contexto = new EntidadesMensajeria(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(nombreModelo)))
        {


            foreach (long c in centrosServicioDestinatario)
            {             

                MensajesEnviados_MEN mensajeDB = new MensajesEnviados_MEN()
                {
                    MEN_Asunto = mensaje.Asunto,
                    MEN_CategoriaMensaje = mensaje.Categoria,
                    MEN_EstadoNotificacion = "ENV",
                    MEN_FechaCreacion = System.DateTime.Now,
                    MEN_IdMensajeOriginal = 0,
                    MEN_IdPuntoAtencionDestino = c,                    
                    MEN_TextoMensaje = mensaje.Texto,
                    MEN_UsuarioOrigen = usuario,
                    MEN_AceptaRespuestas = mensaje.AceptaRespuestas
                };
                contexto.MensajesEnviados_MEN.Add(mensajeDB);

                if(mensaje.Adjuntos!=null)
                {
                    string fecha = DateTime.Now.ToString("dd-MM-yyyy");
                    string path = Path.Combine(this.RutaImagenes, fecha);
                    ConsultarRutaImagenesAdjuntos();

                    mensaje.Adjuntos.ForEach(adjunto =>
                        {
                            if (!Directory.Exists(path))
                            {
                                Directory.CreateDirectory(path);
                            }

                            string archivo = Path.Combine(path,Guid.NewGuid().ToString(),adjunto.ExtensionArchivoAdjunto);

                            var bw = new BinaryWriter(File.Open(archivo, FileMode.OpenOrCreate));
                            bw.Write(adjunto.ArchivoAdjunto);

                            AdjuntosMensajes_MEN adjuntos = new AdjuntosMensajes_MEN()
                            {
                                ADJ_IdMensaje = mensajeDB.MEN_IdMensaje,
                                ADJ_RutaArchivo = archivo

                            };
                        });
                }

            }
            contexto.SaveChanges();
        }
    }



    /// <summary>
    /// Crea un mensaje nuevo dirigido a una agencia
    /// </summary>
    /// <param name="respuesta">Datos de la respuesta del mensaje</param>
    /// <param name="usuario">Usuario que está creando el mensaje</param>
    /// <param name="idMensaje">Mensaje asociado a la respuesta</param>
    public void CrearRespuestaMensaje(MERespuestaMensaje respuesta, string usuario)
    {
      using (EntidadesMensajeria contexto = new EntidadesMensajeria(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(nombreModelo)))
      {
        RespuestasMensajes_MEN respuestaDB = new RespuestasMensajes_MEN()
              {
                RES_FechaRespuesta = System.DateTime.Now,
                RES_IdMensaje = respuesta.IdMensaje,
                RES_IdUsuario = usuario,
                RES_TextoRespuesta = respuesta.Texto
              };
        contexto.RespuestasMensajes_MEN.Add(respuestaDB);
        contexto.SaveChanges();
      }
    }

    /// <summary>
    /// Actualiza el estado de un mensaje especifico a leido.
    /// </summary>
    /// <param name="idMensaje">Id del mensaje que se está actualizando</param>
    /// <param name="usuario">Usuario que leyó el mensaje</param>
    public void NotificarMensajeLeido(int idMensaje, string usuario)
    {
      using (EntidadesMensajeria contexto = new EntidadesMensajeria(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(nombreModelo)))
      {
        MensajesEnviados_MEN mensajeModificar = contexto.MensajesEnviados_MEN.SingleOrDefault(men => men.MEN_IdMensaje == idMensaje);
        mensajeModificar.MEN_EstadoNotificacion = "LEI";
        mensajeModificar.MEN_UsuarioQueLeyo = usuario;
        contexto.SaveChanges();
      }
    }

    #endregion Metodos publicos

    #region Metodos Privados

    /// <summary>
    /// Carga en una funcion recursiva todo el arbol de mensajes y respuestas
    /// </summary>
    /// <param name="mensaje">Mensaje que se quiere enlazar</param>
    /// <param name="contexto">Contexto sobre el cual se está trabajando</param>
    /// <returns></returns>
    private MEMensajeEnviado CargarArbolMensajeEnviado(MensajesEnviados_MEN mensaje, EntidadesMensajeria contexto)
    {
      CategoriaMensaje_MEN categoriaDb = contexto.CategoriaMensaje_MEN.SingleOrDefault(mod => mod.CAT_IdCategoriaMensaje == mensaje.MEN_CategoriaMensaje);
      MECategoriaMensaje categoria = new MECategoriaMensaje() { IdCategoria = categoriaDb.CAT_IdCategoriaMensaje, Descripcion = categoriaDb.CAT_Descripcion };
      //MECategoriaMensaje categoria = new MECategoriaMensaje() { IdCategoria = categoriaDb.CAT_IdCategoriaMensaje, Descripcion = categoriaDb.CAT_Descripcion };

      MEMensajeEnviado mensajeEnviado = new MEMensajeEnviado()
      {
        Asunto = mensaje.MEN_Asunto,
        Categoria = categoria.IdCategoria,
        EstadoNotificacion = mensaje.MEN_EstadoNotificacion.ToUpper().Trim().Equals("ENV") ? MEEnumEstadoNotificacion.Enviado : MEEnumEstadoNotificacion.Leido,
        UsuarioQueLeyo = string.IsNullOrEmpty(mensaje.MEN_UsuarioQueLeyo) ? "" : mensaje.MEN_UsuarioQueLeyo,
        FechaCreacion = mensaje.MEN_FechaCreacion,
        IdMensaje = mensaje.MEN_IdMensaje,
        MensajeOriginal = null,
        RespuestasMensaje = contexto.RespuestasMensajes_MEN.Where(res => res.RES_IdMensaje == mensaje.MEN_IdMensaje).ToList().ConvertAll(respuesta =>// mensaje.RespuestasMensajes_MEN.ToList().ConvertAll(respuesta =>
        {
          return new MERespuestaMensaje
          {
            Fecha = respuesta.RES_FechaRespuesta,
            IdRespuesta = respuesta.RES_IdRespuesta,
            Texto = respuesta.RES_TextoRespuesta,
            Usuario = respuesta.RES_IdUsuario
          };
        }),
        CentroServicioDestino = mensaje.MEN_IdPuntoAtencionDestino,
        DescCentroServicioDestino = mensaje.MEN_IdPuntoAtencionDestino + "/" + contexto.CentroServicios_PUA.Where(c => c.CES_IdCentroServicios == mensaje.MEN_IdPuntoAtencionDestino).SingleOrDefault().CES_Nombre,
        Texto = mensaje.MEN_TextoMensaje,
        UsuarioOrigen = mensaje.MEN_UsuarioOrigen,
        AceptaRespuestas = mensaje.MEN_AceptaRespuestas
      };

      if (mensaje.MEN_IdMensajeOriginal == 0)
      {
        return mensajeEnviado;
      }
      else
      {
        mensajeEnviado.MensajeOriginal = CargarArbolMensajeEnviado(contexto.MensajesEnviados_MEN.SingleOrDefault(mens => mens.MEN_IdMensaje == mensaje.MEN_IdMensajeOriginal), contexto);
        return mensajeEnviado;
      }
    }

    /// <summary>
    /// Consulta las regionales administrativas existentes y activas
    /// </summary>
    /// <returns></returns>
    public IEnumerable<MERacol> ConsultarRegionalesAdministrativas()
    {
      using (EntidadesMensajeria contexto = new EntidadesMensajeria(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(nombreModelo)))
      {
        IEnumerable<MERacol> racoles = contexto.CentroServicios_PUA.Join(contexto.RegionalAdministrativa_PUA, c => c.CES_IdCentroServicios, r => r.REA_IdRegionalAdm, (c, r) => new { idRacol = r.REA_IdRegionalAdm, NombreRacol = c.CES_Nombre, Estado = c.CES_Estado }).Where(r => r.Estado == "ACT").ToList().ConvertAll<MERacol>(r => new MERacol() { Descripcion = r.NombreRacol, IdRacol = r.idRacol });
        if (racoles.Count() > 0)
        {
          return racoles;
        }
        else
        {
          return new List<MERacol>();
        }
      }
    }

    /// <summary>
    /// Consulta las agencias y puntos pertenecientes a un racol
    /// </summary>
    /// <param name="idRacol">Racol por el cual se desea hacer la consulta</param>
    /// <returns></returns>
    public IEnumerable<MEAgencia> ConsultarAgenciasYPuntosXRacol(long idRacol)
    {
      using (EntidadesMensajeria contexto = new EntidadesMensajeria(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(nombreModelo)))
      {
        List<MEAgencia> AgenciasReturn = new List<MEAgencia>();

        List<Agencia_PUA> agencias = contexto.Agencia_PUA.Include("CentroServicios_PUA").Include("CentroLogistico_PUA").Where(ag => ag.CentroServicios_PUA.CES_Estado == "ACT" && ag.CentroLogistico_PUA.CEL_IdRegionalAdm == idRacol).ToList<Agencia_PUA>();
        List<PuntoServicio_PUA> puntos = contexto.PuntoServicio_PUA.Include("CentroServicios_PUA").Include("Agencia_PUA").Include("Agencia_PUA.CentroLogistico_PUA").Where(pu => pu.CentroServicios_PUA.CES_Estado == "ACT" && pu.Agencia_PUA.CentroLogistico_PUA.CEL_IdRegionalAdm == idRacol).ToList<PuntoServicio_PUA>();

        foreach (Agencia_PUA agencia in agencias)
        {
          MEAgencia agenciaMen =
              new MEAgencia()
              {
                IdAgencia = agencia.AGE_IdAgencia,
                Descripcion = agencia.CentroServicios_PUA.CES_Nombre
              };
          AgenciasReturn.Add(agenciaMen);
        }

        foreach (PuntoServicio_PUA punto in puntos)
        {
          MEAgencia agenciaMen =
              new MEAgencia()
              {
                IdAgencia = punto.PUS_IdPuntoServicio,
                Descripcion = punto.CentroServicios_PUA.CES_Nombre
              };
          AgenciasReturn.Add(agenciaMen);
        }

        if (agencias.Count() > 0)
        {
          return AgenciasReturn.OrderBy(or => or.Descripcion);
        }
        else
        {
          return new List<MEAgencia>();
        }
      }
    }

    /// <summary>
    /// Consulta las categorias de mensajes existentes
    /// </summary>
    /// <returns></returns>
    public IEnumerable<MECategoriaMensaje> ConsultarCategoriasMensaje()
    {
      using (EntidadesMensajeria contexto = new EntidadesMensajeria(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(nombreModelo)))
      {
        IEnumerable<MECategoriaMensaje> categorias = contexto.CategoriaMensaje_MEN.Where(c => c.CAT_EsExclusiva == false).Select(c => new MECategoriaMensaje() { IdCategoria = c.CAT_IdCategoriaMensaje, Descripcion = c.CAT_Descripcion });
        return categorias.ToList();
      }
    }

    private void ConsultarRutaImagenesAdjuntos()
    {
        if (string.IsNullOrEmpty(RutaImagenes))
        {
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["ControllerTransaccional"].ConnectionString))
            {
                SqlCommand cmd = new SqlCommand(@"SELECT PAR_ValorParametro FROM ParametrosFramework WHERE PAR_IdParametro = 'FolderServerDigita'", con);
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    RutaImagenes = reader["PAR_ValorParametro"].ToString();
                }

            }
        }
    }

    #endregion Metodos Privados
  }
}