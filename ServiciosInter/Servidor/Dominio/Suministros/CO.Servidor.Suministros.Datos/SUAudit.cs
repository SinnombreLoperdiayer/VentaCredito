using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CO.Servidor.Servicios.ContratoDatos.Suministros;
using CO.Servidor.Suministros.Datos.Modelo;
using Framework.Servidor.Excepciones;

namespace CO.Servidor.Suministros.Datos
{
  /// <summary>
  /// Clase con los metodos de auditoria de suministros
  /// </summary>
  public class SUAudit
  {
    #region Singleton

    private static readonly SUAudit instancia = new SUAudit();

    /// <summary>
    /// Retorna la instancia de la clase SUAudit
    /// </summary>
    public static SUAudit Instancia
    {
      get { return SUAudit.instancia; }
    }

    #endregion Singleton

		#region RACOL
		
    /// <summary>
    /// Audita la reasignacion de los suministros a una agencia
    /// </summary>
    /// <param name="contexto"></param>
    internal void AuditarSuministroAgencia(ModeloSuministros contexto, int idCentroServSum, long anteriorAgencia, long nuevaAgencia, int idSuministro, SUEnumGrupoSuministroDC grupoSumAnterior, SUEnumGrupoSuministroDC grupoSumNuevo)
    {
      contexto.ProvisionSumCentroSvc_SUM.Where(p => p.PSC_IdSuministroCentroServicio == idCentroServSum).ToList()
        .ForEach(p =>
        {
          ProvisionSumCentroSvcHist_SUM provisionSumCentroSerHist = new ProvisionSumCentroSvcHist_SUM()
          {
            PCH_CambiadoPor = ControllerContext.Current.Usuario,
            PCH_FechaCambio = DateTime.Now,
            PCH_CreadoPor = p.PSC_CreadoPor,
            PCH_CantidadAsginada = p.PSC_CantidadAsginada,
            PCH_FechaGrabacion = p.PSC_FechaGrabacion,
            PCH_IdCentroServicios = anteriorAgencia,
            PCH_IdProvisionSumCentroSvc = p.PSC_IdProvisionSumCentroSvc,
            PCH_IdRemisionSuministros = p.PSC_IdRemisionSuministros,
            PCH_IdSuministro = idSuministro,
            PCH_IdSuministroCentroServicio = p.PSC_IdSuministroCentroServicio,
            PCH_TipoCambio = "Actualizacion"
          };

          contexto.ProvisionSumCentroSvcSerial_SUM.Where(ps => ps.PCS_IdProvisionSumCentroSvc == p.PSC_IdProvisionSumCentroSvc).ToList()
         .ForEach(ps =>
         {
           ProvisionSumCentroSvcSerialHist_SUM provisionSerSerHis = new ProvisionSumCentroSvcSerialHist_SUM()
           {
             PSH_CambiadoPor = ControllerContext.Current.Usuario,
             PSH_CreadoPor = ps.PCS_CreadoPor,
             PSH_FechaCambio = DateTime.Now,
             PSH_FechaFinal = ps.PCS_FechaFinal,
             PSH_FechaGrabacion = ps.PCS_FechaGrabacion,
             PSH_FechaInicial = ps.PCS_FechaInicial,
             PSH_IdProvisionSumCentroSvcHist = provisionSumCentroSerHist.PCH_IdProvisionSumCentroSvc,
             PSH_Fin = ps.PCS_Fin,
             PSH_IdSumiCentroSvcSerial = ps.PCS_IdProvisionSumCentroSvcSerial,
             PSH_Inicio = ps.PCS_Inicio,
             PSH_Prefijo = ps.PCS_Prefijo,
             PSH_TipoCambio = "Actualizacion",
             PSH_IdNumerador = ps.PCS_IdNumerador,
             PSH_PorModificacion = true
           };
           contexto.ProvisionSumCentroSvcSerialHist_SUM.Add(provisionSerSerHis);

           SUTrasladoSuministroDC traslado = new SUTrasladoSuministroDC()
           {
             Cantidad = (int)(ps.PCS_Inicio - ps.PCS_Fin + 1),
             GrupoSuministroDestino = grupoSumNuevo,
             GrupoSuministroOrigen = grupoSumAnterior,
             IdentificacionDestino = nuevaAgencia,
             IdentificacionOrigen = anteriorAgencia,
             NumeroSuministro = idSuministro,
             Suministro = (SUEnumSuministro)idSuministro
           };

           SURepositorio.Instancia.GuardarTrasladoSuministro(traslado);
         });
          contexto.ProvisionSumCentroSvcHist_SUM.Add(provisionSumCentroSerHist);
        });
    }

