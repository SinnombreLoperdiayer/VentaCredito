using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.ServiceModel;
using System.Transactions;
using CO.Servidor.Admisiones.Giros.Comun;
using CO.Servidor.Admisiones.Giros.Datos;
using CO.Servidor.Admisiones.Giros.TransaccionCaja;
using CO.Servidor.Dominio.Comun.Cajas;
using CO.Servidor.Dominio.Comun.CentroServicios;
using CO.Servidor.Dominio.Comun.Clientes;
using CO.Servidor.Dominio.Comun.Comisiones;
using CO.Servidor.Dominio.Comun.Middleware;
using CO.Servidor.Dominio.Comun.Suministros;
using CO.Servidor.Dominio.Comun.Tarifas;
using CO.Servidor.Servicios.ContratoDatos.Admisiones.Giros;
using CO.Servidor.Servicios.ContratoDatos.Cajas;
using CO.Servidor.Servicios.ContratoDatos.CentroServicios;
using CO.Servidor.Servicios.ContratoDatos.Comisiones;
using CO.Servidor.Servicios.ContratoDatos.Suministros;
using Framework.Servidor.Comun;
using Framework.Servidor.Excepciones;
using Framework.Servidor.Servicios.ContratoDatos.Parametros;
using System.IO;
using System.Configuration;
using CO.Servidor.Servicios.ContratoDatos.Admisiones.Giros.Venta;

namespace CO.Servidor.Admisiones.Giros.Venta
{
    /// <summary>
    /// Creacion de giros
    /// </summary>
    public class GIVentaGiros : ControllerBase
    {

        /// <summary>
        /// Path almacena imagenes scanneadas
        /// </summary>
        private string filePath = ConfigurationManager.AppSettings["Controller.RutaDescargaArchivos"];

        #region CrearInstancia

        private static readonly GIVentaGiros instancia = (GIVentaGiros)FabricaInterceptores.GetProxy(new GIVentaGiros(), COConstantesModulos.GIROS);

        /// <summary>
        /// Retorna una instancia de centro Servicios
        /// /// </summary>
        public static GIVentaGiros Instancia
        {
            get { return GIVentaGiros.instancia; }
        }

        #endregion CrearInstancia

        #region Metodos

        /// <summary>
        /// Valida que una agencia pueda realizar la venta de un giro
        /// </summary>
        /// <param name="idCentroServicios">Codigo Centro Servicios</param>
        public void ValidarAgenciaServicioGiros(long idCentroServicios)
        {
            COFabricaDominio.Instancia.CrearInstancia<IPUFachadaCentroServicios>().ValidarAgenciaServicioGiros(idCentroServicios);
        }

        /// <summary>
        /// Consulta la agencia a la cual se le suministro la factura de venta, con el numero de giro IdGiro
        /// </summary>
        /// <param name="IdGiro">Numero del giro</param>
        /// <returns>Centro de servicio</returns>
        private PUCentroServiciosDC ConsultarAgenciaPropietariaDelNumeroGiro(long idGiro)
        {
            SUPropietarioGuia propietario = COFabricaDominio.Instancia.CrearInstancia<ISUFachadaSuministros>().ObtenerPropietarioSuministro(idGiro, SUEnumSuministro.FACTURA_VENTA_GIRO_POSTAL_MANUAL);
            return COFabricaDominio.Instancia.CrearInstancia<IPUFachadaCentroServicios>().ObtenerInformacionCentroServicioPorId(propietario.Id);
        }


        /// <summary>
        /// Obtiene la agencia manual dueña del numero del giro
        /// </summary>
        /// <param name="idGiro"></param>
        /// <returns></returns>
        public PUCentroServiciosDC ObtenerAgenciaPropietariaDelNumeroGiro(long idGiro, long idCentroServicio, bool esUsuarioRacol, long idRacol)
        {
            IPUFachadaCentroServicios fachadaCentroServicios = COFabricaDominio.Instancia.CrearInstancia<IPUFachadaCentroServicios>();

            PUCentroServiciosDC agencia = null;
            agencia = ConsultarAgenciaPropietariaDelNumeroGiro(idGiro);
            if (agencia != null)
            {
                if (idCentroServicio == agencia.IdCentroServicio)
                {
                    return agencia;
                }
                else if (esUsuarioRacol && fachadaCentroServicios.ObtenerCentroServicioAdscritoRacol(idRacol, agencia.IdCentroServicio) != null)
                {
                    return agencia;
                }
                else
                {
                    //error
                    throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.GIROS,
                           string.Format(EnumTipoErrorAdmisionesGiros.EX_CENTRO_SERVICIO_NO_PUEDE_ADMITIR_GUIA_SERVIDOR.ToString(), idGiro),
                           MensajesAdmisionesGiros.CargarMensaje(EnumTipoErrorAdmisionesGiros.EX_NUMERO_GIRO_NO_ASOCIADO_A_AGENCIA)));
                }
            }

