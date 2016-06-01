using System;

namespace Biblioteca.Entidad
{
    public class Usuario : Persona
    {
        #region Attributes
        private int _nroFicha;
        private bool _estado;
        private string _fecNacimiento;
        #endregion
        #region Gets and Sets
        public int NroFicha
        {
            get { return _nroFicha; }
            set { _nroFicha = value; }
        }

        public bool Estado
        {
            get { return _estado; }
            set { _estado = value; }
        }

        public string FecNacimiento
        {
            get { return _fecNacimiento; }
            set { _fecNacimiento = value; }
        }
        #endregion
        #region Constructor
        protected Usuario(string run, string nombre, string apellido, string direccion, int codComuna, string fonoFijo, string fonoCel, DateTime fecNacimiento, int nroFicha, bool estado)
            : base(run, nombre, apellido, direccion, codComuna, fonoFijo, fonoCel)
        {
            NroFicha = nroFicha;
            Estado = estado;
            FecNacimiento = fecNacimiento.ToString("yyyy-MM-dd");
        }
        #endregion
    }
}
