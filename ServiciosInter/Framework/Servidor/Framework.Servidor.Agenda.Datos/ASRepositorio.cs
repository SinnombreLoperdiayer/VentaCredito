using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.ServiceModel;
using System.Transactions;
using Framework.Servidor.Agenda.Datos.Modelo;
using Framework.Servidor.Comun;
using Framework.Servidor.Excepciones;
using Framework.Servidor.Excepciones.Modelo;
using Framework.Servidor.ParametrosFW;
using Framework.Servidor.Servicios.ContratoDatos;
using Framework.Servidor.Servicios.ContratoDatos.Agenda;

namespace Framework.Servidor.Agenda.Datos
{
  /// <summary>
  /// Provee los métodos de acceso a los datos del modelo de Agenda
  /// </summary>
  public class ASRepositorio
  {
    #region Instancia Singleton de la clase

    /// <summary>
    /// Instancia de la clase ASRepositorio
    /// </summary>
    private static readonly ASRepositorio instancia = new ASRepositorio();

    /// <summary>
    /// Propiedad de la instancia de la clase
    /// </summary>
    public static ASRepositorio Instancia
    {
      get { return ASRepositorio.instancia; }
    }

    #endregion Instancia Singleton de la clase

    #region Constructor

    public ASRepositorio()
    {
      filePath = ConfigurationManager.AppSettings["Controller.RutaDescargaArchivos"];
    }

    #endregion Constructor

    #region Atributos

    private const string nombreModelo = "ModeloAgenda";
    string filePath = string.Empty;

    #endregion Atributos

    #region Métodos Consulta

    /// <summary>
    /// Consulta las Fallas asociadas a un módulo, que tengan actividades asociadas y en estado activo
    /// </summary>
    /// <param name="filtro">Filtro a aplicar sobre la consulta</param>
    /// <param name="totalRegistros">Total de registros que retorna la consulta</param>
    /// <param name="campoOrdenamiento">Campo por el cual se va a realizar el ordenamiento</param>
    /// <param name="indicePagina">Índie de página</param>
    /// <param name="registrosPorPagina">Cantidad de registros por página</param>
    /// <param name="esAscendente">Indica si el campo es ascendente</param>
    /// <returns>Lista de fallas</returns>
    public IEnumerable<ASFalla> ObtenerFallas(IDictionary<string, string> filtro, out int totalRegistros, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool esAscendente)
    {
      totalRegistros = 0;

      using (EntidadesAgenda contexto = new EntidadesAgenda(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(nombreModelo)))
      {
        if (!filtro.ContainsKey("FAL_Estado"))
          filtro.Add(new KeyValuePair<string, string>("FAL_Estado", ConstantesFramework.ESTADO_ACTIVO));
        filtro.Add(new KeyValuePair<string, string>("FAL_EsEditable", "true"));
        IEnumerable<Fallas_VASG> fallas = contexto.ConsultarContainsFallas_VASG(filtro,
                                campoOrdenamiento, out totalRegistros, indicePagina, registrosPorPagina, esAscendente);

        if (fallas.Count() > 0)
        {
          return fallas.ToList().ConvertAll(falla =>
            {
              return
              new ASFalla
              {
                EsEditable = falla.FAL_EsEditable,
                IdFalla = falla.FAL_IdFalla,
                Descripcion = falla.FAL_Descripcion.ToUpper().Trim(),
                ModuloDescripcion = falla.MOD_Descripcion,
                IdModulo = falla.FAL_IdModulo,
                Estado = new ASEstado() { IdEstado = falla.FAL_Estado },
                TipoFalla = falla.FAL_Tipo.ToUpper().Trim() == "MAN" ? ASETipoFalla.Manual : ASETipoFalla.Automática
              };
            });
        }
        else
        {
          return new List<ASFalla>();
        }
      }
    }

    /// <summary>
    /// Consulta las tareas asociadas a una falla. Valida que se encuentren en estado 'ACT', es decir, activas.
    /// </summary>
    /// <param name="idFalla">Identificador de la falla</param>
    /// <param name="campoOrdenamiento">Campo por el cual se desea hacer el ordenamiento</param>
    /// <param name="esAscendente">Indica si el ordenamiento es ascendente</param>
    /// <param name="indicePagina">Índice de página</param>
    /// <param name="registrosPorPagina">Número de registros a mostrar por página</param>
    /// <param name="totalRegistros">Retorna el total de registros que cumplen con la consulta</param>
    /// <returns>Lista de tareas</returns>
    public IEnumerable<ASTarea> ObtenerTareasAsociadasFalla(int idFalla, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool esAscendente, out int totalRegistros)
    {
      using (EntidadesAgenda contexto = new EntidadesAgenda(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(nombreModelo)))
      {
        // Consultamos las tareas, el campo de ordenamiento lo paso como vacío porque esta consulta no debe soportar ordenamiento debido a que se ejecuta en un datagrid doble cuyo datagrid principal es falla, por tanto ordena por fallas no por tareas
        // Pasamos el índice de página 0
        IEnumerable<Tarea_ASG> tareas = contexto.ConsultarTarea_ASG(tarea => tarea.TAR_IdFalla == idFalla,
                                   string.Empty, out totalRegistros, 0, registrosPorPagina, esAscendente);
        if (tareas != null && tareas.Count() > 0)
        {
          return tareas.ToList().ConvertAll(tarea =>
             new ASTarea
             {
               Cargo = new ASCargo()
                 {
                   IdCargo = tarea.TAR_CargoResponsable,
                   Descripcion = contexto.Cargo_SEG.FirstOrDefault(cargo =>
                   tarea.TAR_CargoResponsable == cargo.CAR_IdCargo).CAR_Descripcion
                 },
               Descripcion = tarea.TAR_Descripcion,
               TiempoEscalamiento = tarea.TAR_TiempoEscalamiento,
               IdTarea = tarea.TAR_IdTarea,
               Estado = tarea.TAR_Estado,
               IdFalla = idFalla,
               EstadoRegistro = EnumEstadoRegistro.SIN_CAMBIOS
             });
        }
        else
        {
          return new List<ASTarea>();
        }
      }
    }

    /// <summary>
    /// Indica cuál debe ser el límite de archivos que se permite adjuntar
    /// </summary>
    /// <returns></returns>
    public int ObtenerLimiteArchivosAdjuntos()
    {
      using (EntidadesAgenda contexto = new EntidadesAgenda(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(nombreModelo)))
      {
        ParametrosFramework parametro = contexto.ParametrosFramework.FirstOrDefault(par => par.PAR_IdParametro == "UserWCF");
        if (parametro != null)
        {
          int valorParametro = 0;
          if (!int.TryParse(parametro.PAR_ValorParametro, out valorParametro))
          {
            ControllerException excepcion =
               new ControllerException(ConstantesFramework.MODULO_FW_AGENDA,
                  ETipoErrorFramework.EX_PARAMETROS_ARCHIVOS_ADJUNTOS_NO_CONFIGURADO.ToString(),
                  MensajesFramework.CargarMensaje(ETipoErrorFramework.EX_PARAMETROS_ARCHIVOS_ADJUNTOS_NO_CONFIGURADO));
            throw new FaultException<ControllerException>(excepcion);
          }
          return valorParametro;
        }
        else
        {
          ControllerException excepcion =
                new ControllerException(ConstantesFramework.MODULO_FW_AGENDA,
                   ETipoErrorFramework.EX_PARAMETROS_ARCHIVOS_ADJUNTOS_NO_CONFIGURADO.ToString(),
                   MensajesFramework.CargarMensaje(ETipoErrorFramework.EX_PARAMETROS_ARCHIVOS_ADJUNTOS_NO_CONFIGURADO));
          throw new FaultException<ControllerException>(excepcion);
        }
      }
    }

