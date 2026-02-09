using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using CO.Servidor.Rutas.Datos;
using CO.Servidor.Rutas.Datos.Modelo;
using CO.Servidor.Servicios.ContratoDatos.Rutas.Optimizacion;
using Framework.Servidor.Comun;
using Framework.Servidor.Excepciones;
using Framework.Servidor.ParametrosFW;
using QuickGraph;
using QuickGraph.Algorithms;
using QuickGraph.Algorithms.Observers;
using QuickGraph.Algorithms.RankedShortestPath;

namespace CO.Servidor.Rutas.Nacional
{
  public class GrafoRutas : ControllerBase
  {
    private static readonly GrafoRutas instancia = (GrafoRutas)FabricaInterceptores.GetProxy(new GrafoRutas(), COConstantesModulos.MODULO_RUTAS);
    private readonly ObjectPool<RUOptimizacion> pool;
    private double tiempoReciclajeGrafo;

    public static GrafoRutas Instancia
    {
      get { return GrafoRutas.instancia; }
    }

    private GrafoRutas()
    {
      int maxPool = 0;
      try
      {
        maxPool = int.Parse(RURepositorio.Instancia.ConsultarParametrosRutas("MaxPoolGrafos"));
      }
      catch { maxPool = 10; }

      pool = new ObjectPool<RUOptimizacion>(maxPool);

      try
      {
        tiempoReciclajeGrafo = double.Parse(RURepositorio.Instancia.ConsultarParametrosRutas("TempReciclajeGrafos"));
      }
      catch { tiempoReciclajeGrafo = 7200000; }
    }

    public RURutaOptimaCalculada CalcularRutaOptima(string idLocalidadOrigen, string idLocalidadDestino, DateTime? fechadmisionEnvio = null, int cantRutasABuscar = 1)
    {
      RUOptimizacion instanciaGrafo = pool.Get();
      try
      {
        return instanciaGrafo.CalcularRutaOptima(idLocalidadOrigen, idLocalidadDestino, fechadmisionEnvio, 1);
      }
      finally
      {
        DateTime fechaFin = DateTime.Now;
        TimeSpan span = fechaFin - instanciaGrafo.fechaInicioGrafo;

        //if (span.TotalMilliseconds >= tiempoReciclajeGrafo)
        //{
        //  //instanciaGrafo =  null;
        //  instanciaGrafo.CargaDatosGrafoRutas();
        //}

        pool.Put(instanciaGrafo);
      }
    }

    public RURutaOptimaCalculada CalcularRutaOptimaOmitiendoCiudades(string idLocalidadOrigen, string idLocalidadDestino, List<string> idLocalidadesAOmitir, DateTime? fechadmisionEnvio = null)
    {
      RUOptimizacion instanciaGrafo = pool.Get();
      try
      {
        return instanciaGrafo.CalcularRutaOptimaOmitiendoCiudades(idLocalidadOrigen, idLocalidadDestino, idLocalidadesAOmitir, fechadmisionEnvio);
      }
      finally
      {
        DateTime fechaFin = DateTime.Now;
        TimeSpan span = fechaFin - instanciaGrafo.fechaInicioGrafo;

        //if (span.TotalMilliseconds >= tiempoReciclajeGrafo)
        //{
        //  instanciaGrafo = null;
        //  instanciaGrafo.CargaDatosGrafoRutas();
        //}

        pool.Put(instanciaGrafo);
      }
    }

    /// <summary>
    /// Clase que contiene la lógica de negocio para el cálculo de las rutas más óptima y más larga
    /// desde una localidad origen a una localidad destino.
    /// Esta clase tiene un cache con un temporizador que actualiza las rutas cada hora
    /// </summary>
    protected class RUOptimizacion : ControllerBase
    {
      #region Atributos Privados

      private List<Vertice> verticesRed;

      private BidirectionalGraph<Vertice, TaggedEdge<Vertice, Arista>> grafo;

      private HoffmanPavleyRankedShortestPathAlgorithm<Vertice, TaggedEdge<Vertice, Arista>> AlgoritmoDeHoffman;

      private HoffmanPavleyRankedShortestPathAlgorithm<Vertice, TaggedEdge<Vertice, Arista>> AlgoritmoDeHoffmanOmitiendo;