		/// <summary>
		/// Audita la provisión de suministros a un Centro de servicios, copiando los datos a la tabla de historicos y elimina el registro de la tabla principal
		/// siempre y cuando el suministro no hay sido consumido.
		/// </summary>
		/// <param name="contexto"></param>
		internal void Auditar_ProvisionSuministrosCentroSvc(ModeloSuministros contexto, long idProvision, int idSuministro, long idCentroServicios, string User)
		{
			contexto.paEliminarProvisionSumCentroSvc_SUM(idProvision, idSuministro, idCentroServicios, User);
		}

		#endregion

		#region Clientes (Sucursal)
		
		/// <summary>
		/// Audita la provisión de suministros a una Sucursal (Cliente), copiando los datos a la tabla de historicos y elimina el registro de la tabla principal
		/// siempre y cuando el suministro no hay sido consumido.
		/// </summary>
		/// <param name="contexto">Modelo de la Base de Datos</param>
		/// <param name="IdProvision">Identificador de los suministros y sus cantidades que fueron aprovisionados para un centro de servicios</param>
		/// <param name="IdSuministro">Identificador del suministro</param>
		/// <param name="IdSucursal">Identificador de la sucursal (Cliente)</param>
		/// <param name="User">Usuario que esta realizando el cambio.</param>
		internal void AuditarProvisionSuministrosSucursal(ModeloSuministros contexto, long IdProvision, int IdSuministro, long IdSucursal, string User)
		{
			contexto.paEliminarProvisionSumSucursal_SUM(IdProvision, IdSuministro, IdSucursal, User);
		}

		#endregion

		#region Mensajero

		/// <summary>
		/// Audita la provisión de suministros a una Sucursal (Cliente), copiando los datos a la tabla de historicos y elimina el registro de la tabla principal
		/// siempre y cuando el suministro no hay sido consumido.
		/// </summary>
		/// <param name="contexto">Modelo de la Base de Datos</param>
		/// <param name="IdProvision">Identificador de los suministros y sus cantidades que fueron aprovisionados para un centro de servicios</param>
		/// <param name="IdSuministro">Identificador del suministro</param>
		/// <param name="IdMensajero">Identificador del Mensajero</param>
		/// <param name="User">Usuario que esta realizando el cambio.</param>
		internal void AuditarProvisionSuministrosMensajero(ModeloSuministros contexto, long IdProvision, int IdSuministro, long IdMensajero, string User)
    {
			contexto.paEliminarProvisionSumMensajero_SUM(IdProvision, IdSuministro, IdMensajero, User);
    }

		#endregion

		#region Proceso

		/// <summary>
		/// Audita la provisión de suministros a una Sucursal (Cliente), copiando los datos a la tabla de historicos y elimina el registro de la tabla principal
		/// siempre y cuando el suministro no hay sido consumido.
		/// </summary>
		/// <param name="contexto">Modelo de la Base de Datos</param>
		/// <param name="IdProvision">Identificador de los suministros y sus cantidades que fueron aprovisionados para un centro de servicios</param>
		/// <param name="IdSuministro">Identificador del suministro</param>
		/// <param name="IdProceso">Identificador del Proceso</param>
		/// <param name="User">Usuario que esta realizando el cambio.</param>
		internal void AuditarProvisionSuministrosProceso(ModeloSuministros contexto, long IdProvision, int IdSuministro, long IdProceso, string User)
		{
			contexto.paEliminarProvisionSumProceso_SUM(IdProvision, IdSuministro, IdProceso, User);
		}
		
		#endregion
  }
}