    public ASFalla ObtenerFalla(int idFalla)
    {
      using (EntidadesAgenda contexto = new EntidadesAgenda(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(nombreModelo)))
      {
        Fallas_VASG falla = contexto.Fallas_VASG.Where(f => f.FAL_IdFalla == idFalla).FirstOrDefault();
        if (falla == null)
        {
          ControllerException excepcion = new ControllerException(COConstantesModulos.AGENDA, ETipoErrorFramework.EX_CONSULTA_DB_NULL.ToString(), MensajesFramework.CargarMensaje(ETipoErrorFramework.EX_CONSULTA_DB_NULL));
          throw new FaultException<ControllerException>(excepcion);
        }
        else
        {
          return new ASFalla()
  {
    IdFalla = falla.FAL_IdFalla,
    Descripcion = falla.FAL_Descripcion,
    Estado = new ASEstado() { IdEstado = falla.FAL_Estado },
    ModuloDescripcion = falla.MOD_Descripcion,
    IdModulo = falla.FAL_IdModulo,
    TipoFalla = falla.FAL_Tipo == "MAN" ? ASETipoFalla.Manual : ASETipoFalla.Automática,
    Tareas = contexto.Tarea_ASG.Where(tarea => tarea.TAR_IdFalla == falla.FAL_IdFalla && tarea.TAR_Estado == ConstantesFramework.ESTADO_ACTIVO).ToList().ConvertAll(
                                    tarea => new ASTarea()
                                    {
                                      IdTarea = tarea.TAR_IdTarea,
                                      Cargo = new ASCargo()
                                      {
                                        IdCargo = tarea.Cargo_SEG != null ? tarea.Cargo_SEG.CAR_IdCargo : 0
                                      },
                                      Descripcion = tarea.TAR_Descripcion,
                                      Estado = tarea.TAR_Estado,
                                      IdFalla = tarea.TAR_IdFalla,
                                      TiempoEscalamiento = tarea.TAR_TiempoEscalamiento
                                    })
  };
        }
      }
    }

    /// <summary>
    /// Retorna la lista de cargos disponibles
    /// </summary>
    /// <returns></returns>
    public IEnumerable<ASCargo> ObtenerCargos()
    {
      using (EntidadesAgenda contexto = new EntidadesAgenda(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(nombreModelo)))
      {
        IEnumerable<Cargo_SEG> cargos = contexto.Cargo_SEG.OrderBy(c => c.CAR_Descripcion);
        if (cargos.Count() > 0)
        {
          return cargos.ToList().ConvertAll(cargo =>
             new ASCargo
             {
               IdCargo = cargo.CAR_IdCargo,
               Descripcion = cargo.CAR_Descripcion
             });
        }
        else
        {
          return new List<ASCargo>();
        }
      }
    }

    /// <summary>
    /// Obtiene estados
    /// </summary>
    /// <returns></returns>
    public IEnumerable<ASEstado> ObtenerEstados()
    {
      using (EntidadesAgenda contexto = new EntidadesAgenda(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(nombreModelo)))
      {
        return contexto.EstadoActivoInactivo_VFRM.ToList().ConvertAll(estado => new ASEstado()
                                                                          {
                                                                            IdEstado = estado.IdEstado,
                                                                            Estado = estado.Estado
                                                                          });
      }
    }

    /// <summary>
    /// Consulta la lista de módulos del sistema
    /// </summary>
    /// <returns></returns>
    public IEnumerable<VEModulo> ObtenerModulos()
    {
      using (EntidadesAgenda contexto = new EntidadesAgenda(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(nombreModelo)))
      {
        return contexto.Modulo_VER.ToList().ConvertAll(modulo => new VEModulo()
        {
          IdModulo = modulo.MOD_IdModulo,
          Descripcion = modulo.MOD_Descripcion
        });
      }
    }

    /// <summary>
    /// Obtiene al lista de tareas en un estado
    /// </summary>
    /// <param name="usuario">Usuario que solicita la lista de tareas</param>
    /// <returns></returns>
    public IEnumerable<ASTareaAsignada> ObtenerTareasAsignadas(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros, string usuario)
    {
      using (EntidadesAgenda contexto = new EntidadesAgenda(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(nombreModelo)))
      {
        Dictionary<LambdaExpression, Framework.Servidor.Agenda.Datos.Modelo.OperadorLogico> where = new Dictionary<LambdaExpression, Framework.Servidor.Agenda.Datos.Modelo.OperadorLogico>();
        LambdaExpression lamda = contexto.CrearExpresionLambda<AsignacionTareaFalla_VASG>("AST_UsuarioResponsable", usuario, Framework.Servidor.Agenda.Datos.Modelo.OperadorComparacion.Equal);
        where.Add(lamda, Framework.Servidor.Agenda.Datos.Modelo.OperadorLogico.And);

        LambdaExpression lamda1 = contexto.CrearExpresionLambda<AsignacionTareaFalla_VASG>("AST_EstaAbierta", true.ToString(), Framework.Servidor.Agenda.Datos.Modelo.OperadorComparacion.Equal);
        where.Add(lamda1, Framework.Servidor.Agenda.Datos.Modelo.OperadorLogico.And);

        IEnumerable<ASTareaAsignada> lista = contexto.ConsultarAsignacionTareaFalla_VASG(filtro, where, campoOrdenamiento, out totalRegistros, indicePagina, registrosPorPagina, ordenamientoAscendente)
          .OrderByDescending(asignacion => asignacion.AST_FechaAsignacion)
          .ToList()
          .ConvertAll(asignacion => new ASTareaAsignada

          {
            Comentarios = asignacion.AST_Comentarios,
            Descripcion = asignacion.AST_DescripcionTarea,
            Falla = new ASFalla() { IdFalla = asignacion.AST_IdFalla, Descripcion = asignacion.AST_DescripcionFalla },
            FechaAsignacion = asignacion.AST_FechaAsignacion,
            IdAsignacionTarea = asignacion.AST_IdAsignacionTarea,
            IdTarea = asignacion.AST_IdTarea,
            FechaVencimiento = asignacion.AST_FechaVencimiento,
            TiempoEscalamiento = asignacion.AST_TiempoEscalamiento,
            TiempoExtension =
            contexto
            .SeguimientoAsignacionTareas_ASG
            .Where(j => j.SEA_IdAsignacionTarea == asignacion.AST_IdAsignacionTarea)
            .Sum(l => l.SEA_TiempoExtension),
            TiempoRestante = Framework.Servidor.ParametrosFW.PAAdministrador.Instancia.ConsultarDiasLaborales(DateTime.Now.Date, asignacion.AST_FechaVencimiento),
            UsuarioResponsable = asignacion.AST_UsuarioResponsable,
            ArchivosAdjuntos = ObtenerArchivosLista(asignacion.AST_IdAsignacionTarea),
          });
        return lista;
      }
    }

    /// <summary>
    /// Metodo para obtener los archivos de una asignacion de tarea
    /// </summary>
    /// <param name="idAsignacion"></param>
    /// <returns></returns>
    private List<ASArchivoFramework> ObtenerArchivosLista(long idAsignacion)
    {
      using (EntidadesAgenda contexto = new EntidadesAgenda(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(nombreModelo)))
      {
        var listaArchivos = contexto.ArchivoAsignacionTarea_ASG
          .Where(a => a.ARA_IdAsignacionTarea == idAsignacion)
          .ToList()
          .ConvertAll<ASArchivoFramework>(g => new ASArchivoFramework()
         {
           IdArchivo = g.ARA_IdArchivo,
           NombreArchivo = contexto.ArchivosFramework.Where(y => y.AFW_IdArchivo == g.ARA_IdArchivo).FirstOrDefault().AFW_NombreAdjunto,
           Fecha = g.ARA_FechaGrabacion,
         });
        return listaArchivos;
      }
    }

    /// <summary>
    /// Retorna la lista de histórico asignado de eventos asignados a una tarea
    /// </summary>
    /// <param name="idTarea">Identificador tarea</param>
    /// <returns></returns>
    public IEnumerable<ASEventoTareaAsignada> ObtenerHistoricoTareaAsignada(long idTarea)
    {
      using (EntidadesAgenda contexto = new EntidadesAgenda(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(nombreModelo)))
      {
        return contexto.SeguimientoAsignacionTareas_ASG.Where(seguimiento => seguimiento.SEA_IdAsignacionTarea == idTarea)
            .ToList()
            .ConvertAll(seguimiento => new ASEventoTareaAsignada()
              {
                Comentarios = seguimiento.SEA_Comentarios,
                EventoAsignacion = (ASEEventoAsignacion)seguimiento.SEA_IdEvento,
                FechaCreacion = seguimiento.SEA_FechaEvento
              });
      }
    }

    /// <summary>
    /// Obtiene un archivo adjunto por su id
    /// </summary>
    /// <param name="idArchivo"></param>
    /// <returns></returns>
    public string ObtenerArchivoAdjunto(long idArchivo)
    {
      using (EntidadesAgenda contexto = new EntidadesAgenda(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(nombreModelo)))
      {
        ArchivosFramework archivo = contexto.ArchivosFramework.FirstOrDefault(a => a.AFW_IdArchivo == idArchivo);
        return Convert.ToBase64String(archivo.AFW_Adjunto);
      }
    }

