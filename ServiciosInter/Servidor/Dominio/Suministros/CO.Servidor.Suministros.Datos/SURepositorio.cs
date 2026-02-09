using System;
using System.Collections.Generic;
using System.Data.Linq.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.ServiceModel;
using System.Threading.Tasks;
using CO.Servidor.Servicios.ContratoDatos.Admisiones.Mensajeria;
using CO.Servidor.Servicios.ContratoDatos.Area;
using CO.Servidor.Servicios.ContratoDatos.CentroServicios;
using CO.Servidor.Servicios.ContratoDatos.Suministros;
using CO.Servidor.Servicios.ContratoDatos.Tarifas;
using CO.Servidor.Suministros.Comun;
using CO.Servidor.Suministros.Datos.Modelo;
using Framework.Servidor.Agenda;
using Framework.Servidor.Comun;
using Framework.Servidor.Comun.Datos;
using Framework.Servidor.Excepciones;
using Framework.Servidor.Servicios.ContratoDatos.Parametros;
using System.Data.SqlClient;
using CO.Servidor.Dominio.Comun.Util;
using System.Configuration;

namespace CO.Servidor.Suministros.Datos
{
    /// <summary>
    /// Clase para consultar y persistir informacion en la base de datos para los procesos de suministros
    /// </summary>
    public class SURepositorio
    {
        private const string NombreModelo = "ModeloSuministros";
        private string conexionStringController = ConfigurationManager.ConnectionStrings["ControllerTransaccional"].ConnectionString;

        #region Singleton

        private static readonly SURepositorio instancia = new SURepositorio();

        /// <summary>
        /// Retorna la instancia de la clase TARepositorio
        /// </summary>
        public static SURepositorio Instancia
        {
            get { return SURepositorio.instancia; }
        }

        #endregion Singleton

        #region Servicio Giros

        /// <summary>
        /// Consulta el id agencia a la cual se le suministro la factura de venta con el numero de giro IdGiro
        /// </summary>
        /// <param name="idGiro">Numero del giro</param>
        /// <returns>Id del centro de servicio</returns>
        public long ConsultarAgenciaPropietariaDelNumeroGiro(long idGiro)
        {
            using (ModeloSuministros contexto = new ModeloSuministros(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                //SUpaObtenerCentroSrvPropietarioSuministro centroServicio = contexto.paObtenerCentroSrvPropietarioSuministro_SUM(SUConstantesSuministros.SUMINISTROS_GIROS, idGiro).FirstOrDefault();
                var centroServicio = contexto.paObtenerPropietarioSuministro_SUM(SUConstantesSuministros.SUMINISTROS_GIROS, idGiro)
                  .FirstOrDefault();

                if (centroServicio == null)
                {
                    throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.MODULO_SUMINISTROS, EnumTipoErrorSuministros.EX_NINGUNA_AGENCIA_TIENE_ASIGNADO_GIRO.ToString(), MensajesSuministros.CargarMensaje(EnumTipoErrorSuministros.EX_NINGUNA_AGENCIA_TIENE_ASIGNADO_GIRO)));
                }

                if (!centroServicio.IdCentroServicios.HasValue)
                {
                    throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.MODULO_SUMINISTROS, EnumTipoErrorSuministros.EX_NINGUNA_AGENCIA_TIENE_ASIGNADO_GIRO.ToString(), MensajesSuministros.CargarMensaje(EnumTipoErrorSuministros.EX_NINGUNA_AGENCIA_TIENE_ASIGNADO_GIRO)));
                }

