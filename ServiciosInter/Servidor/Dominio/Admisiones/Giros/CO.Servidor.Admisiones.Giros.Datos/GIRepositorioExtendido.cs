using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Threading;
using CO.Controller.Servidor.Integraciones.CuatroSieteDos;
using CO.Servidor.Admisiones.Giros.Comun;
using CO.Servidor.Admisiones.Giros.Datos.Modelo;
using CO.Servidor.Dominio.Comun.Suministros;
using CO.Servidor.Dominio.Comun.Util;
using CO.Servidor.Servicios.ContratoDatos.Admisiones.Giros;
using CO.Servidor.Servicios.ContratoDatos.Admisiones.Giros.Pagos;
using CO.Servidor.Servicios.ContratoDatos.Admisiones.Giros.Venta;
using CO.Servidor.Servicios.ContratoDatos.CentroServicios;
using CO.Servidor.Servicios.ContratoDatos.Clientes;
using CO.Servidor.Servicios.ContratoDatos.LogisticaInversa;
using CO.Servidor.Servicios.ContratoDatos.Solicitudes;
using CO.Servidor.Servicios.ContratoDatos.Tarifas;
using Framework.Servidor.Comun;
using Framework.Servidor.Excepciones;
using Framework.Servidor.Servicios.ContratoDatos.Parametros;

namespace CO.Servidor.Admisiones.Giros.Datos
{
    public class GIRepositorioExtendido
    {
        private static readonly GIRepositorioExtendido instancia = new GIRepositorioExtendido();
        public static GIRepositorioExtendido Instancia
        {
            get { return GIRepositorioExtendido.instancia; }
        }

        private const string NombreModelo = "ModeloVentaGiros";

        public List<GIAdmisionGirosDC> ConsultarInformacionGiros_CuatroSieteDos(string pEstadoGiro)
        {
            using (ModeloVentaGiros contexto = new ModeloVentaGiros(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {

                return contexto.paObtenerGiroPPE_paraIntegracion742_GIR(pEstadoGiro).ToList().ConvertAll<GIAdmisionGirosDC>(admGiro => new GIAdmisionGirosDC()
                {
                    IdGiro = admGiro.ADG_IdGiro,
                    FechaGrabacion = admGiro.GPP_FechaGrabacion, // Esta es la Fecha de Admision del Giro

                    EstadoGiro = admGiro.ESG_Estado,
                    IdEstadoGiro= admGiro.ESG_IdEstadosGiro, // TODO:ID Estado del Giro que se va a Reportes a 472
                    FechaEstadoGiro = admGiro.ESG_FechaGrabacion,

                    GirosPeatonPeaton = new GIGirosPeatonPeatonDC()
                    {
                        ClienteRemitente = new CLClienteContadoDC()
                        {
                            Apellido1 = admGiro.GPP_Apellido1Remitente,
                            Apellido2 = admGiro.GPP_Apellido2Remitente == null ? string.Empty : admGiro.GPP_Apellido2Remitente,
                            Direccion = admGiro.GPP_DireccionRemitente,
                            Ocupacion = new PAOcupacionDC() { DescripcionOcupacion = admGiro.GPP_OcupacionRemitente },
                            Email = admGiro.GPP_EmailRemitente,
                            Identificacion = admGiro.GPP_IdRemitente,
                            Nombre = admGiro.GPP_NombreRemitente,
                            Telefono = admGiro.GPP_TelefonoRemitente,
                            TipoId = admGiro.GPP_TipoIdRemitente
                        },
                        ClienteDestinatario = new CLClienteContadoDC()
                        {
                            Apellido1 = admGiro.GPP_Apellido1Destinatario,
                            Apellido2 = admGiro.GPP_Apellido2Destinatario == null ? string.Empty : admGiro.GPP_Apellido2Destinatario,
                            Direccion = admGiro.GPP_DireccionDestinatario,
                            Ocupacion = new PAOcupacionDC() { DescripcionOcupacion = admGiro.GPP_OcupacionDestinatario },
                            Email = admGiro.GPP_EmailDestinatario,
                            Identificacion = admGiro.GPP_IdDestinatario,
                            Nombre = admGiro.GPP_NombreDestinatario,
                            Telefono = admGiro.GPP_TelefonoDestinatario,
                            TipoId = admGiro.GPP_TipoIdDestinatario,
                        }
                    },

                    AgenciaOrigen = new PUCentroServiciosDC()
                    {
                        IdCentroServicio = admGiro.ADG_IdCentroServicioOrigen,
                        IdMunicipio = admGiro.ADG_IdCiudadOrigen,
                    },

                    AgenciaDestino = new PUCentroServiciosDC()
                    {
                        IdCentroServicio = admGiro.ADG_IdCentroServicioDestino,
                        IdMunicipio = admGiro.ADG_IdCiudadDestino,
                    },

                    Precio = new TAPrecioDC
                    {
                        ValorGiro = admGiro.ADG_ValorGiro,
                        ValorServicio = admGiro.ADG_ValorPorte,
                    }
                    ,EstACT_yaTransmitido = admGiro.estACT_yaTransmitido
                    
                });

            }


        }

    }
}