    /// <summary>
    /// Retorna la lista de cargos inferiores al cargo solicitante
    /// </summary>
    /// <param name="idCargo">Identificador del cargo solicitante</param>
    /// <param name="idUsuario">Identificador del usuario</param>
    /// <returns></returns>
    public IEnumerable<ASUsuario> ObtenerUsuariosConCargoInferior(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros, int idCargo)
    {
      string idUsuario = ControllerContext.Current.Usuario;
      using (EntidadesAgenda contexto = new EntidadesAgenda(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(nombreModelo)))
      {
        UsuarioInterno_VSEG usuarioReasignador = contexto.UsuarioInterno_VSEG.Where(u => u.USU_IdUsuario == idUsuario).First();

        Dictionary<LambdaExpression, Framework.Servidor.Agenda.Datos.Modelo.OperadorLogico> where = new Dictionary<LambdaExpression, Framework.Servidor.Agenda.Datos.Modelo.OperadorLogico>();
        LambdaExpression lamda = contexto.CrearExpresionLambda<UsuarioInterno_VSEG>("CAR_IdCargoReporta", idCargo.ToString(), Framework.Servidor.Agenda.Datos.Modelo.OperadorComparacion.Equal);
        where.Add(lamda, Framework.Servidor.Agenda.Datos.Modelo.OperadorLogico.And);

        //LambdaExpression lamda1 = contexto.CrearExpresionLambda<UsuarioInterno_VSEG>("PEI_IdRegionalAdm", usuarioReasignador.PEI_IdRegionalAdm.ToString(), Framework.Servidor.Agenda.Datos.Modelo.OperadorComparacion.Equal);
        //where.Add(lamda1, Framework.Servidor.Agenda.Datos.Modelo.OperadorLogico.And);

        LambdaExpression lamda2 = contexto.CrearExpresionLambda<UsuarioInterno_VSEG>("USU_Estado", ConstantesFramework.ESTADO_ACTIVO, Framework.Servidor.Agenda.Datos.Modelo.OperadorComparacion.Equal);
        where.Add(lamda2, Framework.Servidor.Agenda.Datos.Modelo.OperadorLogico.And);

        IEnumerable<ASUsuario> usuarios = contexto.ConsultarUsuarioInterno_VSEG(filtro, where, campoOrdenamiento, out totalRegistros, indicePagina, registrosPorPagina, ordenamientoAscendente)
          .ToList()
          .ConvertAll(u =>
            {
              CentroLogistico_PUA centrologistoUsuario = ObtenerCentroLogisticoUsuario(u.USP_IdPersonaInterna);
              ASUsuario assu = new ASUsuario()
                                            {
                                              CentroLogistico = new Servicios.ContratoDatos.Seguridad.SECentroLogistico()
                                              {
                                                IdCentroLogistico = centrologistoUsuario != null ? centrologistoUsuario.CEL_IdCentroLogistico : 0,
                                                // IdCentroLogistico = contexto.PersonaIntCentroLogistico_SEG.Where(c => c.PIC_IdPersonaInterna == u.USP_IdPersonaInterna).FirstOrDefault() != null ? contexto.PersonaIntCentroLogistico_SEG.Where(c => c.PIC_IdPersonaInterna == u.USP_IdPersonaInterna).FirstOrDefault().PIC_IdCentroLogistico : 0,
                                                DescripcionCentroLogistico = ConstantesFramework.SIN_CENTRO_SERVICIO
                                              },
                                              Nombre = u.PEI_Nombre,
                                              PrimerApellido = u.PEI_PrimerApellido,
                                              SegundoApellido = u.PEI_SegundoApellido,
                                              Regional = new Servicios.ContratoDatos.Seguridad.SERegional
                                              {
                                                DescripcionRegional = u.REA_Descripcion,
                                                IdRegional = u.PEI_IdRegionalAdm.Value
                                              },
                                              IdUsuario = u.USU_IdUsuario,
                                              Cargo = new ASCargo()
                                              {
                                                Descripcion = u.CAR_Descripcion,
                                                IdCargo = u.PEI_IdCargo
                                              }
                                            };
              return assu;
            }
                                       );

        /// Solo puede reasignar a usuarios que pertenezcan al mismo centro logístico
        ///
        //CentroLogistico_PUA centro = ObtenerCentroLogisticoUsuario(usuarioReasignador.USP_IdPersonaInterna);

        //if (centro != null)
        //{
        //  return usuarios.Where(u => u.CentroLogistico.IdCentroLogistico == centro.CEL_IdCentroLogistico).OrderBy(u => campoOrdenamiento).ToList();
        //}
        return usuarios.OrderBy(u => campoOrdenamiento).ToList();
      }
    }

    /// <summary>
    /// Retorna la lista de cargos inferiores al cargo solicitante
    /// </summary>
    /// <param name="idCargo">Identificador del cargo solicitante</param>
    /// <param name="idUsuario">Identificador del usuario</param>
    /// <returns></returns>
    public IEnumerable<ASUsuario> ObtenerUsuariosConCargoInferior(int idCargo, string idUsuario)
    {
      using (EntidadesAgenda contexto = new EntidadesAgenda(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(nombreModelo)))
      {
        UsuarioInterno_VSEG usuarioReasignador = contexto.UsuarioInterno_VSEG.Where(u => u.USU_IdUsuario == idUsuario).First();

        IEnumerable<ASUsuario> usuarios = contexto.UsuarioInterno_VSEG
                                 .Where(usuario
                                       =>
                                         usuario.CAR_IdCargoReporta == idCargo &&
                                         usuario.PEI_IdRegionalAdm == usuarioReasignador.PEI_IdRegionalAdm &&
                                         usuario.USU_Estado == ConstantesFramework.ESTADO_ACTIVO)
                                 .ToList()
                                 .ConvertAll(u =>
                                   {
                                     CentroLogistico_PUA centrologistoUsuario = ObtenerCentroLogisticoUsuario(u.USP_IdPersonaInterna);
                                     ASUsuario assu = new ASUsuario
                                               {
                                                 CentroLogistico = new Servicios.ContratoDatos.Seguridad.SECentroLogistico()
                                                 {
                                                   IdCentroLogistico = centrologistoUsuario != null ? centrologistoUsuario.CEL_IdCentroLogistico : 0
                                                 },
                                                 Nombre = u.PEI_Nombre,
                                                 PrimerApellido = u.PEI_PrimerApellido,
                                                 SegundoApellido = u.PEI_SegundoApellido,
                                                 Regional = new Servicios.ContratoDatos.Seguridad.SERegional
                                                 {
                                                   DescripcionRegional = u.REA_Descripcion,
                                                   IdRegional = u.PEI_IdRegionalAdm.Value
                                                 },
                                                 IdUsuario = u.USU_IdUsuario,
                                                 Cargo = new ASCargo()
                                                {
                                                  Descripcion = u.CAR_Descripcion,
                                                  IdCargo = u.PEI_IdCargo
                                                }
                                               };
                                     return assu;
                                   }
                                          );

        /// Solo puede reasignar a usuarios que pertenezcan al mismo centro logístico
        ///
        CentroLogistico_PUA centro = ObtenerCentroLogisticoUsuario(usuarioReasignador.USP_IdPersonaInterna);
        if (centro != null)
        {
          return usuarios.Where(u => u.CentroLogistico.IdCentroLogistico == centro.CEL_IdCentroLogistico);
        }
        return usuarios;
      }
    }

    /// <summary>
    /// Obtiene la información básica de un usuario
    /// </summary>
    /// <param name="idUsuario"></param>
    /// <returns></returns>
    public ASUsuario ObtenerUsuario(string idUsuario)
    {
      using (EntidadesAgenda contexto = new EntidadesAgenda(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(nombreModelo)))
      {
        UsuarioInterno_VSEG usuario = contexto.UsuarioInterno_VSEG.Where(u => u.USU_IdUsuario == idUsuario).First();

        return new ASUsuario
        {
          Cargo = new ASCargo
          {
            Descripcion = usuario.CAR_Descripcion,
            IdCargo = usuario.PEI_IdCargo
          },
          IdUsuario = usuario.USU_IdUsuario,
          Nombre = usuario.PEI_Nombre,
          PrimerApellido = usuario.PEI_PrimerApellido,
          SegundoApellido = usuario.PEI_SegundoApellido,
          TipoUsuario = usuario.USU_TipoUsuario
        };
      }
    }

    private CentroLogistico_PUA ObtenerCentroLogisticoUsuario(long idUsuario)
    {
      using (EntidadesAgenda contexto = new EntidadesAgenda(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(nombreModelo)))
      {
        return contexto.UsuarioCentroServicio_SEG.Where(w => w.UCS_IdCodigoUsuario == idUsuario).Join(contexto.CentroLogistico_PUA, left => left.UCS_IdCentroServicios, right => right.CEL_IdCentroLogistico, (left, right) => right).FirstOrDefault();
      }
    }

    #endregion Métodos Consulta

    #region Métodos Inserción

