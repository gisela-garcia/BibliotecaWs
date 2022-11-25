using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BibliotecaWs.Models
{
    public class ReferenciaBibliografica
    {
        public int intid { get; set; }
        public string Titulo { get; set; }
        public string FreqPublicacion { get; set; }
        public string ComputerFile { get; set; }
        public string DescFisica { get; set; }
        public string tipo { get; set; }
        public string txtClave1 { get; set; }
        public string txtClave2 { get; set; }
        public string IDBiblioteca { get; set; }
        public string Autor { get; set; }
        public string Anio { get; set; }
        public string LugarPublicacion { get; set; } //GMM001
        public string Editorial { get; set; }
        public string Edicion { get; set; }
        public string Idioma { get; set; }
        public string ISBN { get; set; }
        public string ClaveBiblioteca { get; set; }
        public string TipoLibro { get; set; }
        public string ClaveTipoLibro { get; set; }
        public string FuenteBiblioteca { get; set; }
        public string mensaje { get; set; }
    }
}