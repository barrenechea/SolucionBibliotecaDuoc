namespace Biblioteca.Entidad
{
    public class Comuna
    {
        #region Attributes
        private int _codComuna;
        private string _nomComuna;
        #endregion
        #region Gets and Sets
        public int CodComuna
        {
            get { return _codComuna; }
            set { _codComuna = value; }
        }

        public string NomComuna
        {
            get { return _nomComuna; }
            set { _nomComuna = value; }
        }
        #endregion
        #region Constructor
        public Comuna(int codComuna, string nomComuna)
        {
            CodComuna = codComuna;
            NomComuna = nomComuna;
        }
        #endregion
    }
}