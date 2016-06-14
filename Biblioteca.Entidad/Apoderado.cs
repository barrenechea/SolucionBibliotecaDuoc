namespace Biblioteca.Entidad
{
    public class Apoderado : Persona
    {
        #region Attributes
        private int _nroFicha;
        private string _parentesco;
        #endregion
        #region Gets and Sets
        public int NroFicha
        {
            get { return _nroFicha; }
            set { _nroFicha = value; }
        }

        public string Parentesco
        {
            get { return _parentesco; }
            set { _parentesco = value; }
        }
        #endregion
        #region Constructor
        public Apoderado(string run, string nombre, string apellido, string direccion, int codComuna, string fonoFijo, string fonoCel, int numFicha, string parentesco)
            : base(run, nombre, apellido, direccion, codComuna, fonoFijo, fonoCel)
        {
            NroFicha = numFicha;
            Parentesco = parentesco;
        }

        #endregion
    }
}
