using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Biblioteca.Entidad
{
    public class Prestamo
    {
        private DateTime _fechaPrestamo;
        private int _codPrestamo;
        private int _nroFicha;

        public int NroFicha
        {
            get { return _nroFicha; }
            set { _nroFicha = value; }
        }

        public int CodPrestamo
        {
            get { return _codPrestamo; }
            set { _codPrestamo = value; }
        }

        public DateTime FechaPrestamo
        {
            get { return _fechaPrestamo; }
            set { _fechaPrestamo = value; }
        }



        public Prestamo(int codPrestamo, DateTime fechaPrestamo, int nroFicha)
        {
            CodPrestamo = codPrestamo;
            FechaPrestamo = fechaPrestamo;
            NroFicha = nroFicha;
        }

        public Prestamo(DateTime fechaPrestamo, int nroFicha)
        {
            FechaPrestamo = fechaPrestamo;
            NroFicha = nroFicha;
        }
    }
}