      #endregion Atributos Privados

      #region Propiedades

      public DateTime fechaInicioGrafo;

      private BidirectionalGraph<Vertice, TaggedEdge<Vertice, Arista>> Grafo
      {
        get
        {
          return this.grafo;
        }
        set
        {
          this.grafo = value;
        }
      }

      private List<Vertice> VerticesRed
      {
        get
        {
          return this.verticesRed;
        }
        set
        {
          this.verticesRed = value;
        }
      }

      #endregion Propiedades

      #region Constructor

      public RUOptimizacion()
      {
        try
        {
          CargaDatosGrafoRutas();
        }
        catch (Exception ex)
        {
          AuditoriaTrace.EscribirAuditoria(ETipoAuditoria.Error, ex.Message, "RUT", ex);
        }
      }

      #endregion Constructor

      #region Metodos Privados

      /// <summary>
      /// Carga lo datos de las rutas de la empresa en un grafo dirigido
      /// </summary>

      public void CargaDatosGrafoRutas()
      {
        fechaInicioGrafo = DateTime.Now;

        this.grafo = new BidirectionalGraph<Vertice, TaggedEdge<Vertice, Arista>>();
        this.verticesRed = new List<Vertice>();

        BidirectionalGraph<Vertice, TaggedEdge<Vertice, Arista>> tempGraph = new BidirectionalGraph<Vertice, TaggedEdge<Vertice, Arista>>();
        List<Vertice> tempVerticesRed = new List<Vertice>();
        List<Ruta_RUT> tempRutas = new List<Ruta_RUT>();

        List<Arista> aristasRutas = RURepositorio.Instancia.ConsultarAristasRutas(out tempVerticesRed);

        //AlgoritmoDeHoffman = null;
        //this.grafo.Clear();
        //this.verticesRed.Clear();
        this.verticesRed = tempVerticesRed;

        foreach (Arista arista in aristasRutas)
        {
          tempGraph.AddVerticesAndEdge(new TaggedEdge<Vertice, Arista>(arista.VerticeOrigen, arista.VerticeDestino, arista));
        }

        this.grafo = tempGraph;
      }

      /// <summary>
      /// Calcula el número entero de un día de la semana
      /// </summary>
      /// <param name="diaSemana">Número asignado al día de la semana</param>
      /// <returns></returns>
      private int CalcularNumeroDiaSemana(DayOfWeek diaSemana)
      {
        switch (diaSemana)
        {
          case DayOfWeek.Monday:
            return 1;
          case DayOfWeek.Tuesday:
            return 2;
          case DayOfWeek.Wednesday:
            return 3;
          case DayOfWeek.Thursday:
            return 4;
          case DayOfWeek.Friday:
            return 5;
          case DayOfWeek.Saturday:
            return 6;
          case DayOfWeek.Sunday:
            return 7;
          default:
            return 0;
        }
      }

      /// <summary>
      /// Obtiene los datos de la próxima salida más cercana después de la llegada de un envío
      /// </summary>
      /// <param name="fechaLlegada">Fecha de llegada del envío</param>
      /// <param name="frecuencias">Lista de frecuencias sobre las que se hará la búsqueda</param>
      /// <returns>Frecuencia más cercana</returns>
      private Frecuencia ObtenerFrecuenciaProximaSalida(DateTime fechaLlegada, List<Frecuencia> frecuencias)
      {
        int diaLlegada = 0;
        int horaLlegada = 0;

        Frecuencia frecuenciaProximaSalida = new Frecuencia();
        diaLlegada = CalcularNumeroDiaSemana(fechaLlegada.DayOfWeek);
        horaLlegada = fechaLlegada.Hour;

        frecuencias.ForEach(f =>
        {
          if (f.HoraLlegadaDestino <= f.HoraSalidaOrigen)
            f.HoraLlegadaDestino = f.HoraLlegadaDestino.AddDays(1);
        }
          );

        if (frecuencias.Where(f => f.Dia >= diaLlegada).Where(f => f.HoraSalidaOrigen.Hour >= horaLlegada || f.Dia > diaLlegada).Count() > 0)
        {
          return frecuencias.Where(f => f.Dia >= diaLlegada).Where(f => f.HoraSalidaOrigen.Hour >= horaLlegada || f.Dia > diaLlegada).
           OrderBy(f => f.Dia).FirstOrDefault();
        }
        else if (frecuencias.Where(f => f.Dia <= diaLlegada).Where(f => f.HoraSalidaOrigen.Hour >= horaLlegada || f.Dia < diaLlegada).Count() > 0)
        {
          return frecuencias.Where(f => f.Dia <= diaLlegada).Where(f => f.HoraSalidaOrigen.Hour >= horaLlegada || f.Dia < diaLlegada).
            OrderBy(f => f.Dia).FirstOrDefault();
        }
        else if (frecuencias.Where(f => f.Dia <= diaLlegada).Count() > 0)
        {
          return frecuencias.Where(f => f.Dia <= diaLlegada).OrderBy(f => f.Dia).FirstOrDefault();
        }

        return null;
      }

