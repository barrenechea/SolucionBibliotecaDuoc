using Biblioteca.Controlador;

namespace Biblioteca.GUI
{
    /// <summary>
    /// Lógica de interacción para App.xaml
    /// </summary>
    public partial class App
    {
        public static readonly ControllerAdmins Admins = new ControllerAdmins();
        public static readonly ControllerUsers Users = new ControllerUsers();
        public static readonly ControllerLibro Libros = new ControllerLibro();
        public static readonly ControllerPrestamo Prestamo = new ControllerPrestamo();
    }
}