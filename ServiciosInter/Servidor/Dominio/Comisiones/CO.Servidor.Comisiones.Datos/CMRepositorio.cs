using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.ServiceModel;
using System.Text;
using CO.Servidor.Comisiones.Comun;
using CO.Servidor.Comisiones.Datos.Modelo;
using CO.Servidor.Servicios.ContratoDatos.CentroServicios;
using CO.Servidor.Servicios.ContratoDatos.Comisiones;
using CO.Servidor.Servicios.ContratoDatos.Tarifas;
using Framework.Servidor.Comun;

using Framework.Servidor.Excepciones;
using System.Data.SqlClient;
using CO.Servidor.Dominio.Comun.Util;
using System.Data;

namespace CO.Servidor.Comisiones.Datos
{
    /// <summary>
    /// Clase para consultar y persistir informacion en la base de datos para los procesos de comisiones
    /// </summary>
    public class CMRepositorio
    {
        private static readonly CMRepositorio instancia = new CMRepositorio();
        private const string NombreModelo = "ModeloComisiones";
        string filePath = string.Empty;

        /// <summary>
        /// Retorna la instancia de la clase TARepositorio
        /// </summary>
        public static CMRepositorio Instancia
        {
            get { return CMRepositorio.instancia; }
        }

        private CMRepositorio() { }

        #region Comisiones por centros de servicio administrados