      /// <summary>
      /// Cálculo de horas que tardará un envío en una estación con respecto a la hora de llegada
      /// </summary>
      /// <param name="fechaLlegadaDestino">Fecha de llegada del envio a la parada</param>
      /// <param name="frecuencia">Proxima salida de la estación</param>
      /// <param name="fechaSalida">Fecha de salida calculada. Parámetro de salida</param>
      /// <returns>Número de horas en la estación</returns>
      private decimal CalcularHorasEnEstacion(DateTime fechaLlegadaDestino, Frecuencia frecuencia, out DateTime fechaSalida, List<Frecuencia> frecuencias)
      {
        decimal tiempoHorasTotal = 0;
        int diaSemanaLlegada = 0;
        int horaDiaLlegadaEnvio;

        diaSemanaLlegada = CalcularNumeroDiaSemana(fechaLlegadaDestino.DayOfWeek);
        horaDiaLlegadaEnvio = fechaLlegadaDestino.Hour;

        if (frecuencia.Dia < diaSemanaLlegada)
          tiempoHorasTotal = (frecuencia.Dia + 7 - diaSemanaLlegada) * 24;
        else if (frecuencia.Dia > diaSemanaLlegada)
          tiempoHorasTotal = (frecuencia.Dia - diaSemanaLlegada) * 24;
        else if (frecuencia.Dia == diaSemanaLlegada && frecuencia.HoraSalidaOrigen.Hour < horaDiaLlegadaEnvio)
          tiempoHorasTotal = 7 * 24;
        else tiempoHorasTotal = 0;

        if (frecuencia.HoraSalidaOrigen.Hour <= horaDiaLlegadaEnvio && !frecuencia.Dia.Equals(diaSemanaLlegada))
          tiempoHorasTotal = tiempoHorasTotal - (horaDiaLlegadaEnvio - frecuencia.HoraSalidaOrigen.Hour);
        else if (frecuencia.HoraSalidaOrigen.Hour > horaDiaLlegadaEnvio)
          tiempoHorasTotal = tiempoHorasTotal + (frecuencia.HoraSalidaOrigen.Hour - horaDiaLlegadaEnvio);
        else
          tiempoHorasTotal = tiempoHorasTotal - (horaDiaLlegadaEnvio - frecuencia.HoraSalidaOrigen.Hour);

        fechaSalida = fechaLlegadaDestino.AddHours(decimal.ToDouble(tiempoHorasTotal));
        fechaSalida = new DateTime(fechaSalida.Year, fechaSalida.Month, fechaSalida.Day, frecuencia.HoraSalidaOrigen.Hour, frecuencia.HoraSalidaOrigen.Minute, frecuencia.HoraSalidaOrigen.Second);

        List<DateTime> Festivos = PAAdministrador.Instancia.ObtenerFestivos(fechaSalida.AddDays(-1), fechaSalida.AddDays(1), "057");
        if (Festivos.Contains(fechaSalida.Date) && fechaSalida != fechaLlegadaDestino)
        {
          frecuencia = this.ObtenerFrecuenciaProximaSalida(fechaSalida.AddHours(24 - fechaSalida.Hour), frecuencias);
          tiempoHorasTotal += CalcularHorasEnEstacion(fechaSalida, frecuencia, out fechaSalida, frecuencias);
        }

        return tiempoHorasTotal;
      }

