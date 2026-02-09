using CO.Servidor.CentroServicios;
using CO.Servidor.Dominio.Comun.Admisiones;
using CO.Servidor.Dominio.Comun.Middleware;
using CO.Servidor.Servicios.ContratoDatos.Admisiones.Mensajeria;
using CO.Servidor.Servicios.ContratoDatos.Area;
using CO.Servidor.Servicios.ContratoDatos.Integraciones;
using Framework.Servidor.Comun;
using Framework.Servidor.Excepciones;
using Framework.Servidor.ParametrosFW;
using Framework.Servidor.Servicios.ContratoDatos.Parametros;
using System;
using System.Collections.Generic;
using CO.Controller.Servidor.Integraciones.AccesoDatos.Claro;
using CO.Controller.Servidor.Integraciones;
using System.Transactions;
using System.Configuration;
using System.Data;
using System.Runtime.Serialization;
using System.ComponentModel;
using CO.Servidor.Dominio.Comun.LogisticaInversa;

namespace CO.Servidor.Integraciones
{
    public class IntegracionSISPostal : IIntegracionSISPostal
    {
        #region  Instancia

        private static readonly IntegracionSISPostal instancia = new IntegracionSISPostal();
        /// <summary>
        /// Retorna una instancia de administracion de produccion
        /// /// </summary>
        public static IntegracionSISPostal Instancia
        {
            get { return instancia; }
        }
        #endregion  Instancia

        private IADFachadaAdmisionesMensajeria fachadaAdmision = COFabricaDominio.Instancia.CrearInstancia<IADFachadaAdmisionesMensajeria>();
        private ILIFachadaLogisticaInversaPruebasEntrega fachadaLogInvPruebaEntrega  = COFabricaDominio.Instancia.CrearInstancia<ILIFachadaLogisticaInversaPruebasEntrega>();

        #region Métodos

        /// <summary>
        /// Permite Crear una Admision desde SisPostal
        /// </summary>
        /// <param name="Credencial"></param>
        /// <param name="admision"></param>
        /// <returns></returns>
        public long AdicionarAdmisionSisPostal(credencialDTO Credencial, ADGuiaSisPostal admision)
        {
            //Valida credenciales
            if (ValidarCredencialSisPostal(Credencial))
            {

                var localidadOrigen = PAAdministrador.Instancia.ObtenerLocalidadPorId(admision.IdLocalidadOrigen);
                var localidadDestino = PAAdministrador.Instancia.ObtenerLocalidadPorId(admision.IdLocalidadDestino);
                var centroServiciosOrigen = PUCentroServicios.Instancia.ObtieneCOLPorLocalidad(admision.IdLocalidadOrigen);
                var centroServiciosDestino = PUCentroServicios.Instancia.ObtieneCOLPorLocalidad(admision.IdLocalidadDestino);

                //Prepara la guia interna
                var guia = new ADGuiaInternaDC
                {
                    EsManual = true,
                    CreadoPor = "SisPostal",
                    GuidDeChequeo = Guid.NewGuid().ToString(),
                    FechaGrabacion = DateTime.Now,
                    IdAdmisionGuia = 0,
                    Observaciones = string.Empty,
                    TipoEntrega = new ADTipoEntrega { Id = "1", Descripcion = "ENTREGA EN DIRECCION" },
                    EmailRemitente = String.Empty,
                    EmailDestinatario = String.Empty,
                    PaisDefault = new PALocalidadDC { IdLocalidad = ConstantesFramework.ID_LOCALIDAD_COLOMBIA, Nombre = ConstantesFramework.DESC_LOCALIDAD_COLOMBIA },
                    IdentificacionDestinatario = "0",
                    TipoIdentificacionRemitente = "NI",
                    IdentificacionRemitente = "800251569",
                    TipoIdentificacionDestinatario = "CC",
                    DiceContener = "MASIVO SISPOSTAL",
                    NombreRemitente = centroServiciosOrigen.infoResponsable.NombreCentroServicio,
                    //Propiedades Obligatorias,
                    NumeroGuia = admision.NumeroGuia,
                    LocalidadOrigen = localidadOrigen,
                    LocalidadDestino = localidadDestino,
                    TelefonoRemitente = admision.TelefonoRemitente,
                    TelefonoDestinatario = admision.TelefonoDestinatario,
                    GestionDestino = new ARGestionDC { IdGestion = 0, Descripcion = string.Empty },
                    GestionOrigen = new ARGestionDC { IdGestion = 3, Descripcion = "GESTION FINANCIERA", IdCasaMatriz = 1 },
                    DireccionRemitente = localidadOrigen.NombreCompleto,
                    DireccionDestinatario = localidadDestino.NombreCompleto,
                    NombreDestinatario = centroServiciosOrigen.infoResponsable.NombreCentroServicio,
                    IdCentroServicioOrigen = centroServiciosOrigen.IdCentroServicio,
                    NombreCentroServicioOrigen = centroServiciosOrigen.infoResponsable.NombreCentroServicio,
                    NombreCentroServicioDestino = centroServiciosDestino.infoResponsable.NombreCentroServicio,


                };

                MockedOperationContextUtil.CrearControllerContextTareas();

                ControllerContext.Current.Usuario = "Sistema";
                var guiaInterna = fachadaAdmision.AdicionarGuiaInterna(guia);

                return guiaInterna.IdAdmisionGuia;
            }
            else
            {
                return -1;
            }
        }
        /// <summary>
        /// consulta entregas claro, enviomensaje y modifica guia 
        /// </summary>
        public void ConsultarEntregasClaro()
        {
            List<GuiasClaro> GuiasClaro = new List<GuiasClaro>();

            GuiasClaro= DALClaro.Instancia.ConsultarEntregasClaro();

            string mensaje = string.Empty;
            
            foreach (GuiasClaro guia in GuiasClaro) {
                using (TransactionScope transaccion = new TransactionScope())
                {
                    mensaje = string.Format(MensajesTexto.Instancia.ObtenerMensajeTexto(SIEmumMensajeTexto.SispostalClaro.ToString()),guia.Guia, guia.Nombre);
                    MensajesTexto.Instancia.EnviarMensajeTexto(guia.Celular, mensaje);
                    DALClaro.Instancia.ModificarGuiaEnvioMsj(guia.Guia);

                    transaccion.Complete();
                }
            }
            


        }
        /// <summary>
        /// valida credenciales para ws sispostal
        /// </summary>
        /// <param name="credencial"></param>
        /// <returns></returns>

