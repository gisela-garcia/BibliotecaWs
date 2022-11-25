using BibliotecaWs.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Xml;
using Zoom.Net.YazSharp;
using Zoom.Net;
using System.Net;
using System.Configuration;
using System.Web.UI.WebControls;

namespace BibliotecaWs.Clases
{
    public class DataAccess
    {
        private static string QueryText = null;
        private static string ZQuery = null;
        private static string ClaveBiblioteca = null;
        private static string TipoMaterial = null;
        private static string Titulo = null;
        private static string FreqPublicacion = null;
        private static string ComputerFile = null;
        private static string DescFisica = null;
        private static string tipo = null;
        private static string txtClave1 = null;
        private static string txtClave2 = null;
        private static string IDBiblioteca = null;
        private static string Autor = null;
        private static string LugarPublicacion = null; //GMM001
        private static string Editorial = null;
        private static string Edicion = null;
        private static string Idioma = null;
        private static string ISBN = null;
        private static string ClaveTipoLibro = null;
        private static string anio = null;

        public Resultado ConsultaBiblioteca(string BusquedaTitulo, string BusquedaAutor, string BusquedaEdicion)
        {
            int MaximoResultados = 100;
            string reveditorial = "";
            List<ReferenciaBibliografica> listalibro = new List<ReferenciaBibliografica>();

            try
            {
                string Z3950_SERVER = ConfigurationManager.AppSettings.Get("ServerBiblioteca"); // "millenium.itesm.mx";     //"ustlib.ust.hk";
                int Z3950_PORT = Convert.ToInt32(ConfigurationManager.AppSettings.Get("PortBiblioteca"));
                var nameBd = ConfigurationManager.AppSettings.Get("nameBd");
                Connection conn = new Connection(Z3950_SERVER, Z3950_PORT);
                conn.DatabaseName = nameBd;
                conn.Syntax = Zoom.Net.RecordSyntax.XML;

                BusquedaTitulo = BusquedaTitulo is null ? null : DepurarTextoBusqueda(BusquedaTitulo);
                BusquedaAutor = BusquedaAutor is null ? null :DepurarTextoBusqueda(BusquedaAutor);

                if (string.IsNullOrEmpty(BusquedaEdicion))
                {
                    if (!string.IsNullOrEmpty(BusquedaTitulo) & !string.IsNullOrEmpty(BusquedaAutor))
                    {
                        ZQuery = "@and @attr 1=1003 @attr 2=3 @attr 3=3 @attr 4=6 @attr 5=100 @attr 6=1 \"" + BusquedaAutor + "\"" + " @attr 1=4 @attr 2=3 @attr 3=3 @attr 4=6 @attr 5=100 @attr 6=1 \"" + BusquedaTitulo + "\"";
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(BusquedaTitulo))
                        {
                            ZQuery = "@attr 1=4 @attr 2=3 @attr 3=3 @attr 4=6 @attr 5=100 @attr 6=1 \"" + BusquedaTitulo + "\"";
                        }
                        else
                        {
                            ZQuery = "@attr 1=1003 @attr 2=3 @attr 3=3 @attr 4=6 @attr 5=100 @attr 6=1 \"" + BusquedaAutor + "\"";
                        }
                    }
                }
                else
                {
                    // en caso de que se agregue el año de edición
                    if (!string.IsNullOrEmpty(BusquedaTitulo) & !string.IsNullOrEmpty(BusquedaAutor))
                    {
                        ZQuery = "@and @and @attr 1=1003 @attr 2=3 @attr 3=3 @attr 4=6 @attr 5=100 @attr 6=1 \"" + BusquedaAutor + "\"" + " @attr 1=4 @attr 2=3 @attr 3=3 @attr 4=6 @attr 5=100 @attr 6=1 \"" + BusquedaTitulo + "\"" + " @attr 1=30  @attr 2=3 " + BusquedaEdicion;
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(BusquedaTitulo))
                        {
                            ZQuery = "@and @attr 1=4 @attr 2=3 @attr 3=3 @attr 4=6 @attr 5=100 @attr 6=1 \"" + BusquedaTitulo + "\" " + " @attr 1=30  @attr 2=3 " + BusquedaEdicion;
                        }
                        else
                        {
                            ZQuery = "@and @attr 1=1003 @attr 2=3 @attr 3=3 @attr 4=6 @attr 5=100 @attr 6=1 \"" + BusquedaAutor + "\" " + " @attr 1=30  @attr 2=3 " + BusquedaEdicion;
                        }
                    }

                }

