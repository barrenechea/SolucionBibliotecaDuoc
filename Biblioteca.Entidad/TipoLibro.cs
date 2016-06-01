namespace Biblioteca.Entidad
{
    public class TipoLibro
    {
        public int CodTipo { get; set; }
        public string NomTipo { get; set; }

        public TipoLibro(int codTipo, string nomTipo)
        {
            CodTipo = codTipo;
            NomTipo = nomTipo;
        }
    }
}