    /// <summary>
    /// Hace asignación de una tarea manual
    /// </summary>
    /// <param name="tarea">Tarea a asignar</param>
    /// <param name="comentario">Comentario relacionados con la asignación</param>
    /// <param name="eventoAsignacion"></param>
    /// <param name="usuarioAplicacion">Usuario de la aplicación</param>
    /// <returns></returns>
    public bool AsignarTareaManual(ASTarea tarea, string comentario, ASEEventoAsignacion eventoAsignacion, string usuarioAplicacion, List<string> archivos)
    {
      using (EntidadesAgenda contexto = new EntidadesAgenda(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(nombreModelo)))
      {
        Tarea_ASG t = contexto.Tarea_ASG.Include("Cargo_SEG").FirstOrDefault(ta => ta.TAR_IdTarea == tarea.IdTarea);
        if (t != null)
        {
          UsuarioInterno_VSEG usuarioSolicitante = contexto.UsuarioInterno_VSEG.Where(u =>
            u.USU_IdUsuario == usuarioAplicacion)
            .First();

          if (usuarioSolicitante != null)
          {
            UsuarioInterno_VSEG usuarioResponsable;
            CentroLogistico_PUA centroSolicitante = ObtenerCentroLogisticoUsuario(usuarioSolicitante.USP_IdPersonaInterna);

            // * Si el Usuario que esta asignando la falla no pertenece a un Centro Logístico se buscan los usuarios de la misma Regional Administrativa a
            //   la que pertenece el Usuario que esta asignando la falla, y que esté en la jerarquía de cargos inmediatamente inferiores

            if (centroSolicitante == null)
            {
              usuarioResponsable = contexto.UsuarioInterno_VSEG.Where(u =>
                                   u.CAR_IdCargoReporta == t.Cargo_SEG.CAR_IdCargo
                                && u.PEI_IdRegionalAdm == usuarioSolicitante.PEI_IdRegionalAdm
                                && u.USU_Estado == ConstantesFramework.ESTADO_ACTIVO).FirstOrDefault();
            }
            else
            {
              // El usuario al que se le asigna la tarea debe cumplir con las siguientes condiciones:
              // * Tener el cargo configurado para resolver la tarea.
              // * Debe pertenecer al mismo centro logístico del usuario que hace la solicitud
              // * El usuario debe estar activo

              usuarioResponsable = contexto.UsuarioInterno_VSEG.Where(u =>
                           u.PEI_IdCargo == t.Cargo_SEG.CAR_IdCargo
                        && u.PEI_IdRegionalAdm == usuarioSolicitante.PEI_IdRegionalAdm
                        && u.USU_Estado == ConstantesFramework.ESTADO_ACTIVO)
                        .Join(contexto.UsuarioCentroServicio_SEG, left => left.USP_IdPersonaInterna, right => right.UCS_IdCodigoUsuario, (left, right) => new { left, right })
                        .Where(c => c.right.UCS_IdCentroServicios == centroSolicitante.CEL_IdCentroLogistico).Select(c => c.left)
                        .FirstOrDefault();
            }
            if (usuarioResponsable != null)
            {
              Falla_ASG falla = contexto.Falla_ASG.Where(d => d.FAL_IdFalla == tarea.IdFalla).FirstOrDefault();
              Modulo_VER modulo = contexto.Modulo_VER.Where(s => s.MOD_IdModulo == falla.FAL_IdModulo).FirstOrDefault();
              AsignacionTarea_ASG asignacion = new AsignacionTarea_ASG()
                    {
                      AST_Comentarios = comentario,
                      AST_DescCargoResponsable = t.Cargo_SEG.CAR_Descripcion,
                      AST_DescripcionFalla = falla.FAL_Descripcion,
                      AST_DescripcionTarea = tarea.Descripcion,
                      AST_DescripcionModulo = modulo.MOD_Descripcion,
                      AST_FechaAsignacion = DateTime.Now,
                      AST_IdCargoResponsable = t.Cargo_SEG.CAR_IdCargo,
                      AST_IdFalla = tarea.IdFalla,
                      AST_IdModulo = modulo.MOD_IdModulo,
                      AST_IdTarea = tarea.IdTarea,
                      AST_UsuarioResponsable = usuarioResponsable.USU_IdUsuario,
                      AST_TiempoEscalamiento = tarea.TiempoEscalamiento,
                      AST_EstaAbierta = true,
                      AST_FechaVencimiento = Framework.Servidor.ParametrosFW.PAAdministrador.Instancia.AgregarDiasLaborales(DateTime.Now, tarea.TiempoEscalamiento),
                      AST_CreadoPor = ControllerContext.Current.Usuario,
                    };

              // Grabando la asignación de la tarea
              contexto.AsignacionTarea_ASG.Add(asignacion);

              // Registrar el seguimiento de las tareas
              contexto.SeguimientoAsignacionTareas_ASG.Add(new SeguimientoAsignacionTareas_ASG()
                    {
                      SEA_IdAsignacionTarea = asignacion.AST_IdAsignacionTarea,
                      SEA_FechaEscalamiento = Framework.Servidor.ParametrosFW.PAAdministrador.Instancia.AgregarDiasLaborales(DateTime.Now, tarea.TiempoEscalamiento),
                      SEA_TiempoEscalamiento = tarea.TiempoEscalamiento,
                      SEA_FechaEvento = DateTime.Now,
                      SEA_IdEvento = (int)eventoAsignacion,
                      SEA_Comentarios = comentario,
                      SEA_DescCargoResponsable = t.Cargo_SEG.CAR_Descripcion,
                      SEA_IdCargoResponsable = t.Cargo_SEG.CAR_IdCargo,
                      SEA_UsuarioResponsable = usuarioResponsable.USU_IdUsuario,
                      SEA_UsuarioEvento = usuarioAplicacion,
                      SEA_CreadoPor = ControllerContext.Current.Usuario,
                    });

              contexto.SaveChanges();

              // Adjuntar archivos
              if (archivos != null && archivos.Count > 0)
              {
                archivos.ForEach(archivo =>
                  {
                    string rutaArchivo = Path.Combine(this.filePath, COConstantesModulos.PARAMETROS_GENERALES, archivo);
                    ArchivosFramework arch = new ArchivosFramework()
                    {
                      //AFW_Adjunto = System.IO.File.ReadAllBytes(rutaArchivo),
                      AFW_Descripcion = string.Empty,
                      AFW_FechaCargaArchivo = DateTime.Now,
                      AFW_IdAdjunto = Guid.NewGuid(),
                      AFW_NombreAdjunto = System.IO.Path.GetFileName(archivo),
                      AFW_Usuario = usuarioAplicacion
                    };

                    using (FileStream fs = File.OpenRead(rutaArchivo))
                    {
                      byte[] bytes = new byte[fs.Length];
                      fs.Read(bytes, 0, Convert.ToInt32(fs.Length));
                      fs.Close();
                      arch.AFW_Adjunto = bytes;
                    }

                    contexto.ArchivosFramework.Add(arch);

                    ArchivoAsignacionTarea_ASG archivoAsignacion = new ArchivoAsignacionTarea_ASG()
                    {
                      ARA_IdArchivo = arch.AFW_IdArchivo,
                      ARA_IdAsignacionTarea = asignacion.AST_IdAsignacionTarea,
                      ARA_CreadoPor = usuarioAplicacion,
                      ARA_FechaGrabacion = DateTime.Now
                    };
                    contexto.ArchivoAsignacionTarea_ASG.Add(archivoAsignacion);
                    contexto.SaveChanges();
                  });
              }
              return true;
            }
            else
            {
              ControllerException excepcion = new ControllerException(ConstantesFramework.MODULO_FW_AGENDA, ETipoErrorFramework.EX_ERROR_CARGO_SIN_USUARIO_ASIGNADO.ToString(), MensajesFramework.CargarMensaje(ETipoErrorFramework.EX_ERROR_CARGO_SIN_USUARIO_ASIGNADO));
              throw new FaultException<ControllerException>(excepcion);
            }
          }
          else
          {
            ControllerException excepcion = new ControllerException(ConstantesFramework.MODULO_FW_AGENDA, ETipoErrorFramework.EX_ERROR_USUARIO_NO_AUTORIZADO_ASIGNACION_TAREAS.ToString(), MensajesFramework.CargarMensaje(ETipoErrorFramework.EX_ERROR_USUARIO_NO_AUTORIZADO_ASIGNACION_TAREAS));
            throw new FaultException<ControllerException>(excepcion);
          }
        }

        else
        {
          ControllerException excepcion = new ControllerException(ConstantesFramework.MODULO_FW_AGENDA, ETipoErrorFramework.EX_ERROR_TAREA_MAL_CONFIGURADA.ToString(), MensajesFramework.CargarMensaje(ETipoErrorFramework.EX_ERROR_TAREA_MAL_CONFIGURADA));
          throw new FaultException<ControllerException>(excepcion);
        }

        //else
        //{
        //  ControllerException excepcion = new ControllerException(ConstantesFramework.MODULO_FW_AGENDA, ETipoErrorFramework.EX_CONSULTA_DB_NULL.ToString(), MensajesFramework.CargarMensaje(ETipoErrorFramework.EX_CONSULTA_DB_NULL));
        //  throw new FaultException<ControllerException>(excepcion);
        //}
      }
    }

