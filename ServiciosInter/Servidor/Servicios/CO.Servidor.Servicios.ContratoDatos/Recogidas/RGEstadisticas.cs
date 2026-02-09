using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CO.Servidor.Servicios.ContratoDatos.Recogidas
{
    public class RGGraficaLineal
    {
        public RGGraficaLineal()
        {
            this.Data = new List<List<RGDictionary>>();
        }

        public string Titulo { get; set; }
        public string Descripcion { get; set; }
        public List<List<RGDictionary>> Data { get; set; }
        public string Color { get; set; }
    }

    public class RGGraficaDonnaPie
    {
        public RGGraficaDonnaPie() {
            this.Axis = new List<RGDictionary>();
        }

        public string Titulo { get; set; }
        public List<RGDictionary> Axis { get; set; }
    }

    public class RGBestSellingChart
    {
        public RGBestSellingChart()
        {
            this.Axis = new List<List<RGDictionary>>();
        }

        public string Titulo { get; set; }
        public List<List<RGDictionary>> Axis { get; set; }
    }

    public class RGEstadisticas
    {
        public RGEstadisticas()
        {
            this.graficoLineal = new RGGraficaLineal();
            this.graficosDonnut = new List<RGGraficaDonnaPie>();
            this.chartResults = new List<RGBestSellingChart>();
            this.smallCharts = new List<RGDictionary>();
        }

        public RGGraficaLineal graficoLineal { get; set; }
        public List<RGGraficaDonnaPie> graficosDonnut { get; set; }
        public List<RGBestSellingChart> chartResults { get; set; }
        public List<RGDictionary> smallCharts { get; set; }
    }

    public class RGDictionary
    {
        public string Name { get; set; }
        public string Value { get; set; }
        public string Color { get; set; }
    }
}
