using System.Linq;
using System.Windows;

namespace Biblioteca.GUI
{
    /// <summary>
    /// Lógica de interacción para PrestamoAdd.xaml
    /// </summary>
    public partial class PrestamoAdd
    {

        #region Constructor
        /// <summary>
        /// Generates a new instance of PrestamoAdd
        /// </summary>
        public PrestamoAdd()
        {
            InitializeComponent();
        }
        #endregion
        #region Custom Methods
        /// <summary>
        /// validaciones del formulario segun el modelo de negocio
        /// </summary>
        /// <returns>bool de confirmacion de que están todos los parametros bien</returns>
        private bool Validation()
        {
            if (string.IsNullOrWhiteSpace(txtNroFicha.Text))
            {
                lblStatus.Content = "Debe llenar todos los campos";
                txtNroFicha.Focus();
                return false;
            }
            int i;
            if (!int.TryParse(txtNroFicha.Text, out i))
            {
                lblStatus.Content = "El número de ficha es de solo números";
                txtNroFicha.Focus();
                return false;
            }
            if (string.IsNullOrWhiteSpace(txtCodLibro.Text))
            {
                lblStatus.Content = "Debe llenar todos los campos";
                txtCodLibro.Focus();
                return false;
            }
            var codigos = txtCodLibro.Text.ToUpper().Split(',');
            if (codigos.Any(string.IsNullOrWhiteSpace))
            {
                lblStatus.Content = "No debe ingresar espacios en blanco entre las comas";
                txtCodLibro.Focus();
                return false;
            }
            var resultado = App.Users.ExistsNroFicha(txtNroFicha.Text);
            if (resultado.Status)
            {
                var prestados = App.Libros.CantLibrosPrestados(txtNroFicha.Text);
                if (App.Users.IsStudent(int.Parse(txtNroFicha.Text)).Status)
                {
                    if (codigos.Length > 1)
                    {
                        lblStatus.Content = "Los estudiantes no pueden solicitar más de un libro";
                        txtCodLibro.Focus();
                        return false;
                    }
                    if (prestados >= 1)
                    {
                        lblStatus.Content = "El usuario no puede tener más de un libro";
                        return false;
                    }
                }
                else
                {
                    if (prestados + codigos.Length > 5)
                    {
                        lblStatus.Content = "El usuario no puede tener más de cinco libros";
                        txtCodLibro.Focus();
                        return false;
                    }
                }
            }
            else
            {
                lblStatus.Content = resultado.Mensaje;
                txtNroFicha.Focus();
                return false;
            }

            resultado = App.Users.IsEnabled(txtNroFicha.Text);
            if (!resultado.Status)
            {
                lblStatus.Content = resultado.Mensaje;
                txtNroFicha.Focus();
                return false;
            }
            foreach (var cod in codigos)
            {
                resultado = App.Libros.ExistsLibro(cod.Trim());
                if (!resultado.Status)
                {
                    lblStatus.Content = resultado.Mensaje;
                    txtCodLibro.Focus();
                    return false;
                }
                var librosDisponibles = App.Libros.AmountAvailable(cod.Trim());

                if (librosDisponibles < 1 || codigos.Count(t => t.Trim().Equals(cod.Trim())) > librosDisponibles)
                {
                    lblStatus.Content = "No hay suficientes libros";
                    txtCodLibro.Focus();
                    return false;
                }
            }
            lblStatus.Content = string.Empty;
            return true;
        }
        /// <summary>
        /// First part of the execution logic.
        /// Attempts to insert into the Prestamo table
        /// </summary>
        private void ExecutePrestamo()
        {
            var preload = App.Prestamo.PreloadPrestamo(int.Parse(txtNroFicha.Text));
            if (preload.Status)
            {
                var result = App.Prestamo.InsertPrestamo();

                if (result.Status) ExecuteDetallePrestamo();
                else lblStatus.Content = result.Mensaje;
            }
            else lblStatus.Content = preload.Mensaje;
        }
        /// <summary>
        /// Second and last part of execution logic.
        /// Attempts to insert into Detalle_prestamo table and returns to PanelAdmin Window
        /// </summary>
        private void ExecuteDetallePrestamo()
        {
            var preload = App.Prestamo.PreloadDetallePrestamo(int.Parse(txtNroFicha.Text),  txtCodLibro.Text.Split(','));
            if (preload.Status)
            {
                var result = App.Prestamo.InsertDetallePrestamo();

                if (result.Status)
                {
                    foreach (var libro in txtCodLibro.Text.Split(','))
                    {
                        App.Libros.Discount(libro.ToUpper().Trim());
                    }
                    new PanelAdmin(result).Show();
                    Close();
                }
                else lblStatus.Content = result.Mensaje;
            }
            else lblStatus.Content = preload.Mensaje;
        }
        #endregion
        #region Event Handlers
        /// <summary>
        /// Event that loads when user clicks on the Back button
        /// </summary>
        /// <param name="sender">The object that triggered this event</param>
        /// <param name="e">Parameters (optional)</param>
        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            new PanelAdmin().Show();
            Close();
        }
        /// <summary>
        /// Event that loads when user clicks on the Logout button
        /// </summary>
        /// <param name="sender">The object that triggered this event</param>
        /// <param name="e">Parameters (optional)</param>
        private void BtnLogout_OnClick(object sender, RoutedEventArgs e)
        {
            App.Admins.Logout();
            new Inicio().Show();
            Close();
        }
        /// <summary>
        /// Event that loads when user clicks on the Execute button
        /// </summary>
        /// <param name="sender">The object that triggered this event</param>
        /// <param name="e">Parameters (optional)</param>
        private void BtnExecute_Click(object sender, RoutedEventArgs e)
        {
            if (App.Prestamo.TestConnection().Status)
            {
                if (!Validation()) return;
                ExecutePrestamo();
            }
            else lblStatus.Content = "Se ha perdido la conexión con el servidor";
        }
        #endregion
    }
}