            else
                throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.GIROS, EnumTipoErrorAdmisionesGiros.EX_NUMERO_GIRO_NO_ASOCIADO_A_AGENCIA.ToString(), MensajesAdmisionesGiros.CargarMensaje(EnumTipoErrorAdmisionesGiros.EX_NUMERO_GIRO_NO_ASOCIADO_A_AGENCIA)));
        }

        /// <summary>
        /// Creacion de un giro
        /// </summary>
        /// <param name="giro"></param>
        public GINumeroGiro CrearGiro(GIAdmisionGirosDC giro)
        {
            SUNumeradorPrefijo numeroSuministro = null;
            bool obligaDeclaracionVoluntariaFondos = false;

            ValidarAgenciaServicioGiros(giro.AgenciaOrigen.IdCentroServicio);

            ISUFachadaSuministros fachadaSuministros = COFabricaDominio.Instancia.CrearInstancia<ISUFachadaSuministros>();
            if (giro.NumeroGiroAgenciaManual <= 0)
            {
                using (TransactionScope transaccionNumSum = new TransactionScope())
                {
                    // Se obtiene el número del giro
                    numeroSuministro = fachadaSuministros.ObtenerNumeroPrefijoValor(SUEnumSuministro.FACTURA_VENTA_GIRO_POSTAL_AUTOMATICO);
                    transaccionNumSum.Complete();
                }
            }

            using (TransactionScope transaccion = new TransactionScope())
            {
                GINumeroGiro numeroGiro = null;

                COFabricaDominio.Instancia.CrearInstancia<IPUFachadaCentroServicios>().AcumularVentaGirosAgencia(giro);
                decimal valorAcumulado = COFabricaDominio.Instancia.CrearInstancia<ICLFachadaClientes>().AdmGuardarClienteContado(giro.GirosPeatonPeaton.ClienteRemitente, giro.GirosPeatonPeaton.ClienteDestinatario, giro.Precio.ValorGiro, giro.AgenciaDestino.Nombre, giro.AgenciaDestino.IdCentroServicio);

                if (giro.ArchivoDeclaracionVoluntariaOrigenes == null)
                {
                    //Se cambio la logia para que la validacion no fuera por acumulado sino por valor del giro
                    if (GIRepositorio.Instancia.ValidarDeclaracionFondos(giro.Precio.ValorGiro))
                        obligaDeclaracionVoluntariaFondos = true;
                }
                else
                {
                    giro.ArchivoDeclaracionVoluntariaOrigenes = GuardarImagenCarpeta(giro.ArchivoDeclaracionVoluntariaOrigenes);
                    giro.DeclaracionVoluntariaOrigenes = GIRepositorio.Instancia.AlmacenarDeclaracionFondos(giro.ArchivoDeclaracionVoluntariaOrigenes, giro);
                }

                if (giro.NumeroGiroAgenciaManual > 0)
                {
                    // el giro proviene de una agencia manual
                    giro.IdGiro = giro.NumeroGiroAgenciaManual;
                    giro.GiroAutomatico = false;
                }
                else
                {
                    giro.IdGiro = numeroSuministro.ValorActual;
                    giro.PrefijoIdGiro = numeroSuministro.Prefijo;
                    giro.GiroAutomatico = true;
                }

                numeroGiro = GIRepositorio.Instancia.CrearGiro(giro);

                if (giro.NumeroGiroAgenciaManual > 0)
                {
                    SUConsumoSuministroDC consumo = new SUConsumoSuministroDC()
                    {
                        Cantidad = 1,
                        EstadoConsumo = SUEnumEstadoConsumo.CON,
                        IdDuenoSuministro = giro.AgenciaOrigen.IdCentroServicio,
                        IdServicioAsociado = TAConstantesServicios.SERVICIO_GIRO,
                        NumeroSuministro = giro.NumeroGiroAgenciaManual,
                        Suministro = SUEnumSuministro.FACTURA_VENTA_GIRO_POSTAL_MANUAL
                    };
                    PUCentroServiciosDC centroServ = COFabricaDominio.Instancia.CrearInstancia<IPUFachadaCentroServicios>().ObtenerCentroServicio(giro.AgenciaOrigen.IdCentroServicio);
                    SUEnumGrupoSuministroDC grupo = (SUEnumGrupoSuministroDC)Enum.Parse(typeof(SUEnumGrupoSuministroDC), centroServ.Tipo);
                    consumo.GrupoSuministro = grupo;
                    fachadaSuministros.GuardarConsumoSuministro(consumo);
                }

                numeroGiro.PrefijoIdGiro = giro.PrefijoIdGiro;
                numeroGiro.ObligaDeclaracionVoluntariaFondos = obligaDeclaracionVoluntariaFondos;
                GITransaccionCaja.EnviarTransaccionCajaVentaGiro(giro);

                /*EnviarMensajeDestinatario EnvioGiro*/
                string tipoNotificacion = "VentaGiro";
                var tipo = (GIEnumMensajeTexto)Enum.Parse(typeof(GIEnumMensajeTexto), tipoNotificacion.ToString());

                if (!string.IsNullOrEmpty(giro.GirosPeatonPeaton.ClienteDestinatario.Telefono))
                { 
                GIMensajesTexto.Instancia.EnviarMensajeTexto(tipo, giro.GirosPeatonPeaton.ClienteDestinatario.Telefono, giro.IdGiro, giro.AgenciaDestino.NombreMunicipio);
                }
                /**/


                transaccion.Complete();
                return numeroGiro;
            }
        }

        /// <summary>
        /// Creacion de un giro
        /// </summary>
        /// <param name="giro"></param>
        public GINumeroGiro CrearGiroProduccion(GIAdmisionGirosDC giro)
        {
            GINumeroGiro numeroGiro = null;
            SUNumeradorPrefijo numeroSuministro = null;
            using (TransactionScope transaccion = new TransactionScope())
            {
                ISUFachadaSuministros fachadaSuministros = COFabricaDominio.Instancia.CrearInstancia<ISUFachadaSuministros>();
                numeroSuministro = fachadaSuministros.ObtenerNumeroPrefijoValor(SUEnumSuministro.GIROS_PRODUCCION);
                transaccion.Complete();
            }

            if (numeroSuministro != null)
            {
                using (TransactionScope transaccion = new TransactionScope())
                {
                    giro.IdGiro = numeroSuministro.ValorActual;
                    giro.PrefijoIdGiro = numeroSuministro.Prefijo;

                    numeroGiro = GIRepositorio.Instancia.CrearGiro(giro, true);
                    numeroGiro.PrefijoIdGiro = giro.PrefijoIdGiro;

                    //GITransaccionCaja.EnviarTransaccionCajaVentaGiro(giro);
                    transaccion.Complete();
                }
            }
            return numeroGiro;
        }

        /// <summary>
        /// Consulta la informacion de un giro a partir de el guid
        /// </summary>
        /// <param name="GuidDeChequeo"></param>
        /// <returns></returns>
        public GINumeroGiro ConsultarGiroPorGuid(string guidDeChequeo)
        {
            return GIRepositorio.Instancia.ConsultarGiroPorGuid(guidDeChequeo);
        }

        /// <summary>
        /// Valida el monto para la declaracion volutaria de fondos
        /// </summary>
        /// <param name="valorAcumulado"></param>
        /// <returns></returns>
        public bool ValidarDeclaracionFondos(decimal valorAcumulado)
        {
            return GIRepositorio.Instancia.ValidarDeclaracionFondos(valorAcumulado);
        }

        /// <summary>
        /// consulta los giros activos realizados el dia actual Peaton Peaton
        /// No retorna todos los valores del giro
        /// </summary>
        /// <param name="idCentroServicio"></param>
        /// <returns></returns>
        public IList<GIAdmisionGirosDC> ConsultarGirosPeatonPeatonPorAgencia(long idCentroServicio, long idGiro, string tipoIdRemitente, string identificacionRemitente, string tipoIdDestinatario, string identificacionDestinatario, string tipoDocumentoGiro, int indicePagina, int registrosPorPagina, string tipoCentroServicio)
        {
            if (tipoDocumentoGiro.CompareTo(GIEnumTipoDocumento.FAC.ToString()) == 0)
                return GIRepositorio.Instancia.ConsultarGirosPeatonPeatonPorAgencia(idCentroServicio, idGiro, tipoIdRemitente, identificacionRemitente, tipoIdDestinatario, identificacionDestinatario, indicePagina, registrosPorPagina, tipoCentroServicio);
            else
                return PGRepositorio.Instancia.ConsultarPagoGirosPeatonPeatonPorAgencia(idCentroServicio, idGiro, tipoIdRemitente, identificacionRemitente, tipoIdDestinatario, identificacionDestinatario, indicePagina, registrosPorPagina, tipoCentroServicio);
        }

        /// <summary>
        /// consulta los giros activos realizados el dia actual Peaton convenio
        /// No retorna todos los valores del giro
        /// </summary>
        /// <param name="idCentroServicio"></param>
        /// <returns></returns>
        public List<GIAdmisionGirosDC> ConsultarGirosPeatonConvenioPorAgencia(long idCentroServicio, long idGiro, string tipoIdRemitente, string identificacionRemitente, string tipoIdDestinatario, string identificacionDestinatario, string tipoDocumentoGiro, int indicePagina, int registrosPorPagina, string tipoCentroServicio)
        {
            if (tipoDocumentoGiro.CompareTo(GIEnumTipoDocumento.FAC.ToString()) == 0)
                return GIRepositorio.Instancia.ConsultarGirosPeatonConvenioPorAgencia(idCentroServicio, idGiro, tipoIdRemitente, identificacionRemitente, tipoIdDestinatario, identificacionDestinatario, indicePagina, registrosPorPagina, tipoCentroServicio);

            else
                return PGRepositorio.Instancia.ConsultarPagoGirosPeatonConvenioPorAgencia(idCentroServicio, idGiro, tipoIdRemitente, identificacionRemitente, tipoIdDestinatario, identificacionDestinatario, indicePagina, registrosPorPagina, tipoCentroServicio);
        }

        /// <summary>
        /// Consultar la informacion de la tabla peaton peaton
        /// </summary>
        /// <param name="idAdmisionGiro"></param>
        public GIAdmisionGirosDC ConsultarInformacionPeatonPeaton(long idAdmisionGiro)
        {
            return GIRepositorio.Instancia.ConsultarInformacionPeatonPeaton(idAdmisionGiro);
        }

        // TODO:ID Consulta de Giros para Integracion 742
        public List<GIAdmisionGirosDC> ConsultarGirosPPEIntegracionSieteCuatroDos(string pEstado)
        {
            return GIRepositorioExtendido.Instancia.ConsultarInformacionGiros_CuatroSieteDos(pEstado);
        }



        /// <summary>
        /// Consultar la informacion de la tabla peaton Convenio
        /// </summary>
        /// <param name="idAdmisionGiro"></param>
        public GIAdmisionGirosDC ConsultarInformacionPeatonConvenio(long idAdmisionGiro)
        {
            return GIRepositorio.Instancia.ConsultarInformacionPeatonConvenio(idAdmisionGiro);
        }

        /// <summary>
        /// Obtiene el identificador de admisión giro
        /// </summary>
        /// <param name="idGiro">Identificador giro</param>
        /// <returns>Identificador admisión giro</returns>
        public long? ObtenerIdentificadorAdmisionGiro(long idGiro)
        {
            return GIRepositorio.Instancia.ObtenerIdentificadorAdmisionGiro(idGiro);
        }

        /// <summary>
        /// Consulta los tipos de identificacion con los que se reclama el giro
        /// </summary>
        /// <returns>Lista de tipos de identificacion</returns>
        internal IList<PATipoIdentificacion> ConsultarTiposIdentificacionReclamaGiros()
        {
            return GIRepositorio.Instancia.ConsultarTiposIdentificacionReclamaGiros();
        }

        /// <summary>
        /// Método para guardar las imagenes en la carpeta compartida
        /// </summary>
        /// <param name="rutaArchivo"></param>
        public string GuardarImagenCarpeta(string rutaArchivo)
        {
            string rutaImagenes = Framework.Servidor.ParametrosFW.PAParametros.Instancia.ConsultarParametrosFramework("FolderImagenGiros");
            string carpetaDestino = Path.Combine(rutaImagenes + DateTime.Now.Day + "-" + DateTime.Now.Month + "-" + DateTime.Now.Year);
            string rutaDestino = Path.Combine(carpetaDestino, rutaArchivo);
            if (!Directory.Exists(carpetaDestino))
            {
                Directory.CreateDirectory(carpetaDestino);
            }

            lock (this)
            {
                File.Move(Path.Combine(this.filePath, COConstantesModulos.GIROS, rutaArchivo), rutaDestino);
            }
            return rutaDestino;
        }

        /// <summary>
        /// Consultar la informacion inicial peaton peaton
        /// </summary>
        /// <param name="idAdmisionGiro"></param>
        public GIAdmisionGirosDC ConsultarInformacionPeatonPeatonInicial(long idAdmisionGiro)
        {
            return GIRepositorio.Instancia.ConsultarInformacionPeatonPeatonInicial(idAdmisionGiro);
        }

        /// <summary>
        /// Consultar la informacion inicial peaton convenio
        /// </summary>
        /// <param name="idAdmisionGiro"></param>
        /// <returns></returns>
        public GIAdmisionGirosDC ConsultarInformacionPeatonConvenioInicial(long idAdmisionGiro)
        {
            return GIRepositorio.Instancia.ConsultarInformacionPeatonConvenioInicial(idAdmisionGiro);
        }


        #endregion Metodos
    }
}