        /// <summary>
        /// Obtiene los puntos de una agencia
        /// </summary>
        /// <param name="filtro">Diccionario con los parametros del filtro</param>
        /// <param name="campoOrdenamiento">Campo por el que se realiza la ordenacion</param>
        /// <param name="indicePagina">Indice de la pagina</param>
        /// <param name="registrosPorPagina">Cantidad de registros por pagina</param>
        /// <param name="ordenamientoAscendente">Ordenamiento</param>
        /// <param name="totalRegistros">Total de registros de la consult</param>
        /// <param name="IdCentroServicios">Id del centro de servicio</param>
        /// <returns>Lista puntos de la agencia</returns>
        public IList<CMCentroServicioAdministrado> ObtenerPuntosDeAgencia(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros, long IdAgencia)
        {
            using (ModeloComisiones contexto = new ModeloComisiones(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                Dictionary<LambdaExpression, OperadorLogico> where = new Dictionary<LambdaExpression, OperadorLogico>();
                LambdaExpression lamda = contexto.CrearExpresionLambda<PuntosDeAgencia_VPUA>("PUS_IdAgencia", IdAgencia.ToString(), OperadorComparacion.Equal);
                LambdaExpression lamda2 = contexto.CrearExpresionLambda<PuntosDeAgencia_VPUA>("CES_Estado", "ACT", OperadorComparacion.Equal);
                where.Add(lamda, OperadorLogico.And);
                where.Add(lamda2, OperadorLogico.And);

                return contexto.ConsultarPuntosDeAgencia_VPUA(filtro, where, campoOrdenamiento, out totalRegistros, indicePagina, registrosPorPagina, ordenamientoAscendente)
                  .ToList().ConvertAll<CMCentroServicioAdministrado>(s =>
                    new CMCentroServicioAdministrado()
                    {
                        Direccion = s.CES_Direccion,
                        Estado = s.CES_Estado,
                        IdCentroServicios = s.CES_IdCentroServicios,
                        IdCentroServiciosAdministrador = IdAgencia,
                        Municipio = s.LOC_Nombre,
                        NombreCentroServicios = s.CES_Nombre,
                        TipoCentroServicio = ConstantesFramework.TIPO_CENTRO_SERVICIO_PUNTO
                    });
            }
        }

        /// <summary>
        /// Obtiene las agencias de un col
        /// </summary>
        /// <param name="filtro">Diccionario con los parametros del filtro</param>
        /// <param name="campoOrdenamiento">Campo por el que se realiza la ordenacion</param>
        /// <param name="indicePagina">Indice de la pagina</param>
        /// <param name="registrosPorPagina">Cantidad de registros por pagina</param>
        /// <param name="ordenamientoAscendente">Ordenamiento</param>
        /// <param name="totalRegistros">Total de registros de la consult</param>
        /// <param name="IdCentroServicios">Id del centro de servicio</param>
        /// <returns>Lista agencias de un col</returns>
        public IList<CMCentroServicioAdministrado> ObtenerAgenciasDeCol(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros, long IdCol)
        {
            using (ModeloComisiones contexto = new ModeloComisiones(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                Dictionary<LambdaExpression, OperadorLogico> where = new Dictionary<LambdaExpression, OperadorLogico>();
                LambdaExpression lamda = contexto.CrearExpresionLambda<PuntosDeAgencia_VPUA>("AGE_IdCentroLogistico", IdCol.ToString(), OperadorComparacion.Equal);
                LambdaExpression lamda2 = contexto.CrearExpresionLambda<PuntosDeAgencia_VPUA>("CES_Estado", "ACT", OperadorComparacion.Equal);
                where.Add(lamda, OperadorLogico.And);
                where.Add(lamda2, OperadorLogico.And);

                return contexto.ConsultarAgenciasDeCentroLogistico_VPUA(filtro, where, campoOrdenamiento, out totalRegistros, indicePagina, registrosPorPagina, ordenamientoAscendente)
                  .ToList().ConvertAll<CMCentroServicioAdministrado>(s =>
                    new CMCentroServicioAdministrado()
                    {
                        Direccion = s.CES_Direccion,
                        Estado = s.CES_Estado,
                        IdCentroServicios = s.AGE_IdAgencia,
                        IdCentroServiciosAdministrador = s.AGE_IdCentroLogistico,
                        Municipio = s.LOC_Nombre,
                        NombreCentroServicios = s.CES_Nombre,
                        IdTipoAgencia = s.AGE_IdTipoAgencia,
                        TipoCentroServicio = "AGE"
                    });
            }
        }

        /// <summary>
        /// Obtiene los col de un racol
        /// </summary>
        /// <param name="filtro">Diccionario con los parametros del filtro</param>
        /// <param name="campoOrdenamiento">Campo por el que se realiza la ordenacion</param>
        /// <param name="indicePagina">Indice de la pagina</param>
        /// <param name="registrosPorPagina">Cantidad de registros por pagina</param>
        /// <param name="ordenamientoAscendente">Ordenamiento</param>
        /// <param name="totalRegistros">Total de registros de la consult</param>
        /// <param name="IdRacol">Id del racol</param>
        /// <returns>Lista col de un racol</returns>
        public IList<CMCentroServicioAdministrado> ObtenerColDeRacol(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros, long IdRacol)
        {
            using (ModeloComisiones contexto = new ModeloComisiones(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                Dictionary<LambdaExpression, OperadorLogico> where = new Dictionary<LambdaExpression, OperadorLogico>();
                LambdaExpression lamda = contexto.CrearExpresionLambda<PuntosDeAgencia_VPUA>("REA_IdRegionalAdm", IdRacol.ToString(), OperadorComparacion.Equal);
                LambdaExpression lamda2 = contexto.CrearExpresionLambda<PuntosDeAgencia_VPUA>("CES_Estado", "ACT", OperadorComparacion.Equal);
                where.Add(lamda, OperadorLogico.And);
                where.Add(lamda2, OperadorLogico.And);

                return contexto.ConsultarCentroLogisDeRegionalAdmi_VPUA(filtro, where, campoOrdenamiento, out totalRegistros, indicePagina, registrosPorPagina, ordenamientoAscendente)
                  .ToList().ConvertAll<CMCentroServicioAdministrado>(s =>
                    new CMCentroServicioAdministrado()
                    {
                        Direccion = s.CES_Direccion,
                        Estado = s.CES_Estado,
                        IdCentroServicios = s.CEL_IdCentroLogistico,
                        IdCentroServiciosAdministrador = s.REA_IdRegionalAdm,
                        Municipio = s.LOC_Nombre,
                        NombreCentroServicios = s.CES_Nombre,
                        TipoCentroServicio = "COL"
                    });
            }
        }

        /// <summary>
        /// Obtiene  las comisiones de los servicios asignados a un centro de servicios administrado
        /// </summary>
        /// <param name="filtro">Diccionario con los parametros del filtro</param>
        /// <param name="campoOrdenamiento">Campo por el que se realiza la ordenacion</param>
        /// <param name="indicePagina">Indice de la pagina</param>
        /// <param name="registrosPorPagina">Cantidad de registros por pagina</param>
        /// <param name="ordenamientoAscendente">Ordenamiento</param>
        /// <param name="totalRegistros">Total de registros de la consult</param>
        /// <param name="IdCentroServicios">Id del centro de servicio</param>
        /// <returns>Lista  de comisiones por servicios de los centros de servicio</returns>
        public IList<CMComisionesServiciosCentroServiciosAdmin> ObtenerComisionesServiciosCentroServiciosAdmin(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros, long IdCentroServicios)
        {
            using (ModeloComisiones contexto = new ModeloComisiones(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                Dictionary<LambdaExpression, OperadorLogico> where = new Dictionary<LambdaExpression, OperadorLogico>();
                LambdaExpression lamda = contexto.CrearExpresionLambda<ComisionSrvCentroSrvAdmin_VCOM>("CSS_IdCentroServicios", IdCentroServicios.ToString(), OperadorComparacion.Equal);
                LambdaExpression lamda2 = contexto.CrearExpresionLambda<ComisionSrvCentroSrvAdmin_VCOM>("CSS_Estado", ConstantesFramework.ESTADO_ACTIVO, OperadorComparacion.Equal);

                where.Add(lamda, OperadorLogico.And);
                where.Add(lamda2, OperadorLogico.And);

                return contexto.ConsultarComisionSrvCentroSrvAdmin_VCOM(filtro, where, campoOrdenamiento, out totalRegistros, indicePagina, registrosPorPagina, ordenamientoAscendente)
                  .ToList().ConvertAll<CMComisionesServiciosCentroServiciosAdmin>(s =>
                    new CMComisionesServiciosCentroServiciosAdmin()
                    {
                        IdServicio = s.SER_IdServicio,
                        IdTipoComision = s.TCO_IdTipoComision,
                        IdUnidadNegocio = s.UNE_IdUnidad,
                        NombreServicio = s.SER_Nombre,
                        NombreUnidadNegocio = s.UNE_Nombre,
                        Porcentaje = s.CSA_PorcentajeComision,
                        Valor = s.CSA_ValorComision,
                        IdCentroServicioServicio = s.CSS_IdCentroServicioServicio,
                        IdTipoComisionOriginal = s.TCO_IdTipoComision,
                        IdAgenciaAdministradora = s.CSA_IdAgencia,
                        IdCentroServicioAdministrado = s.CSS_IdCentroServicios,
                        NombreComision = s.TCO_Descripcion
                    });
            }
        }

        /// <summary>
        /// Obtiene todos los tipos de comision
        /// </summary>
        /// <returns>Lista con los tipos de comision</returns>
        public IList<PUTiposComision> ObtenerTiposComision()
        {
            using (ModeloComisiones contexto = new ModeloComisiones(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.TipoComision_COM.Select(obj =>
                  new PUTiposComision()
                  {
                      Descripcion = obj.TCO_Descripcion,
                      IdTipoComision = obj.TCO_IdTipoComision
                  }
                  ).OrderBy(obj => obj.Descripcion).ToList();
            }
        }

        /// <summary>
        /// Obtiene todos los tipos de comision fija
        /// </summary>
        /// <returns>Lista con los tipos de comision fija</returns>
        public IList<PUTipoComisionFija> ObtenerTiposComisionFija()
        {
            using (ModeloComisiones contexto = new ModeloComisiones(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.TipoComisionFija_COM.Select(obj =>
                  new PUTipoComisionFija()
                  {
                      Descripcion = obj.TCF_Descripcion,
                      IdTipoComFija = obj.TCF_IdTipoComisionFija
                  }
                  ).OrderBy(obj => obj.Descripcion).ToList();
            }
        }

        /// <summary>
        /// Guarda o edita comisiones de los servicios por un centro de servicios
        /// </summary>
        /// <param name="comisionesServicios"></param>
        public void ActualizarComisionesServiciosAsignadosCentrosServiciosAdmin(CMComisionesServiciosCentroServiciosAdmin comisionesServicios)
        {
            using (ModeloComisiones contexto = new ModeloComisiones(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                CentroSvcAdministradoComi_COM centroServiciosSrvComi = contexto.CentroSvcAdministradoComi_COM
                 .Where(obj => obj.CSA_IdCentroServicioServicio == comisionesServicios.IdCentroServicioServicio && obj.CSA_IdTipoComision == comisionesServicios.IdTipoComisionOriginal && obj.CSA_IdAgencia == comisionesServicios.IdAgenciaAdministradora)
                 .SingleOrDefault();

                if (centroServiciosSrvComi != null)
                {
                    centroServiciosSrvComi.CSA_IdTipoComision = (short)comisionesServicios.IdTipoComision;
                    centroServiciosSrvComi.CSA_PorcentajeComision = comisionesServicios.Porcentaje.Value;
                    centroServiciosSrvComi.CSA_ValorComision = comisionesServicios.Valor.Value;
                    centroServiciosSrvComi.CSA_IdCentroServicioServicio = comisionesServicios.IdCentroServicioServicio;
                    centroServiciosSrvComi.CSA_IdAgencia = comisionesServicios.IdAgenciaAdministradora.Value;

                    contexto.SaveChanges();
                }
                else
                {
                    CentroSvcAdministradoComi_COM comision = new CentroSvcAdministradoComi_COM()
                    {
                        CSA_CreadoPor = ControllerContext.Current.Usuario,
                        CSA_FechaGrabacion = DateTime.Now,
                        CSA_IdCentroServicioServicio = comisionesServicios.IdCentroServicioServicio,
                        CSA_IdTipoComision = (short)comisionesServicios.IdTipoComision,
                        CSA_PorcentajeComision = comisionesServicios.Porcentaje.Value,
                        CSA_ValorComision = comisionesServicios.Valor.Value,
                        CSA_IdAgencia = comisionesServicios.IdAgenciaAdministradora.Value,
                        CSA_IdCentroSvcAdministrado = 0
                    };

                    contexto.CentroSvcAdministradoComi_COM.Add(comision);
                    contexto.SaveChanges();
                }
            }
        }

        /// <summary>
        /// Obtiene  los conceptos adicionales (comisiones fijas) de un centro de servicio sin contrato
        /// </summary>
        /// <param name="filtro">Diccionario con los parametros del filtro</param>
        /// <param name="campoOrdenamiento">Campo por el que se realiza la ordenacion</param>
        /// <param name="indicePagina">Indice de la pagina</param>
        /// <param name="registrosPorPagina">Cantidad de registros por pagina</param>
        /// <param name="ordenamientoAscendente">Ordenamiento</param>
        /// <param name="totalRegistros">Total de registros de la consult</param>
        /// <param name="IdCentroServicios">Id del centro de servicio</param>
        /// <returns>Lista  de los conceptos adicionales (comisiones fijas) de un centro de servicio</returns>
        public IList<CMComisionesConceptosAdicionales> ObtenerConceptosAdicionalesCentroServicioNoContrato(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros, long IdCentroServicios)
        {
            using (ModeloComisiones contexto = new ModeloComisiones(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                Dictionary<LambdaExpression, OperadorLogico> where = new Dictionary<LambdaExpression, OperadorLogico>();
                LambdaExpression lamda = contexto.CrearExpresionLambda<CentroServicioComiFijas_VCOM>("CSD_IdCentroServicios", IdCentroServicios.ToString(), OperadorComparacion.Equal);
                //LambdaExpression lamda2 = contexto.CrearExpresionLambda<CentroServicioComiFijas_VCOM>("CSD_Estado", ConstantesFramework.ESTADO_ACTIVO, OperadorComparacion.Equal);

                where.Add(lamda, OperadorLogico.And);
                //where.Add(lamda2, OperadorLogico.And);

                return contexto.ConsultarCentroServicioComiFijas_VCOM(filtro, where, campoOrdenamiento, out totalRegistros, indicePagina, registrosPorPagina, ordenamientoAscendente)
                  .ToList().ConvertAll<CMComisionesConceptosAdicionales>(s =>
                  {
                      CMComisionesConceptosAdicionales com = new CMComisionesConceptosAdicionales()
                      {
                          Descripcion = s.CSD_Descripcion,
                          Estado = s.CSD_Estado == ConstantesFramework.ESTADO_ACTIVO ? true : false,
                          FechaInicio = s.CSD_FechaInicio,
                          IdCentroServicio = s.CSD_IdCentroServicios,
                          IdTipoComisionFija = s.CSD_IdTipoComisionFija,
                          IdTipoComisionFijaOriginal = s.CSD_IdTipoComisionFija,
                          Valor = s.CSD_Valor,
                          ConContrato = false
                      };
                      return com;
                  });
            }
        }

        /// <summary>
        /// Obtiene  los conceptos adicionales (comisiones fijas) de un centro de servicio con contrato
        /// </summary>
        /// <param name="filtro">Diccionario con los parametros del filtro</param>
        /// <param name="campoOrdenamiento">Campo por el que se realiza la ordenacion</param>
        /// <param name="indicePagina">Indice de la pagina</param>
        /// <param name="registrosPorPagina">Cantidad de registros por pagina</param>
        /// <param name="ordenamientoAscendente">Ordenamiento</param>
        /// <param name="totalRegistros">Total de registros de la consult</param>
        /// <param name="IdCentroServicios">Id del centro de servicio</param>
        /// <returns>Lista  de los conceptos adicionales (comisiones fijas) de un centro de servicio</returns>
        public IList<CMComisionesConceptosAdicionales> ObtenerConceptosAdicionalesCentroServicioConContrato(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros, long IdCentroServicios)
        {
            using (ModeloComisiones contexto = new ModeloComisiones(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                Dictionary<LambdaExpression, OperadorLogico> where = new Dictionary<LambdaExpression, OperadorLogico>();
                LambdaExpression lamda = contexto.CrearExpresionLambda<CentroSvcComiFijaContrato_VCOM>("CCC_IdCentroServicios", IdCentroServicios.ToString(), OperadorComparacion.Equal);
                //LambdaExpression lamda2 = contexto.CrearExpresionLambda<CentroSvcComiFijaContrato_VCOM>("CCC_Estado", ConstantesFramework.ESTADO_ACTIVO, OperadorComparacion.Equal);

                where.Add(lamda, OperadorLogico.And);
                //where.Add(lamda2, OperadorLogico.And);

                return contexto.ConsultarCentroSvcComiFijaContrato_VCOM(filtro, where, campoOrdenamiento, out totalRegistros, indicePagina, registrosPorPagina, ordenamientoAscendente)
                  .ToList().ConvertAll<CMComisionesConceptosAdicionales>(s =>
                  {
                      CMComisionesConceptosAdicionales com = new CMComisionesConceptosAdicionales()
                      {
                          Descripcion = s.CCC_Descripcion,
                          Estado = s.CCC_Estado == ConstantesFramework.ESTADO_ACTIVO ? true : false,
                          FechaInicio = s.CCC_FechaInicio,
                          IdCentroServicio = s.CCC_IdCentroServicios,
                          IdTipoComisionFija = s.CCC_IdTipoComisionFija,
                          IdTipoComisionFijaOriginal = s.CCC_IdTipoComisionFija,
                          Valor = s.CCC_Valor,
                          IdCentroSrvComiFijaContrato = s.CCC_IdCentroSrvComFijaContrato,
                          IdContrato = s.CON_IdContrato,
                          NombreContrato = s.CON_NombreContrato,
                          RazonSocialCliente = s.CLI_RazonSocial,
                          IdClienteCredito = s.CLI_IdCliente,
                          ConContrato = true
                      };
                      return com;
                  });
            }
        }

        /// <summary>
        /// inserta una nueva comision por concepto adicional (comision fija) a un centro de servicio asociado a un contrato
        /// </summary>
        /// <param name="comisionFija">Objeto comision fija</param>
        public void AdicionarConceptoAdicionalConContrato(CMComisionesConceptosAdicionales comisionFija)
        {
            using (ModeloComisiones contexto = new ModeloComisiones(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                CentroSrvComiFijaContrato_COM comi = new CentroSrvComiFijaContrato_COM()
                {
                    CCC_CreadoPor = ControllerContext.Current.Usuario,
                    CCC_FechaGrabacion = DateTime.Now,
                    CCC_Descripcion = comisionFija.Descripcion,
                    CCC_Estado = comisionFija.Estado ? ConstantesFramework.ESTADO_ACTIVO : ConstantesFramework.ESTADO_INACTIVO,
                    CCC_FechaInicio = comisionFija.FechaInicio,
                    CCC_IdCentroServicios = comisionFija.IdCentroServicio,
                    CCC_IdCentroSrvComFijaContrato = 0,
                    CCC_IdContrato = comisionFija.IdContrato,
                    CCC_Valor = comisionFija.Valor,
                    CCC_IdTipoComisionFija = (short)comisionFija.IdTipoComisionFija.Value
                };

                contexto.CentroSrvComiFijaContrato_COM.Add(comi);
                AuditarComisionesConceptosAdicionalesConContrato(contexto, comi);

                contexto.SaveChanges();
            }
        }

        /// <summary>
        /// Verifica si un concepto adicional esta creado con contrato
        /// </summary>
        /// <param name="comisionFija"></param>
        /// <returns></returns>
        public bool VerificarConceptoAdicionalConContrato(CMComisionesConceptosAdicionales comisionFija)
        {
            using (ModeloComisiones contexto = new ModeloComisiones(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                CentroSrvComiFijaContrato_COM comi = contexto.CentroSrvComiFijaContrato_COM
                  .Where(obj => obj.CCC_IdCentroSrvComFijaContrato == comisionFija.IdCentroSrvComiFijaContrato)
                  .SingleOrDefault();

                if (comi == null)
                    return false;
                else
                    return true;
            }
        }

        /// <summary>
        /// Modifica una comision por concepto adicional (comision fija) a un centro de servicio asociado a un contrato
        /// </summary>
        /// <param name="comisionFija">Objeto comision fija</param>
        public void EditarConceptoAdicionalConContrato(CMComisionesConceptosAdicionales comisionFija)
        {
            using (ModeloComisiones contexto = new ModeloComisiones(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                CentroSrvComiFijaContrato_COM comi = contexto.CentroSrvComiFijaContrato_COM
                  .Where(obj => obj.CCC_IdCentroSrvComFijaContrato == comisionFija.IdCentroSrvComiFijaContrato)
                  .SingleOrDefault();

                if (comi == null)
                {
                    ControllerException excepcion = new ControllerException(COConstantesModulos.CENTRO_SERVICIOS, ETipoErrorFramework.EX_CONSULTA_DB_NULL.ToString(), MensajesFramework.CargarMensaje(ETipoErrorFramework.EX_CONSULTA_DB_NULL));
                    throw new FaultException<ControllerException>(excepcion);
                }

                comi.CCC_Descripcion = comisionFija.Descripcion;
                comi.CCC_Estado = comisionFija.Estado ? ConstantesFramework.ESTADO_ACTIVO : ConstantesFramework.ESTADO_INACTIVO;
                comi.CCC_FechaInicio = comisionFija.FechaInicio;
                comi.CCC_IdCentroServicios = comisionFija.IdCentroServicio;
                comi.CCC_IdContrato = comisionFija.IdContrato;
                comi.CCC_Valor = comisionFija.Valor;
                comi.CCC_IdTipoComisionFija = (short)comisionFija.IdTipoComisionFija.Value;

                AuditarComisionesConceptosAdicionalesConContrato(contexto, comi);

                contexto.SaveChanges();
            }
        }

        /// <summary>
        /// Elimina una comision por concepto adicional (comision fija) a un centro de servicio asociado a un contrato
        /// </summary>
        /// <param name="comisionFija">Objeto comision fija</param>
        public void EliminarConceptoAdicionalConContrato(CMComisionesConceptosAdicionales comisionFija)
        {
            using (ModeloComisiones contexto = new ModeloComisiones(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                CentroSrvComiFijaContrato_COM comi = contexto.CentroSrvComiFijaContrato_COM
                  .Where(obj => obj.CCC_IdCentroSrvComFijaContrato == comisionFija.IdCentroSrvComiFijaContrato)
                  .SingleOrDefault();

                if (comi == null)
                {
                    ControllerException excepcion = new ControllerException(COConstantesModulos.CENTRO_SERVICIOS, ETipoErrorFramework.EX_CONSULTA_DB_NULL.ToString(), MensajesFramework.CargarMensaje(ETipoErrorFramework.EX_CONSULTA_DB_NULL));
                    throw new FaultException<ControllerException>(excepcion);
                }

                contexto.CentroSrvComiFijaContrato_COM.Remove(comi);
                AuditarComisionesConceptosAdicionalesConContrato(contexto, comi);
                contexto.SaveChanges();
            }
        }

        /// <summary>
        /// inserta una nueva comision por concepto adicional (comision fija) a un centro de servicio sin contrato
        /// </summary>
        /// <param name="comisionFija">Objeto comision fija</param>
        public void AdicionarConceptoAdicionalSinContrato(CMComisionesConceptosAdicionales comisionFija)
        {
            using (ModeloComisiones contexto = new ModeloComisiones(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                CentroServicioComiFija_COM comi = new CentroServicioComiFija_COM()
                {
                    CSD_CreadoPor = ControllerContext.Current.Usuario,
                    CSD_FechaGrabacion = DateTime.Now,
                    CSD_Descripcion = comisionFija.Descripcion,
                    CSD_Estado = comisionFija.Estado ? ConstantesFramework.ESTADO_ACTIVO : ConstantesFramework.ESTADO_INACTIVO,
                    CSD_FechaInicio = comisionFija.FechaInicio,
                    CSD_IdCentroServicios = comisionFija.IdCentroServicio,
                    CSD_Valor = comisionFija.Valor,
                    CSD_IdTipoComisionFija = (short)comisionFija.IdTipoComisionFija.Value
                };

                contexto.CentroServicioComiFija_COM.Add(comi);
                AuditarComisionesConceptosAdicionalesSinContrato(contexto, comi);

                contexto.SaveChanges();
            }
        }

        /// <summary>
        /// Verifica si un concepto adicional esta creado sin contrato
        /// </summary>
        /// <param name="comisionFija"></param>
        /// <returns></returns>
        public bool VerificarConceptoAdicionalSinContrato(CMComisionesConceptosAdicionales comisionFija)
        {
            using (ModeloComisiones contexto = new ModeloComisiones(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                CentroServicioComiFija_COM comi = contexto.CentroServicioComiFija_COM
                 .Where(obj => obj.CSD_IdCentroServicios == comisionFija.IdCentroServicio && obj.CSD_IdTipoComisionFija == comisionFija.IdTipoComisionFija)
                 .SingleOrDefault();

                if (comi == null)
                    return false;
                else
                    return true;
            }
        }

        /// <summary>
        /// Modifica una comision por concepto adicional (comision fija) a un centro de servicio sin un contrato
        /// </summary>
        /// <param name="comisionFija">Objeto comision fija</param>
        public void EditarConceptoAdicionalSinContrato(CMComisionesConceptosAdicionales comisionFija)
        {
            using (ModeloComisiones contexto = new ModeloComisiones(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                CentroServicioComiFija_COM comi = contexto.CentroServicioComiFija_COM
                  .Where(obj => obj.CSD_IdCentroServicios == comisionFija.IdCentroServicio && obj.CSD_IdTipoComisionFija == comisionFija.IdTipoComisionFija)
                  .SingleOrDefault();

                if (comi == null)
                {
                    ControllerException excepcion = new ControllerException(COConstantesModulos.CENTRO_SERVICIOS, ETipoErrorFramework.EX_CONSULTA_DB_NULL.ToString(), MensajesFramework.CargarMensaje(ETipoErrorFramework.EX_CONSULTA_DB_NULL));
                    throw new FaultException<ControllerException>(excepcion);
                }

                comi.CSD_Descripcion = comisionFija.Descripcion;
                comi.CSD_Estado = comisionFija.Estado ? ConstantesFramework.ESTADO_ACTIVO : ConstantesFramework.ESTADO_INACTIVO;
                comi.CSD_FechaInicio = comisionFija.FechaInicio;
                comi.CSD_IdTipoComisionFija = (short)comisionFija.IdTipoComisionFija.Value;
                comi.CSD_IdCentroServicios = comisionFija.IdCentroServicio;
                comi.CSD_Valor = comisionFija.Valor;

                AuditarComisionesConceptosAdicionalesSinContrato(contexto, comi);

                contexto.SaveChanges();
            }
        }

        /// <summary>
        /// Elimina una comision por concepto adicional (comision fija) a un centro de servicio sin un contrato
        /// </summary>
        /// <param name="comisionFija">Objeto comision fija</param>
        public void EliminarConceptoAdicionalSinContrato(CMComisionesConceptosAdicionales comisionFija)
        {
            using (ModeloComisiones contexto = new ModeloComisiones(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                CentroServicioComiFija_COM comi = contexto.CentroServicioComiFija_COM
                  .Where(obj => obj.CSD_IdCentroServicios == comisionFija.IdCentroServicio && obj.CSD_IdTipoComisionFija == comisionFija.IdTipoComisionFija)
                  .SingleOrDefault();

                if (comi == null)
                {
                    ControllerException excepcion = new ControllerException(COConstantesModulos.CENTRO_SERVICIOS, ETipoErrorFramework.EX_CONSULTA_DB_NULL.ToString(), MensajesFramework.CargarMensaje(ETipoErrorFramework.EX_CONSULTA_DB_NULL));
                    throw new FaultException<ControllerException>(excepcion);
                }
                contexto.CentroServicioComiFija_COM.Remove(comi);
                AuditarComisionesConceptosAdicionalesSinContrato(contexto, comi);
                contexto.SaveChanges();
            }
        }

        /// <summary>
        /// Audita las comisiones por concepto adicionales de un centro de servicio con contrato
        /// </summary>
        /// <param name="contexto"></param>
        /// <param name="comi"></param>
        private void AuditarComisionesConceptosAdicionalesConContrato(ModeloComisiones contexto, CentroSrvComiFijaContrato_COM comi)
        {
            contexto.Audit<CentroSrvComiFijaContrato_COM, CentroSrvComiFijaContrHist_COM>((record, action) => new CentroSrvComiFijaContrHist_COM()
            {
                CCC_CambiadoPor = ControllerContext.Current.Usuario,
                CCC_CreadoPor = record.Field<CentroSrvComiFijaContrato_COM, string>(c => c.CCC_CreadoPor),
                CCC_Descripcion = record.Field<CentroSrvComiFijaContrato_COM, string>(c => c.CCC_Descripcion),
                CCC_Estado = record.Field<CentroSrvComiFijaContrato_COM, string>(c => c.CCC_Estado),
                CCC_FechaCambio = DateTime.Now,
                CCC_FechaGrabacion = record.Field<CentroSrvComiFijaContrato_COM, DateTime>(c => c.CCC_FechaGrabacion),
                CCC_FechaInicio = record.Field<CentroSrvComiFijaContrato_COM, DateTime>(c => c.CCC_FechaInicio),
                CCC_IdAuditoria = 0,
                CCC_IdCentroServicios = record.Field<CentroSrvComiFijaContrato_COM, long>(c => c.CCC_IdCentroServicios),
                CCC_IdCentroSrvComFijaContrato = comi.CCC_IdCentroSrvComFijaContrato,
                CCC_IdContrato = record.Field<CentroSrvComiFijaContrato_COM, int>(c => c.CCC_IdContrato),
                CCC_IdTipoComisionFija = record.Field<CentroSrvComiFijaContrato_COM, short>(c => c.CCC_IdTipoComisionFija),
                CCC_TipoCambio = action.ToString(),
                CCC_Valor = record.Field<CentroSrvComiFijaContrato_COM, decimal>(c => c.CCC_Valor)
            }, (cm) => contexto.CentroSrvComiFijaContrHist_COM.Add(cm));
        }

        /// <summary>
        /// Audita las comisiones por concepto adicionales de un centro de servicio sin contrato
        /// </summary>
        /// <param name="contexto"></param>
        /// <param name="comi"></param>
        private void AuditarComisionesConceptosAdicionalesSinContrato(ModeloComisiones contexto, CentroServicioComiFija_COM comi)
        {
            contexto.Audit<CentroServicioComiFija_COM, CentroServicioComiFijaHist_COM>((record, action) => new CentroServicioComiFijaHist_COM()
            {
                CSD_CambiadoPor = ControllerContext.Current.Usuario,
                CSD_TipoCambio = action.ToString(),
                CSD_FechaCambio = DateTime.Now,
                CSD_CreadoPor = record.Field<CentroServicioComiFija_COM, string>(c => c.CSD_CreadoPor),
                CSD_Descripcion = record.Field<CentroServicioComiFija_COM, string>(c => c.CSD_Descripcion),
                CSD_Estado = record.Field<CentroServicioComiFija_COM, string>(c => c.CSD_Estado),
                CSD_FechaGrabacion = record.Field<CentroServicioComiFija_COM, DateTime>(c => c.CSD_FechaGrabacion),
                CSD_FechaInicio = record.Field<CentroServicioComiFija_COM, DateTime>(c => c.CSD_FechaInicio),
                CSD_IdAuditoria = 0,
                CSD_IdCentroServicios = record.Field<CentroServicioComiFija_COM, long>(c => c.CSD_IdCentroServicios),
                CSD_IdTipoComisionFija = record.Field<CentroServicioComiFija_COM, short>(c => c.CSD_IdTipoComisionFija),
                CSD_Valor = record.Field<CentroServicioComiFija_COM, decimal>(c => c.CSD_Valor)
            }, (cm) => contexto.CentroServicioComiFijaHist_COM.Add(cm));
        }

        /// <summary>
        /// Calcula las comisiones por ventas
        /// de un punto, su responsable y de una Agencia.
        /// </summary>
        /// <param name="consulta">Parametros de entrada de idCentroServicio-TipoServicio.</param>
        /// <returns>Las Comisiones del Punto y su responsable - Agencia </returns>
        public CMComisionXVentaCalculadaDC CalcularComisionesxVentas(CMConsultaComisionVenta consulta)
        {
            decimal TotalCentroServiciosComision;
            decimal TotalResponsableCentroServiciosComision;
            decimal porcentajeCentroServicio;
            decimal porcentajeCentroServicioResponsable;

            CMComisionXVentaCalculadaDC ComisionVenta = new CMComisionXVentaCalculadaDC();
            ComisionVenta.BaseComision = consulta.ValorBaseComision;
            ComisionVenta.NumeroOperacion = consulta.NumeroOperacion;

            //if (consulta.ValorBaseComision != 0)
            //{
            using (ModeloComisiones contexto = new ModeloComisiones(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                var comisionCentroServicio = contexto.paObtenerComiCentroServicio_COM
                  (consulta.IdCentroServicios, consulta.IdServicio, (short?)consulta.TipoComision).FirstOrDefault();
                if (comisionCentroServicio != null)
                {
                    porcentajeCentroServicio = comisionCentroServicio.CSC_Porcentaje;
                    if (porcentajeCentroServicio > 0)
                        TotalCentroServiciosComision = (((consulta.ValorBaseComision * porcentajeCentroServicio) / 100) + comisionCentroServicio.CSC_Valor);
                    else
                        TotalCentroServiciosComision = comisionCentroServicio.CSC_Valor;
                }
                else
                {
                    //error de porcentaje comision centro servicio $0
                    ControllerException excepcion = new ControllerException(COConstantesModulos.COMISIONES,
                                                   CMEnumTipoErrorComisiones.EX_ERROR_NO_TIENE_COMISION_ASIGNADA.ToString(),
                                                   CMComisionesServerMensajes.CargarMensaje(CMEnumTipoErrorComisiones.EX_ERROR_NO_TIENE_COMISION_ASIGNADA));
                    throw new FaultException<ControllerException>(excepcion);
                }

                //Adjuto Informacion Inicial
                ComisionVenta.IdCentroServicioVenta = comisionCentroServicio.CSS_IdCentroServicios;
                ComisionVenta.NombreCentroServicioVenta = comisionCentroServicio.CES_Nombre;
                ComisionVenta.ValorFijoComisionCentroServicioVenta = comisionCentroServicio.CSC_Valor;
                ComisionVenta.PorcComisionCentroServicioVenta = porcentajeCentroServicio;
                ComisionVenta.TotalComisionCentroServicioVenta = TotalCentroServiciosComision;
                ComisionVenta.IdServicio = consulta.IdServicio;
                ComisionVenta.TipoComision = consulta.TipoComision;

                if (comisionCentroServicio.CES_Tipo == ConstantesFramework.TIPO_CENTRO_SERVICIO_PUNTO)
                {
                    var comisionResponsable = contexto.paObtenerComiAdmonCentroSvc_CAJ
                      (consulta.IdCentroServicios, consulta.IdServicio).FirstOrDefault();
                    if (comisionResponsable != null)
                    {
                        //Calculo el total de la comision del responsable si el valor del % es > 0
                        porcentajeCentroServicioResponsable = comisionResponsable.CSA_PorcentajeComision;
                        if (porcentajeCentroServicioResponsable > 0)
                            TotalResponsableCentroServiciosComision = ((consulta.ValorBaseComision * porcentajeCentroServicioResponsable) / 100) + comisionResponsable.CSA_ValorComision;
                        else
                            TotalResponsableCentroServiciosComision = comisionResponsable.CSA_ValorComision;

                        ComisionVenta.IdCentroServicioResponsable = comisionResponsable.CSA_IdAgencia;
                        ComisionVenta.NombreCentroServicioResponsable = comisionResponsable.NombreAgencia;
                        ComisionVenta.ValorFijoComisionCentroServicioResponsable = comisionResponsable.CSA_ValorComision;
                        ComisionVenta.PorcComisionCentroServicioResponsable = comisionResponsable.CSA_PorcentajeComision;
                        ComisionVenta.TotalComisionCentroServicioResponsable = TotalResponsableCentroServiciosComision;
                        ComisionVenta.TotalComisionEmpresa = consulta.ValorBaseComision - TotalCentroServiciosComision - TotalResponsableCentroServiciosComision;
                    }
                    else
                        LlenarLiquidacionSinCentroServicioAdministrado(ComisionVenta);
                }
                else
                    LlenarLiquidacionSinCentroServicioAdministrado(ComisionVenta);

                return ComisionVenta;
            }
            //}
            //else
            //{
            //  //Error de base de comisión valor Cero
            //  //ControllerException excepcion = new ControllerException(COConstantesModulos.COMISIONES,
            //  //                                   CMEnumTipoErrorComisiones.EX_ERROR_VALOR_BASE_COMISION_ES_CERO.ToString(),
            //  //                                  CMComisionesServerMensajes.CargarMensaje(CMEnumTipoErrorComisiones.EX_ERROR_VALOR_BASE_COMISION_ES_CERO));
            //  //throw new FaultException<ControllerException>(excepcion);
            //}
        }

        /// <summary>
        /// Calcula las comisiones por ventas
        /// de un punto, su responsable y de una Agencia.
        /// </summary>
        /// <param name="consulta">Parametros de entrada de idCentroServicio-TipoServicio.</param>
        /// <returns>Las Comisiones del Punto y su responsable - Agencia </returns>
        public CMComisionXVentaCalculadaDC CalcularComisionesxVentas(CMConsultaComisionVenta consulta, SqlConnection conexion, SqlTransaction transaccion)
        {
            decimal TotalCentroServiciosComision;
            decimal TotalResponsableCentroServiciosComision;
            decimal porcentajeCentroServicio;
            decimal porcentajeCentroServicioResponsable;

            CMComisionXVentaCalculadaDC ComisionVenta = new CMComisionXVentaCalculadaDC();
            ComisionVenta.BaseComision = consulta.ValorBaseComision;
            ComisionVenta.NumeroOperacion = consulta.NumeroOperacion;

            //if (consulta.ValorBaseComision != 0)
            //{


            SqlCommand cmd = new SqlCommand("paObtenerComiCentroServicio_COM", conexion, transaccion);
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.Parameters.Add(Utilidades.AddParametro("@IdCentroServicio", consulta.IdCentroServicios));
            cmd.Parameters.Add(Utilidades.AddParametro("@IdServicio", consulta.IdServicio));
            cmd.Parameters.Add(Utilidades.AddParametro("@tipoComision", (short?)consulta.TipoComision));

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);

            var comiCentroServ = dt.AsEnumerable().FirstOrDefault();

            if (comiCentroServ == null)
            {
                //error de porcentaje comision centro servicio $0
                ControllerException excepcion = new ControllerException(COConstantesModulos.COMISIONES,
                                               CMEnumTipoErrorComisiones.EX_ERROR_NO_TIENE_COMISION_ASIGNADA.ToString(),
                                               CMComisionesServerMensajes.CargarMensaje(CMEnumTipoErrorComisiones.EX_ERROR_NO_TIENE_COMISION_ASIGNADA));
                throw new FaultException<ControllerException>(excepcion);

            }

            porcentajeCentroServicio = comiCentroServ.Field<decimal>("CSC_Porcentaje");

            if (porcentajeCentroServicio > 0)
                TotalCentroServiciosComision = (((consulta.ValorBaseComision * porcentajeCentroServicio) / 100) + comiCentroServ.Field<decimal>("CSC_Valor"));
            else
                TotalCentroServiciosComision = comiCentroServ.Field<decimal>("CSC_Valor");



            //Adjuto Informacion Inicial
            ComisionVenta.IdCentroServicioVenta = comiCentroServ.Field<long>("CSS_IdCentroServicios");
            ComisionVenta.NombreCentroServicioVenta = comiCentroServ.Field<string>("CES_Nombre");
            ComisionVenta.ValorFijoComisionCentroServicioVenta = comiCentroServ.Field<decimal>("CSC_Valor");
            ComisionVenta.PorcComisionCentroServicioVenta = porcentajeCentroServicio;
            ComisionVenta.TotalComisionCentroServicioVenta = TotalCentroServiciosComision;
            ComisionVenta.IdServicio = consulta.IdServicio;
            ComisionVenta.TipoComision = consulta.TipoComision;

            if (comiCentroServ.Field<string>("CES_Tipo") == ConstantesFramework.TIPO_CENTRO_SERVICIO_PUNTO)
            {

                SqlCommand cmdAdmonCentros = new SqlCommand("paObtenerComiAdmonCentroSvc_CAJ", conexion, transaccion);
                cmdAdmonCentros.CommandType = System.Data.CommandType.StoredProcedure;
                cmdAdmonCentros.Parameters.Add(Utilidades.AddParametro("@idPuntoCentro", consulta.IdCentroServicios));
                cmdAdmonCentros.Parameters.Add(Utilidades.AddParametro("@idServicio", consulta.IdServicio));

                SqlDataAdapter daAdmonCentros = new SqlDataAdapter(cmdAdmonCentros);
                DataTable dtAdmonCentros = new DataTable();
                daAdmonCentros.Fill(dtAdmonCentros);

                var comisionResponsa = dtAdmonCentros.AsEnumerable().FirstOrDefault();
                if (comisionResponsa != null)
                {
                    porcentajeCentroServicioResponsable = comisionResponsa.Field<decimal>("CSA_PorcentajeComision");
                    if (porcentajeCentroServicioResponsable > 0)
                        TotalResponsableCentroServiciosComision = ((consulta.ValorBaseComision * porcentajeCentroServicioResponsable) / 100) + comisionResponsa.Field<decimal>("CSA_ValorComision");
                    else
                        TotalResponsableCentroServiciosComision = comisionResponsa.Field<decimal>("CSA_ValorComision");

                    ComisionVenta.IdCentroServicioResponsable = comisionResponsa.Field<long>("CSA_IdAgencia");
                    ComisionVenta.NombreCentroServicioResponsable = comisionResponsa.Field<string>("NombreAgencia");
                    ComisionVenta.ValorFijoComisionCentroServicioResponsable = comisionResponsa.Field<decimal>("CSA_ValorComision");
                    ComisionVenta.PorcComisionCentroServicioResponsable = comisionResponsa.Field<decimal>("CSA_PorcentajeComision");
                    ComisionVenta.TotalComisionCentroServicioResponsable = TotalResponsableCentroServiciosComision;
                    ComisionVenta.TotalComisionEmpresa = consulta.ValorBaseComision - TotalCentroServiciosComision - TotalResponsableCentroServiciosComision;
                }
                else
                    LlenarLiquidacionSinCentroServicioAdministrado(ComisionVenta);
            }
            else
                LlenarLiquidacionSinCentroServicioAdministrado(ComisionVenta);

            return ComisionVenta;

            //}
            //else
            //{
            //  //Error de base de comisión valor Cero
            //  //ControllerException excepcion = new ControllerException(COConstantesModulos.COMISIONES,
            //  //                                   CMEnumTipoErrorComisiones.EX_ERROR_VALOR_BASE_COMISION_ES_CERO.ToString(),
            //  //                                  CMComisionesServerMensajes.CargarMensaje(CMEnumTipoErrorComisiones.EX_ERROR_VALOR_BASE_COMISION_ES_CERO));
            //  //throw new FaultException<ControllerException>(excepcion);
            //}
        }



        /// <summary>
        /// Retorna el centro de servicio responsable de las comisiones del centro de servicio pasado como parámetro para el servicio dado
        /// </summary>
        /// <param name="idCentroServicio"></param>
        /// <param name="idServicio"></param>
        /// <returns></returns>
        public PUCentroServiciosDC ObtenerCentroServicioResponsableComisiones(long idCentroServicio, int idServicio)
        {
            using (ModeloComisiones contexto = new ModeloComisiones(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                ComiAdmonCentroSvc_CAJ res = contexto.paObtenerComiAdmonCentroSvc_CAJ(idCentroServicio, idServicio).FirstOrDefault();
                if (res != null)
                {
                    return new PUCentroServiciosDC
                    {
                        IdCentroServicio = res.CSA_IdAgencia,
                        Nombre = res.NombreAgencia
                    };
                }
                else
                {
                    return new PUCentroServiciosDC()
                    {
                        IdCentroServicio = 0,
                        Nombre = "N/A"
                    };
                }
            }
        }

        /// <summary>
        /// Llenars los datos de la liquidación para una agencia o para un punto sin comisiones para su agencia
        /// </summary>
        /// <param name="ComisionVenta">Información de la comisión.</param>
        private void LlenarLiquidacionSinCentroServicioAdministrado(CMComisionXVentaCalculadaDC ComisionVenta)
        {
            //si no es Punto retorno valores en 0
            ComisionVenta.IdCentroServicioResponsable = 0000;
            ComisionVenta.NombreCentroServicioResponsable = "N/A";
            ComisionVenta.ValorFijoComisionCentroServicioResponsable = 0;
            ComisionVenta.PorcComisionCentroServicioResponsable = 0;
            ComisionVenta.TotalComisionCentroServicioResponsable = 0;
            ComisionVenta.TotalComisionEmpresa = 0;
        }

        #endregion Comisiones por centros de servicio administrados

        #region Servicios de Centro servicios

        /// <summary>
        /// Obtiene  los descuentos de los servicios asignados a un centro de servicios
        /// </summary>
        /// <param name="filtro">Diccionario con los parametros del filtro</param>
        /// <param name="campoOrdenamiento">Campo por el que se realiza la ordenacion</param>
        /// <param name="indicePagina">Indice de la pagina</param>
        /// <param name="registrosPorPagina">Cantidad de registros por pagina</param>
        /// <param name="ordenamientoAscendente">Ordenamiento</param>
        /// <param name="totalRegistros">Total de registros de la consult</param>
        /// <param name="IdCentroServicios">Id del centro de servicio</param>
        /// <returns>Lista  de descuentos por servicios de los centros de servicio</returns>
        public ObservableCollection<CMCentroServiciosServicioDescuento> ObtenerDescuentosServiciosCentroServicios(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros, long IdCentroServiciosServicios, int idServicio)
        {
            using (ModeloComisiones contexto = new ModeloComisiones(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                Dictionary<LambdaExpression, OperadorLogico> where = new Dictionary<LambdaExpression, OperadorLogico>();
                LambdaExpression lamda = contexto.CrearExpresionLambda<DescuentosServicioCenSrv_VCOM>("CSD_IdCentroServicioServicio", IdCentroServiciosServicios.ToString(), OperadorComparacion.Equal);
                LambdaExpression lamda2 = contexto.CrearExpresionLambda<DescuentosServicioCenSrv_VCOM>("CSS_IdServicio", idServicio.ToString(), OperadorComparacion.Equal);

                where.Add(lamda, OperadorLogico.And);
                where.Add(lamda2, OperadorLogico.And);

                if (filtro.ContainsKey("SER_IdUnidadNegocio"))
                {
                    filtro.Remove("SER_IdUnidadNegocio");
                }

                if (filtro.ContainsKey("SER_Nombre"))
                {
                    filtro.Remove("SER_Nombre");
                }

                ObservableCollection<CMCentroServiciosServicioDescuento> retorno = new ObservableCollection<CMCentroServiciosServicioDescuento>();
                contexto.ConsultarDescuentosServicioCenSrv_VCOM(filtro, where, campoOrdenamiento, out totalRegistros, indicePagina, registrosPorPagina, ordenamientoAscendente).ToList().ConvertAll<CMCentroServiciosServicioDescuento>(d =>
               new CMCentroServiciosServicioDescuento()
               {
                   IdCentroServicioServDescuento = d.CSD_IdCentroServiSrvDescuento,
                   IdCentroServicioServicio = d.CSD_IdCentroServicioServicio,
                   IdTipoDescuento = d.CSD_IdTipoDescuento,
                   NombreTipoDescuento = d.TDE_Descripcion,
                   PorcentajeDescuento = d.CSD_PorcentajeDescuento,
                   ValorCriterioPenalizacion = d.CSD_ValorCriterioPenalizacion,
                   ValorDescuento = d.CSD_ValorDescuento,
                   Descripcion = d.CSD_Descripcion,
                   EstadoRegistro = EnumEstadoRegistro.SIN_CAMBIOS
               }).ToList().ForEach(d => retorno.Add(d));

                return retorno;
            }
        }

        /// <summary>
        /// Obtiene  las comisiones de un servicio asignados a un centro de servicios
        /// </summary>
        /// <param name="filtro">Diccionario con los parametros del filtro</param>
        /// <param name="campoOrdenamiento">Campo por el que se realiza la ordenacion</param>
        /// <param name="indicePagina">Indice de la pagina</param>
        /// <param name="registrosPorPagina">Cantidad de registros por pagina</param>
        /// <param name="ordenamientoAscendente">Ordenamiento</param>
        /// <param name="totalRegistros">Total de registros de la consult</param>
        /// <param name="IdCentroServicios">Id del centro de servicio</param>
        /// <returns>Lista  de comisiones por servicios de los centros de servicio</returns>
        public ObservableCollection<CMCentroServicioServicioComi> ObtenerComisionesServiciosCentroServicios(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros, long idCentroServiciosServicios, int idServicio)
        {
            using (ModeloComisiones contexto = new ModeloComisiones(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                Dictionary<LambdaExpression, OperadorLogico> where = new Dictionary<LambdaExpression, OperadorLogico>();
                LambdaExpression lamda = contexto.CrearExpresionLambda<ComisionesServicioCenSrv_VCOM>("CSC_IdCentroServicioServicio", idCentroServiciosServicios.ToString(), OperadorComparacion.Equal);
                LambdaExpression lamda2 = contexto.CrearExpresionLambda<ComisionesServicioCenSrv_VCOM>("CSS_IdServicio", idServicio.ToString(), OperadorComparacion.Equal);

                where.Add(lamda, OperadorLogico.And);
                where.Add(lamda2, OperadorLogico.And);
                ObservableCollection<CMCentroServicioServicioComi> retorno = new ObservableCollection<CMCentroServicioServicioComi>();

                contexto.ConsultarComisionesServicioCenSrv_VCOM(filtro, where, campoOrdenamiento, out totalRegistros, indicePagina, registrosPorPagina, ordenamientoAscendente).ToList()
                 .ConvertAll<CMCentroServicioServicioComi>(c =>
                   new CMCentroServicioServicioComi()
                   {
                       IdCentroServicioServicio = c.CSC_IdCentroServicioServicio,
                       IdTipoComision = c.CSC_IdTipoComision,
                       NombreTipoComision = c.TCO_Descripcion,
                       Porcentaje = c.CSC_Porcentaje,
                       Valor = c.CSC_Valor,
                       IdServicio = c.CSS_IdServicio,
                       NombreServicio = c.SER_Nombre,
                       NombreUnidadNegocio = c.UNE_Nombre,
                       EstadoRegistro = EnumEstadoRegistro.SIN_CAMBIOS
                   }).ToList().ForEach(c => retorno.Add(c));
                return retorno;
            }
        }

        /// <summary>
        /// Obtiene  los servicios asignados a un centro de servicios junto con sus comisiones y sus descuentos
        /// </summary>
        /// <param name="filtro">Diccionario con los parametros del filtro</param>
        /// <param name="campoOrdenamiento">Campo por el que se realiza la ordenacion</param>
        /// <param name="indicePagina">Indice de la pagina</param>
        /// <param name="registrosPorPagina">Cantidad de registros por pagina</param>
        /// <param name="ordenamientoAscendente">Ordenamiento</param>
        /// <param name="totalRegistros">Total de registros de la consult</param>
        /// <param name="IdCentroServicios">Id del centro de servicio</param>
        /// <returns>Lista  de comisiones por servicios de los centros de servicio</returns>
        public IList<CMServiciosCentroServicios> ObtenerServiciosCentroServicios(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros, long IdCentroServicios)
        {
            using (ModeloComisiones contexto = new ModeloComisiones(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                Dictionary<LambdaExpression, OperadorLogico> where = new Dictionary<LambdaExpression, OperadorLogico>();
                LambdaExpression lamda = contexto.CrearExpresionLambda<CentroServiciosSrvUniNeg_VCOM>("CSS_IdCentroServicios", IdCentroServicios.ToString(), OperadorComparacion.Equal);
                //LambdaExpression lamda2 = contexto.CrearExpresionLambda<CentroServiciosSrvUniNeg_VCOM>("CSS_Estado", ConstantesFramework.ESTADO_ACTIVO, OperadorComparacion.Equal);

                where.Add(lamda, OperadorLogico.And);
                //where.Add(lamda2, OperadorLogico.And);

                if (filtro.ContainsKey("SER_IdUnidadNegocio"))
                {
                    if (string.IsNullOrEmpty(filtro["SER_IdUnidadNegocio"]))
                    {
                        filtro.Remove("SER_IdUnidadNegocio");
                    }
                    //else
                    //  where.Remove(lamda2);
                }

                /* if (filtro.ContainsKey("SER_Nombre"))
                 {
                   if (!string.IsNullOrEmpty(filtro["SER_Nombre"]))
                   {
                     where.Remove(lamda2);
                   }
                 }*/

                var srv = contexto.ConsultarCentroServiciosSrvUniNeg_VCOM(filtro, where, campoOrdenamiento, out totalRegistros, indicePagina, registrosPorPagina, ordenamientoAscendente).ToList();
                var srvUnicos = srv.GroupBy(s => s.CSS_IdServicio).Select(s => s.First()).ToList();
                var servicios = srvUnicos.ConvertAll<CMServiciosCentroServicios>(s =>
                {
                    CMServiciosCentroServicios com = new CMServiciosCentroServicios()
                    {
                        Estado = s.CSS_Estado == ConstantesFramework.ESTADO_ACTIVO ? true : false,
                        FechaInicioVenta = s.CSS_FechaInicioVenta,
                        IdCentroServiciosServicio = s.CSS_IdCentroServicioServicio,
                        Servicios = new TAServicioDC()
                        {
                            IdUnidadNegocio = s.SER_IdUnidadNegocio,
                            UnidadNegocio = s.UNE_Descripcion,
                            IdServicio = s.CSS_IdServicio,
                            Nombre = s.SER_Nombre
                        },
                        IdCentroServicios = IdCentroServicios,
                    };
                    ///Seleccion de los nombres de los dias en que se presta el servicio
                    var dias = contexto.CentroServicioServicioDia_PUA.Where(cssd => cssd.CSD_IdCentroServicioServicio == s.CSS_IdCentroServicioServicio).ToList();
                    com.DiasServicio = string.Join(",", dias.Join(contexto.Dia_PAR, cssd => cssd.CSD_IdDia, d => d.DIA_IdDia,
                    (cssd, d) => d.DIA_NombreDia).ToArray());
                    ///Seleccion de los horarios en que se presta el servicio
                    com.HorariosServicios = dias.ConvertAll<PUHorariosServiciosCentroServicios>(h =>
                         new PUHorariosServiciosCentroServicios()
                         {
                             HoraFin = h.CSD_HoraFinal,
                             HoraInicio = h.CSD_HoraInicial,
                             IdCentroServicioSrvDia = h.CSD_IdCentroSrvSrvDia,
                             IdCentroServiciosServicio = h.CSD_IdCentroServicioServicio,
                             IdDia = h.CSD_IdDia
                         });

                    return com;
                });

                return servicios;
            }
        }

        /// <summary>
        /// Adiciona un servicios a un centro de servicios junto con los descuentos, comisiones y horarios
        /// </summary>
        /// <param name="servicios">Objeto con la informacion de los servicios</param>
        public void AdicionarServiciosCentroServicios(CMServiciosCentroServicios servicios)
        {
            using (ModeloComisiones contexto = new ModeloComisiones(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                DateTime fechaGrabacion = DateTime.Now;
                ///Asigna los servicios a un centro de servicios
                CentroServicioServicio_PUA centroSer = new CentroServicioServicio_PUA()
                {
                    CSS_CreadoPor = ControllerContext.Current.Usuario,
                    CSS_Estado = servicios.Estado ? ConstantesFramework.ESTADO_ACTIVO : ConstantesFramework.ESTADO_INACTIVO,
                    CSS_FechaGrabacion = fechaGrabacion,
                    CSS_FechaInicioVenta = servicios.FechaInicioVenta,
                    CSS_IdCentroServicios = servicios.IdCentroServicios,
                    CSS_IdServicio = servicios.Servicios.IdServicio
                };

                contexto.CentroServicioServicio_PUA.Add(centroSer);

                ///Graba los horarios de los servicios
                if (servicios.HorariosServicios != null)
                {
                    servicios.HorariosServicios.ToList().ForEach(h =>
                    {
                        CentroServicioServicioDia_PUA CentroSerDia = new CentroServicioServicioDia_PUA()
                        {
                            CSD_CreadoPor = ControllerContext.Current.Usuario,
                            CSD_FechaGrabacion = fechaGrabacion,
                            CSD_HoraFinal = h.HoraFin,
                            CSD_IdDia = h.IdDia,
                            CSD_HoraInicial = h.HoraInicio,
                            CSD_IdCentroSrvSrvDia = 0,
                            CSD_IdCentroServicioServicio = centroSer.CSS_IdCentroServicioServicio
                        };
                        contexto.CentroServicioServicioDia_PUA.Add(CentroSerDia);
                    });
                }
                ///Graba los descuentos de los servicios
                if (servicios.DescuentosServicios != null)
                {
                    servicios.DescuentosServicios.ToList().ForEach(d =>
                    {
                        CentroServicioSrvDescuento_COM CentroSerDes = new CentroServicioSrvDescuento_COM()
                        {
                            CSD_CreadoPor = ControllerContext.Current.Usuario,
                            CSD_Descripcion = d.Descripcion,
                            CSD_FechaGrabacion = fechaGrabacion,
                            CSD_IdCentroServicioServicio = centroSer.CSS_IdCentroServicioServicio,
                            CSD_IdCentroServiSrvDescuento = 0,
                            CSD_IdTipoDescuento = (short)d.IdTipoDescuento,
                            CSD_PorcentajeDescuento = d.PorcentajeDescuento,
                            CSD_ValorCriterioPenalizacion = d.ValorCriterioPenalizacion,
                            CSD_ValorDescuento = d.ValorDescuento
                        };
                        contexto.CentroServicioSrvDescuento_COM.Add(CentroSerDes);
                    });
                }

                ///Graba las comisiones de los servicios
                if (servicios.ComisionesServicios != null)
                {
                    servicios.ComisionesServicios.ToList().ForEach(c =>
                    {
                        CentroServicioSrvComi_COM CentroSerCom = new CentroServicioSrvComi_COM()
                        {
                            CSC_CreadoPor = ControllerContext.Current.Usuario,
                            CSC_FechaGrabacion = fechaGrabacion,
                            CSC_IdCentroServicioServicio = centroSer.CSS_IdCentroServicioServicio,
                            CSC_IdTipoComision = (short)c.IdTipoComision.Value,
                            CSC_Porcentaje = c.Porcentaje.Value,
                            CSC_Valor = c.Valor.Value
                        };
                        contexto.CentroServicioSrvComi_COM.Add(CentroSerCom);
                    });
                }
                AuditarCentroServicioServicio_PUA(contexto);
                contexto.SaveChanges();
            }
        }

        /// <summary>
        /// Modifica un servicio asignado a un centro de servicios junto con los descuentos, comisiones y horarios
        /// </summary>
        /// <param name="servicios">Objeto con la informacion de los servicios</param>
        public void EditarServiciosCentroServicios(CMServiciosCentroServicios servicios)
        {
            using (ModeloComisiones contexto = new ModeloComisiones(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                CentroServicioServicio_PUA centroSer = contexto.CentroServicioServicio_PUA.Where(c => c.CSS_IdCentroServicioServicio == servicios.IdCentroServiciosServicio).SingleOrDefault();
                if (centroSer == null)
                {
                    ControllerException excepcion = new ControllerException(COConstantesModulos.CENTRO_SERVICIOS, ETipoErrorFramework.EX_CONSULTA_DB_NULL.ToString(), MensajesFramework.CargarMensaje(ETipoErrorFramework.EX_CONSULTA_DB_NULL));
                    throw new FaultException<ControllerException>(excepcion);
                }

                centroSer.CSS_Estado = servicios.Estado ? ConstantesFramework.ESTADO_ACTIVO : ConstantesFramework.ESTADO_INACTIVO;
                centroSer.CSS_FechaInicioVenta = servicios.FechaInicioVenta;
                centroSer.CSS_IdCentroServicios = servicios.IdCentroServicios;
                centroSer.CSS_IdServicio = servicios.Servicios.IdServicio;

                if (servicios.HorariosServicios != null)
                {
                    ///Agrega o modifica los horarios de los servicios de los centros de servicios
                    var horarios = contexto.CentroServicioServicioDia_PUA.Where(c => c.CSD_IdCentroServicioServicio == servicios.IdCentroServiciosServicio).ToList();
                    for (int i = horarios.Count - 1; i >= 0; i--)
                    {
                        contexto.CentroServicioServicioDia_PUA.Remove(horarios[i]);
                    }
                    servicios.HorariosServicios.ToList().ForEach(h =>
                    {
                        CentroServicioServicioDia_PUA centroSerDia = new CentroServicioServicioDia_PUA()
                        {
                            CSD_CreadoPor = ControllerContext.Current.Usuario,
                            CSD_FechaGrabacion = DateTime.Now,
                            CSD_HoraFinal = h.HoraFin,
                            CSD_IdDia = h.IdDia,
                            CSD_HoraInicial = h.HoraInicio,
                            CSD_IdCentroServicioServicio = centroSer.CSS_IdCentroServicioServicio
                        };
                        contexto.CentroServicioServicioDia_PUA.Add(centroSerDia);
                    });
                }
                if (servicios.ComisionesServicios != null)
                {
                    ///Agrega o modifica las comisiones por los servicios de los centros de servicio
                    servicios.ComisionesServicios.Where(c => c.EstadoRegistro != EnumEstadoRegistro.SIN_CAMBIOS).ToList().ForEach(c =>
                    {
                        if (c.EstadoRegistro != EnumEstadoRegistro.BORRADO)
                        {
                            CentroServicioSrvComi_COM CentroSerComi = contexto.CentroServicioSrvComi_COM.Where(co => co.CSC_IdCentroServicioServicio == c.IdCentroServicioServicio && co.CSC_IdTipoComision == c.IdTipoComision).SingleOrDefault();
                            if (CentroSerComi == null)
                            {
                                CentroServicioSrvComi_COM centroSerCom = new CentroServicioSrvComi_COM()
                                {
                                    CSC_CreadoPor = ControllerContext.Current.Usuario,
                                    CSC_FechaGrabacion = DateTime.Now,
                                    CSC_IdCentroServicioServicio = centroSer.CSS_IdCentroServicioServicio,
                                    CSC_IdTipoComision = (short)c.IdTipoComision.Value,
                                    CSC_Porcentaje = c.Porcentaje.Value,
                                    CSC_Valor = c.Valor.Value
                                };
                                contexto.CentroServicioSrvComi_COM.Add(centroSerCom);
                                AuditarCentroServicioSrvComi_COM(contexto);
                            }
                            else
                            {
                                CentroSerComi.CSC_Porcentaje = c.Porcentaje.Value;
                                CentroSerComi.CSC_Valor = c.Valor.Value;
                                AuditarCentroServicioSrvComi_COM(contexto);
                            }
                        }
                        else
                        {
                            CentroServicioSrvComi_COM CentroSerComi = contexto.CentroServicioSrvComi_COM.Where(co => co.CSC_IdCentroServicioServicio == c.IdCentroServicioServicio && co.CSC_IdTipoComision == c.IdTipoComision).SingleOrDefault();
                            if (CentroSerComi != null)
                            {
                                contexto.CentroServicioSrvComi_COM.Remove(CentroSerComi);
                                AuditarCentroServicioSrvComi_COM(contexto);
                            }
                        }
                    });
                }
                if (servicios.DescuentosServicios != null)
                {
                    /// Agrega o modifica los descuentos por los servicios de los centros de servicio
                    servicios.DescuentosServicios.Where(d => d.EstadoRegistro != EnumEstadoRegistro.SIN_CAMBIOS).ToList().ForEach(d =>
                    {
                        if (d.EstadoRegistro != EnumEstadoRegistro.BORRADO)
                        {
                            CentroServicioSrvDescuento_COM CentroSerDes = contexto.CentroServicioSrvDescuento_COM.Where(de => de.CSD_IdCentroServiSrvDescuento == d.IdCentroServicioServDescuento).SingleOrDefault();
                            if (CentroSerDes == null)
                            {
                                CentroServicioSrvDescuento_COM centroSerDes = new CentroServicioSrvDescuento_COM()
                                {
                                    CSD_CreadoPor = ControllerContext.Current.Usuario,
                                    CSD_Descripcion = d.Descripcion,
                                    CSD_FechaGrabacion = DateTime.Now,
                                    CSD_IdCentroServicioServicio = centroSer.CSS_IdCentroServicioServicio,
                                    CSD_IdTipoDescuento = (short)d.IdTipoDescuento,
                                    CSD_PorcentajeDescuento = d.PorcentajeDescuento,
                                    CSD_ValorCriterioPenalizacion = d.ValorCriterioPenalizacion,
                                    CSD_ValorDescuento = d.ValorDescuento
                                };
                                contexto.CentroServicioSrvDescuento_COM.Add(centroSerDes);
                                AuditarCentroServicioSrvDescuento_COM(contexto);
                            }
                            else
                            {
                                CentroSerDes.CSD_Descripcion = d.Descripcion;
                                CentroSerDes.CSD_IdCentroServicioServicio = centroSer.CSS_IdCentroServicioServicio;
                                CentroSerDes.CSD_IdTipoDescuento = (short)d.IdTipoDescuento;
                                CentroSerDes.CSD_PorcentajeDescuento = d.PorcentajeDescuento;
                                CentroSerDes.CSD_ValorCriterioPenalizacion = d.ValorCriterioPenalizacion;
                                CentroSerDes.CSD_ValorDescuento = d.ValorDescuento;
                                AuditarCentroServicioSrvDescuento_COM(contexto);
                            }
                        }
                        else
                        {
                            CentroServicioSrvDescuento_COM CentroSerDes = contexto.CentroServicioSrvDescuento_COM.Where(de => de.CSD_IdCentroServiSrvDescuento == d.IdCentroServicioServDescuento).SingleOrDefault();
                            if (CentroSerDes != null)
                            {
                                contexto.CentroServicioSrvDescuento_COM.Remove(CentroSerDes);
                                AuditarCentroServicioSrvDescuento_COM(contexto);
                            }
                        }
                    });
                }
                AuditarCentroServicioServicio_PUA(contexto);
                contexto.SaveChanges();
            }
        }

        /// <summary>
        /// Desactivar un servicio signado a un centro de servicios junto con los descuentos, comisiones y horarios
        /// </summary>
        /// <param name="servicios">Objeto con la informacion de los servicios</param>
        public void DesactivarServiciosCentroServicios(CMServiciosCentroServicios servicios)
        {
            using (ModeloComisiones contexto = new ModeloComisiones(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                CentroServicioServicio_PUA centroSer = contexto.CentroServicioServicio_PUA.Where(c => c.CSS_IdCentroServicioServicio == servicios.IdCentroServiciosServicio).SingleOrDefault();
                if (centroSer == null)
                {
                    ControllerException excepcion = new ControllerException(COConstantesModulos.CENTRO_SERVICIOS, ETipoErrorFramework.EX_CONSULTA_DB_NULL.ToString(), MensajesFramework.CargarMensaje(ETipoErrorFramework.EX_CONSULTA_DB_NULL));
                    throw new FaultException<ControllerException>(excepcion);
                }
                centroSer.CSS_Estado = ConstantesFramework.ESTADO_INACTIVO;
                contexto.SaveChanges();
            }
        }

        /// <summary>
        /// Audita las comisiones de los servicios de un centro de servicios
        /// </summary>
        /// <param name="contexto"></param>
        /// <param name="comi">objeto con la informacion de las comisiones</param>
        private void AuditarCentroServicioSrvComi_COM(ModeloComisiones contexto)
        {
            contexto.Audit<CentroServicioSrvComi_COM, CentroServicioSrvComiHist_COM>((record, action) => new CentroServicioSrvComiHist_COM()
            {
                CCS_CambiadoPor = ControllerContext.Current.Usuario,
                CCS_TipoCambio = action.ToString(),
                CCS_FechaCambio = DateTime.Now,
                CSC_CreadoPor = record.Field<CentroServicioSrvComi_COM, string>(c => c.CSC_CreadoPor),
                CSC_FechaGrabacion = record.Field<CentroServicioSrvComi_COM, DateTime>(c => c.CSC_FechaGrabacion),
                CSC_IdAuditoria = 0,
                CSC_IdCentroServicioServicio = record.Field<CentroServicioSrvComi_COM, long>(c => c.CSC_IdCentroServicioServicio),
                CSC_IdTipoComision = record.Field<CentroServicioSrvComi_COM, short>(c => c.CSC_IdTipoComision),
                CSC_Porcentaje = record.Field<CentroServicioSrvComi_COM, decimal>(c => c.CSC_Porcentaje),
                CSC_Valor = record.Field<CentroServicioSrvComi_COM, decimal>(c => c.CSC_Valor)
            }, (cm) => contexto.CentroServicioSrvComiHist_COM.Add(cm));
        }

        /// <summary>
        /// Audita los descuentos de los servicios de un centro de servicios
        /// </summary>
        /// <param name="contexto"></param>
        /// <param name="comi">objeto con la informacion de los descuentos</param>
        private void AuditarCentroServicioSrvDescuento_COM(ModeloComisiones contexto)
        {
            contexto.Audit<CentroServicioSrvDescuento_COM, CentroSrvSrvDescuentoHist_COM>((record, action) => new CentroSrvSrvDescuentoHist_COM()
            {
                CSD_CambiadoPor = ControllerContext.Current.Usuario,
                CSD_TipoCambio = action.ToString(),
                CSD_FechaCambio = DateTime.Now,
                CSD_CreadoPor = record.Field<CentroSrvSrvDescuentoHist_COM, string>(c => c.CSD_CreadoPor),
                CSD_FechaGrabacion = record.Field<CentroSrvSrvDescuentoHist_COM, DateTime>(c => c.CSD_FechaGrabacion),
                CSD_IdAuditoria = 0,

                CSD_Descripcion = record.Field<CentroSrvSrvDescuentoHist_COM, string>(c => c.CSD_Descripcion),
                CSD_IdCentroServicioServicio = record.Field<CentroSrvSrvDescuentoHist_COM, long>(c => c.CSD_IdCentroServicioServicio),
                CSD_IdCentroServiSrvDescuento = record.Field<CentroSrvSrvDescuentoHist_COM, int>(c => c.CSD_IdCentroServiSrvDescuento),
                CSD_IdTipoDescuento = record.Field<CentroSrvSrvDescuentoHist_COM, short>(c => c.CSD_IdTipoDescuento),
                CSD_PorcentajeDescuento = record.Field<CentroSrvSrvDescuentoHist_COM, decimal>(c => c.CSD_PorcentajeDescuento),
                CSD_ValorCriterioPenalizacion = record.Field<CentroSrvSrvDescuentoHist_COM, decimal>(c => c.CSD_ValorCriterioPenalizacion),
                CSD_ValorDescuento = record.Field<CentroSrvSrvDescuentoHist_COM, decimal>(c => c.CSD_ValorDescuento)
            }, (cm) => contexto.CentroSrvSrvDescuentoHist_COM.Add(cm));
        }

        /// <summary>
        /// Audita los servicios de un centro de servicios
        /// </summary>
        /// <param name="contexto"></param>
        /// <param name="comi">objeto con la informacion de los servicios de un centro de servicios</param>
        private void AuditarCentroServicioServicio_PUA(ModeloComisiones contexto)
        {
            contexto.Audit<CentroServicioServicio_PUA, CentroServicioServicioHist_PUA>((record, action) => new CentroServicioServicioHist_PUA()
            {
                CSS_CambiadoPor = ControllerContext.Current.Usuario,
                CSS_TipoCambio = action.ToString(),
                CCS_FechaCambio = DateTime.Now,
                CSS_CreadoPor = record.Field<CentroServicioServicioHist_PUA, string>(c => c.CSS_CreadoPor),
                CSS_FechaGrabacion = record.Field<CentroServicioServicioHist_PUA, DateTime>(c => c.CSS_FechaGrabacion),
                CSS_IdAuditoria = 0,

                CSS_Estado = record.Field<CentroServicioServicioHist_PUA, string>(c => c.CSS_Estado),
                CSS_FechaInicioVenta = record.Field<CentroServicioServicioHist_PUA, DateTime>(c => c.CSS_FechaInicioVenta),
                CSS_IdCentroServicios = record.Field<CentroServicioServicioHist_PUA, long>(c => c.CSS_IdCentroServicios),
                CSS_IdCentroServicioServicio = record.Field<CentroServicioServicioHist_PUA, long>(c => c.CSS_IdCentroServicioServicio),
                CSS_IdServicio = record.Field<CentroServicioServicioHist_PUA, int>(c => c.CSS_IdServicio),
            }, (cm) => contexto.CentroServicioServicioHist_PUA.Add(cm));
        }

        #endregion Servicios de Centro servicios

        #region Comision fija centro svc

        /// <summary>
        /// Obtener las comisiones fijas de un centro servicios activas
        /// </summary>
        /// <param name="fechaCorte">Fecha de corte para validar el inicio de la comision</param>
        /// <param name="idCentroServicios">id Centro servicios</param>
        /// <returns></returns>
        public List<CMComisionesConceptosAdicionales> ObtenerComisionesFijasCentroSvcContrato(long idCentroServicios)
        {
            using (ModeloComisiones contexto = new ModeloComisiones(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.ComisionFijaCentroSvcContrato_COM.Where(r => r.CCC_IdCentroServicios == idCentroServicios && r.CCC_Estado == ConstantesFramework.ESTADO_ACTIVO)
                  .ToList()
                  .ConvertAll(r => new CMComisionesConceptosAdicionales()
                  {
                      IdContrato = r.CCC_IdContrato,
                      Valor = r.CCC_Valor,
                      FechaInicio = r.CCC_FechaInicio,
                      Descripcion = r.TCF_Descripcion,
                      IdTipoComisionFija = r.CCC_IdTipoComisionFija,
                      FechaTeminacionContrato = r.CON_FechaFin
                  });
            }
        }

        /// <summary>
        /// Obtiene las comisiones fijas que no estan incluidas en un contrato
        /// </summary>
        /// <param name="idCentroServicios"></param>
        /// <returns></returns>
        public List<CMComisionesConceptosAdicionales> ObtenerComisionesFijasCentroSvc(long idCentroServicios)
        {
            using (ModeloComisiones contexto = new ModeloComisiones(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.CentroServicioComiFijas_VCOM.Where(r => r.CSD_IdCentroServicios == idCentroServicios && r.CSD_Estado == ConstantesFramework.ESTADO_ACTIVO)
                  .ToList()
                  .ConvertAll(r => new CMComisionesConceptosAdicionales()
                  {
                      Descripcion = r.CSD_Descripcion,
                      IdTipoComisionFija = r.CSD_IdTipoComisionFija,
                      Valor = r.CSD_Valor,
                  });
            }
        }

        #endregion Comision fija centro svc

        #region Guardar Comision

        /// <summary>
        /// Almacena las comisiones de una venta, una entrega o un pago
        /// </summary>
        /// <param name="comision">valores de la comision ganada por la agencia/punto</param>
        public void GuardarComision(CMComisionXVentaCalculadaDC comision)
        {
            //using (ModeloComisiones contexto = new ModeloComisiones(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            //{
            //    DateTime fecha = new DateTime();

            //    if (comision.FechaProduccion == fecha)
            //    {
            //        comision.FechaProduccion = DateTime.Now;
            //    }
            //    if (comision.FechaProduccionResponsable == fecha)
            //    {
            //        comision.FechaProduccionResponsable = DateTime.Now;
            //    }

            //    contexto.paInsertarLiquidacionComision_COM(comision.NumeroOperacion, comision.IdServicio, (short?)comision.TipoComision,
            //    comision.IdCentroServicioVenta, comision.NombreCentroServicioVenta, comision.IdCentroServicioResponsable, comision.NombreCentroServicioResponsable,
            //    comision.BaseComision, comision.TotalComisionCentroServicioVenta, comision.ValorFijoComisionCentroServicioVenta, comision.PorcComisionCentroServicioVenta,
            //    comision.TotalComisionCentroServicioResponsable, comision.ValorFijoComisionCentroServicioResponsable, comision.PorcComisionCentroServicioResponsable, comision.FechaProduccion,
            //    0, false, string.Empty, comision.FechaProduccionResponsable, DateTime.Now, ControllerContext.Current != null ? ControllerContext.Current.Usuario : "APPTARIFAS", comision.EsRegistroValido);

            //    contexto.SaveChanges();
            //}
        }

        /// <summary>
        /// Almacena las comisiones de una venta, una entrega o un pago
        /// </summary>
        /// <param name="comision">valores de la comision ganada por la agencia/punto</param>
        public void GuardarComision(CMComisionXVentaCalculadaDC comision, SqlConnection conexion, SqlTransaction transaccion)
        {

            //DateTime fecha = new DateTime();

            //if (comision.FechaProduccion == fecha)
            //{
            //    comision.FechaProduccion = DateTime.Now;
            //}
            //if (comision.FechaProduccionResponsable == fecha)
            //{
            //    comision.FechaProduccionResponsable = DateTime.Now;
            //}

            //SqlCommand cmd = new SqlCommand("paInsertarLiquidacionComision_COM", conexion, transaccion);
            //cmd.CommandType = CommandType.StoredProcedure;

            //cmd.Parameters.Add(Utilidades.AddParametro("@LIC_NumeroOperacion", comision.NumeroOperacion));
            //cmd.Parameters.Add(Utilidades.AddParametro("@LIC_IdServicio", comision.IdServicio));
            //cmd.Parameters.Add(Utilidades.AddParametro("@LIC_IdTipoComision", (short?)comision.TipoComision));
            //cmd.Parameters.Add(Utilidades.AddParametro("@LIC_IdCentroServiciosVenta", comision.IdCentroServicioVenta));
            //cmd.Parameters.Add(Utilidades.AddParametro("@LIC_NombreCentroServiciosVenta", comision.NombreCentroServicioVenta));
            //cmd.Parameters.Add(Utilidades.AddParametro("@LIC_IdCentroServiosResponsable", comision.IdCentroServicioResponsable));
            //cmd.Parameters.Add(Utilidades.AddParametro("@LIC_NombreCtroSvcsResponsable", comision.NombreCentroServicioResponsable));
            //cmd.Parameters.Add(Utilidades.AddParametro("@LIC_ValorBaseComision", comision.BaseComision));
            //cmd.Parameters.Add(Utilidades.AddParametro("@LIC_ComisionVenta", comision.TotalComisionCentroServicioVenta));
            //cmd.Parameters.Add(Utilidades.AddParametro("@LIC_ValorFijoComisionVenta", comision.ValorFijoComisionCentroServicioVenta));
            //cmd.Parameters.Add(Utilidades.AddParametro("@LIC_PorcentajeComisionVenta", comision.PorcComisionCentroServicioVenta));
            //cmd.Parameters.Add(Utilidades.AddParametro("@LIC_ComisionResponsable", comision.TotalComisionCentroServicioResponsable));
            //cmd.Parameters.Add(Utilidades.AddParametro("@LIC_ValorFijoComisionResponsable", comision.ValorFijoComisionCentroServicioResponsable));
            //cmd.Parameters.Add(Utilidades.AddParametro("@LIC_PorcentajeComisionResponsable", comision.PorcComisionCentroServicioResponsable));
            //cmd.Parameters.Add(Utilidades.AddParametro("@LIC_FechaProduccion", comision.FechaProduccion));
            //cmd.Parameters.Add(Utilidades.AddParametro("@LIC_NumeroProduccion", 0));
            //cmd.Parameters.Add(Utilidades.AddParametro("@LIC_EstaAprobada", false));
            //cmd.Parameters.Add(Utilidades.AddParametro("@LIC_NumeroProduccionResponsabl", string.Empty));
            //cmd.Parameters.Add(Utilidades.AddParametro("@LIC_FechaProduccionResponsable", comision.FechaProduccionResponsable));
            //cmd.Parameters.Add(Utilidades.AddParametro("@LIC_FechaGrabacion", DateTime.Now));
            //cmd.Parameters.Add(Utilidades.AddParametro("@LIC_CreadoPor", ControllerContext.Current != null ? ControllerContext.Current.Usuario : "APPTARIFAS"));
            //cmd.Parameters.Add(Utilidades.AddParametro("@LIC_EsRegistroValido", comision.EsRegistroValido));

            //cmd.ExecuteNonQuery();

        }

        #endregion Guardar Comision

        #region Consultas

        /// <summary>
        /// Obtiene las comisiones del punto y del responsable
        /// por el numero de la Operacion
        /// </summary>
        /// <param name="numeroOperacion">el numero de la Operacion sea giro, Guia
        /// ó el valor guardado en RTD_Numero en la tbl RegistroTransacDetalleCaja_CAJ</param>
        /// <returns>lista de las Comisiones asociadas</returns>
        public List<CMComisionXVentaCalculadaDC> ObtenerComisionPtoYCentroResponsable(long numeroOperacion)
        {
            using (ModeloComisiones contexto = new ModeloComisiones(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                List<ComisionRealizadaPuntoYResponsable_COM> dataComision = contexto.paObtenerComisionRealizadaPuntoYResponsable_COM(numeroOperacion).ToList();

                List<CMComisionXVentaCalculadaDC> comisionesPtoYResponsable = null;

                if (dataComision != null && dataComision.Count > 0)
                {
                    comisionesPtoYResponsable = dataComision.ConvertAll<CMComisionXVentaCalculadaDC>(r => new CMComisionXVentaCalculadaDC()
                    {
                        BaseComision = r.LIC_ValorBaseComision,
                        IdServicio = r.LIC_IdServicio,
                        EsRegistroValido = r.LIC_EsRegistroValido,
                        NumeroOperacion = r.LIC_NumeroOperacion,
                        TotalComisionEmpresa = r.LIC_ValorBaseComision - r.LIC_ComisionVenta - r.LIC_ComisionResponsable,

                        IdCentroServicioResponsable = r.LIC_IdCentroServiosResponsable,
                        NombreCentroServicioResponsable = r.LIC_NombreCtroSvcsResponsable,
                        PorcComisionCentroServicioResponsable = r.LIC_PorcentajeComisionResponsable,
                        ValorFijoComisionCentroServicioResponsable = r.LIC_ValorFijoComisionResponsable,
                        TotalComisionCentroServicioResponsable = r.LIC_ComisionResponsable,

                        IdCentroServicioVenta = r.LIC_IdCentroServiciosVenta,
                        NombreCentroServicioVenta = r.LIC_NombreCentroServiciosVenta,
                        PorcComisionCentroServicioVenta = r.LIC_PorcentajeComisionVenta,
                        ValorFijoComisionCentroServicioVenta = r.LIC_ValorFijoComisionVenta,
                        TotalComisionCentroServicioVenta = r.LIC_ComisionVenta,
                    });
                }

                return comisionesPtoYResponsable;
            }
        }

        #endregion Consultas
    }
}