    /// <summary>
    /// Registra asignación de tarea manual por agenda
    /// </summary>
    /// <param name="tarea"></param>
    /// <param name="usuarioAplicacion"></param>
    /// <param name="usuarioResponsable"></param>
    /// <returns></returns>
    public bool AsignarTareaManualPorAgenda(ASTareaPorAgenda tarea, string usuarioAplicacion, ASUsuario usuarioResponsable)
    {
      using (EntidadesAgenda contexto = new EntidadesAgenda(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(nombreModelo)))
      {
        using (TransactionScope scope = new TransactionScope())
        {
          if (usuarioResponsable != null)
          {
            AsignacionTarea_ASG asignacion = new AsignacionTarea_ASG()
            {
              AST_Comentarios = tarea.Comentario,
              AST_DescCargoResponsable = usuarioResponsable.Cargo.Descripcion,
              AST_DescripcionFalla = string.Empty,
              AST_DescripcionTarea = string.Empty,
              AST_DescripcionModulo = string.Empty,
              AST_FechaAsignacion = DateTime.Now,
              AST_IdCargoResponsable = usuarioResponsable.Cargo.IdCargo,
              AST_IdFalla = 0,
              AST_IdModulo = string.Empty,
              AST_IdTarea = 0,
              AST_UsuarioResponsable = usuarioResponsable.IdUsuario,
              AST_TiempoEscalamiento = tarea.TiempoRespuesta,
              AST_EstaAbierta = true,
              AST_FechaVencimiento = Framework.Servidor.ParametrosFW.PAAdministrador.Instancia.AgregarDiasLaborales(DateTime.Now, tarea.TiempoRespuesta),
              AST_CreadoPor = ControllerContext.Current.Usuario,
            };

            // Grabando la asignación de la tarea
            contexto.AsignacionTarea_ASG.Add(asignacion);

            // Registrar el seguimiento de las tareas
            contexto.SeguimientoAsignacionTareas_ASG.Add(new SeguimientoAsignacionTareas_ASG()
            {
              SEA_IdAsignacionTarea = asignacion.AST_IdAsignacionTarea,
              SEA_FechaEscalamiento = Framework.Servidor.ParametrosFW.PAAdministrador.Instancia.AgregarDiasLaborales(DateTime.Now, tarea.TiempoRespuesta),
              SEA_TiempoEscalamiento = tarea.TiempoRespuesta,
              SEA_FechaEvento = DateTime.Now,
              SEA_IdEvento = (int)tarea.EventoAsignacion,
              SEA_Comentarios = tarea.Comentario,
              SEA_DescCargoResponsable = usuarioResponsable.Cargo.Descripcion,
              SEA_IdCargoResponsable = usuarioResponsable.Cargo.IdCargo,
              SEA_UsuarioResponsable = usuarioResponsable.IdUsuario,
              SEA_UsuarioEvento = usuarioAplicacion,
              SEA_CreadoPor = ControllerContext.Current.Usuario,
            });

            contexto.SaveChanges();

            // Adjuntar archivos
            if (tarea.Archivos != null && tarea.Archivos.Count > 0)
            {
              tarea.Archivos.ForEach(archivo =>
              {
                string rutaArchivo = Path.Combine(this.filePath, COConstantesModulos.PARAMETROS_GENERALES, archivo);
                ArchivosFramework arch = new ArchivosFramework()
                {
                  //AFW_Adjunto = System.IO.File.ReadAllBytes(rutaArchivo),
                  AFW_Descripcion = string.Empty,
                  AFW_FechaCargaArchivo = DateTime.Now,
                  AFW_IdAdjunto = Guid.NewGuid(),
                  AFW_NombreAdjunto = System.IO.Path.GetFileName(archivo),
                  AFW_Usuario = usuarioAplicacion
                };
                using (FileStream fs = File.OpenRead(rutaArchivo))
                {
                  byte[] bytes = new byte[fs.Length];
                  fs.Read(bytes, 0, Convert.ToInt32(fs.Length));
                  fs.Close();
                  arch.AFW_Adjunto = bytes;
                }

                contexto.ArchivosFramework.Add(arch);

                ArchivoAsignacionTarea_ASG archivoAsignacion = new ArchivoAsignacionTarea_ASG()
                {
                  ARA_IdArchivo = arch.AFW_IdArchivo,
                  ARA_IdAsignacionTarea = asignacion.AST_IdAsignacionTarea,
                  ARA_CreadoPor = usuarioAplicacion,
                  ARA_FechaGrabacion = DateTime.Now
                };
                contexto.ArchivoAsignacionTarea_ASG.Add(archivoAsignacion);
                contexto.SaveChanges();
              });
            }

            scope.Complete();
            return true;
          }
          else
          {
            ControllerException excepcion = new ControllerException(ConstantesFramework.MODULO_FW_AGENDA, ETipoErrorFramework.EX_ERROR_USUARIO_NO_AUTORIZADO_ASIGNACION_TAREAS.ToString(), MensajesFramework.CargarMensaje(ETipoErrorFramework.EX_ERROR_USUARIO_NO_AUTORIZADO_ASIGNACION_TAREAS));
            throw new FaultException<ControllerException>(excepcion);
          }
        }
      }
    }