      #endregion Metodos Privados

      #region Metodos publicos

      private CostoArista CalcularCostoArista(Arista arista)
      {
        CostoArista costoArista = null;
        decimal tiempoEnEstacion = 0;
        decimal tiempoEnTransito = 0;

        int minutosunahora = 60;

        Frecuencia fre = arista.Frecuencias.FirstOrDefault(); // this.ObtenerFrecuenciaProximaSalida(fechaLlegadaEstacion, arista.Frecuencias);

        if (fre != null)
        {
          if (fre.HoraLlegadaDestino < fre.HoraSalidaOrigen)
            fre.HoraLlegadaDestino = fre.HoraLlegadaDestino.AddDays(1);
          tiempoEnTransito = Math.Abs((decimal)(fre.HoraLlegadaDestino - fre.HoraSalidaOrigen).TotalMinutes / minutosunahora);
          tiempoEnEstacion = (decimal)arista.TiempoParadaEstacionorigen / minutosunahora;
        }
        else
        {
          tiempoEnEstacion = (decimal)arista.TiempoParadaEstacionorigen / minutosunahora;
        }

        costoArista = new CostoArista()
        {
          Arista = arista,
          HorasEnEstacionOrigen = tiempoEnEstacion,
          HorasEnTransito = tiempoEnTransito
        };

        return costoArista;
      }