                return centroServicio.IdCentroServicios.Value;
            }
        }

        /// <summary>
        /// Consulta el id de la agencia a la cual se le suministro es el comprobante de pago manual con el numero de
        /// comprobante de pago
        /// </summary>
        /// <param name="idComprobantePago" >Comprobante de pago</param>
        /// <returns>id de la agencia</returns>
        public long ConsultarAgenciaPropietariaDelNumeroComprobante(long idComprobantePago)
        {
            using (ModeloSuministros contexto = new ModeloSuministros(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                // SUpaObtenerCentroSrvPropietarioSuministro centroServicio = contexto.paObtenerCentroSrvPropietarioSuministro_SUM(SUConstantesSuministros.SUMINISTROS_COMPROBANTE_PAGO_MANUAL, idComprobantePago).FirstOrDefault();
                var centroServicio = contexto.paObtenerPropietarioSuministro_SUM(SUConstantesSuministros.SUMINISTROS_COMPROBANTE_PAGO_MANUAL, idComprobantePago)
                  .FirstOrDefault();

                if (centroServicio == null)
                {
                    throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.MODULO_SUMINISTROS, EnumTipoErrorSuministros.EX_NINGUNA_AGENCIA_TIENE_ASIGNADO_COMPROBANTE_PAGO.ToString(), MensajesSuministros.CargarMensaje(EnumTipoErrorSuministros.EX_NINGUNA_AGENCIA_TIENE_ASIGNADO_COMPROBANTE_PAGO)));
                }

                if (!centroServicio.IdCentroServicios.HasValue)
                {
                    throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.MODULO_SUMINISTROS, EnumTipoErrorSuministros.EX_NINGUNA_AGENCIA_TIENE_ASIGNADO_COMPROBANTE_PAGO.ToString(), MensajesSuministros.CargarMensaje(EnumTipoErrorSuministros.EX_NINGUNA_AGENCIA_TIENE_ASIGNADO_COMPROBANTE_PAGO)));
                }

                return centroServicio.IdCentroServicios.Value;
            }
        }

        #endregion Servicio Giros

        /// <summary>
        /// Retorna los suministros asignados a un centro de servicio
        /// </summary>
        /// <param name="centroServicio"></param>
        /// <returns></returns>
        public IEnumerable<SUSuministro> ObtenerSuministrosCentroServicio(PUCentroServiciosDC centroServicio)
        {
            using (ModeloSuministros contexto = new ModeloSuministros(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                List<SUSuministro> suministros =
                    contexto
                        .paObtenerSuministrosCentroServicio_SUM(centroServicio.IdCentroServicio)
                        .GroupBy(sum => sum.SUM_IdSuministro)
                        .Select(sum => new SUSuministro()
                        {
                            Descripcion = sum.First().SUM_Descripcion,
                            Id = sum.First().SUM_IdSuministro,
                            RangosAsignados = sum.Where(rango => rango.PCS_FechaInicial != null).Select(rango => new SURango { Fin = rango.PCS_Fin.Value, Inicio = rango.PCS_Inicio.Value, Prefijo = rango.PCS_Prefijo }).ToList(),
                            Suministro = AsignarSuministro(sum.First().SUM_IdSuministro),
                            Categoria = (SUEnumCategoria)sum.First().SUM_IdCategoria,
                            Prefijo = sum.First().SUM_Prefijo
                        })
                         .ToList();
                return suministros;
            }
        }

        /// <summary>
        /// Obtiene los tipos de  suministros existentes
        /// </summary>
        /// <returns></returns>
        public IEnumerable<SUSuministro> ObtenerTiposSuministros()
        {
            using (ModeloSuministros contexto = new ModeloSuministros(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.Suministro_SUM.ToList().ConvertAll<SUSuministro>(sum =>
                {
                    return new SUSuministro()
                    {
                        Id = sum.SUM_IdSuministro,
                        Descripcion = sum.SUM_Descripcion,
                        Prefijo = sum.SUM_Prefijo,
                        Categoria = (SUEnumCategoria)sum.SUM_IdCategoria
                    };
                });
            }
        }

        /// <summary>
        /// Consulta el suministro asociado a un prefijo especifico
        /// </summary>
        /// <param name="prefijo"></param>
        /// <returns></returns>
        public SUSuministro ConsultarSuministroxPrefijo(string prefijo)
        {
            using (ModeloSuministros contexto = new ModeloSuministros(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                Suministro_SUM sumbd = contexto.Suministro_SUM.Where(s => s.SUM_Prefijo == prefijo).FirstOrDefault();
                SUSuministro suministro = null;
                if (sumbd != null)
                    suministro = new SUSuministro()
                    {
                        Id = sumbd.SUM_IdSuministro,
                        Descripcion = sumbd.SUM_Descripcion,
                        Categoria = (SUEnumCategoria)sumbd.SUM_IdCategoria
                    };
                return suministro;
            }
        }

        /// <summary>
        /// Retorna el suministro dado su id
        /// </summary>
        /// <param name="idSuministro"></param>
        /// <returns></returns>
        public SUEnumSuministro AsignarSuministro(int idSuministro)
        {
            try
            {
                SUEnumSuministro suministro =
                (SUEnumSuministro)Enum.ToObject(typeof(SUEnumSuministro), idSuministro);
                if (Enum.IsDefined(typeof(SUEnumSuministro), suministro))
                {
                    return suministro;
                }
                return SUEnumSuministro.NOCONFIGURADO;
            }
            catch
            {
                return SUEnumSuministro.NOCONFIGURADO;
            }
        }

        /// <summary>
        /// Retorna el consecutivo dle suministro dado
        /// </summary>
        /// <param name="idSuministro"></param>
        /// <returns></returns>
        public long ObtenerConsecutivoSuministro(SUEnumSuministro Suministro)
        {
            using (ModeloSuministros contexto = new ModeloSuministros(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                int idSuministro = (int)Suministro;
                System.Data.Objects.ObjectParameter actual = new System.Data.Objects.ObjectParameter("ACTUAL", typeof(Int64));
                SUMpaObtenerSuministrosValorActualNumerador res = contexto.paObtenerSuministrosValorActualNumerador_SUM(idSuministro, actual).FirstOrDefault();
                if (res != null && res.ValorActual.HasValue)
                {
                    return res.ValorActual.Value;
                }
                else
                {
                    throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.MODULO_SUMINISTROS, EnumTipoErrorSuministros.EX_CONSECUTIVO_NO_DISPONIBLE.ToString(), MensajesSuministros.CargarMensaje(EnumTipoErrorSuministros.EX_CONSECUTIVO_NO_DISPONIBLE)));
                }
            }
        }

        /// <summary>
        /// Obtiene el numero de suministro prefijo + valorActual
        /// </summary>
        /// <param name="idSuministro">id del suministro</param>
        /// <returns>numero del giro</returns>
        public SUNumeradorPrefijo ObtenerNumeroPrefijoValor(SUEnumSuministro Suministro)
        {
            using (ModeloSuministros contexto = new ModeloSuministros(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                int idSuministro = (int)Suministro;
                System.Data.Objects.ObjectParameter actual = new System.Data.Objects.ObjectParameter("ACTUAL", typeof(Int64));
                SUMpaObtenerSuministrosValorActualNumerador numerador = contexto.paObtenerSuministrosValorActualNumerador_SUM(idSuministro, actual).FirstOrDefault();

                if (numerador == null)
                {
                    throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.MODULO_SUMINISTROS, EnumTipoErrorSuministros.EX_NUM_SUMINISTRO_NO_EXISTE.ToString(), MensajesSuministros.CargarMensaje(EnumTipoErrorSuministros.EX_NUM_SUMINISTRO_NO_EXISTE)));
                }
                else if (numerador.ValorActual == null)
                {
                    throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.MODULO_SUMINISTROS, EnumTipoErrorSuministros.EX_NUM_SUMINISTRO_INVALIDO.ToString(), MensajesSuministros.CargarMensaje(EnumTipoErrorSuministros.EX_NUM_SUMINISTRO_INVALIDO)));
                }
                return new SUNumeradorPrefijo()
                {
                    Prefijo = numerador.Prefijo,
                    ValorActual = numerador.ValorActual.Value
                };
            }
        }

        /// <summary>
        /// Método para obtener suministros a validar propietario
        /// </summary>
        /// <returns></returns>
        public SUSuministro ObtenerSuministro(SUEnumSuministro suministro)
        {
            using (ModeloSuministros contexto = new ModeloSuministros(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                Suministro_SUM sumCon = contexto.Suministro_SUM.Where(sum => sum.SUM_IdSuministro == (int)suministro).FirstOrDefault();
                return new SUSuministro
                {
                    Id = sumCon.SUM_IdSuministro,
                    Descripcion = sumCon.SUM_Descripcion,
                    CodigoERP = sumCon.SUM_CodigoERP,
                    CodigoAlterno = sumCon.SUM_CodigoAlterno,
                    UnidadMedida = new PAUnidadMedidaDC()
                    {
                        IdUnidadMedida = sumCon.SUM_IdUnidadMedida,
                    },
                    EstaActivo = sumCon.SUM_Estado == ConstantesFramework.ESTADO_ACTIVO ? true : false,
                    ValidaPropietario = sumCon.SUM_DebeValidarPropietario
                };
            }
        }


        /// <summary>
        /// Retorna el propietario de una guía  dada
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <returns></returns>
        public SUPropietarioGuia ObtenerPropietarioGuia(long numeroSuministro)
        {
            SUPropietarioGuia propietarioGuia = new SUPropietarioGuia();
            using (ModeloSuministros contexto = new ModeloSuministros(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                paObtenerPropietarioGuia_SUM_Result infoPropietarioSuministro = contexto.paObtenerPropietarioGuia_SUM(numeroSuministro).FirstOrDefault();
                if (infoPropietarioSuministro != null)
                {
                    if (infoPropietarioSuministro.EstadoConsumo == "CON")
                    {
                        // Lanza excepción de que la guía ha sido usada previamente
                        throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.MODULO_SUMINISTROS, ((int)EnumTipoErrorSuministros.EX_GUIA_USADA).ToString(), string.Format(MensajesSuministros.CargarMensaje(EnumTipoErrorSuministros.EX_GUIA_USADA), "", numeroSuministro)));
                    }
                    else if (infoPropietarioSuministro.EstadoConsumo == "ANU")
                    {
                        // Lanza excepción de que la guía ha sido anulada
                        throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.MODULO_SUMINISTROS, ((int)EnumTipoErrorSuministros.EX_SUMINISTRO_ANULADO).ToString(), string.Format(MensajesSuministros.CargarMensaje(EnumTipoErrorSuministros.EX_SUMINISTRO_ANULADO), "", numeroSuministro)));
                    }
                    else if (infoPropietarioSuministro.EstadoConsumo == "NOC")
                    {
                        if (infoPropietarioSuministro.Tipo == "CES")
                        {
                            if (infoPropietarioSuministro.CES_Tipo == "AGE")
                                propietarioGuia.Propietario = SUEnumGrupoSuministroDC.AGE;
                            else if (infoPropietarioSuministro.CES_Tipo == "PTO")
                                propietarioGuia.Propietario = SUEnumGrupoSuministroDC.PTO;
                            else if (infoPropietarioSuministro.CES_Tipo == "RAC")
                                propietarioGuia.Propietario = SUEnumGrupoSuministroDC.RAC;
                            else if (infoPropietarioSuministro.CES_Tipo == "BDG")
                                propietarioGuia.Propietario = SUEnumGrupoSuministroDC.PTO;

                            propietarioGuia.Id = infoPropietarioSuministro.CES_IdCentroServicios.HasValue ? infoPropietarioSuministro.CES_IdCentroServicios.Value : 0;
                            propietarioGuia.Nombre = infoPropietarioSuministro.CES_Nombre;
                            propietarioGuia.CentroServicios = new PUCentroServiciosDC
                            {
                                IdCentroServicio = infoPropietarioSuministro.CES_IdCentroServicios.HasValue ? infoPropietarioSuministro.CES_IdCentroServicios.Value : 0,
                                Tipo = infoPropietarioSuministro.CES_Tipo,
                                Nombre = infoPropietarioSuministro.CES_Nombre,
                                BaseInicialCaja = infoPropietarioSuministro.CES_BaseInicialCaja.HasValue ? infoPropietarioSuministro.CES_BaseInicialCaja.Value : 0,
                                Direccion = infoPropietarioSuministro.CES_Direccion,
                                IdMunicipio = infoPropietarioSuministro.LOC_IdLocalidad,
                                RecibeGiros = infoPropietarioSuministro.CES_PuedeRecibirGiros.HasValue ? infoPropietarioSuministro.CES_PuedeRecibirGiros.Value : false,
                                VendePrepago = infoPropietarioSuministro.CES_VendePrepago.HasValue ? infoPropietarioSuministro.CES_VendePrepago.Value : false,
                                IdTipoPropiedad = infoPropietarioSuministro.CES_IdTipoPropiedad.HasValue ? infoPropietarioSuministro.CES_IdTipoPropiedad.Value : 0,
                                IdCentroCostos = infoPropietarioSuministro.CES_IdCentroCostos,
                                CodigoBodega = infoPropietarioSuministro.CES_CodigoBodega,
                                Telefono1 = infoPropietarioSuministro.CES_Telefono1,
                                Telefono2 = infoPropietarioSuministro.CES_Telefono2,
                                CiudadUbicacion = new PALocalidadDC()
                                {
                                    IdLocalidad = infoPropietarioSuministro.LOC_IdLocalidad,
                                    Nombre = infoPropietarioSuministro.LOC_Nombre,
                                    CodigoPostal = infoPropietarioSuministro.LOC_CodigoPostal
                                }
                            };
                        }

                        else if (infoPropietarioSuministro.Tipo == "CLI")
                        {
                            propietarioGuia.Propietario = SUEnumGrupoSuministroDC.CLI;
                            propietarioGuia.Id = infoPropietarioSuministro.SUS_IdSucursal.HasValue ? infoPropietarioSuministro.SUS_IdSucursal.Value : 0;
                            propietarioGuia.Nombre = infoPropietarioSuministro.SUC_Nombre;
                            propietarioGuia.IdContrato = infoPropietarioSuministro.CON_IdContrato;
                            propietarioGuia.IdListaPrecios = infoPropietarioSuministro.CON_ListaPrecios;
                            propietarioGuia.Cliente = new Servicios.ContratoDatos.Clientes.CLClientesDC
                            {
                                IdCliente = infoPropietarioSuministro.CLI_IdCliente.HasValue ? infoPropietarioSuministro.CLI_IdCliente.Value : 0,
                                Telefono = infoPropietarioSuministro.CLI_Telefono,
                                Direccion = infoPropietarioSuministro.CLI_Direccion,
                                RazonSocial = infoPropietarioSuministro.CLI_RazonSocial,
                                DigitoVerificacion = infoPropietarioSuministro.CLI_DigitoVerificacion.Trim(),
                                Fax = infoPropietarioSuministro.CLI_Fax,
                                FechaConstitucion = infoPropietarioSuministro.CLI_FechaConstitucion.HasValue ? infoPropietarioSuministro.CLI_FechaConstitucion.Value : DateTime.Now,
                                FechaVinculacion = infoPropietarioSuministro.CLI_FechaVinculacion.HasValue ? infoPropietarioSuministro.CLI_FechaVinculacion.Value : DateTime.Now,
                                IdRepresentanteLegal = infoPropietarioSuministro.CLI_IdRepresentanteLegal.HasValue ? infoPropietarioSuministro.CLI_IdRepresentanteLegal.Value : 0,
                                Localidad = infoPropietarioSuministro.CLI_Municipio,
                                Nit = infoPropietarioSuministro.CLI_Nit,
                                NombreGerente = infoPropietarioSuministro.CLI_NombreGerenteGeneral,
                                Estado = infoPropietarioSuministro.CLI_Estado
                            };
                            propietarioGuia.CiudadSucursal = new PALocalidadDC() { IdLocalidad = infoPropietarioSuministro.SUC_Municipio };
                            propietarioGuia.CentroServicios = new PUCentroServiciosDC
                            {
                                IdCentroServicio = infoPropietarioSuministro.CES_IdCentroServicios.HasValue ? infoPropietarioSuministro.CES_IdCentroServicios.Value : 0,
                                Tipo = infoPropietarioSuministro.CES_Tipo,
                                Nombre = infoPropietarioSuministro.CES_Nombre,
                                BaseInicialCaja = infoPropietarioSuministro.CES_BaseInicialCaja.HasValue ? infoPropietarioSuministro.CES_BaseInicialCaja.Value : 0,
                                Direccion = infoPropietarioSuministro.CES_Direccion,
                                IdMunicipio = infoPropietarioSuministro.LOC_IdLocalidad,
                                RecibeGiros = infoPropietarioSuministro.CES_PuedeRecibirGiros.HasValue ? infoPropietarioSuministro.CES_PuedeRecibirGiros.Value : false,
                                VendePrepago = infoPropietarioSuministro.CES_VendePrepago.HasValue ? infoPropietarioSuministro.CES_VendePrepago.Value : false,
                                IdTipoPropiedad = infoPropietarioSuministro.CES_IdTipoPropiedad.HasValue ? infoPropietarioSuministro.CES_IdTipoPropiedad.Value : 0,
                                IdCentroCostos = infoPropietarioSuministro.CES_IdCentroCostos,
                                CodigoBodega = infoPropietarioSuministro.CES_CodigoBodega,
                                Telefono1 = infoPropietarioSuministro.CES_Telefono1,
                                Telefono2 = infoPropietarioSuministro.CES_Telefono2,
                                CiudadUbicacion = new PALocalidadDC()
                                {
                                    IdLocalidad = infoPropietarioSuministro.LOC_IdLocalidad,
                                    Nombre = infoPropietarioSuministro.LOC_Nombre,
                                    CodigoPostal = infoPropietarioSuministro.LOC_CodigoPostal
                                }
                            };

                            if (propietarioGuia.Cliente.Estado == ConstantesFramework.ESTADO_INACTIVO)
                                throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.MODULO_SUMINISTROS, ((int)EnumTipoErrorSuministros.EX_ERROR_CLIENTE_INACTIVO).ToString(), MensajesSuministros.CargarMensaje(EnumTipoErrorSuministros.EX_ERROR_CLIENTE_INACTIVO)));

                        }

                        else if (infoPropietarioSuministro.Tipo == "MEN")
                        {
                            propietarioGuia.Propietario = SUEnumGrupoSuministroDC.MEN;
                            propietarioGuia.Id = infoPropietarioSuministro.SME_IdMensajero.HasValue ? infoPropietarioSuministro.SME_IdMensajero.Value : 0;
                            propietarioGuia.CedulaMensajero = infoPropietarioSuministro.PEI_Identificacion;
                            propietarioGuia.Nombre = infoPropietarioSuministro.NombreMensajero;
                            propietarioGuia.CentroServicios = new PUCentroServiciosDC
                            {
                                IdCentroServicio = infoPropietarioSuministro.CES_IdCentroServicios.HasValue ? infoPropietarioSuministro.CES_IdCentroServicios.Value : 0,
                                Tipo = infoPropietarioSuministro.CES_Tipo,
                                Nombre = infoPropietarioSuministro.CES_Nombre,
                                BaseInicialCaja = infoPropietarioSuministro.CES_BaseInicialCaja.HasValue ? infoPropietarioSuministro.CES_BaseInicialCaja.Value : 0,
                                Direccion = infoPropietarioSuministro.CES_Direccion,
                                IdMunicipio = infoPropietarioSuministro.LOC_IdLocalidad,
                                RecibeGiros = infoPropietarioSuministro.CES_PuedeRecibirGiros.HasValue ? infoPropietarioSuministro.CES_PuedeRecibirGiros.Value : false,
                                VendePrepago = infoPropietarioSuministro.CES_VendePrepago.HasValue ? infoPropietarioSuministro.CES_VendePrepago.Value : false,
                                IdTipoPropiedad = infoPropietarioSuministro.CES_IdTipoPropiedad.HasValue ? infoPropietarioSuministro.CES_IdTipoPropiedad.Value : 0,
                                IdCentroCostos = infoPropietarioSuministro.CES_IdCentroCostos,
                                CodigoBodega = infoPropietarioSuministro.CES_CodigoBodega,
                                Telefono1 = infoPropietarioSuministro.CES_Telefono1,
                                Telefono2 = infoPropietarioSuministro.CES_Telefono2,
                                CiudadUbicacion = new PALocalidadDC()
                                {
                                    IdLocalidad = infoPropietarioSuministro.LOC_IdLocalidad,
                                    Nombre = infoPropietarioSuministro.LOC_Nombre,
                                    CodigoPostal = infoPropietarioSuministro.LOC_CodigoPostal
                                }
                            };
                        }


                        if (infoPropietarioSuministro.LOC_IdAncestroTercerGrado != null)
                        {
                            propietarioGuia.CentroServicios.IdPais = infoPropietarioSuministro.LOC_IdAncestroTercerGrado;
                            propietarioGuia.CentroServicios.NombrePais = infoPropietarioSuministro.LOC_NombreTercero;
                        }
                        else if (infoPropietarioSuministro.LOC_IdAncestroSegundoGrado != null)
                        {
                            propietarioGuia.CentroServicios.IdPais = infoPropietarioSuministro.LOC_IdAncestroSegundoGrado;
                            propietarioGuia.CentroServicios.NombrePais = infoPropietarioSuministro.LOC_NombreSegundo;
                        }
                        else if (infoPropietarioSuministro.LOC_IdAncestroPrimerGrado != null)
                        {
                            propietarioGuia.CentroServicios.IdPais = infoPropietarioSuministro.LOC_IdAncestroPrimerGrado;
                            propietarioGuia.CentroServicios.NombrePais = infoPropietarioSuministro.LOC_NombrePrimero;
                        }

                    }

                }
                return propietarioGuia;
            }
        }

        /// <summary>
        /// Retorna el propietario de una guía  dada
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <param name="idSucursalCentroServicio">Número de la sucursal o centro de servicio solicitante</param>
        /// <returns></returns>
        public SUPropietarioGuia ObtenerPropietarioSuministro(long numeroSuministro, SUEnumSuministro idSuministro)
        {
            using (ModeloSuministros contexto = new ModeloSuministros(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                paObtenerPropietarioSuministro_SUM1_Result infoPropietarioSuministro = contexto.paObtenerPropietarioSuministro_SUM((int)idSuministro, numeroSuministro).FirstOrDefault();

                if (infoPropietarioSuministro != null && infoPropietarioSuministro.EstadoConsumo != null
                    && infoPropietarioSuministro.EstadoConsumo != "CON" && infoPropietarioSuministro.EstadoConsumo != "ANU")
                {
                    // Retorne el tipo de Propietario
                    if (infoPropietarioSuministro.IdCentroServicios.HasValue)
                    {
                        return new SUPropietarioGuia() { Propietario = (SUEnumGrupoSuministroDC)Enum.Parse(typeof(SUEnumGrupoSuministroDC), infoPropietarioSuministro.TipoCentroServicios), Id = infoPropietarioSuministro.IdCentroServicios.Value };
                    }
                    if (infoPropietarioSuministro.IdMensajero.HasValue)
                    {
                        return new SUPropietarioGuia() { Propietario = SUEnumGrupoSuministroDC.MEN, Id = infoPropietarioSuministro.IdMensajero.Value };
                    }
                    if (infoPropietarioSuministro.IdSucursal.HasValue)
                    {
                        return new SUPropietarioGuia() { Propietario = SUEnumGrupoSuministroDC.CLI, Id = infoPropietarioSuministro.IdSucursal.Value, IdContrato = infoPropietarioSuministro.IdContrato, IdListaPrecios = infoPropietarioSuministro.IdListaPrecios, ContratoAplicaValidacionPesoAdm = infoPropietarioSuministro.ContAplicaValPesoAdm };
                    }
                    if (infoPropietarioSuministro.IdProceso.HasValue)
                    {
                        return new SUPropietarioGuia() { Propietario = SUEnumGrupoSuministroDC.PRO, Id = infoPropietarioSuministro.IdProceso.Value };
                    }
                    throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.MODULO_SUMINISTROS, ((int)EnumTipoErrorSuministros.EX_GUIA_SIN_ASIGNAR).ToString(), string.Format(MensajesSuministros.CargarMensaje(EnumTipoErrorSuministros.EX_GUIA_SIN_ASIGNAR), idSuministro.ToString(), numeroSuministro)));
                }
                else if (infoPropietarioSuministro != null && infoPropietarioSuministro.EstadoConsumo != null && infoPropietarioSuministro.EstadoConsumo == "CON")
                {
                    // Lanza excepción de que la guía ha sido usada previamente
                    throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.MODULO_SUMINISTROS, ((int)EnumTipoErrorSuministros.EX_GUIA_USADA).ToString(), string.Format(MensajesSuministros.CargarMensaje(EnumTipoErrorSuministros.EX_GUIA_USADA), idSuministro.ToString(), numeroSuministro)));
                }
                else if (infoPropietarioSuministro != null && infoPropietarioSuministro.EstadoConsumo != null && infoPropietarioSuministro.EstadoConsumo == "ANU")
                {
                    // Lanza excepción de que el suministro esta anulado
                    throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.MODULO_SUMINISTROS, ((int)EnumTipoErrorSuministros.EX_SUMINISTRO_ANULADO).ToString(), string.Format(MensajesSuministros.CargarMensaje(EnumTipoErrorSuministros.EX_SUMINISTRO_ANULADO), idSuministro.ToString(), numeroSuministro)));
                }
                else
                {
                    // Lanza excepción de que la guía no ha sido usada pero no se encuentra a quién está asignada
                    throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.MODULO_SUMINISTROS, ((int)EnumTipoErrorSuministros.EX_GUIA_SIN_ASIGNAR).ToString(), string.Format(MensajesSuministros.CargarMensaje(EnumTipoErrorSuministros.EX_GUIA_SIN_ASIGNAR), idSuministro.ToString(), numeroSuministro)));
                }
            }
        }



        /// <summary>
        /// Retorna el propietario de una guía  dada
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <param name="idSucursalCentroServicio">Número de la sucursal o centro de servicio solicitante</param>
        /// <returns></returns>
        public SUPropietarioGuia ObtenerPropietarioSuministroSinValidar(long numeroSuministro, SUEnumSuministro idSuministro)
        {
            using (ModeloSuministros contexto = new ModeloSuministros(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                paObtenerPropietarioSuministro_SUM1_Result infoPropietarioSuministro = contexto.paObtenerPropietarioSuministro_SUM((int)idSuministro, numeroSuministro).FirstOrDefault();

                if (infoPropietarioSuministro != null && infoPropietarioSuministro.EstadoConsumo != null
                    && infoPropietarioSuministro.EstadoConsumo != "CON" && infoPropietarioSuministro.EstadoConsumo != "ANU")
                {
                    // Retorne el tipo de Propietario
                    if (infoPropietarioSuministro.IdCentroServicios.HasValue)
                    {
                        return new SUPropietarioGuia() { Propietario = (SUEnumGrupoSuministroDC)Enum.Parse(typeof(SUEnumGrupoSuministroDC), infoPropietarioSuministro.TipoCentroServicios), Id = infoPropietarioSuministro.IdCentroServicios.Value };
                    }
                    else if (infoPropietarioSuministro.IdMensajero.HasValue)
                    {
                        return new SUPropietarioGuia() { Propietario = SUEnumGrupoSuministroDC.MEN, Id = infoPropietarioSuministro.IdMensajero.Value };
                    }
                    else if (infoPropietarioSuministro.IdSucursal.HasValue)
                    {
                        return new SUPropietarioGuia() { Propietario = SUEnumGrupoSuministroDC.CLI, Id = infoPropietarioSuministro.IdSucursal.Value, IdContrato = infoPropietarioSuministro.IdContrato, IdListaPrecios = infoPropietarioSuministro.IdListaPrecios, ContratoAplicaValidacionPesoAdm = infoPropietarioSuministro.ContAplicaValPesoAdm };
                    }
                    else if (infoPropietarioSuministro.IdProceso.HasValue)
                    {
                        return new SUPropietarioGuia() { Propietario = SUEnumGrupoSuministroDC.PRO, Id = infoPropietarioSuministro.IdProceso.Value };
                    }
                    else
                        return new SUPropietarioGuia { Propietario = SUEnumGrupoSuministroDC.AGE, Id = 0 };
                }
                else if (infoPropietarioSuministro != null && infoPropietarioSuministro.EstadoConsumo != null && infoPropietarioSuministro.EstadoConsumo == "CON")
                {
                    // Lanza excepción de que la guía ha sido usada previamente
                    throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.MODULO_SUMINISTROS, ((int)EnumTipoErrorSuministros.EX_GUIA_USADA).ToString(), string.Format(MensajesSuministros.CargarMensaje(EnumTipoErrorSuministros.EX_GUIA_USADA), idSuministro.ToString(), numeroSuministro)));
                }
                else if (infoPropietarioSuministro != null && infoPropietarioSuministro.EstadoConsumo != null && infoPropietarioSuministro.EstadoConsumo == "ANU")
                {
                    // Lanza excepción de que el suministro esta anulado
                    throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.MODULO_SUMINISTROS, ((int)EnumTipoErrorSuministros.EX_SUMINISTRO_ANULADO).ToString(), string.Format(MensajesSuministros.CargarMensaje(EnumTipoErrorSuministros.EX_SUMINISTRO_ANULADO), idSuministro.ToString(), numeroSuministro)));
                }
                else
                    return new SUPropietarioGuia { Propietario = SUEnumGrupoSuministroDC.AGE, Id = 0 };
            }
        }



        /// <summary>
        /// Almacena el consumo de un suministro en la base de datos
        /// </summary>
        /// <param name="consumoSuministro"></param>
        /// <param name="datosCentroServicio"></param>
        /// <param name="datosServicio"></param>
        public void GuardarConsumoSuministro(SUConsumoSuministroDC consumoSuministro)
        {
            using (ModeloSuministros contexto = new ModeloSuministros(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                contexto.paCrearConsumoSuministro_SUM((int)consumoSuministro.Suministro, consumoSuministro.GrupoSuministro.ToString(), consumoSuministro.IdDuenoSuministro,
                  consumoSuministro.NumeroSuministro, consumoSuministro.Cantidad, consumoSuministro.EstadoConsumo.ToString(), consumoSuministro.IdServicioAsociado, ControllerContext.Current.Usuario);

                contexto.SaveChanges();
            }
        }


        /// <summary>
        /// Almacena el consumo de un suministro en la base de datos
        /// </summary>
        /// <param name="consumoSuministro"></param>
        /// <param name="datosCentroServicio"></param>
        /// <param name="datosServicio"></param>
        /// /// <param name="conexion"> conexion principal</param>
        /// /// <param name="transaccion">transaccion principal</param>
        public void GuardarConsumoSuministro(SUConsumoSuministroDC consumoSuministro, SqlConnection conexion, SqlTransaction transaccion)
        {

            SqlCommand cmd = new SqlCommand("paCrearConsumoSuministro_SUM", conexion, transaccion);
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.Parameters.Add(Utilidades.AddParametro("@IdSuministro", (int)consumoSuministro.Suministro));
            cmd.Parameters.Add(Utilidades.AddParametro("@IdGrupoSuministro", consumoSuministro.GrupoSuministro.ToString()));
            cmd.Parameters.Add(Utilidades.AddParametro("@IdPropietarioSuministro", consumoSuministro.IdDuenoSuministro));
            cmd.Parameters.Add(Utilidades.AddParametro("@NumeroSuministro", consumoSuministro.NumeroSuministro));
            cmd.Parameters.Add(Utilidades.AddParametro("@CantidadConsumida", consumoSuministro.Cantidad));
            cmd.Parameters.Add(Utilidades.AddParametro("@EstadoConsumo", consumoSuministro.EstadoConsumo.ToString()));
            cmd.Parameters.Add(Utilidades.AddParametro("@IdServicio", consumoSuministro.IdServicioAsociado));
            cmd.Parameters.Add(Utilidades.AddParametro("@CreadoPor", ControllerContext.Current.Usuario));
            cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// Almacena el traslado de un suministro entre un origen y un destino
        /// </summary>
        /// <param name="trasladoSuministro"></param>
        public void GuardarTrasladoSuministro(SUTrasladoSuministroDC trasladoSuministro)
        {
            using (ModeloSuministros contexto = new ModeloSuministros(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                contexto.paCrearTrasladoSuministro_SUM((int)trasladoSuministro.Suministro, trasladoSuministro.NumeroSuministro, trasladoSuministro.Cantidad,
                  trasladoSuministro.GrupoSuministroOrigen.ToString(), trasladoSuministro.IdentificacionOrigen, trasladoSuministro.GrupoSuministroDestino.ToString(),
                  trasladoSuministro.IdentificacionDestino, ControllerContext.Current.Usuario);

                contexto.SaveChanges();
            }
        }

        /// <summary>
        /// Reasigna los suministros de una agencia a otra
        /// </summary>
        /// <param name="anteriorAgencia"></param>
        /// <param name="nuevaAgencia"></param>
        public void ModificarSuministroAgencia(long anteriorAgencia, long nuevaAgencia, SUEnumGrupoSuministroDC grupoSumAnterior, SUEnumGrupoSuministroDC grupoSumNuevo)
        {

            using (ModeloSuministros contexto = new ModeloSuministros(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                contexto.SuministrosCentroServicios_SUM.Where(s => s.SCS_IdCentroServicios == anteriorAgencia).ToList()
                  .ForEach(s =>
                  {
                      SUAudit.Instancia.AuditarSuministroAgencia(contexto, s.SCS_IdSuministroCentroServicio, anteriorAgencia, nuevaAgencia, s.SCS_IdSuministro, grupoSumAnterior, grupoSumNuevo);

                      s.SCS_IdCentroServicios = nuevaAgencia;
                  });
                contexto.SaveChanges();
            }

        }

        /// <summary>
        /// Obtiene los suministros de una sucursal
        /// </summary>
        /// <param name="idSucursal"></param>
        /// <returns></returns>
        public List<SUSuministroSucursalDC> ObtenerSuministrosSucursal(int idSucursal)
        {
            using (ModeloSuministros contexto = new ModeloSuministros(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.SuministroSucursal_VSUM.Where(s => s.SUS_Estado == ConstantesFramework.ESTADO_ACTIVO && s.SUM_Estado == ConstantesFramework.ESTADO_ACTIVO && s.SUS_IdSucursal == idSucursal)
                  .ToList()
                  .ConvertAll<SUSuministroSucursalDC>(s =>
                    new SUSuministroSucursalDC()
                    {
                        CantidadInicialAutorizada = s.SUS_CantidadInicialAutorizada,
                        IdSucursal = s.SUS_IdSucursal,
                        IdSuministro = s.SUS_IdSuministro,
                        IdSuministroSucursal = s.SUS_IdSuministroSucursal,
                        StockMinimo = s.SUS_StockMinimo,
                        Suministro = new SUSuministro()
                        {
                            Id = s.SUS_IdSuministro,
                            CodigoERP = s.SUM_CodigoERP,
                            Descripcion = s.SUM_Descripcion,
                            UnidadMedida = new Framework.Servidor.Servicios.ContratoDatos.Parametros.PAUnidadMedidaDC()
                            {
                                IdUnidadMedida = s.SUM_IdUnidadMedida,
                                Descripcion = s.UNM_Descripcion
                            },
                            Categoria = (SUEnumCategoria)s.SUM_IdCategoria
                        }
                    });
            }
        }

        /// <summary>
        /// agrega o modifica un suministro de una sucursal
        /// </summary>
        /// <param name="sumSuc"></param>
        public void AgregarModificarSuministroSucursal(SUSuministroSucursalDC sumSuc)
        {
            using (ModeloSuministros contexto = new ModeloSuministros(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                SuministrosSucursal_SUM suministroSuc = null;

                suministroSuc = contexto.SuministrosSucursal_SUM.Where(s => s.SUS_IdSuministro == sumSuc.Suministro.Id && sumSuc.IdSucursal == s.SUS_IdSucursal).FirstOrDefault();
                if (suministroSuc != null)
                {
                    suministroSuc.SUS_CantidadInicialAutorizada = sumSuc.CantidadInicialAutorizada;
                    if (sumSuc.Suministro.SuministroAutorizado)
                        suministroSuc.SUS_Estado = ConstantesFramework.ESTADO_ACTIVO;
                    else
                        suministroSuc.SUS_Estado = ConstantesFramework.ESTADO_INACTIVO;

                    suministroSuc.SUS_StockMinimo = sumSuc.StockMinimo;
                    contexto.SaveChanges();
                }
                else
                {
                    suministroSuc = new SuministrosSucursal_SUM()
                    {
                        SUS_CantidadInicialAutorizada = sumSuc.CantidadInicialAutorizada,
                        SUS_CreadoPor = ControllerContext.Current.Usuario,
                        SUS_Estado = ConstantesFramework.ESTADO_ACTIVO,
                        SUS_FechaGrabacion = DateTime.Now,
                        SUS_IdSucursal = sumSuc.IdSucursal,
                        SUS_IdSuministro = sumSuc.Suministro.Id,
                        SUS_StockMinimo = sumSuc.StockMinimo
                    };

                    if (sumSuc.Suministro.SuministroAutorizado)
                        suministroSuc.SUS_Estado = ConstantesFramework.ESTADO_ACTIVO;
                    else
                        suministroSuc.SUS_Estado = ConstantesFramework.ESTADO_INACTIVO;

                    contexto.SuministrosSucursal_SUM.Add(suministroSuc);
                    contexto.SaveChanges();
                }
            }
        }

        /// <summary>
        /// Obtiene los suministros que no estan en el grupo seleccionado ni asignados en la sucursal
        /// </summary>
        /// <param name="idGrupo">id del grupo</param>
        /// <returns>Lista de suministro</returns>
        public List<SUSuministro> ObtenerSuministrosSucursalNoIncluidosEnGrupo(string idGrupo, int idSucursal)
        {
            using (ModeloSuministros contexto = new ModeloSuministros(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.paObtenerSumSucursalNoGrupoSuministro_SUM(idGrupo, idSucursal).ToList()
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

        /// <summary>
        /// Obtiene los suministros de un proceso
        /// </summary>
        /// <param name="idSucursal"></param>
        /// <returns></returns>
        public List<SUSuministrosProcesoDC> ObtenerSuministrosProceso(long codProceso)
        {
            using (ModeloSuministros contexto = new ModeloSuministros(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return
                  contexto.SuministrosProceso_VSUM
                  .Where(s => s.SUP_Estado == ConstantesFramework.ESTADO_ACTIVO && s.SUM_Estado == ConstantesFramework.ESTADO_ACTIVO && s.SUP_CodigoProceso == codProceso)
                  .ToList()
                  .ConvertAll<SUSuministrosProcesoDC>(s =>
                    new SUSuministrosProcesoDC()
                    {
                        CantidadInicialAutorizada = s.SUP_CantidadInicialAutorizada,
                        CodProceso = s.SUP_CodigoProceso,
                        IdSuministro = s.SUM_IdSuministro,
                        IdSuministroProceso = s.SUP_IdSuministroProceso,
                        StockMinimo = s.SUP_StockMinimo,
                        Suministro = new SUSuministro()
                        {
                            Id = s.SUM_IdSuministro,
                            CodigoERP = s.SUM_CodigoERP,
                            Descripcion = s.SUM_Descripcion,

                            AplicaResolucion = s.SUM_AplicaResolucion,
                            UnidadMedida = new Framework.Servidor.Servicios.ContratoDatos.Parametros.PAUnidadMedidaDC()
                            {
                                IdUnidadMedida = s.SUM_IdUnidadMedida,
                                Descripcion = s.UNM_Descripcion
                            },
                            Categoria = (SUEnumCategoria)s.SUM_IdCategoria
                        }
                    });
            }
        }

        /// <summary>
        /// Obtiene los suministros aprabados para realizar la remision al proceso
        /// </summary>
        /// <param name="idProceso"></param>
        /// <returns></returns>
        public List<SUSuministro> ObtenerSuministrosAsignadosProceso(long idProceso)
        {
            using (ModeloSuministros contexto = new ModeloSuministros(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.SuministrosProceso_SUM.Include("Suministro_SUM.UnidadMedida_PAR")
                  .Where(r => r.SUP_CodigoProceso == idProceso && r.SUP_Estado == ConstantesFramework.ESTADO_ACTIVO).ToList()
                  .ConvertAll(r => new SUSuministro()
                  {
                      Id = r.SUP_IdSuministro,
                      IdAsignacionSuministro = r.SUP_IdSuministroProceso,
                      Descripcion = r.Suministro_SUM.SUM_Descripcion,
                      UnidadMedida = new PAUnidadMedidaDC()
                      {
                          IdUnidadMedida = r.Suministro_SUM.UnidadMedida_PAR.UNM_IdUnidadMedida,
                          Descripcion = r.Suministro_SUM.UnidadMedida_PAR.UNM_Descripcion,
                      },
                      CantidadInicialAutorizada = r.SUP_CantidadInicialAutorizada,
                      AplicaResolucion = r.Suministro_SUM.SUM_AplicaResolucion,
                      CodigoAlterno = r.Suministro_SUM.SUM_CodigoAlterno,
                      CodigoERP = r.Suministro_SUM.SUM_CodigoERP,
                      CategoriaSuministro = new SUCategoriaSuministro()
                      {
                          IdCategoria = r.Suministro_SUM.SUM_IdCategoria
                      },
                      CuentaGasto = r.Suministro_SUM.SUM_CuentaGasto,
                      Rango = new SURango()
                  });
            }
        }

        /// <summary>
        /// agrega o modifica un suministro de un proceso
        /// </summary>
        /// <param name="sumPro"></param>
        public void AgregarModificarSuministroProceso(SUSuministrosProcesoDC sumPro)
        {
            using (ModeloSuministros contexto = new ModeloSuministros(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                SuministrosProceso_SUM suministroPro = null;

                suministroPro = contexto.SuministrosProceso_SUM.Where(s => s.SUP_IdSuministro == sumPro.Suministro.Id && sumPro.CodProceso == s.SUP_CodigoProceso).FirstOrDefault();
                if (suministroPro != null)
                {
                    suministroPro.SUP_CantidadInicialAutorizada = sumPro.CantidadInicialAutorizada;
                    if (sumPro.Suministro.SuministroAutorizado)
                        suministroPro.SUP_Estado = ConstantesFramework.ESTADO_ACTIVO;
                    else
                        suministroPro.SUP_Estado = ConstantesFramework.ESTADO_INACTIVO;

                    suministroPro.SUP_StockMinimo = sumPro.StockMinimo;
                    contexto.SaveChanges();
                }
                else
                {
                    suministroPro = new SuministrosProceso_SUM()
                    {
                        SUP_CantidadInicialAutorizada = sumPro.CantidadInicialAutorizada,
                        SUP_CreadoPor = ControllerContext.Current.Usuario,
                        SUP_Estado = ConstantesFramework.ESTADO_ACTIVO,
                        SUP_FechaGrabacion = DateTime.Now,
                        SUP_CodigoProceso = sumPro.CodProceso,
                        SUP_IdSuministro = sumPro.Suministro.Id,
                        SUP_StockMinimo = sumPro.StockMinimo
                    };

                    if (sumPro.Suministro.SuministroAutorizado)
                        suministroPro.SUP_Estado = ConstantesFramework.ESTADO_ACTIVO;
                    else
                        suministroPro.SUP_Estado = ConstantesFramework.ESTADO_INACTIVO;

                    contexto.SuministrosProceso_SUM.Add(suministroPro);
                    contexto.SaveChanges();
                }
            }
        }

        /// <summary>
        /// Obtiene los suministros que no están en el grupo seleccionado ni asignados a un proceso
        /// </summary>
        /// <param name="idGrupo">id del grupo</param>
        /// <returns>Lista de suministro</returns>
        public List<SUSuministro> ObtenerSuministrosProcesoNoIncluidosEnGrupo(string idGrupo, long CodProceso, Dictionary<string, string> filtro)
        {
            using (ModeloSuministros contexto = new ModeloSuministros(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                var resultado = contexto.paObtenerSumProcesoNoGrupoSuministro_SUM(idGrupo, CodProceso).ToList()
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
                       EstaActivo = r.SUM_Estado == ConstantesFramework.ESTADO_ACTIVO ? true : false,
                   });

                if (filtro.ContainsKey("SUM_CodigoERP") && !filtro.ContainsKey("SUM_Descripcion"))
                {
                    string codERP = filtro["SUM_CodigoERP"];
                    if (!string.IsNullOrWhiteSpace(codERP))
                        resultado = resultado.Where(o => o.CodigoERP.Contains(codERP)).ToList();
                }
                if (filtro.ContainsKey("SUM_Descripcion") && !filtro.ContainsKey("SUM_CodigoERP"))
                {
                    string descrip = filtro["SUM_Descripcion"];
                    if (!string.IsNullOrWhiteSpace(descrip))
                        resultado = resultado.Where(o => o.Descripcion.ToUpper().Contains(descrip.ToUpper())).ToList();
                }

                if (filtro.ContainsKey("SUM_Descripcion") && filtro.ContainsKey("SUM_CodigoERP"))
                {
                    string descrip = filtro["SUM_Descripcion"];
                    string codERP = filtro["SUM_CodigoERP"];
                    if (!string.IsNullOrWhiteSpace(descrip))
                        resultado = resultado.Where(o => o.Descripcion.ToUpper().Contains(descrip.ToUpper()) && o.CodigoERP.Contains(codERP)).ToList();
                }

                return resultado;
            }
        }

        /// <summary>
        /// Guarda la remision del suministro
        /// </summary>
        public long GuardaRemisionSuministroProceso(SURemisionSuministroDC remision)
        {
            using (ModeloSuministros contexto = new ModeloSuministros(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                RemisionSuministros_SUM remisionEn = new RemisionSuministros_SUM()
                {
                    RES_NombreDestinatario = remision.ProcesoAsignacion.Descripcion,
                    RES_IdLocalidadDestino = remision.CasaMatrizDestinoRemision.IdLocalidad,
                    RES_IdAdminisionMensajeriaDespacho = remision.IdGuiaInternaRemision,
                    RES_NumeroGuiaInternaDespacho = remision.NumeroGuiaDespacho,
                    RES_IdGrupoSuministroDestinatario = SUEnumGrupoSuministroDC.PRO.ToString(),
                    RES_FechaGrabacion = DateTime.Now,
                    RES_CreadoPor = ControllerContext.Current.Usuario,
                    RES_IdCasaMatrizElabora = (short)remision.IdCasaMatriz,
                    RES_Estado = remision.Estado.ToString()
                };

                contexto.RemisionSuministros_SUM.Add(remisionEn);
                contexto.SaveChanges();
                return remisionEn.RES_IdRemisionSuministros;
            }
        }

        /// <summary>
        /// guarda la provision del suministro del proceso
        /// </summary>
        /// <param name="remision"></param>
        public long GuardarProvisionSuministrosProceso(SUSuministro suministro, long idRemision, int idAsignacion)
        {
            using (ModeloSuministros contexto = new ModeloSuministros(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                ProvisionSumProceso_SUM sumProceso = new ProvisionSumProceso_SUM()
                {
                    PSP_IdRemisionSuministros = idRemision,
                    PSP_IdSuministroProceso = idAsignacion,
                    PSP_CantidadAsginada = suministro.CantidadAsignada,
                    PSP_FechaGrabacion = DateTime.Now,
                    PSP_CreadoPor = ControllerContext.Current.Usuario
                };
                contexto.ProvisionSumProceso_SUM.Add(sumProceso);
                contexto.SaveChanges();
                return sumProceso.PSP_IdProvisionSumProceso;
            }
        }

        /// <summary>
        /// Guarda la provision de suministros del proceso
        /// </summary>
        /// <param name="suministro"></param>
        /// <param name="idProvision"></param>
        public void GuardarProvisionSuministroSerialProceso(SUSuministro suministro, long idProvision, long rangoInicial, long rangoFinal, bool esModificacion)
        {
            using (ModeloSuministros contexto = new ModeloSuministros(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                ProvisionSumProcesoSerial_SUM provisionSerial = new ProvisionSumProcesoSerial_SUM()
                {
                    PPS_Inicio = rangoInicial,
                    PPS_Fin = rangoFinal,
                    PPS_IdProvisionSumProceso = idProvision,
                    PPS_FechaGrabacion = DateTime.Now,
                    PPS_CreadoPor = ControllerContext.Current.Usuario,
                    PPS_FechaFinal = suministro.FechaFinalResolucion,
                    PPS_FechaInicial = suministro.FechaInicialResolucion,
                    PPS_IdNumerador = suministro.IdResolucion,
                    PPS_Prefijo = string.Empty,
                    PPS_PorModificacion = esModificacion
                };

                contexto.ProvisionSumProcesoSerial_SUM.Add(provisionSerial);
                contexto.SaveChanges();
            }
        }

        #region Suministros referencia

        /// <summary>
        ///  Guardar la referencia del suministro provisionado para un canal de venta
        /// </summary>
        /// <param name="remision">Objeto con la información de la remisión</param>
        /// <param name="traslado">Objeto con la información del traslado</param>
        public void GuardaSuministroProvisionReferenciaCanalVenta(SURemisionSuministroDC remision, SUTrasladoSuministroDC traslado)
        {
            List<SuministroProvisionReferencia_SUM> referencias = new List<SuministroProvisionReferencia_SUM>();

            for (int i = 0; i < remision.GrupoSuministros.SuministroGrupo.CantidadAsignada; i++)
            {
                SuministroProvisionReferencia_SUM referencia = new SuministroProvisionReferencia_SUM()
                {
                    //SPR_CodigoBodegaAprovisionado = remision.CentroServicioAsignacion.CodigoBodega,
                    //SPR_IdCentroCostosAprovisionado = remision.CentroServicioAsignacion.IdCentroCostos,
                    SPR_IdSuministro = remision.GrupoSuministros.SuministroGrupo.Id,
                    SPR_IdGrupoSuministro = remision.GrupoSuministroDestino.ToString(),
                    SPR_IdPropietarioSuministro = remision.CentroServicioAsignacion.IdCentroServicio,
                    SPR_NumeroSuministro = remision.GrupoSuministros.SuministroGrupo.RangoInicial + i,
                    SPR_DescripcionGrupo = remision.GrupoSuministros.Descripcion,
                    SPR_FechaGrabacion = DateTime.Now,
                    SPR_CreadoPor = ControllerContext.Current.Usuario
                };

                referencias.Add(referencia);
            }

            //guardar el traslado del suministro, como son varios se inserta un solo registro con el numero de suminstro en 0 y la cantidad trasladada
            //traslado.NumeroSuministro = 0;
            //traslado.Cantidad = remision.GrupoSuministros.SuministroGrupo.CantidadAsignada;
            //SURepositorio.Instancia.GuardarTrasladoSuministro(traslado);

            // guardar el uno a uno de los suministros aprovisionados en la tabla de referencia
            GuardarSuministrosReferenciaBulk(referencias, NombreModelo);
        }

        /// <summary>
        ///  Guardar la referencia del suministro provisionado para una sucursal
        /// </summary>
        /// <param name="remision">Objeto con la información de la remisión</param>
        /// <param name="traslado">Objeto con la información del traslado</param>
        public void GuardaSuministroProvisionReferenciaSucursal(SURemisionSuministroDC remision, SUTrasladoSuministroDC traslado)
        {
            List<SuministroProvisionReferencia_SUM> referencias = new List<SuministroProvisionReferencia_SUM>();

            for (int i = 0; i < remision.GrupoSuministros.SuministroGrupo.CantidadAsignada; i++)
            {
                SuministroProvisionReferencia_SUM referencia = new SuministroProvisionReferencia_SUM()
                {
                    //SPR_CodigoBodegaAprovisionado = remision.CentroServicioAsignacion.CodigoBodega,
                    //SPR_IdCentroCostosAprovisionado = remision.CentroServicioAsignacion.IdCentroCostos,
                    SPR_IdSuministro = remision.GrupoSuministros.SuministroGrupo.Id,
                    SPR_IdGrupoSuministro = SUEnumGrupoSuministroDC.CLI.ToString(),
                    SPR_IdPropietarioSuministro = remision.Sucursal.IdSucursal,
                    SPR_NumeroSuministro = remision.GrupoSuministros.SuministroGrupo.RangoInicial + i,
                    SPR_DescripcionGrupo = remision.GrupoSuministros.Descripcion,
                    SPR_FechaGrabacion = DateTime.Now,
                    SPR_CreadoPor = ControllerContext.Current.Usuario,
                };

                referencias.Add(referencia);
            }

            //guardar el traslado del suministro, como son varios se inserta un solo registro con el numero de suminstro en 0 y la cantidad trasladada
            traslado.NumeroSuministro = 0;
            traslado.Cantidad = remision.GrupoSuministros.SuministroGrupo.CantidadAsignada;
            SURepositorio.Instancia.GuardarTrasladoSuministro(traslado);

            // guardar el uno a uno de los suministros aprovisionados en la tabla de referencia
            GuardarSuministrosReferenciaBulk(referencias, NombreModelo);
        }

        /// <summary>
        /// Guardar la referencia del suministro provisionado para mensajeros
        /// </summary>
        /// <param name="remision">Información de la remisión</param>
        /// <param name="traslado">Información del traslado</param>
        public void GuardaSuministroProvisionReferencia(SURemisionSuministroDC remision, SUTrasladoSuministroDC traslado)
        {
            List<SuministroProvisionReferencia_SUM> referencias = new List<SuministroProvisionReferencia_SUM>();

            for (int i = 0; i < remision.GrupoSuministros.SuministroGrupo.CantidadAsignada; i++)
            {
                SuministroProvisionReferencia_SUM referencia = new SuministroProvisionReferencia_SUM()
                {
                    //SPR_CodigoBodegaAprovisionado = remision.CentroServicioAsignacion.CodigoBodega,
                    //SPR_IdCentroCostosAprovisionado = remision.CentroServicioAsignacion.IdCentroCostos,
                    SPR_IdSuministro = remision.GrupoSuministros.SuministroGrupo.Id,
                    SPR_IdGrupoSuministro = SUEnumGrupoSuministroDC.MEN.ToString(),
                    SPR_IdPropietarioSuministro = remision.MensajeroAsignacion.IdMensajero,
                    SPR_NumeroSuministro = remision.GrupoSuministros.SuministroGrupo.RangoInicial + i,
                    SPR_DescripcionGrupo = remision.GrupoSuministros.Descripcion,
                    SPR_FechaGrabacion = DateTime.Now,
                    SPR_CreadoPor = ControllerContext.Current.Usuario,
                };

                referencias.Add(referencia);
            }

            //guardar el traslado del suministro, como son varios se inserta un solo registro con el numero de suminstro en 0 y la cantidad trasladada
            traslado.NumeroSuministro = 0;
            traslado.Cantidad = remision.GrupoSuministros.SuministroGrupo.CantidadAsignada;
            SURepositorio.Instancia.GuardarTrasladoSuministro(traslado);

            // guardar el uno a uno de los suministros aprovisionados en la tabla de referencia
            GuardarSuministrosReferenciaBulk(referencias, NombreModelo);
        }

        /// <summary>
        ///  Guardar la referencia del suministro provisionado para un proceso
        /// </summary>
        /// <param name="remision">Objeto con la información de la remisión</param>
        /// <param name="traslado">Objeto con la información del traslado</param>
        public void GuardaSuministroProvisionReferenciaProceso(SURemisionSuministroDC remision, SUTrasladoSuministroDC traslado)
        {
            List<SuministroProvisionReferencia_SUM> referencias = new List<SuministroProvisionReferencia_SUM>();

            for (int i = 0; i < remision.GrupoSuministros.SuministroGrupo.CantidadAsignada; i++)
            {
                SuministroProvisionReferencia_SUM referencia = new SuministroProvisionReferencia_SUM()
                {
                    //SPR_CodigoBodegaAprovisionado = remision.CentroServicioAsignacion.CodigoBodega,
                    //SPR_IdCentroCostosAprovisionado = remision.CentroServicioAsignacion.IdCentroCostos,
                    SPR_IdSuministro = remision.GrupoSuministros.SuministroGrupo.Id,
                    SPR_IdGrupoSuministro = SUEnumGrupoSuministroDC.PRO.ToString(),
                    SPR_IdPropietarioSuministro = remision.ProcesoAsignacion.IdProceso,
                    SPR_NumeroSuministro = remision.GrupoSuministros.SuministroGrupo.RangoInicial + i,
                    SPR_DescripcionGrupo = remision.GrupoSuministros.Descripcion,
                    SPR_FechaGrabacion = DateTime.Now,
                    SPR_CreadoPor = ControllerContext.Current.Usuario,
                };

                referencias.Add(referencia);
            }

            //guardar el traslado del suministro, como son varios se inserta un solo registro con el numero de suminstro en 0 y la cantidad trasladada
            traslado.NumeroSuministro = 0;
            traslado.Cantidad = remision.GrupoSuministros.SuministroGrupo.CantidadAsignada;
            SURepositorio.Instancia.GuardarTrasladoSuministro(traslado);

            // guardar el uno a uno de los suministros aprovisionados en la tabla de referencia
            GuardarSuministrosReferenciaBulk(referencias, NombreModelo);
        }

        /// <summary>
        /// Actualiza el suministro provision referencia
        /// </summary>
        /// <param name="remision"></param>
        public void ActualizarSuministroProvisionReferencia(SURemisionSuministroDC remision, long idPropietario)
        {
            using (ModeloSuministros contexto = new ModeloSuministros(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                var suministrosReferencia = contexto.SuministroProvisionReferencia_SUM.Where(r => r.SPR_IdSuministro == remision.GrupoSuministros.SuministroGrupo.Id
                                                                && r.SPR_NumeroSuministro >= remision.GrupoSuministros.SuministroGrupo.RangoInicial
                                                                && r.SPR_NumeroSuministro <= remision.GrupoSuministros.SuministroGrupo.RangoFinal).ToList();

                suministrosReferencia.ForEach(referencia =>
                {
                    //referencia.SPR_CodigoBodegaAprovisionado = remision.CentroServicioAsignacion.CodigoBodega;
                    //referencia.SPR_IdCentroCostosAprovisionado = remision.CentroServicioAsignacion.IdCentroCostos;
                    referencia.SPR_IdPropietarioSuministro = idPropietario;
                    referencia.SPR_IdGrupoSuministro = remision.GrupoSuministros.IdGrupoSuministro;
                    referencia.SPR_DescripcionGrupo = remision.GrupoSuministros.Descripcion;
                });
                contexto.SaveChanges();
            }
        }

        /// <summary>
        /// Actualiza el número de la guía de una remisión
        /// </summary>
        /// <param name="idAdmision"></param>
        /// <param name="numeroguia"></param>
        /// <param name="idRemision"></param>
        public void ActualizarNumeroGuiaRemision(long idAdmision, long numeroguia, long idRemision)
        {
            using (ModeloSuministros contexto = new ModeloSuministros(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                RemisionSuministros_SUM remision = contexto.RemisionSuministros_SUM.Where(rem => rem.RES_IdRemisionSuministros == idRemision).FirstOrDefault();

                if (remision != null)
                {
                    remision.RES_NumeroGuiaInternaDespacho = numeroguia;
                    remision.RES_IdAdminisionMensajeriaDespacho = idAdmision;

                    contexto.SaveChanges();
                }
            }
        }

        #endregion Suministros referencia

        /// <summary>
        /// Guardar la referencia de suministros a través de bulkcopy
        /// </summary>
        /// <param name="referencias">Colección con las referencias de los suminstros</param>
        /// <param name="nombreModelo">Nombre del modelo para extraer la cadena de conexión a la base de datos</param>
        public void GuardarSuministrosReferenciaBulk(IEnumerable<SuministroProvisionReferencia_SUM> referencias, string nombreModelo)
        {
            if (referencias == null)
            {
                throw new FaultException<ControllerException>(new ControllerException("SUM", "SUM", "Error en la creación de las referencias del suministro, la lista llegó vacía"));
            }

            if (referencias.Count() == 0)
            {
                return;
            }

            string conexion = COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionStringNoEntity(nombreModelo);

            BulkCopyController bulkCopy = new BulkCopyController
            {
                BatchSize = 200,
                ConnectionString = conexion,
                DestinationTableName = "dbo.SuministroProvisionReferencia_SUM"
            };

            //bulkCopy.WriteToServer(referencias);
        }

        /// <summary>
        /// Obtiene los Correos a notificar la alerta del fallo en
        /// la sincronización a Novasoft de la salida ó
        /// traslado de suministros asignados desde Controller
        /// </summary>
        /// <returns>Lista de Correos</returns>
        public IList<SUCorreoNotificacionesSumDC> ObtenerCorreosNotificacionesSuministro()
        {
            using (ModeloSuministros contexto = new ModeloSuministros(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.CorreoNotificacionSuministros_SUM.OrderBy(or => or.CNS_Email).ToList().ConvertAll<SUCorreoNotificacionesSumDC>(not => new SUCorreoNotificacionesSumDC()
                {
                    IdCorreoNotificacion = not.CNS_IdCorreoNotificacionSuministros,
                    Email = not.CNS_Email
                });
            }
        }

        /// <summary>
        /// Adiciona un mail a la lista de notificaciones
        /// de sincronizacion de Novasoft
        /// </summary>
        /// <param name="email">Correo a Adicionar</param>
        public void AdicionarCorreoNotificacionSuministro(string email)
        {
            using (ModeloSuministros contexto = new ModeloSuministros(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                CorreoNotificacionSuministros_SUM adicionarEmail = new CorreoNotificacionSuministros_SUM()
                {
                    CNS_Email = email,
                    CNS_FechaGrabacion = DateTime.Now,
                    CNS_CreadoPor = ControllerContext.Current.Usuario,
                };
                contexto.CorreoNotificacionSuministros_SUM.Add(adicionarEmail);
                contexto.SaveChanges();
            }
        }

        /// <summary>
        /// Elimina un mail de la lista de notificaciones
        /// de sincronizacion de Novasoft
        /// </summary>
        /// <param name="infoEmail">Mail para realizar el borrado</param>
        public void BorrarCorreoNotificacionSuministro(int idEMailBorrar)
        {
            using (ModeloSuministros contexto = new ModeloSuministros(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                CorreoNotificacionSuministros_SUM borrarEmail = contexto.CorreoNotificacionSuministros_SUM.FirstOrDefault(mail => mail.CNS_IdCorreoNotificacionSuministros == idEMailBorrar);
                if (borrarEmail != null)
                {
                    contexto.CorreoNotificacionSuministros_SUM.Remove(borrarEmail);
                    contexto.SaveChanges();
                }
            }
        }

        /// <summary>
        /// Obtiene informacion de las provisiones de un suministro por el numero de remision
        /// </summary>
        /// <param name="fechaInicial">fecha inicial de la consulta por defecto deja la actual menos 3 dias</param>
        /// <param name="fechaFinal">fecha final de la consulta por defecto deja la actual</param>
        /// <param name="usuario">Usuario que realizo la remision</param>
        /// <param name="remisionInicial">numero de la remision inicial</param>
        /// <param name="remisionFinal">numero de la remision final</param>
        /// <param name="pageIndex">indice de la pagina</param>
        /// <param name="pageSize">tamaño de la paginacion</param>
        /// <returns>Lista con la informacion para impresion los suministro de un Centro Servicio</returns>
        public List<SURemisionSuministroDC> ObtenerRemisionesGuiasInternas(SUFiltroSuministroPorRemisionDC filtroPorRemision)
        {
            using (ModeloSuministros contexto = new ModeloSuministros(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                if (!filtroPorRemision.ConsultaIncluyeFecha)
                {
                    filtroPorRemision.FechaInicial = DateTime.Now.AddYears(-1);
                    filtroPorRemision.FechaFinal = DateTime.Now;
                }

                var guiasInternas = contexto.paObtenerRemisiones_SUM(filtroPorRemision.RemisionInicial, filtroPorRemision.RemisionFinal, filtroPorRemision.FechaInicial, filtroPorRemision.FechaFinal, filtroPorRemision.ConsultaIncluyeFecha)
                   .ToList();

                if (guiasInternas.Any())
                {
                    return guiasInternas.ConvertAll(r => new SURemisionSuministroDC
                    {
                        GuiaInterna = new ADGuiaInternaDC
                        {
                            DiceContener = r.AGI_Contenido,
                            CreadoPor = r.AGI_CreadoPor,
                            DireccionDestinatario = r.AGI_DireccionDestinatario,
                            DireccionRemitente = r.AGI_DireccionRemitente,
                            FechaGrabacion = r.AGI_FechaGrabacion,
                            IdAdmisionGuia = r.AGI_IdAdmisionMensajeria,
                            GestionOrigen = new ARGestionDC
                            {
                                IdGestion = r.AGI_IdGestionOrigen,
                                Descripcion = r.AGI_DescripcionGestionOrig
                            },
                            GestionDestino = new ARGestionDC
                            {
                                IdGestion = r.AGI_IdGestionDestino,
                                Descripcion = r.AGI_DescripcionGestionDest
                            },

                            IdCentroServicioDestino = r.ADM_IdCentroServicioDestino,
                            IdCentroServicioOrigen = r.ADM_IdCentroServicioOrigen,
                            LocalidadDestino = new PALocalidadDC
                            {
                                IdLocalidad = r.ADM_IdCiudadDestino,
                                Nombre = r.ADM_NombreCiudadDestino,
                            },
                            LocalidadOrigen = new PALocalidadDC
                            {
                                IdLocalidad = r.ADM_IdCiudadOrigen,
                                Nombre = r.ADM_IdCiudadOrigen,
                            },
                            NombreCentroServicioDestino = r.ADM_NombreCentroServicioDestino,
                            NombreCentroServicioOrigen = r.ADM_NombreCentroServicioOrigen,
                            NombreDestinatario = r.AGI_NombreDestinatario,
                            NombreRemitente = r.AGI_NombreRemitente,
                            NumeroGuia = r.RES_NumeroGuiaInternaDespacho,
                            TelefonoDestinatario = r.AGI_TelefonoDestinatario,
                            TelefonoRemitente = r.AGI_TelefonoRemitente,
                        },
                    });
                }
                else
                    return new List<SURemisionSuministroDC>();
            }
        }

        /// <summary>
        /// Actualizar el valor del número actual de un suministro específico
        /// </summary>
        /// <param name="tipoSuministro"></param>
        /// <param name="numeroActual"></param>
        public void ActualizarNumeroActualSuministro(SUEnumSuministro tipoSuministro, long numeroActual)
        {
            using (ModeloSuministros contexto = new ModeloSuministros(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                NumeradorAutomatico_SUM numeradorAutom = contexto.NumeradorAutomatico_SUM.Where(n => n.NUM_IdSuministro == (int)tipoSuministro && n.NUM_EstaActivo == true).FirstOrDefault();

                numeradorAutom.NUM_Actual = numeroActual;
                contexto.SaveChanges();

            }
        }

        /// <summary>
        /// Metodo para obtener suministros segun mensajero
        /// </summary>
        /// <param name="IdMensajero"></param>
        /// <param name="IdSuministro"></param>
        /// <returns></returns>
        public List<long> GenerarSuministrosDisponiblesMensajero(long IdMensajero, long IdSuministro)
        {
            List<long> suministros = new List<long>();
            using (SqlConnection conn = new SqlConnection(conexionStringController))
            {
                SqlCommand cmd = new SqlCommand(@"paObtenerSuministrosMensajero_SUM", conn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@idMensajero", IdMensajero);
                cmd.Parameters.AddWithValue("@idSuministro", IdSuministro);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    suministros.Add((long)reader["NumeroGuia"]);
                }
                return suministros;
            }
        }
    }
}