        private bool ValidarCredencialSisPostal(credencialDTO credencial)
        {
            return fachadaAdmision.ValidarCredencialSispostal(credencial);
        }

        
        public List<INEstadosSWSispostal> ObtenerEstadosGuiaSispostal(long NGuia)
        {
            string Usuario = ConfigurationManager.AppSettings["UsuarioSispostal"];
            string Password = ConfigurationManager.AppSettings["PasswordSispostal"];

            WSSispostal.sispostal1SoapClient proxySispostal = new WSSispostal.sispostal1SoapClient();

            var results = proxySispostal.wssisptracking(Usuario, Password, NGuia.ToString());
            List<INEstadosSWSispostal> estados = null;
          
            if (results != null) {
                estados = new List<INEstadosSWSispostal>();
                foreach (DataTable result in results.Tables)
                {
                    foreach (DataRow row in result.Rows)
                    {
                        if (row["estado"].ToString() != "ALISTAMIENTO" && row["estado"].ToString() != "NO NEGOCIACION")
                        {

                            estados.Add(new INEstadosSWSispostal
                            {
                                Estado = row["estado"] == DBNull.Value ? string.Empty : row["estado"].ToString(),
                                Fecha = row["fecha"] == DBNull.Value ? DateTime.Now : DateTime.Parse(row["fecha"].ToString()),
                                Ciudad = row["ciudad"] == DBNull.Value ? string.Empty : row["ciudad"].ToString(),
                                Guia = row["guia"].ToString()
                            });
                        }
                    }
                }

                
                var type = typeof(SIEnumEstadosGuiasMasivos);
                foreach (var item in estados) {
                    foreach (var field in type.GetFields())
                    {
                        var attribute = Attribute.GetCustomAttribute(field,
                            typeof(DescriptionAttribute)) as DescriptionAttribute;
                        if (attribute != null)
                        {
                            if (field.Name.ToUpper()== item.Estado.Replace(" " ,"").ToUpper())
                            {
                                 item.Estado = attribute.Description;
                                break;
                            }
                        }
                       
                    }
                }
               
            }
            return estados;
        }

        public bool ValidarGuiaDescargadaXAppMasivos(long NGuia)
        {
            return fachadaLogInvPruebaEntrega.ValidarGuiaDescargadaXAppMasivos(NGuia);
        }

        #endregion
    }
}
