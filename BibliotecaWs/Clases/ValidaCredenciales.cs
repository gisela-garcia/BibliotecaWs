using BibliotecaWs.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http.Headers;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Web;

    public class ValidaCredenciales : IHttpModule
    {
        public void Dispose()
        {
            //throw new NotImplementedException();
        }

        public void Init(HttpApplication context)
        {
            context.AuthenticateRequest += new EventHandler(ValidaAutorizacion);
        }

        protected void ValidaAutorizacion(object sender, EventArgs e)
        {
            var application = (HttpApplication)sender;
            var request = new HttpRequestWrapper(application.Request);
            var authData = request.Headers["Authorization"];
            bool respuesta = Autenticar(authData);
            if (respuesta)
            {
                string[] desencriptar = authData.Split(' ');
                var base64EncodedBytes = Convert.FromBase64String(desencriptar[1]);
                string[] separa = Encoding.UTF8.GetString(base64EncodedBytes).Split(':');
                string user = separa[0];
                var principal = new GenericPrincipal(new GenericIdentity(user), null);
                PutPrincipal(principal);
            }
        }

        public bool Autenticar(string authData)
        {
            try
            {
                var usuario = ConfigurationManager.AppSettings.Get("WsUser");
                var Password = ConfigurationManager.AppSettings.Get("Password");
            Tracker.RegisterLog($"ususario {usuario} - {Password}", authData, null);
            if (!string.IsNullOrEmpty(authData))
                {
                    string[] respuesta = authData.Split(' ');
                    string encriptar;
                    byte[] resultadoEncriptado;
                    encriptar = usuario + ":" + Password;
                    resultadoEncriptado = Encoding.UTF8.GetBytes(encriptar);
                    encriptar = Convert.ToBase64String(resultadoEncriptado);
                Tracker.RegisterLog($"entro {respuesta[1]} - {encriptar}", authData, null);
                if (respuesta[1] == encriptar)
                        return true;                        
                }
            return false;
        }
            catch (Exception) { return false; }
        }

        private void PutPrincipal(IPrincipal principal)
        {
            Thread.CurrentPrincipal = principal;
            if (HttpContext.Current != null)
            {
                HttpContext.Current.User = principal;
            }
        }
    }