                Zoom.Net.YazSharp.PrefixQuery _prefixQuery = new PrefixQuery(ZQuery);
                IResultSet iresults = (ResultSet)conn.Search(_prefixQuery);

                object rr = iresults;
                if ((int)iresults.Size < MaximoResultados)
                    MaximoResultados = (int)iresults.Size;

                if ((int)iresults.Size > 0)
                {
                    for (int Contador = 0; Contador <= MaximoResultados - 1; Contador++)
                    {
                        resetVariables();
                        uint i = (uint)Contador;
                        QueryText = Encoding.UTF8.GetString(iresults[i].Content);

                        if (QueryText.Contains((char)(0x1F)))
                            QueryText = QueryText.Replace((char)(0x1F), ' ');

                        if (QueryText.Contains((char)('&')))
                            QueryText = QueryText.Replace((char)('&'), 'y');

                        string removeString = "><c";
                        int index = QueryText.IndexOf(removeString);
                        QueryText = (index < 0) ? QueryText : QueryText.Remove(index + 1, "c".Length);

                        removeString = "<<";
                        index = QueryText.IndexOf(removeString);
                        QueryText = (index < 0) ? QueryText : QueryText.Replace("<<", "[");

                        removeString = ">>";
                        index = QueryText.IndexOf(removeString);
                        QueryText = (index < 0) ? QueryText : QueryText.Replace(">>", "]");

                        removeString = "</dublin-core-simple>";
                        index = QueryText.IndexOf(removeString);
                        QueryText = (index < 0) ? QueryText : QueryText.Remove(index, removeString.Length);

                        XmlDocument doc = new XmlDocument { XmlResolver = null }; ///();
                        doc.LoadXml(QueryText);
                        XmlNodeList libro = doc.SelectNodes("record-list/dc-record");   //<record-list> <dc-record>
                        XmlNode texto = libro.Item(0);

                        if (QueryText.Contains("<title>"))
                        {
                            if (texto.SelectSingleNode("title").InnerText.Contains('/'))
                                Titulo = texto.SelectSingleNode("title").InnerText.Split('/')[0].Trim();
                            else
                                Titulo = texto.SelectSingleNode("title").InnerText;

                            if (Titulo.Contains(" : b"))
                                Titulo = Titulo.Replace(" : b", " : ");

                            if (Titulo.Contains(": b"))
                                Titulo = Titulo.Replace(": b", ": ");
                        }

                        if (string.IsNullOrEmpty(Titulo))
                            Titulo = "n/d";

                        if (QueryText.Contains("<description>"))   // if (QueryText.Contains("<description>"))
                            DescFisica = texto.SelectSingleNode("description").InnerText;   //GetField("//marc:datafield[@tag='300']", "xmlns:marc='http://www.loc.gov/MARC21/slim'"), " | ");

                        if (QueryText.Contains("<type>"))   // if (QueryText.Contains("<description>"))
                            tipo = texto.SelectSingleNode("type").InnerText;

                        if (tipo == "")
                            tipo = null;

                        //txtClave1 = texto.SelectSingleNode("050/a").InnerText;   //GetField("//marc:datafield[@tag='050']", "xmlns:marc='http://www.loc.gov/MARC21/slim'"), ".");
                        //txtClave1 = txtClave1.Replace("..", ".");

                        //txtClave2 = texto.SelectSingleNode("090/a").InnerText;   //GetField("//marc:datafield[@tag='090']", "xmlns:marc='http://www.loc.gov/MARC21/slim'"), ".");
                        //txtClave2 = txtClave2.Replace("..", ".");

                        if (QueryText.Contains("<creator>"))
                            Autor = texto.SelectSingleNode("creator").InnerText;   


                        if (QueryText.Contains("<publisher>"))
                        {
                            if (texto.SelectSingleNode("publisher").InnerText.Trim() != string.Empty)
                            {
                                reveditorial = texto.SelectSingleNode("publisher").InnerText;
                                if (reveditorial.IndexOf(':') > 0)
                                {
                                    LugarPublicacion = texto.SelectSingleNode("publisher").InnerText.Split(':')[0].Trim();
                                    Editorial = reveditorial.Substring(reveditorial.LastIndexOf(':') + 1).Trim();
                                    if (Editorial.IndexOf('b') == 0)
                                        Editorial = Editorial.Substring(1).Trim();
                                }
                                else
                                    LugarPublicacion = texto.SelectSingleNode("publisher").InnerText;
                            }
                        }

                        if (QueryText.Contains("<date>"))
                        {
                            anio = texto.SelectSingleNode("date").InnerText;   //GetField("//marc:datafield[@tag='250']", "xmlns:marc='http://www.loc.gov/MARC21/slim'"), "");
                            if (anio.Contains('c'))
                                anio = anio.Substring(1);

                            if (anio.Contains('.'))
                                anio = anio.Substring(0, anio.LastIndexOf('.'));
                        }
                        if (QueryText.Contains("<edition>"))
                            Edicion = texto.SelectSingleNode("edition").InnerText;   //GetField("//marc:datafield[@tag='250']", "xmlns:marc='http://www.loc.gov/MARC21/slim'"), "");

                        if (QueryText.Contains("<language>"))
                            Idioma = texto.SelectSingleNode("language").InnerText;   //GetField("//marc:datafield[@tag='041']", "xmlns:marc='http://www.loc.gov/MARC21/slim'"), "");

                        if (!string.IsNullOrEmpty(DescFisica))
                        {
                            if (DescFisica.ToUpper().Contains("MIN") || DescFisica.ToUpper().Contains("VHS") || DescFisica.ToUpper().Contains("DVD"))
                            {
                                TipoMaterial = "Video";
                            }
                            else if (DescFisica.ToUpper().Contains("CD-ROM"))
                            {
                                TipoMaterial = "Software";
                            }
                            else if (DescFisica.ToUpper().Contains("COMPUTER DATA.") || !string.IsNullOrEmpty(ComputerFile))
                            {
                                TipoMaterial = "Recurso Electrónico";
                            }
                            else if (!string.IsNullOrEmpty(FreqPublicacion) || Titulo.ToUpper().Contains("REVISTA") || Titulo.ToUpper().Contains("MAGAZINE") || DescFisica.ToUpper().Contains("FASC."))
                            {
                                TipoMaterial = "Revista";
                            }
                            else
                            {
                                TipoMaterial = "Libro";
                            }
                        }
                        else
                            TipoMaterial = "Libro";


                        if (TipoMaterial == "Libro")
                        {
                            if (!string.IsNullOrEmpty(txtClave1))
                            {
                                ClaveBiblioteca = txtClave1;
                            }
                            else if (!string.IsNullOrEmpty(txtClave2))
                            {
                                ClaveBiblioteca = txtClave2;
                            }
                        }

                        listalibro.Add(new ReferenciaBibliografica
                        {
                            intid = Contador + 1,
                            Titulo = Titulo,
                            FreqPublicacion = FreqPublicacion,
                            ComputerFile = ComputerFile,
                            DescFisica = DescFisica,
                            tipo = tipo,    // MRG
                            txtClave1 = txtClave1,
                            txtClave2 = txtClave2,
                            IDBiblioteca = IDBiblioteca,
                            Autor = Autor,
                            Anio = anio,
                            LugarPublicacion = LugarPublicacion,
                            Editorial = Editorial,
                            Edicion = Edicion,
                            Idioma = Idioma,
                            ISBN = ISBN,
                            ClaveBiblioteca = ClaveBiblioteca,
                            TipoLibro = TipoMaterial,
                            ClaveTipoLibro = ClaveTipoLibro,
                            FuenteBiblioteca = "1"  //"Biblioteca"
                        });

                    } // END FOR
                }
                else
                {
                    listalibro.Add(new ReferenciaBibliografica { mensaje = "No se encuentran libros de acuerdo al criterio." });
                }