      /// <summary>
      /// Calcula la ruta más optima y la ruta menos óptima desde un origen a un destino
      /// </summary>
      /// <param name="idLocalidadOrigen">Identificador de la localidad origen del envío</param>
      /// <param name="idLocalidadDestino">Identificador de la localidad destino del envío</param>
      /// <returns>ruta óptima calculada</returns>
      public RURutaOptimaCalculada CalcularRutaOptima(string idLocalidadOrigen, string idLocalidadDestino, DateTime? fechadmisionEnvio = null, int cantRutasABuscar = 1)
      {
        try
        {
          RURutaOptimaCalculada rutaOptima = new RURutaOptimaCalculada();
          Vertice CiudadOrigen;
          Vertice CiudadDestino;
          CiudadOrigen = this.verticesRed.Where(v => v.IdVertice == idLocalidadOrigen).FirstOrDefault();
          CiudadDestino = this.verticesRed.Where(v => v.IdVertice == idLocalidadDestino).FirstOrDefault();

          if (CiudadOrigen != null && CiudadDestino != null)
          {
            if (idLocalidadOrigen != idLocalidadDestino)
            {
              IEnumerable<IEnumerable<TaggedEdge<Vertice, Arista>>> caminosHoffman = null;

              bool adyacentes = grafo.ContainsEdge(CiudadOrigen, CiudadDestino);
              int cantEntradas = grafo.InEdges(CiudadDestino).Count();

              Func<TaggedEdge<Vertice, Arista>, double> weights = c =>
              {
                try
                {
                  if (adyacentes || cantEntradas > 1)
                    return 1;
                  CostoArista costo = CalcularCostoArista(c.Tag);
                  double cost = (double)(costo.HorasEnEstacionOrigen + costo.HorasEnTransito);
                  if (cost <= 0)
                    return 1;
                  return cost;
                }
                catch
                {
                  return 1;
                }
              };

              if (AlgoritmoDeHoffman == null)
                AlgoritmoDeHoffman = new HoffmanPavleyRankedShortestPathAlgorithm<Vertice, TaggedEdge<Vertice, Arista>>(this.grafo, weights);

              try
              {
                AlgoritmoDeHoffman.ShortestPathCount = cantRutasABuscar;

                AlgoritmoDeHoffman.ClearRootVertex();

                AlgoritmoDeHoffman.Compute(CiudadOrigen, CiudadDestino);
                caminosHoffman = AlgoritmoDeHoffman.ComputedShortestPaths;
              }
              catch (KeyNotFoundException)
              {
                return null;
              }
              if (fechadmisionEnvio == null)
                fechadmisionEnvio = DateTime.Now;

              rutaOptima.TodosLosCaminos = new List<Camino>();
              foreach (var caminoHoffman in caminosHoffman)
              {
                DateTime fechaLlegadaEstacion = (DateTime)fechadmisionEnvio;
                Camino caminoRutaOptima = null;
                Arista aristaAnterior = null;
                Dictionary<int, CostoArista> costosAristas = new Dictionary<int, CostoArista>();
                int contadorAristas = 0;
                foreach (var arista in caminoHoffman.ToList())
                {
                  CostoArista costoArista = null;
                  decimal tiempoEnEstacion = 0;
                  decimal tiempoEnTransito = 0;
                  DateTime fechaProximaSalida;

                  Frecuencia fre = this.ObtenerFrecuenciaProximaSalida(fechaLlegadaEstacion, arista.Tag.Frecuencias);

                  //Esto para saber si se debe tomar tiempo de parada en la estación
                  //Si la ruta anterior es diferente a la ruta que se tomará en este nuevo camino el envío debe permanecer en la estación hasta la salida de la siguiente frecuencia
                  //if (aristaAnterior == null || (aristaAnterior != null && aristaAnterior.RutaArista.IdRuta != arista.Tag.RutaArista.IdRuta))
                  //{
                  decimal minutosunahora = 60;
                  if (fre != null)
                  {
                    if (fre.HoraLlegadaDestino < fre.HoraSalidaOrigen)
                      fre.HoraLlegadaDestino = fre.HoraLlegadaDestino.AddDays(1);

                    //Horas que está el envio en la estacion
                    //tiempoEnEstacion = CalcularHorasEnEstacion(fechaLlegadaEstacion.AddHours((double)arista.Tag.TiempoParadaEstacionorigen), fre, out fechaProximaSalida);
                    tiempoEnEstacion = CalcularHorasEnEstacion(fechaLlegadaEstacion, fre, out fechaProximaSalida, arista.Tag.Frecuencias);
                    tiempoEnTransito += (decimal)(fre.HoraLlegadaDestino - fre.HoraSalidaOrigen).TotalMinutes / minutosunahora;
                  }
                  else
                  {
                    //La fecha de proxima salida será el tiempo que se demore en la estación el cambion despues de la fecha de llegada
                    tiempoEnEstacion = (decimal)arista.Tag.TiempoParadaEstacionorigen / minutosunahora;
                    fechaProximaSalida = fechaLlegadaEstacion.AddHours(decimal.ToDouble(tiempoEnEstacion));
                  }

                  //}
                  //else
                  //{
                  //  //La fecha de proxima salida será el tiempo que se demore en la estación el cambion despues de la fecha de llegada
                  //  tiempoEnEstacion = arista.Tag.TiempoParadaEstacionorigen;
                  //  fechaProximaSalida = fechaLlegadaEstacion.AddHours(decimal.ToDouble(tiempoEnEstacion));
                  //}

                  //if (fre != null)
                  //{
                  //  //Horas que está el envío en tránsito (siendo transportado desde el origen al destino)
                  //  tiempoEnTransito += (fre.HoraLlegadaDestino - fre.HoraSalidaOrigen).Hours;
                  //}
                  aristaAnterior = arista.Tag;

                  costoArista = new CostoArista()
                  {
                    Arista = arista.Tag,
                    FechaLlegadaDestino = fechaProximaSalida.AddMinutes(decimal.ToDouble(tiempoEnTransito * minutosunahora)),
                    FechaSalidaOrigen = fechaProximaSalida,
                    HorasEnEstacionOrigen = tiempoEnEstacion,
                    HorasEnTransito = tiempoEnTransito
                  };
                  costosAristas.Add(contadorAristas, costoArista);

                  fechaLlegadaEstacion = fechaProximaSalida.AddMinutes(decimal.ToDouble(tiempoEnTransito * minutosunahora));
                  contadorAristas++;
                }
                caminoRutaOptima = new Camino()
                {
                  CostosAristas = costosAristas
                };
                rutaOptima.TodosLosCaminos.Add(caminoRutaOptima);
              }
            }
            else
            {
              rutaOptima.TodosLosCaminos = new List<Camino>();
              Dictionary<int, CostoArista> costosAristas = new Dictionary<int, CostoArista>();
              costosAristas.Add(1, new CostoArista()
              {
                HorasEnEstacionOrigen = CalcularNumeroDiaSemana(DateTime.Now.DayOfWeek) == 6 ? 23 : 11,
                HorasEnTransito = 0,
                FechaLlegadaDestino = DateTime.Now.AddHours(CalcularNumeroDiaSemana(DateTime.Now.DayOfWeek) == 6 ? 23 : 11),
                FechaSalidaOrigen = DateTime.Now,
                Arista = new Arista()
                {
                  TiempoParadaEstacionorigen = 12,
                  VerticeOrigen = new Vertice()
                  {
                    IdVertice = idLocalidadOrigen,
                    Descripcion = CiudadOrigen.Descripcion
                  },
                  VerticeDestino = new Vertice()
                  {
                    IdVertice = idLocalidadDestino,
                    Descripcion = CiudadDestino.Descripcion
                  }
                }
              });
              rutaOptima.TodosLosCaminos.Add(new Camino()
              {
                CostosAristas = costosAristas
              });
            }
            rutaOptima.Origen = CiudadOrigen;
            rutaOptima.Destino = CiudadDestino;

            rutaOptima.CaminoMasCorto = rutaOptima.TodosLosCaminos.OrderBy(camino => camino.CostosAristas.Sum(costo => costo.Value.HorasEnEstacionOrigen + costo.Value.HorasEnTransito)).FirstOrDefault();
            rutaOptima.CaminoMasLargo = rutaOptima.TodosLosCaminos.OrderByDescending(camino => camino.CostosAristas.Sum(costo => costo.Value.HorasEnEstacionOrigen + costo.Value.HorasEnTransito)).FirstOrDefault();
          }
          return rutaOptima;

          //}
        }
        catch (Exception ex)
        {
          AuditoriaTrace.EscribirAuditoria(ETipoAuditoria.Error, ex.Message, "RUT", ex);
          return null;
        }
      }

