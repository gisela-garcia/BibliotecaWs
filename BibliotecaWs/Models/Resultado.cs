using System.Collections.Generic;

namespace BibliotecaWs.Models
{
    public class Resultado
    {
        public int Codigo { get; set; }
        public string Respuesta { get; set; }
        public List<ReferenciaBibliografica> Datos { get; set; }
    }

}