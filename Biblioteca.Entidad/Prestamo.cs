using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Biblioteca.Entidad
{
    public class Prestamo
    {
        #region Attributes
        private DateTime _fechaPrestamo;
        private int _codPrestamo;
        private int _nroFicha;
        #endregion
        #region Gets and Sets
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
        #endregion
        #region Constructor
        public Prestamo(int codPrestamo, DateTime fechaPrestamo, int nroFicha)
        {
            _codPrestamo = codPrestamo;
            _fechaPrestamo = fechaPrestamo;
            _nroFicha = nroFicha;
        }

        public Prestamo(DateTime fechaPrestamo, int nroFicha)
        {
            _fechaPrestamo = fechaPrestamo;
            _nroFicha = nroFicha;
        }
        #endregion
    }
}