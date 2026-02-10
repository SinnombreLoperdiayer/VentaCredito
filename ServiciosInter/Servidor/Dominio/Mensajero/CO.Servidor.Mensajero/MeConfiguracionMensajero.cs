using CO.Servidor.Mensajero.Datos;
using CO.Servidor.Servicios.ContratoDatos.Mensajero;
using CO.Servidor.Servicios.ContratoDatos.ParametrosOperacion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Transactions;

namespace CO.Servidor.Mensajero
{
    public class MeConfiguracionMensajero
    {
        private static readonly MeConfiguracionMensajero instancia = new MeConfiguracionMensajero();

        public static MeConfiguracionMensajero Instancia
        {
            get { return MeConfiguracionMensajero.instancia; }
        }

        private MeConfiguracionMensajero(){}

        public List<MECargo> ObtenerCargos()
        {
            //return new List<MECargo>();
            return MeRepositorio.Instancia.ObtenerCargos();
        }

        public List<METipoContrato> ObtenerTiposContrato()
        {
            return MeRepositorio.Instancia.ObtenerTiposContrato();
        }

        public List<METipoMensajero> ObtenerTipoMensajero()
        {
            return MeRepositorio.Instancia.ObtenerTipoMensajero();
        }
        public List<MEEstadoMensajero> ObtenerEstadosMensajero()
        {
            return MeRepositorio.Instancia.ObtenerEstadosMensajero();
        }
        public List<MEGruposLiquidacion> ObtenerGruposliquidacion(int pagina, int nRegistros, bool estado)
        {
            return MeRepositorio.Instancia.ObtenerGruposliquidacion(pagina,nRegistros,estado);
        }
        public List<MEGruposLiquidacion> ObtenerGruposliquidacionUnico()
        {
            return MeRepositorio.Instancia.ObtenerGruposliquidacionUnico();
        }

        public List<MEGrupoBasico> ObtenerGruposBasicos(int pagina, int nRegistros, bool estado)
        {
            return MeRepositorio.Instancia.ObtenerGruposBasicos(pagina,nRegistros,estado);
        }

        public List<MEGrupoRodamiento> ObtenerGruposRodamiento(string idCiudad, int pagina, int nRegistros, bool estado)
        {
            return MeRepositorio.Instancia.ObtenerGruposRodamiento(idCiudad,pagina,nRegistros,estado);
        }

        public List<MEBasicoLiquidacion> ObtenerLiquidacionBasico(int idBasico)
        {
            return MeRepositorio.Instancia.ObtenerLiquidacionBasico(idBasico);
        }
        public List<METipoTransporte> ObtenerTiposTransporte()
        {
            return MeRepositorio.Instancia.ObtenerTiposTransporte();
        }

        public bool InsertarGrupoRodamiento(MEGrupoRodamiento GrupoRodamiento)
        {
            int grupo=0;
            int i= 0;

            grupo = MeRepositorio.Instancia.InsertarGrupoRodamiento(GrupoRodamiento);

            if(grupo > 0)
            {
                foreach(METipoRodamiento item in GrupoRodamiento.TipoRodamiento)
                {
                    item.IdGrupoRodamiento = grupo;
                    var tipo = MeRepositorio.Instancia.InsertarTipoRodamiento(item);
                    if(!tipo)
                    {
                        i++;
                    }
                }
            }

            return grupo > 0 && i == 0 ? true : false;
           
        }

        public bool ModificarGrupoRodamiento(MEGrupoRodamiento GrupoRodamiento)
        {
            bool grupo = false;
            int i = 0;

            using (TransactionScope transaccion = new TransactionScope())
            {
                grupo = MeRepositorio.Instancia.ModificarGrupoRodamiento(GrupoRodamiento);

                foreach (METipoRodamiento item in GrupoRodamiento.TipoRodamiento)
                {
                    item.IdGrupoRodamiento = GrupoRodamiento.Id;
                    var tipo = MeRepositorio.Instancia.ModificarTipoRodamiento(item);
                    if (!tipo)
                    { i++; }
                }

                transaccion.Complete();
            }

            return grupo && i == 0 ? true : false;
        }

        public List<MEBasicoLiquidacion> InsertarBasico(MEGrupoBasico GrupoBasico)
        {
            List<MEBasicoLiquidacion> basicoLiq = new List<MEBasicoLiquidacion>();
            int basico;
            double anterior = GrupoBasico.ValorInicial;
            double restante = GrupoBasico.ValorInicial - GrupoBasico.ValorFinal;
            int cuotasDif = GrupoBasico.NumeroCuotas - 2;
            double actual = 0;
            int count = 0;


            using (TransactionScope transaccion = new TransactionScope())
            {
                basico = MeRepositorio.Instancia.InsertarBasico(GrupoBasico);

               
                 int cuota = 1;
                while (cuota <= GrupoBasico.NumeroCuotas)
                {
                    if (cuota == 1 || cuota == 2)
                    {
                        basicoLiq.Add(new MEBasicoLiquidacion
                        {
                            IdGrupoBasico = basico,
                            Mes = cuota,
                            Valor = GrupoBasico.ValorInicial
                        });
                    }
                    else
                    {

                        actual = anterior - (restante / cuotasDif);
                        anterior = actual;

                        basicoLiq.Add(new MEBasicoLiquidacion
                        {
                            IdGrupoBasico = basico,
                            Mes = cuota,
                            Valor = actual
                        });
                    }
                    cuota++;
                }

                foreach (MEBasicoLiquidacion item in basicoLiq)
                {
                    var tipo = MeRepositorio.Instancia.InsertarBasicoLiquidacion(item);

                    if (!tipo) { count++; }
                }

                transaccion.Complete();
            }

            //return basico && count == 0 ? true : false;
            return basicoLiq;


        }

