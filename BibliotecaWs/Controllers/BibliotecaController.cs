using System;
using System.Net;
using System.Web.Http;
using System.Web;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Xml;
using BibliotecaWs.Models;
using Zoom.Net;
using System.Web.Http.Cors;
using Zoom.Net.YazSharp;
using BibliotecaWs.Clases;

namespace BibliotecaWs.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    [RoutePrefix("Biblioteca")]
    public class BibliotecaController : ApiController
    {
        [HttpPost]
        [Route("ConsultaBiblioteca")]
        [Authorize]
        public IHttpActionResult ConsultaBiblioteca(string BusquedaTitulo, string BusquedaAutor, string BusquedaEdicion)
        {
            Mensaje mensaje = new Mensaje();
            try
            {
                string version = "1.0";
                string originalPath = new Uri(HttpContext.Current.Request.Url.AbsoluteUri).OriginalString;
                if (originalPath.Substring(originalPath.Length - 1, 1) == "/")
                {
                    originalPath = originalPath.Substring(0, originalPath.LastIndexOf("/"));
                }
                DataAccess services = new DataAccess();
                TopLevel topLevel = new TopLevel();

                Resultado resultado = services.ConsultaBiblioteca(BusquedaTitulo, BusquedaAutor, BusquedaEdicion);

                if (resultado.Codigo == (int)HttpStatusCode.OK)
                {
                    Jsonapi jsonapi = new Jsonapi
                    {
                        version = version
                    };
                    topLevel.jsonapi = jsonapi;
                    topLevel.data = resultado.Datos;
                    return Ok(topLevel);
                }
                else
                {
                    mensaje.status = resultado.Codigo;
                    mensaje.message = resultado.Respuesta;
                    return Content((HttpStatusCode)mensaje.status, mensaje);
                }
            }
            catch (Exception ex)
            {
                mensaje.status = (int)HttpStatusCode.InternalServerError;
                mensaje.message = ex.Message;
                return Content(HttpStatusCode.InternalServerError, mensaje);
            }
        }

        [HttpPost]
        [Route("ConsultaBibliotecaCongreso")]
        public IHttpActionResult ConsultaBibliotecaCongreso(string BusquedaTitulo, string BusquedaAutor, string BusquedaEdicion)
        {
            Mensaje mensaje = new Mensaje();
            try
            {
                string version = "1.0";
                string originalPath = new Uri(HttpContext.Current.Request.Url.AbsoluteUri).OriginalString;
                if (originalPath.Substring(originalPath.Length - 1, 1) == "/")
                {
                    originalPath = originalPath.Substring(0, originalPath.LastIndexOf("/"));
                }
                DataAccess services = new DataAccess();
                TopLevel topLevel = new TopLevel();

                Resultado resultado = services.ConsultaBibliotecaCongreso(BusquedaTitulo, BusquedaAutor, BusquedaEdicion);

                if (resultado.Codigo == (int)HttpStatusCode.OK)
                {
                    Jsonapi jsonapi = new Jsonapi
                    {
                        version = version
                    };
                    topLevel.jsonapi = jsonapi;
                    topLevel.data = resultado.Datos;
                    return Ok(topLevel);
                }
                else
                {
                    mensaje.status = resultado.Codigo;
                    mensaje.message = resultado.Respuesta;
                    return Content((HttpStatusCode)mensaje.status, mensaje);
                }
            }
            catch (Exception ex)
            {
                mensaje.status = (int)HttpStatusCode.InternalServerError;
                mensaje.message = ex.Message;
                return Content(HttpStatusCode.InternalServerError, mensaje);
            }
        }
    }
}