    /// <summary>
    /// Hace asignación manual de muchas tareas asocidas a la misma falla. Este proceso aplica solo para fallas que no son editables, es decir que son automáticas NO manuales. Para el caso de uso de auto asignación de tareas
    /// </summary>
    /// <param name="falla">Falla sobre la cual se van a asignar las tareas</param>
    /// <param name="tareas">Tareas a asignar</param>
    /// <param name="comentario">Comentario asociados a las tareas asignadas</param>
    /// <param name="eventoAsignacion"></param>
    /// <param name="usuarioAplicacion">Usuario que ejecuta la aplicación al momento de hacer asignación de tareas</param>
    /// <returns></returns>
    public bool AsignarTareas(ASFalla falla, IEnumerable<ASTarea> tareas, string comentario, ASEEventoAsignacion eventoAsignacion, string usuarioAplicacion)
    {
      using (EntidadesAgenda contexto = new EntidadesAgenda(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(nombreModelo)))
      {
        tareas.ToList().ForEach(tarea =>
          {
            Tarea_ASG t = contexto.Tarea_ASG.Include("Cargo_SEG").FirstOrDefault(ta => ta.TAR_IdTarea == tarea.IdTarea);
            if (t != null)
            {
              UsuarioInterno_VSEG usuarioSolicitante = contexto.UsuarioInterno_VSEG.Where(u => u.USU_IdUsuario == usuarioAplicacion).FirstOrDefault();
              if (usuarioSolicitante != null)
              {
                CentroLogistico_PUA centroLogistico = ObtenerCentroLogisticoUsuario(usuarioSolicitante.USP_IdPersonaInterna);

                if (centroLogistico != null)
                {
                  // El usuario al que se le asigna la tarea debe cumplir con las siguientes condiciones:
                  // * Tener el cargo configurado para resolver la tarea.
                  // * Debe pertenecer al mismo centro logístico del usuario que hace la solicitud
                  // * El usuario debe estar activo

                  UsuarioInterno_VSEG usuarioResponsable = contexto.UsuarioInterno_VSEG.Where(u =>
                             u.PEI_IdCargo == t.Cargo_SEG.CAR_IdCargo
                          && u.PEI_IdRegionalAdm == usuarioSolicitante.PEI_IdRegionalAdm
                          && u.USU_Estado == ConstantesFramework.ESTADO_ACTIVO)
                          .Join(contexto.UsuarioCentroServicio_SEG, left => left.USP_IdPersonaInterna, right => right.UCS_IdCodigoUsuario, (left, right) => new { left, right })
                        .Where(c => c.right.UCS_IdCentroServicios == centroLogistico.CEL_IdCentroLogistico).Select(c => c.left)
                        .FirstOrDefault();

                  if (usuarioResponsable != null)
                  {
                    Falla_ASG fallaTarea = contexto.Falla_ASG.Where(d => d.FAL_IdFalla == tarea.IdFalla).FirstOrDefault();

                    Modulo_VER modulo = contexto.Modulo_VER.Where(s => s.MOD_IdModulo == fallaTarea.FAL_IdModulo).FirstOrDefault();

                    AsignacionTarea_ASG asignacion = new AsignacionTarea_ASG()
                    {
                      AST_Comentarios = comentario,
                      AST_DescCargoResponsable = t.Cargo_SEG.CAR_Descripcion,
                      AST_DescripcionFalla = fallaTarea.FAL_Descripcion,
                      AST_DescripcionTarea = tarea.Descripcion,
                      AST_DescripcionModulo = modulo.MOD_Descripcion,
                      AST_FechaAsignacion = DateTime.Now,
                      AST_IdCargoResponsable = t.Cargo_SEG.CAR_IdCargo,
                      AST_IdFalla = tarea.IdFalla,
                      AST_IdModulo = modulo.MOD_IdModulo,
                      AST_IdTarea = tarea.IdTarea,
                      AST_UsuarioResponsable = usuarioResponsable.USU_IdUsuario,
                      AST_TiempoEscalamiento = tarea.TiempoEscalamiento,
                      AST_EstaAbierta = true,
                      AST_FechaVencimiento = Framework.Servidor.ParametrosFW.PAAdministrador.Instancia.AgregarDiasLaborales(DateTime.Now, tarea.TiempoEscalamiento),
                      AST_CreadoPor = usuarioAplicacion,
                    };

                    // Grabando la asignación de la tarea
                    contexto.AsignacionTarea_ASG.Add(asignacion);

                    SeguimientoAsignacionTareas_ASG seguimientoAsignacionTareas = new SeguimientoAsignacionTareas_ASG()
                    {
                      SEA_IdAsignacionTarea = asignacion.AST_IdAsignacionTarea,
                      SEA_FechaEvento = DateTime.Now,
                      SEA_FechaEscalamiento = Framework.Servidor.ParametrosFW.PAAdministrador.Instancia.AgregarDiasLaborales(DateTime.Now, tarea.TiempoEscalamiento),
                      SEA_TiempoEscalamiento = tarea.TiempoEscalamiento,
                      SEA_IdEvento = (int)eventoAsignacion,
                      SEA_Comentarios = comentario,
                      SEA_DescCargoResponsable = t.Cargo_SEG.CAR_Descripcion,
                      SEA_IdCargoResponsable = t.Cargo_SEG.CAR_IdCargo,
                      SEA_UsuarioResponsable = usuarioResponsable.USU_IdUsuario,
                      SEA_UsuarioEvento = usuarioAplicacion,
                      SEA_CreadoPor = usuarioAplicacion,
                    };

                    contexto.SeguimientoAsignacionTareas_ASG.Add(seguimientoAsignacionTareas);

                    contexto.SaveChanges();
                    // Registrar el seguimiento de las tareas
                  }
                  else
                  {
                    ControllerException excepcion = new ControllerException(ConstantesFramework.MODULO_FW_AGENDA, ETipoErrorFramework.EX_ERROR_CARGO_SIN_USUARIO_ASIGNADO.ToString(), MensajesFramework.CargarMensaje(ETipoErrorFramework.EX_ERROR_CARGO_SIN_USUARIO_ASIGNADO));
                    throw new FaultException<ControllerException>(excepcion);
                  }
                }
                else
                {
                  ControllerException excepcion = new ControllerException(ConstantesFramework.MODULO_FW_AGENDA, ETipoErrorFramework.EX_ERROR_USUARIO_NO_AUTORIZADO_ASIGNACION_TAREAS.ToString(), MensajesFramework.CargarMensaje(ETipoErrorFramework.EX_ERROR_USUARIO_NO_AUTORIZADO_ASIGNACION_TAREAS));
                  throw new FaultException<ControllerException>(excepcion);
                }
              }
              else
              {
                ControllerException excepcion = new ControllerException(ConstantesFramework.MODULO_FW_AGENDA, ETipoErrorFramework.EX_ERROR_USUARIO_NO_AUTORIZADO_ASIGNACION_TAREAS.ToString(), MensajesFramework.CargarMensaje(ETipoErrorFramework.EX_ERROR_USUARIO_NO_AUTORIZADO_ASIGNACION_TAREAS));
                throw new FaultException<ControllerException>(excepcion);
              }
            }
          });
        return true;
      }
    }

    /// <summary>
    /// Hace asignación manual de muchas tareas asocidas a la misma falla. Este proceso aplica solo para fallas que no son editables, es decir que son automáticas NO manuales. Para el caso de uso de auto asignación de tareas
    /// Centro de servicio Origen
    /// </summary>
    /// <param name="falla">Falla sobre la cual se van a asignar las tareas</param>
    /// <param name="tareas">Tareas a asignar</param>
    /// <param name="comentario">Comentario asociados a las tareas asignadas</param>
    /// <param name="eventoAsignacion"></param>
    /// <param name="usuarioAplicacion">Usuario que ejecuta la aplicación al momento de hacer asignación de tareas</param>
    /// <returns></returns>
    public bool AsignarTareasOrigen(ASFalla falla, IEnumerable<ASTarea> tareas, string comentario, ASEEventoAsignacion eventoAsignacion, string usuarioAplicacion, long idCentroServicios)
    {
      using (EntidadesAgenda contexto = new EntidadesAgenda(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(nombreModelo)))
      {
        tareas.ToList().ForEach(tarea =>
        {
          Tarea_ASG t = contexto.Tarea_ASG.Include("Cargo_SEG").FirstOrDefault(ta => ta.TAR_IdTarea == tarea.IdTarea);
          if (t != null)
          {
            // El usuario al que se le asigna la tarea debe cumplir con las siguientes condiciones:
            // * Tener el cargo configurado para resolver la tarea.
            // * Debe pertenecer al mismo centro logístico del usuario que hace la solicitud
            // * El usuario debe estar activo
            UsuarioInterno_VSEG usuarioResponsable = contexto.UsuarioInterno_VSEG.Where(u =>
                       u.PEI_IdCargo == t.Cargo_SEG.CAR_IdCargo
                    && u.USU_Estado == ConstantesFramework.ESTADO_ACTIVO)
                    .Join(contexto.UsuarioCentroServicio_SEG, left => left.USP_IdPersonaInterna, right => right.UCS_IdCodigoUsuario, (left, right) => new { left, right })
                        .Where(c => c.right.UCS_IdCentroServicios == idCentroServicios).Select(c => c.left)
                        .FirstOrDefault();

            if (usuarioResponsable != null)
            {
              Falla_ASG fallaTarea = contexto.Falla_ASG.Where(d => d.FAL_IdFalla == tarea.IdFalla).FirstOrDefault();

              Modulo_VER modulo = contexto.Modulo_VER.Where(s => s.MOD_IdModulo == fallaTarea.FAL_IdModulo).FirstOrDefault();

              AsignacionTarea_ASG asignacion = new AsignacionTarea_ASG()
              {
                AST_Comentarios = comentario,
                AST_DescCargoResponsable = t.Cargo_SEG.CAR_Descripcion,
                AST_DescripcionFalla = fallaTarea.FAL_Descripcion,
                AST_DescripcionTarea = tarea.Descripcion,
                AST_DescripcionModulo = modulo.MOD_Descripcion,
                AST_FechaAsignacion = DateTime.Now,
                AST_IdCargoResponsable = t.Cargo_SEG.CAR_IdCargo,
                AST_IdFalla = tarea.IdFalla,
                AST_IdModulo = modulo.MOD_IdModulo,
                AST_IdTarea = tarea.IdTarea,
                AST_UsuarioResponsable = usuarioResponsable.USU_IdUsuario,
                AST_TiempoEscalamiento = tarea.TiempoEscalamiento,
                AST_EstaAbierta = true,
                AST_FechaVencimiento = Framework.Servidor.ParametrosFW.PAAdministrador.Instancia.AgregarDiasLaborales(DateTime.Now, tarea.TiempoEscalamiento),
                AST_CreadoPor = usuarioAplicacion,
              };

              // Grabando la asignación de la tarea
              contexto.AsignacionTarea_ASG.Add(asignacion);

              SeguimientoAsignacionTareas_ASG seguimientoAsignacionTareas = new SeguimientoAsignacionTareas_ASG()
              {
                SEA_IdAsignacionTarea = asignacion.AST_IdAsignacionTarea,
                SEA_FechaEvento = DateTime.Now,
                SEA_FechaEscalamiento = Framework.Servidor.ParametrosFW.PAAdministrador.Instancia.AgregarDiasLaborales(DateTime.Now, tarea.TiempoEscalamiento),
                SEA_TiempoEscalamiento = tarea.TiempoEscalamiento,
                SEA_IdEvento = (int)eventoAsignacion,
                SEA_Comentarios = comentario,
                SEA_DescCargoResponsable = t.Cargo_SEG.CAR_Descripcion,
                SEA_IdCargoResponsable = t.Cargo_SEG.CAR_IdCargo,
                SEA_UsuarioResponsable = usuarioResponsable.USU_IdUsuario,
                SEA_UsuarioEvento = usuarioAplicacion,
                SEA_CreadoPor = usuarioAplicacion,
              };

              contexto.SeguimientoAsignacionTareas_ASG.Add(seguimientoAsignacionTareas);

              contexto.SaveChanges();
              // Registrar el seguimiento de las tareas
            }
            else
            {
              ControllerException excepcion = new ControllerException(ConstantesFramework.MODULO_FW_AGENDA, ETipoErrorFramework.EX_ERROR_CARGO_SIN_USUARIO_ASIGNADO.ToString(), MensajesFramework.CargarMensaje(ETipoErrorFramework.EX_ERROR_CARGO_SIN_USUARIO_ASIGNADO));
              throw new FaultException<ControllerException>(excepcion);
            }
          }
        });
        return true;
      }
    }