                Resultado resultado = new Resultado
                {
                    Codigo = (int)HttpStatusCode.OK,
                    Respuesta = "Operación Exitosa",
                    Datos = listalibro
                };

                return resultado; //xmlBibliografia.xml;
            }  // END TRY

            catch (XmlException xmlex)
            {
                listalibro.Add(new ReferenciaBibliografica { mensaje = "Error al generar el xml: " + xmlex.Message });
                Resultado resultadoFall = new Resultado
                {
                    Codigo = (int)HttpStatusCode.InternalServerError,
                    Respuesta = xmlex.Message
                };
                return resultadoFall;
            }
            catch (Zoom.Net.ZoomImplementationException exx)
            {
                listalibro.Add(new ReferenciaBibliografica { mensaje = "Error en conexión Zoom: " + exx.Data + "-" + exx.Message });
                Resultado resultadoFall = new Resultado
                {
                    Codigo = (int)HttpStatusCode.InternalServerError,
                    Respuesta = exx.Message
                };
                return resultadoFall;
            }
            catch (Exception ex)
            {
                listalibro.Add(new ReferenciaBibliografica { mensaje = "Error: " + ex.Data + " - " + ex.Message });
                Resultado resultadoFall = new Resultado
                {
                    Codigo = (int)HttpStatusCode.InternalServerError,
                    Respuesta = ex.Message
                };
                return resultadoFall;
            }
        }
        public Resultado ConsultaBibliotecaCongreso(string BusquedaTitulo, string BusquedaAutor, string BusquedaEdicion)
        {
            int Contador = 0;
            int MaximoResultados = 100;
            List<ReferenciaBibliografica> listalibro = new List<ReferenciaBibliografica>();

            try
            {
                string Z3950_SERVER = ConfigurationManager.AppSettings.Get("ServerBibliotecaCongreso"); // "millenium.itesm.mx";     //"ustlib.ust.hk";
                int Z3950_PORT = Convert.ToInt32(ConfigurationManager.AppSettings.Get("PortBibliotecaCongreso"));
                var NameBd = ConfigurationManager.AppSettings.Get("NameBdCongreso");

                Connection conn = new Connection(Z3950_SERVER, Z3950_PORT);
                conn.DatabaseName = NameBd;
                conn.Syntax = Zoom.Net.RecordSyntax.XML;

                BusquedaTitulo = BusquedaTitulo is null ? null : DepurarTextoBusqueda(BusquedaTitulo);
                BusquedaAutor = BusquedaAutor is null ? null : DepurarTextoBusqueda(BusquedaAutor);
                
                if (string.IsNullOrEmpty(BusquedaEdicion))
                {
                    if (!string.IsNullOrEmpty(BusquedaTitulo) & !string.IsNullOrEmpty(BusquedaAutor))
                    {
                        ZQuery = "@and @attr 1=1003 @attr 2=3 @attr 3=3 @attr 4=6 @attr 5=100 @attr 6=1 \"" + BusquedaAutor + "\"" + " @attr 1=4 @attr 2=3 @attr 3=3 @attr 4=6 @attr 5=100 @attr 6=1 \"" + BusquedaTitulo + "\"";
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(BusquedaTitulo))
                        {
                            ZQuery = "@attr 1=4 @attr 2=3 @attr 3=3 @attr 4=6 @attr 5=100 @attr 6=1 \"" + BusquedaTitulo + "\"";
                        }
                        else
                        {
                            ZQuery = "@attr 1=1003 @attr 2=3 @attr 3=3 @attr 4=6 @attr 5=100 @attr 6=1 \"" + BusquedaAutor + "\"";
                        }
                    }
                }
                else
                {
                    // en caso de que se agregue el año de edición
                    if (!string.IsNullOrEmpty(BusquedaTitulo) & !string.IsNullOrEmpty(BusquedaAutor))
                    {
                        ZQuery = "@and @and @attr 1=1003 @attr 2=3 @attr 3=3 @attr 4=6 @attr 5=100 @attr 6=1 \"" + BusquedaAutor + "\"" + " @attr 1=4 @attr 2=3 @attr 3=3 @attr 4=6 @attr 5=100 @attr 6=1 \"" + BusquedaTitulo + "\"" + " @attr 1=30  @attr 2=3 " + BusquedaEdicion;
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(BusquedaTitulo))
                        {
                            ZQuery = "@and @attr 1=4 @attr 2=3 @attr 3=3 @attr 4=6 @attr 5=100 @attr 6=1 \"" + BusquedaTitulo + "\" " + " @attr 1=30  @attr 2=3 " + BusquedaEdicion;
                        }
                        else
                        {
                            ZQuery = "@and @attr 1=1003 @attr 2=3 @attr 3=3 @attr 4=6 @attr 5=100 @attr 6=1 \"" + BusquedaAutor + "\" " + " @attr 1=30  @attr 2=3 " + BusquedaEdicion;
                        }
                    }

                }

                Zoom.Net.YazSharp.PrefixQuery _prefixQuery = new PrefixQuery(ZQuery);
                IResultSet iresults = (ResultSet)conn.Search(_prefixQuery);

                object rr = iresults;
                if ((int)iresults.Size < MaximoResultados)
                    MaximoResultados = (int)iresults.Size;

                if ((int)iresults.Size > 0)
                {
                    for (Contador = 0; Contador <= MaximoResultados - 1; Contador++)
                    {
                        resetVariables();
                        uint i = (uint)Contador;
                        QueryText = Encoding.UTF8.GetString(iresults[i].Content);

                        if (QueryText.Contains((char)(0x1F)))
                            QueryText = QueryText.Replace((char)(0x1F), ' ');

                        XmlDocument doc = new XmlDocument();
                        doc.LoadXml(QueryText);

                        XmlNodeList librodatos = doc.GetElementsByTagName("record");  //doc.SelectNodes("//record");   //<record-list> <dc-record>
                        XmlNodeList datos = ((XmlElement)librodatos[0]).GetElementsByTagName("datafield");
                        //XmlNode texto = datos.Item(0);   //libro.Item(0);

                        foreach (XmlElement element in datos)
                        {
                            string tag = element.GetAttribute("tag");

                            if (tag == "245") //titulo principal
                            {
                                Titulo = element.FirstChild.ChildNodes[0].InnerText;
                            }
                            if (Titulo == null && tag == "222 ") //titulo revistas
                            {
                                Titulo = element.FirstChild.ChildNodes[0].InnerText;
                            }

                            if (tag == "310")  // Current Publication Frequency
                                FreqPublicacion = element.FirstChild.ChildNodes[0].InnerText;

                            if (tag == "256")  //Computer File Characteristics
                                ComputerFile = element.FirstChild.ChildNodes[0].InnerText;

                            if (tag == "300") //Physical Description
                                DescFisica = element.FirstChild.ChildNodes[0].InnerText;

                            if (tag == "050")  // Library of Congress Call Number
                                txtClave1 = element.FirstChild.ChildNodes[0].InnerText;

                            if (tag == "090") //Local Call Numbers
                                txtClave2 = element.FirstChild.ChildNodes[0].InnerText;

                            if (tag == "100") //Personal Name
                            {
                                Autor = element.FirstChild.ChildNodes[0].InnerText;  //Forename
                                if (Autor == null)
                                    Autor = element.FirstChild.ChildNodes[3].InnerText; //Family name
                            }
                            if (Autor == null && tag == "110")  // Corporate Name
                                Autor = element.FirstChild.ChildNodes[0].InnerText; //Inverted name


                            if (tag == "260")   // OJO   
                                Editorial = element.FirstChild.ChildNodes[0].InnerText;

                            if (tag == "250")   //OK
                                Edicion = element.FirstChild.ChildNodes[0].InnerText;

                            if (tag == "041")
                                Idioma = element.FirstChild.ChildNodes[0].InnerText;

                            if (tag == "010")
                                IDBiblioteca = element.FirstChild.ChildNodes[0].InnerText;

                            if (tag == "020")   //Ok
                                ISBN = element.FirstChild.ChildNodes[0].InnerText;
                        }

                        if (string.IsNullOrEmpty(Titulo))
                            Titulo = "n/d";

                        if (!string.IsNullOrEmpty(DescFisica))
                        {
                            if (DescFisica.ToUpper().Contains("MIN") || DescFisica.ToUpper().Contains("VHS") || DescFisica.ToUpper().Contains("DVD"))
                            {
                                TipoMaterial = "Video";
                            }
                            else if (DescFisica.ToUpper().Contains("CD-ROM"))
                            {
                                TipoMaterial = "Software";
                            }
                            else if (DescFisica.ToUpper().Contains("COMPUTER DATA.") || !string.IsNullOrEmpty(ComputerFile))
                            {
                                TipoMaterial = "Recurso Electrónico";
                            }
                            else if (!string.IsNullOrEmpty(FreqPublicacion) || Titulo.ToUpper().Contains("REVISTA") || Titulo.ToUpper().Contains("MAGAZINE") || DescFisica.ToUpper().Contains("FASC."))
                            {
                                TipoMaterial = "Revista";
                            }
                            else
                            {
                                TipoMaterial = "Libro";
                            }
                        }
                        else
                            TipoMaterial = "Libro";

                        if (TipoMaterial == "Libro")
                        {
                            if (!string.IsNullOrEmpty(txtClave1))
                            {
                                ClaveBiblioteca = txtClave1;
                            }
                            else if (!string.IsNullOrEmpty(txtClave2))
                            {
                                ClaveBiblioteca = txtClave2;
                            }
                        }

                        listalibro.Add(new ReferenciaBibliografica
                        {
                            intid = Contador + 1,
                            Titulo = Titulo,
                            FreqPublicacion = FreqPublicacion,
                            ComputerFile = ComputerFile,
                            DescFisica = DescFisica,
                            tipo = tipo,
                            txtClave1 = txtClave1,
                            txtClave2 = txtClave2,
                            IDBiblioteca = IDBiblioteca,
                            Autor = Autor,
                            Anio = anio,
                            LugarPublicacion = LugarPublicacion,
                            Editorial = Editorial,
                            Edicion = Edicion,
                            Idioma = Idioma,
                            ISBN = ISBN,
                            ClaveBiblioteca = ClaveBiblioteca,
                            TipoLibro = TipoMaterial,
                            ClaveTipoLibro = ClaveTipoLibro,
                            FuenteBiblioteca = "0"
                        });

                    } // END FOR
                }
                else
                {
                    listalibro.Add(new ReferenciaBibliografica { mensaje = "No se encuentran libros de acuerdo al criterio." });
                }

                Resultado resultado = new Resultado
                {
                    Codigo = (int)HttpStatusCode.OK,
                    Respuesta = "Operación Exitosa",
                    Datos = listalibro
                };

                return resultado;
            }  // END TRY

            catch (XmlException xmlex)
            {
                listalibro.Add(new ReferenciaBibliografica { mensaje = "Error al generar el xml: " + xmlex.Message });
                Resultado resultadoFall = new Resultado
                {
                    Codigo = (int)HttpStatusCode.InternalServerError,
                    Respuesta = xmlex.Message
                };
                return resultadoFall;
            }
            catch (Zoom.Net.ZoomImplementationException exx)
            {
                listalibro.Add(new ReferenciaBibliografica { mensaje = "Error en conexión Zoom: " + exx.Data + "-" + exx.Message });
                Resultado resultadoFall = new Resultado
                {
                    Codigo = (int)HttpStatusCode.InternalServerError,
                    Respuesta = exx.Message
                };
                return resultadoFall;
            }
            catch (Exception ex)
            {
                listalibro.Add(new ReferenciaBibliografica { mensaje = "Error: " + ex.Data + " - " + ex.Message });
                Resultado resultadoFall = new Resultado
                {
                    Codigo = (int)HttpStatusCode.InternalServerError,
                    Respuesta = ex.Message
                };
                return resultadoFall;
            }            
        }


        private string DepurarTextoBusqueda(string TextoBusqueda)
        {
            string QueryText;

            QueryText = TextoBusqueda;
            QueryText = QueryText.Replace("á", "a");
            QueryText = QueryText.Replace("Á", "a");
            QueryText = QueryText.Replace("é", "e");
            QueryText = QueryText.Replace("É", "e");
            QueryText = QueryText.Replace("í", "i");
            QueryText = QueryText.Replace("Í", "i");
            QueryText = QueryText.Replace("ó", "o");
            QueryText = QueryText.Replace("Ó", "o");
            QueryText = QueryText.Replace("ú", "u");
            QueryText = QueryText.Replace("Ú", "u");
            QueryText = QueryText.Replace("ñ", "n");
            return QueryText;
        }

        private void resetVariables()
        {
            ClaveBiblioteca = null;
            TipoMaterial = null;
            Titulo = null;
            FreqPublicacion = null;
            ComputerFile = null;
            DescFisica = null;
            tipo = null;
            txtClave1 = null;
            txtClave2 = null;
            IDBiblioteca = null;
            Autor = null;
            LugarPublicacion = null; //GMM001
            Editorial = null;
            anio = null;
            Edicion = null;
            Idioma = null;
            ISBN = null;
            ClaveTipoLibro = null;
            anio = null;
        }
    }
}