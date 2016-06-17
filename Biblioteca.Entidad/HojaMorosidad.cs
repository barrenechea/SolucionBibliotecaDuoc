using System;

namespace Biblioteca.Entidad
{
    public class HojaMorosidad
    {
        private DateTime _fecha;
        private string _sancion;
        private int _diasAtraso;
        private int _nroFicha;
        private string _codLibro;

        public HojaMorosidad(DateTime fecha, string sancion, int diasAtraso, int nroFicha, string codLibro)
        {
            _fecha = fecha;
            _sancion = sancion;
            _diasAtraso = diasAtraso;
            _nroFicha = nroFicha;
            _codLibro = codLibro;
        }

        public string CodLibro
        {
            get { return _codLibro; }
            set { _codLibro = value; }
        }

        public int NroFicha
        {
            get { return _nroFicha; }
            set { _nroFicha = value; }
        }

        public int DiasAtraso
        {
            get { return _diasAtraso; }
            set { _diasAtraso = value; }
        }

        public string Sancion
        {
            get { return _sancion; }
            set { _sancion = value; }
        }

        public DateTime Fecha
        {
            get { return _fecha; }
            set { _fecha = value; }
        }
    }
}