      /// <summary>
      /// Calcula la ruta más optima y la ruta menos óptima desde un origen a un destino
      /// </summary>
      /// <param name="idLocalidadOrigen">Identificador de la localidad origen del envío</param>
      /// <param name="idLocalidadDestino">Identificador de la localidad destino del envío</param>
      /// <returns>ruta óptima calculada</returns>
      public RURutaOptimaCalculada CalcularRutaOptimaOmitiendoCiudades(string idLocalidadOrigen, string idLocalidadDestino, List<string> idLocalidadesAOmitir, DateTime? fechadmisionEnvio = null)
      {
        try
        {
          RURutaOptimaCalculada rutaOptima = new RURutaOptimaCalculada();
          Vertice CiudadOrigen;
          Vertice CiudadDestino;
          CiudadOrigen = this.verticesRed.Where(v => v.IdVertice == idLocalidadOrigen).FirstOrDefault();
          CiudadDestino = this.verticesRed.Where(v => v.IdVertice == idLocalidadDestino).FirstOrDefault();

          if (CiudadOrigen != null && CiudadDestino != null)
          {
            if (idLocalidadOrigen != idLocalidadDestino)
            {
              IEnumerable<IEnumerable<TaggedEdge<Vertice, Arista>>> caminosHoffman = null;

              bool adyacentes = grafo.ContainsEdge(CiudadOrigen, CiudadDestino);
              int cantEntradas = grafo.InEdges(CiudadDestino).Count();

              Func<TaggedEdge<Vertice, Arista>, double> weights = c =>
              {
                try
                {
                  if (idLocalidadesAOmitir.Contains(c.Source.IdVertice) || idLocalidadesAOmitir.Contains(c.Target.IdVertice))
                    return 50000000;
                  if (adyacentes || cantEntradas > 1)
                    return 1;
                  CostoArista costo = CalcularCostoArista(c.Tag);
                  double cost = (double)(costo.HorasEnEstacionOrigen + costo.HorasEnTransito);
                  if (cost <= 0)
                    return 1;
                  return cost;
                }
                catch
                {
                  return 1;
                }
              };

              if (AlgoritmoDeHoffmanOmitiendo == null)
                AlgoritmoDeHoffmanOmitiendo = new HoffmanPavleyRankedShortestPathAlgorithm<Vertice, TaggedEdge<Vertice, Arista>>(this.grafo, weights);

              try
              {
                AlgoritmoDeHoffmanOmitiendo.ShortestPathCount = 1;

                AlgoritmoDeHoffmanOmitiendo.ClearRootVertex();

                AlgoritmoDeHoffmanOmitiendo.Compute(CiudadOrigen, CiudadDestino);
                caminosHoffman = AlgoritmoDeHoffmanOmitiendo.ComputedShortestPaths;
              }
              catch (KeyNotFoundException)
              {
                return null;
              }
              if (fechadmisionEnvio == null)
                fechadmisionEnvio = DateTime.Now;

              rutaOptima.TodosLosCaminos = new List<Camino>();
              foreach (var caminoHoffman in caminosHoffman)
              {
                DateTime fechaLlegadaEstacion = (DateTime)fechadmisionEnvio;
                Camino caminoRutaOptima = null;
                Arista aristaAnterior = null;
                Dictionary<int, CostoArista> costosAristas = new Dictionary<int, CostoArista>();
                int contadorAristas = 0;
                foreach (var arista in caminoHoffman.ToList())
                {
                  CostoArista costoArista = null;
                  decimal tiempoEnEstacion = 0;
                  decimal tiempoEnTransito = 0;
                  DateTime fechaProximaSalida;

                  Frecuencia fre = this.ObtenerFrecuenciaProximaSalida(fechaLlegadaEstacion, arista.Tag.Frecuencias);

                  //Esto para saber si se debe tomar tiempo de parada en la estación
                  //Si la ruta anterior es diferente a la ruta que se tomará en este nuevo camino el envío debe permanecer en la estación hasta la salida de la siguiente frecuencia
                  //if (aristaAnterior == null || (aristaAnterior != null && aristaAnterior.RutaArista.IdRuta != arista.Tag.RutaArista.IdRuta))
                  //{
                  decimal minutosunahora = 60;
                  if (fre != null)
                  {
                    if (fre.HoraLlegadaDestino < fre.HoraSalidaOrigen)
                      fre.HoraLlegadaDestino = fre.HoraLlegadaDestino.AddDays(1);

                    //Horas que está el envio en la estacion
                    //tiempoEnEstacion = CalcularHorasEnEstacion(fechaLlegadaEstacion.AddHours((double)arista.Tag.TiempoParadaEstacionorigen), fre, out fechaProximaSalida);
                    tiempoEnEstacion = CalcularHorasEnEstacion(fechaLlegadaEstacion, fre, out fechaProximaSalida, arista.Tag.Frecuencias);
                    tiempoEnTransito += (decimal)(fre.HoraLlegadaDestino - fre.HoraSalidaOrigen).TotalMinutes / minutosunahora;
                  }
                  else
                  {
                    //La fecha de proxima salida será el tiempo que se demore en la estación el cambion despues de la fecha de llegada
                    tiempoEnEstacion = (decimal)arista.Tag.TiempoParadaEstacionorigen / minutosunahora;
                    fechaProximaSalida = fechaLlegadaEstacion.AddHours(decimal.ToDouble(tiempoEnEstacion));
                  }

                  //}
                  //else
                  //{
                  //  //La fecha de proxima salida será el tiempo que se demore en la estación el cambion despues de la fecha de llegada
                  //  tiempoEnEstacion = arista.Tag.TiempoParadaEstacionorigen;
                  //  fechaProximaSalida = fechaLlegadaEstacion.AddHours(decimal.ToDouble(tiempoEnEstacion));
                  //}

                  //if (fre != null)
                  //{
                  //  //Horas que está el envío en tránsito (siendo transportado desde el origen al destino)
                  //  tiempoEnTransito += (fre.HoraLlegadaDestino - fre.HoraSalidaOrigen).Hours;
                  //}
                  aristaAnterior = arista.Tag;

                  costoArista = new CostoArista()
                  {
                    Arista = arista.Tag,
                    FechaLlegadaDestino = fechaProximaSalida.AddMinutes(decimal.ToDouble(tiempoEnTransito * minutosunahora)),
                    FechaSalidaOrigen = fechaProximaSalida,
                    HorasEnEstacionOrigen = tiempoEnEstacion,
                    HorasEnTransito = tiempoEnTransito
                  };
                  costosAristas.Add(contadorAristas, costoArista);

                  fechaLlegadaEstacion = fechaProximaSalida.AddMinutes(decimal.ToDouble(tiempoEnTransito * minutosunahora));
                  contadorAristas++;
                }
                caminoRutaOptima = new Camino()
                {
                  CostosAristas = costosAristas
                };
                rutaOptima.TodosLosCaminos.Add(caminoRutaOptima);
              }
            }
            else
            {
              rutaOptima.TodosLosCaminos = new List<Camino>();
              Dictionary<int, CostoArista> costosAristas = new Dictionary<int, CostoArista>();
              costosAristas.Add(1, new CostoArista()
              {
                HorasEnEstacionOrigen = CalcularNumeroDiaSemana(DateTime.Now.DayOfWeek) == 6 ? 23 : 11,
                HorasEnTransito = 0,
                FechaLlegadaDestino = DateTime.Now.AddHours(CalcularNumeroDiaSemana(DateTime.Now.DayOfWeek) == 6 ? 23 : 11),
                FechaSalidaOrigen = DateTime.Now,
                Arista = new Arista()
                {
                  TiempoParadaEstacionorigen = 12,
                  VerticeOrigen = new Vertice()
                  {
                    IdVertice = idLocalidadOrigen,
                    Descripcion = CiudadOrigen.Descripcion
                  },
                  VerticeDestino = new Vertice()
                  {
                    IdVertice = idLocalidadDestino,
                    Descripcion = CiudadDestino.Descripcion
                  }
                }
              });
              rutaOptima.TodosLosCaminos.Add(new Camino()
              {
                CostosAristas = costosAristas
              });
            }
            rutaOptima.Origen = CiudadOrigen;
            rutaOptima.Destino = CiudadDestino;

            rutaOptima.CaminoMasCorto = rutaOptima.TodosLosCaminos.OrderBy(camino => camino.CostosAristas.Sum(costo => costo.Value.HorasEnEstacionOrigen + costo.Value.HorasEnTransito)).FirstOrDefault();
            rutaOptima.CaminoMasLargo = rutaOptima.TodosLosCaminos.OrderByDescending(camino => camino.CostosAristas.Sum(costo => costo.Value.HorasEnEstacionOrigen + costo.Value.HorasEnTransito)).FirstOrDefault();
          }
          return rutaOptima;

          //}
        }
        catch (Exception ex)
        {
          AuditoriaTrace.EscribirAuditoria(ETipoAuditoria.Error, ex.Message, "RUT", ex);
          return null;
        }
      }