    /// <summary>
    /// Reasignar tareas
    /// </summary>
    /// <param name="tareaAsignada"></param>
    /// <param name="user"></param>
    /// <returns></returns>
    public void ReasignarTarea(ASTareaAsignada tareaAsignada, string user)
    {
      using (EntidadesAgenda contexto = new EntidadesAgenda(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(nombreModelo)))
      {
        AsignacionTarea_ASG asignacion = contexto.AsignacionTarea_ASG.FirstOrDefault(a => a.AST_IdAsignacionTarea == tareaAsignada.IdAsignacionTarea);
        if (asignacion != null)
        {
          UsuarioInterno_VSEG usuario = contexto.UsuarioInterno_VSEG.Where(u => u.USU_IdUsuario == tareaAsignada.UsuarioResponsable).FirstOrDefault();
          asignacion.AST_UsuarioResponsable = tareaAsignada.UsuarioResponsable;
          asignacion.AST_DescCargoResponsable = usuario.CAR_Descripcion;
          asignacion.AST_IdCargoResponsable = usuario.PEI_IdCargo;

          Tarea_ASG tarea = contexto.Tarea_ASG.FirstOrDefault(t => t.TAR_IdTarea == tareaAsignada.IdTarea);
          List<SeguimientoAsignacionTareas_ASG> s = contexto.SeguimientoAsignacionTareas_ASG.Where(se => se.SEA_IdAsignacionTarea == asignacion.AST_IdAsignacionTarea).ToList();

          SeguimientoAsignacionTareas_ASG seguimiento = new SeguimientoAsignacionTareas_ASG()
          {
            SEA_Comentarios = tareaAsignada.NuevosComentarios,
            SEA_DescCargoResponsable = usuario.CAR_Descripcion,
            SEA_FechaEscalamiento = Framework.Servidor.ParametrosFW.PAAdministrador.Instancia.AgregarDiasLaborales(s.Last().SEA_FechaEscalamiento, tareaAsignada.TiempoExtension > 0 ? tareaAsignada.TiempoExtension : 0),
            SEA_FechaEvento = DateTime.Now,
            SEA_IdAsignacionTarea = tareaAsignada.IdAsignacionTarea,
            SEA_IdCargoResponsable = usuario.PEI_IdCargo,
            SEA_IdEvento = (int)ASEEventoAsignacion.Seguimiento,
            SEA_TiempoEscalamiento = tarea.TAR_TiempoEscalamiento,
            SEA_TiempoExtension = tareaAsignada.TiempoExtension,
            SEA_UsuarioEvento = user,
            SEA_CreadoPor = user,
            SEA_UsuarioResponsable = tareaAsignada.UsuarioResponsable,
          };
          contexto.SeguimientoAsignacionTareas_ASG.Add(seguimiento);

          contexto.SaveChanges();
        }
      }
    }

    /// <summary>
    /// Agrega una tarea
    /// </summary>
    /// <param name="tarea"></param>
    public void AdicionarTarea(ASTarea tarea)
    {
      using (EntidadesAgenda contexto = new EntidadesAgenda(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(nombreModelo)))
      {
        contexto.Tarea_ASG.Add(new Tarea_ASG()
        {
          TAR_IdFalla = tarea.IdFalla,
          TAR_Estado = tarea.Estado,
          TAR_Descripcion = tarea.Descripcion.ToUpper().Trim(),
          TAR_UsuarioInsercion = ControllerContext.Current.Usuario,
          TAR_FechaGrabacion = DateTime.Now,
          TAR_CargoResponsable = tarea.Cargo.IdCargo,
          TAR_TiempoEscalamiento = tarea.TiempoEscalamiento,
        });
        contexto.SaveChanges();
      }
    }

    /// <summary>
    /// Registra falla en el sistema
    /// </summary>
    /// <param name="falla">Falla a registrar</param>
    /// <param name="usuario">Usuario que hace la operación</param>
    public int AdicionarFalla(ASFalla falla)
    {
      using (EntidadesAgenda contexto = new EntidadesAgenda(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(nombreModelo)))
      {
        Falla_ASG f = new Falla_ASG()
              {
                FAL_CreadoPor = ControllerContext.Current.Usuario,
                FAL_Descripcion = falla.Descripcion.ToUpper().Trim(),
                FAL_EsEditable = falla.EsEditable,
                FAL_Estado = falla.Estado.IdEstado,
                FAL_FechaGrabacion = DateTime.Now,
                FAL_IdModulo = falla.IdModulo,
                FAL_Tipo = falla.TipoFalla == ASETipoFalla.Manual ? "MAN" : "AUT",
              };
        contexto.Falla_ASG.Add(f);
        contexto.SaveChanges();
        return f.FAL_IdFalla;
      }
    }