        public List<MEBasicoLiquidacion> ModificarBasico(MEGrupoBasico GrupoBasico)
        {
            List<MEBasicoLiquidacion> basicoLiq = new List<MEBasicoLiquidacion>();
            bool basico;
            double anterior = GrupoBasico.ValorInicial;
            double restante = GrupoBasico.ValorInicial - GrupoBasico.ValorFinal;
            int cuotasDif = GrupoBasico.NumeroCuotas - 2;
            double actual = 0;
            int count = 0;


            using (TransactionScope transaccion = new TransactionScope())
            {
                
                basico = MeRepositorio.Instancia.ModificarBasico(GrupoBasico);


                int cuota = 1;
                while (cuota <= GrupoBasico.NumeroCuotas)
                {
                    if (cuota == 1 || cuota == 2)
                    {
                        basicoLiq.Add(new MEBasicoLiquidacion
                        {
                            IdGrupoBasico = GrupoBasico.IdGrupoBasico,
                            Mes = cuota,
                            Valor = GrupoBasico.ValorInicial
                        });
                    }
                    else
                    {

                        actual = anterior - (restante / cuotasDif);
                        anterior = actual;

                        basicoLiq.Add(new MEBasicoLiquidacion
                        {
                            IdGrupoBasico = GrupoBasico.IdGrupoBasico,
                            Mes = cuota,
                            Valor = actual
                        });
                    }
                    cuota++;
                }
                MeRepositorio.Instancia.EliminarBasicoLiquidacion(basicoLiq[basicoLiq.Count() - 1].IdGrupoBasico, basicoLiq.Count());

                foreach (MEBasicoLiquidacion item in basicoLiq)
                {
                    var tipo = MeRepositorio.Instancia.ModificarBasicoLiquidacion(item);

                    if (!tipo) { count++; }
                }

                transaccion.Complete();
            }

            //return basico && count == 0 ? true : false;
            return basicoLiq;

            
        }

        public List<METipoAccion> ObtenerTiposAccion()
        {
            return MeRepositorio.Instancia.ObtenerTiposAccion();
        }

        public List<METipoPenalidad> ObtenerTiposPenalidad(int pagina, int nRegistros, bool estado)
        {
            return MeRepositorio.Instancia.ObtenerTiposPenalidad(pagina,nRegistros,estado);
        }

        public List<METipoPenalidad> ObtenerTiposPenalidadesRaps()
        {
            List<METipoPenalidad> PenalidadesConfig = new List<METipoPenalidad>();

            PenalidadesConfig= MeRepositorio.Instancia.ObtenerTiposPenalidadesConfig();

            List<METipoPenalidad> ParametrosRaps = new List<METipoPenalidad>();

            ParametrosRaps = MeRepositorio.Instancia.ObtenerTiposPenalidadesRaps();

            var joinQuery = (from penalidad in PenalidadesConfig
                            join parametro in ParametrosRaps
                            on penalidad.IdParametroRaps equals parametro.IdParametroRaps
                            select new METipoPenalidad {IdPenalidadRaps=penalidad.IdPenalidadRaps,
                                                        IdParametroRaps=penalidad.IdParametroRaps,
                                                        Penalidad=parametro.Penalidad}).ToList();

            return joinQuery;
            
        }

        public List<METipoUsuario> ObtenerTiposUsuarios()
        {
            return MeRepositorio.Instancia.ObtenerTiposUsuarios();
        }

        public List<METipoRodamiento> ObtenerTiposRodamientoXGrupo(int idGrupoRodamiento)
        {
            return MeRepositorio.Instancia.ObtenerTiposRodamientoXGrupo(idGrupoRodamiento);
        }

        public List<METiposLiquidacion> ObtenerTiposLiquidacion(int idGrupoLiquidacion, int pagina, int nRegistros)
        {
            return MeRepositorio.Instancia.ObtenerTiposLiquidacion(idGrupoLiquidacion,pagina,nRegistros);
        }

