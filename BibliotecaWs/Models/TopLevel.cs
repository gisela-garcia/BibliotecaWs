using BibliotecaWs.Models;
using System.Collections.Generic;

namespace BibliotecaWs.Models
{
    public class TopLevel
    {
        public Jsonapi jsonapi { get; set; }
        public List<ReferenciaBibliografica> data { get; set; }
    }
}