    /// <summary>
    /// Hace escalamiento automático de tareas cuando el tiempo límite de resolución ha sido superado
    /// </summary>
    public void AutoEscalarTareas()
    {
      using (TransactionScope scope = new TransactionScope())
      {
        using (EntidadesAgenda contexto = new EntidadesAgenda(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(nombreModelo)))
        {
          List<AsignacionTarea_ASG> tareasExpiradasSinCerrar = contexto.AsignacionTarea_ASG
                      .Include("SeguimientoAsignacionTareas_ASG")
                      .Where(asignacion =>
                                         asignacion.SeguimientoAsignacionTareas_ASG
                                           //  Todas las asignaciones que no han sido cerradas y que están vencidas
                                             .Where(seg => seg.SEA_IdEvento == (int)ASEEventoAsignacion.Cierre).Count() == 0
                                                && asignacion.SeguimientoAsignacionTareas_ASG.FirstOrDefault(seg => seg.SEA_IdSeguimientoAsg == asignacion.SeguimientoAsignacionTareas_ASG.Max(s => s.SEA_IdSeguimientoAsg)).SEA_FechaEscalamiento < DateTime.Now)
                                             .ToList();

          tareasExpiradasSinCerrar.ForEach(tarea =>
            {
              Cargo_SEG cargoResponsableTarea = contexto.Cargo_SEG.FirstOrDefault(c => c.CAR_IdCargo == tarea.AST_IdCargoResponsable);
              UsuarioInterno_VSEG usuarioResponsable = contexto.UsuarioInterno_VSEG.Where(c => c.USU_IdUsuario == tarea.AST_UsuarioResponsable).FirstOrDefault();
              // Si el cargo existe

              if (cargoResponsableTarea != null)
              {
                if (usuarioResponsable != null)
                {
                  Cargo_SEG cargoSuperior = contexto.Cargo_SEG.FirstOrDefault(c => c.CAR_IdCargo == cargoResponsableTarea.CAR_IdCargoReporta);
                  if (cargoSuperior.CAR_IdCargoReporta != null)
                  {
                    // Encontrar primer usuario asociado al cargo con base en la regional
                    UsuarioInterno_VSEG usuarioSuperior = contexto.UsuarioInterno_VSEG
                              .Where(usu => usu.PEI_IdCargo == cargoSuperior.CAR_IdCargo
                                              && usu.PEI_IdRegionalAdm == usuarioResponsable.PEI_IdRegionalAdm
                                              && usu.USU_Estado == ConstantesFramework.ESTADO_ACTIVO).FirstOrDefault();
                    // Se reasigna la tarea
                    if (usuarioSuperior != null)
                    {
                      AsignacionTarea_ASG asignacion = contexto.AsignacionTarea_ASG.FirstOrDefault(asig => asig.AST_IdAsignacionTarea == tarea.AST_IdAsignacionTarea);
                      asignacion.AST_IdCargoResponsable = cargoResponsableTarea.CAR_IdCargo;
                      asignacion.AST_DescCargoResponsable = cargoResponsableTarea.CAR_Descripcion;
                      asignacion.AST_UsuarioResponsable = usuarioSuperior.USU_IdUsuario;

                      SeguimientoAsignacionTareas_ASG nuevoSeguimiento = new SeguimientoAsignacionTareas_ASG()
                      {
                        SEA_Comentarios = MensajesFramework.CargarMensaje(ETipoErrorFramework.EX_TAREA_ESCALADA),
                        SEA_DescCargoResponsable = cargoSuperior.CAR_Descripcion,
                        SEA_FechaEvento = DateTime.Now,
                        SEA_IdAsignacionTarea = tarea.AST_IdAsignacionTarea,
                        SEA_IdCargoResponsable = cargoSuperior.CAR_IdCargo,
                        SEA_IdEvento = (int)ASEEventoAsignacion.Escalamiento,
                        SEA_UsuarioEvento = COUsuariosSistema.USUARIO_SISTEMA,
                        SEA_UsuarioResponsable = usuarioSuperior.USU_IdUsuario,
                        SEA_TiempoEscalamiento = tarea.AST_TiempoEscalamiento,
                        SEA_TiempoExtension = tarea.AST_TiempoEscalamiento,
                        SEA_FechaEscalamiento = Framework.Servidor.ParametrosFW.PAAdministrador.Instancia.AgregarDiasLaborales(DateTime.Now.Date, tarea.AST_TiempoEscalamiento),
                        SEA_CreadoPor = ControllerContext.Current.Usuario
                      };
                      contexto.SeguimientoAsignacionTareas_ASG.Add(nuevoSeguimiento);
                      contexto.SaveChanges();
                    }
                    else
                    {
                      // no se encontraron usuarios activos para escalar la tarea
                      AuditoriaTrace.EscribirAuditoria(ETipoAuditoria.Info, MensajesFramework.CargarMensaje(ETipoErrorFramework.EX_NO_USUARIOS_ACTIVOS_ESCALAMIENTO), COConstantesModulos.AGENDA);
                    }
                  }
                  else
                  {
                    // No se encontraron cargos superiores para escalar
                    AuditoriaTrace.EscribirAuditoria(ETipoAuditoria.Info, MensajesFramework.CargarMensaje(ETipoErrorFramework.EX_NO_CARGOS_SUPERIORES), COConstantesModulos.AGENDA);
                  }
                }
                else
                {
                  // NO hay usuario responsable válido no existe
                  AuditoriaTrace.EscribirAuditoria(ETipoAuditoria.Info, MensajesFramework.CargarMensaje(ETipoErrorFramework.EX_USUARIO_ASIGNADO_NO_EXISTE), COConstantesModulos.AGENDA);
                }
              }
              else
              {
                // Cargo Responsable no encontrado
                AuditoriaTrace.EscribirAuditoria(ETipoAuditoria.Info, MensajesFramework.CargarMensaje(ETipoErrorFramework.EX_CARGO_NO_EXISTE), COConstantesModulos.AGENDA);
              }
            });
        }
        scope.Complete();
      }
    }

    #endregion Métodos Inserción

    #region Métodos Edición

    /// <summary>
    /// Registra falla en el sistema
    /// </summary>
    /// <param name="falla">Falla a registrar</param>
    /// <param name="usuario">Usuario que hace la operación</param>
    public void EditarFalla(ASFalla falla)
    {
      using (EntidadesAgenda contexto = new EntidadesAgenda(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(nombreModelo)))
      {
        // Encontrar falla
        Falla_ASG fallaAEditar = contexto.Falla_ASG.FirstOrDefault(f => f.FAL_IdFalla == falla.IdFalla);
        if (fallaAEditar != null)
        {
          fallaAEditar.FAL_CreadoPor = ControllerContext.Current.Usuario;
          fallaAEditar.FAL_Descripcion = falla.Descripcion;
          fallaAEditar.FAL_EsEditable = falla.EsEditable;
          fallaAEditar.FAL_Estado = falla.Estado.IdEstado;
          fallaAEditar.FAL_IdModulo = falla.IdModulo;
          fallaAEditar.FAL_Tipo = falla.TipoFalla == ASETipoFalla.Manual ? "MAN" : "AUT";
          ASRepositorioAuditoria.MapeoAuditoriaFalla(contexto);
          contexto.SaveChanges();
        }
      }
    }

    public void EditarTarea(ASTarea tarea)
    {
      using (EntidadesAgenda contexto = new EntidadesAgenda(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(nombreModelo)))
      {
        Tarea_ASG dato = contexto.Tarea_ASG
          .Where(t => t.TAR_IdTarea == tarea.IdTarea)
          .FirstOrDefault();

        if (dato != null)
        {
          dato.TAR_IdFalla = tarea.IdFalla;
          dato.TAR_Estado = tarea.Estado;
          dato.TAR_Descripcion = tarea.Descripcion;
          dato.TAR_CargoResponsable = tarea.Cargo.IdCargo;
          dato.TAR_TiempoEscalamiento = tarea.TiempoEscalamiento;
        }
        contexto.SaveChanges();
      }
    }

    /// <summary>
    /// Gestiona la tarea con base en la tarea asignada
    /// </summary>
    /// <param name="tareaAsignada">Tarea asignada</param>
    /// <param name="usuario">Usuario que realiza la operación</param>
    public void GestionarTarea(ASTareaAsignada tareaAsignada)
    {
      using (EntidadesAgenda contexto = new EntidadesAgenda(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(nombreModelo)))
      {
        AsignacionTarea_ASG asignacionTarea = contexto.AsignacionTarea_ASG.Include("SeguimientoAsignacionTareas_ASG").FirstOrDefault(asig => asig.AST_IdAsignacionTarea == tareaAsignada.IdAsignacionTarea);
        SeguimientoAsignacionTareas_ASG seguimiento = new SeguimientoAsignacionTareas_ASG()
          {
            SEA_Comentarios = tareaAsignada.NuevosComentarios,
            SEA_DescCargoResponsable = asignacionTarea.AST_DescCargoResponsable,
            SEA_FechaEvento = DateTime.Now,
            SEA_IdAsignacionTarea = asignacionTarea.AST_IdAsignacionTarea,
            SEA_IdCargoResponsable = asignacionTarea.AST_IdCargoResponsable,
            SEA_IdEvento = tareaAsignada.TareaCerrada ? (int)ASEEventoAsignacion.Cierre : (int)ASEEventoAsignacion.Seguimiento,
            SEA_UsuarioEvento = ControllerContext.Current.Usuario,
            SEA_CreadoPor = ControllerContext.Current.Usuario,
            SEA_UsuarioResponsable = tareaAsignada.UsuarioResponsable,
            SEA_TiempoExtension = 0,
            SEA_FechaEscalamiento = asignacionTarea.SeguimientoAsignacionTareas_ASG.Last().SEA_FechaEscalamiento,
            SEA_TiempoEscalamiento = asignacionTarea.SeguimientoAsignacionTareas_ASG.Last().SEA_TiempoEscalamiento
          };
        contexto.SeguimientoAsignacionTareas_ASG.Add(seguimiento);
        contexto.SaveChanges();
      }
    }

    #endregion Métodos Edición

    #region Métodos Eliminación

    /// <summary>
    /// Elimina falla, cambia estado de activo a inactivo en la falla
    /// </summary>
    /// <param name="falla"></param>
    public void EliminarFalla(ASFalla falla)
    {
      using (EntidadesAgenda contexto = new EntidadesAgenda(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(nombreModelo)))
      {
        Falla_ASG fallaAEliminar = contexto.Falla_ASG.FirstOrDefault(f => f.FAL_IdFalla == falla.IdFalla);
        if (fallaAEliminar != null)
        {
          fallaAEliminar.FAL_Estado = "INA";

          // Auditando cambio
          ASRepositorioAuditoria.MapeoAuditoriaFalla(contexto);
          contexto.SaveChanges();
        }
      }
    }

    public void EliminarTarea(ASTarea tarea)
    {
      using (EntidadesAgenda contexto = new EntidadesAgenda(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(nombreModelo)))
      {
        Tarea_ASG tareaAEliminar = contexto.Tarea_ASG.FirstOrDefault(f => f.TAR_IdTarea == tarea.IdTarea);
        if (tareaAEliminar != null)
        {
          // Auditando cambio
          contexto.Tarea_ASG.Remove(tareaAEliminar);
          ASRepositorioAuditoria.MapeoAuditoriaTarea(contexto);
          contexto.SaveChanges();
        }
      }
    }

    #endregion Métodos Eliminación
  }
}