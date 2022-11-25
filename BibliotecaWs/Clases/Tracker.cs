using System;
using System.Configuration;

public class Tracker
{
    public static void RegisterLog(string mensaje, string accion, Exception ex)
    {
        System.IO.StreamWriter writer = System.IO.File.AppendText(ConfigurationManager.AppSettings["PathLog"] + "WsBiblioteca-" + DateTime.Now.ToString("yyyyMMdd") + ".log");
        writer.WriteLine("****************************************************************");
        writer.WriteLine("Fecha: " + DateTime.Now.ToString("dd-MMM-yyyy hh:mm:ss tt"));
        writer.WriteLine("Accion: " + accion);
        writer.WriteLine("Mensaje: " + mensaje);
        if (ex != null)
        {
            writer.WriteLine(String.Concat("Error: ", ex));
        }
        writer.Close();
    }
}
