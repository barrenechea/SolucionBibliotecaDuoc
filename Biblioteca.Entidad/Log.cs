using System;

namespace Biblioteca.Entidad
{
    public class Log
    {
        private string _actividad;
        private string _idUsuario;
        private DateTime _fecha;

        public string Actividad
        {
            get { return _actividad; }
            set { _actividad = value; }
        }

        public string IdUsuario
        {
            get { return _idUsuario; }
            set { _idUsuario = value; }
        }

        public DateTime Fecha
        {
            get { return _fecha; }
            set { _fecha = value; }
        }
        public Log(string actividad, string idUsuario, DateTime fecha)
        {
            Actividad = actividad;
            IdUsuario = idUsuario;
            Fecha = fecha;
        }
    }
}