        public bool InsertarGrupoLiquidacion(MEGruposLiquidacion GrupoLiquidacion)
        {
            int Grupo = 0;
            int count = 0;

            using (TransactionScope transaccion = new TransactionScope())
            {
                Grupo = MeRepositorio.Instancia.InsertarGrupoLiquidacion(GrupoLiquidacion);

                foreach (METiposLiquidacion item in GrupoLiquidacion.TiposLiquidacion)
                {
                    item.IdGrupoLiq = Grupo;
                //    item.IdTipoLiq = Grupo + "" + item.IdUnidad + "" + item.IdTipoAccion +""+item.IdFormaPago;
                   var tipo= MeRepositorio.Instancia.InsertarTipoLiquidacion(item);

                    if (!tipo) { count++; }
                }
                transaccion.Complete();
            }

            return Grupo>0 && count==0 ? true : false;

        }

        public bool InsertarTipoLiquidacion(METiposLiquidacion TipoLiquidacion)
        {
            return MeRepositorio.Instancia.InsertarTipoLiquidacion(TipoLiquidacion);
        }

        public bool ModificarGrupoLiquidacion(MEGruposLiquidacion GrupoLiquidacionn)
        {
            bool Grupo = false;
         
            using (TransactionScope transaccion = new TransactionScope()) {
                Grupo=MeRepositorio.Instancia.ModificarGrupoLiquidacion(GrupoLiquidacionn);
                foreach (METiposLiquidacion item in GrupoLiquidacionn.TiposLiquidacion)
                {
                    if(MeRepositorio.Instancia.ConsultarTipoLiquidacion(item.IdTipoLiq, item.IdGrupoLiq))
                    {
                        MeRepositorio.Instancia.ModificarTipoLiquidacion(item);
                    }
                    else { MeRepositorio.Instancia.InsertarTipoLiquidacion(item); }
                }
                transaccion.Complete();
                return true;
            }
           
        }

        public List<MEUnidadNegocioFormaPago> ObtenerUnidadesNegocioFormaPago(string idTipoAccion, int pagina, int nRegistros)
        {

            //string[] idTiposAccion = idTipoAccion.Split(',');
            //bool res = false;

           return MeRepositorio.Instancia.ObtenerUnidadesNegocioFormaPago(idTipoAccion,pagina,nRegistros);
        }

        public List<MEUnidadNegocioFormaPago> ObtenerUnidadesNegocio()
        {
            return MeRepositorio.Instancia.ObtenerUnidadesNegocio();
        }
        public List<MEUnidadNegocioFormaPago> ObtenerFormaPago(int IdTipoAccion)
        {
            return MeRepositorio.Instancia.ObtenerFormaPago(IdTipoAccion);
        }

        public bool ModificarTipoLiquidacion(METiposLiquidacion TipoLiquidacion)
        {
            return MeRepositorio.Instancia.ModificarTipoLiquidacion(TipoLiquidacion);
        }

        public List<METipoCuenta> ObtenerTipoCuenta()
        {
            return MeRepositorio.Instancia.ObtenerTipoCuenta();
        }

        public bool InsertarPenalidad(METipoPenalidad TipoPenalidad)
        {
            return MeRepositorio.Instancia.InsertarPenalidad(TipoPenalidad);
        }

        public bool ModificarPenalidad(METipoPenalidad TipoPenalidad)
        {
            return MeRepositorio.Instancia.ModificarPenalidad(TipoPenalidad);
        }

        public List<POVehiculo> ConsultaVehiculosNovasoft()
        {
            return MeRepositorio.Instancia.ConsultaVehiculosNovasoft();
        }

        public List<POVehiculo> ConsultaVehiculosNovasoftXDocumento(string idDocumento)
        {
            return MeRepositorio.Instancia.ConsultaVehiculosNovasoftXDocumento(idDocumento);
        }

        public List<METipoPAM> ObtenerTipoPAM()
        {
            return MeRepositorio.Instancia.ObtenerTipoPAM();
        }

        public bool InsertarModificarLiquidacionPersonaInterna(long idPersona, string valor, string tipo)
        {
            return MeRepositorio.Instancia.InsertarModificarLiquidacionPersonaInterna(idPersona, valor, tipo);
        }

        public bool InsertarModificarLiquidacionPersonaExterna(long idPersona, string valor, string tipo)
        {
            return MeRepositorio.Instancia.InsertarModificarLiquidacionPersonaExterna(idPersona, valor, tipo);
        }

        public double ConsultarIPC(DateTime fecha)
        {
            return MeRepositorio.Instancia.ConsultarIPC(fecha);
        }

        public double ConsultarActivo(string tabla, string campo, DateTime fechaInicio)
        {
            return MeRepositorio.Instancia.ConsultarActivo(tabla, campo,fechaInicio);
        }

        public MEConfigComisiones ConsultarConfiguracionComisionesEmpleado(int idPersona, string tabla, string campo)
        {
            return MeRepositorio.Instancia.ConsultarConfiguracionComisionesEmpleado(idPersona,tabla,campo);
        }

        public bool ModificarComisionesIPC(double IPC)
        {
            return MeRepositorio.Instancia.ModificarComisionesIPC(IPC);
        }

        public List<MEMensajero> ObtenerPersonasConfig()
        {
            return MeRepositorio.Instancia.ObtenerPersonasConfig();
        }
    }
}
