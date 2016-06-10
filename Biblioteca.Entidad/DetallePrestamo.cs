using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Biblioteca.Entidad
{
    public class DetallePrestamo
    {
        #region Attributes
        private DateTime _fecDevolucion;
        private bool _libroDevuelto;
        private int _renovacion;
        private int _codPrestamo;
        private string _codLibro;
        #endregion
        #region Gets and Sets


        public string CodLibro
        {
            get { return _codLibro; }
            set { _codLibro = value; }
        }

        public int CodPrestamo
        {
            get { return _codPrestamo; }
            set { _codPrestamo = value; }
        }

        public int Renovacion
        {
            get { return _renovacion; }
            set { _renovacion = value; }
        }

        public bool LibroDevuelto
        {
            get { return _libroDevuelto; }
            set { _libroDevuelto = value; }
        }

        public DateTime FecDevolucion
        {
            get { return _fecDevolucion; }
            set { _fecDevolucion = value; }
        }
        #endregion
        #region Constructor
        public DetallePrestamo(DateTime fecDevolucion, bool libroDevuelto, int renovacion, int codPrestamo, string codLibro)
        {
            _fecDevolucion = fecDevolucion;
            _libroDevuelto = libroDevuelto;
            _renovacion = renovacion;
            _codPrestamo = codPrestamo;
            _codLibro = codLibro;
        }

        public DetallePrestamo(DateTime fecDevolucion, string codLibro, int renovacion)
        {
            _fecDevolucion = fecDevolucion;
            _codLibro = codLibro;
            _renovacion = renovacion;
        }
        #endregion
    }
}