      #endregion Metodos publicos
    }

    /// <summary>
    /// Represents a pool of objects with a size limit.
    /// </summary>
    /// <typeparam name="T">The type of object in the pool.</typeparam>
    protected sealed class ObjectPool<T> : IDisposable
        where T : new()
    {
      private readonly int size;
      private readonly object locker;
      private readonly Queue<T> queue;
      private int count;

      /// <summary>
      /// Initializes a new instance of the ObjectPool class.
      /// </summary>
      /// <param name="size">The size of the object pool.</param>
      public ObjectPool(int size)
      {
        if (size <= 0)
        {
          const string message = "The size of the pool must be greater than zero.";
          throw new ArgumentOutOfRangeException("size", size, message);
        }

        this.size = size;
        locker = new object();
        queue = new Queue<T>();
      }

      /// <summary>
      /// Retrieves an item from the pool.
      /// </summary>
      /// <returns>The item retrieved from the pool.</returns>
      public T Get()
      {
        lock (locker)
        {
          if (queue.Count > 0)
          {
            return queue.Dequeue();
          }

          count++;
          return new T();
        }
      }

      /// <summary>
      /// Places an item in the pool.
      /// </summary>
      /// <param name="item">The item to place to the pool.</param>
      public void Put(T item)
      {
        lock (locker)
        {
          if (count < size)
          {
            queue.Enqueue(item);
          }
          else
          {
            using (item as IDisposable)
            {
              count--;
            }
          }
        }
      }

      /// <summary>
      /// Disposes of items in the pool that implement IDisposable.
      /// </summary>
      public void Dispose()
      {
        lock (locker)
        {
          count = 0;
          while (queue.Count > 0)
          {
            using (queue.Dequeue() as IDisposable)
            {
            }
          }
        }
      }
    }
  }
}