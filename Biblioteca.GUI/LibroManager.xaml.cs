using System.Windows;
using MahApps.Metro.Controls.Dialogs;

namespace Biblioteca.GUI
{
    /// <summary>
    /// Lógica de interacción para NewLibro.xaml
    /// </summary>
    public partial class LibroManager
    {
        #region Attributes
        private readonly bool _isAdd;
        #endregion
        #region Constructor
        /// <summary>
        /// Generates a new instance of LibroManager
        /// </summary>
        /// <param name="isAdd">Determines if the Window is going to be used to Add a Libro or not</param>
        public LibroManager(bool isAdd)
        {
            InitializeComponent();
            _isAdd = isAdd;

            LblTitulo.Content = BtnExecute.Content = _isAdd ? "Agregar Libro" : "Modificar Libro";
            TxtCodLibro.IsEnabled = _isAdd;
        }
        #endregion
        #region Custom Methods
        /// <summary>
        /// Opens an Input Dialog inside the Window
        /// </summary>
        private async void SearchLibroDialog()
        {
            var settings = new MetroDialogSettings
            {
                AffirmativeButtonText = "Buscar",
                NegativeButtonText = "Cancelar",
                ColorScheme = MetroDialogOptions.ColorScheme
            };

            var codLibro = await this.ShowInputAsync("Buscar", "Ingrese un código de libro", settings);

            var check = App.Libros.CheckEmptySearchString(codLibro);
            if (!check.Status)
            {
                if (check.Mensaje == null)
                {
                    new PanelAdmin().Show();
                    Close();
                }
                else
                {
                    new PanelAdmin(check).Show();
                    Close();
                }
                return;
            }

            SearchLibro(codLibro);
        }
        /// <summary>
        /// Fetch and load a Libro data onto the Window Controls
        /// </summary>
        /// <param name="codLibro">Codigo de Libro to fetch</param>
        private void SearchLibro(string codLibro)
        {
            var exists = App.Libros.FetchLibro(codLibro);
            if (exists.Status)
            {
                TxtCodLibro.Text = App.Libros.LibroPersistence.CodLibro;
                TxtTitulo.Text = App.Libros.LibroPersistence.Titulo;
                TxtAutor.Text = App.Libros.LibroPersistence.Autor;
                TxtCategoria.Text = App.Libros.LibroPersistence.Categoria;
                TxtArgumento.Text = App.Libros.LibroPersistence.Argumento;
                TxtUbicacion.Text = App.Libros.LibroPersistence.Ubicacion;
                TxtEditorial.Text = App.Libros.LibroPersistence.Editorial;
                CmbTipo.SelectedValue = App.Libros.LibroPersistence.CodTipo;
                TxtNroPag.Text = App.Libros.LibroPersistence.NroPaginas.ToString();
                TxtNroCopias.Text = App.Libros.LibroPersistence.NroCopias.ToString();

                App.Libros.ClearPersistantData();
            }
            else
            {
                new PanelAdmin(exists).Show();
                Close();
            }
        }
        /// <summary>
        /// Method that validates the form inside the Window
        /// </summary>
        /// <returns>If the validation was successful or not</returns>
        private bool Validation()
        {
            if (string.IsNullOrWhiteSpace(TxtCodLibro.Text))
            {
                LblStatus.Content = "Debe llenar todos los campos";
                TxtCodLibro.Focus();
                return false;
            }
            if (TxtCodLibro.Text.Length < 4)
            {
                LblStatus.Content = "Debe tener mínimo 4 caracteres";
                TxtCodLibro.Focus();
                return false;
            }
            if (string.IsNullOrWhiteSpace(TxtTitulo.Text))
            {
                LblStatus.Content = "Debe llenar todos los campos";
                TxtTitulo.Focus();
                return false;
            }
            if (TxtTitulo.Text.Length < 2)
            {
                LblStatus.Content = "Debe tener mínimo 2 caracteres";
                TxtTitulo.Focus();
                return false;
            }
            if (string.IsNullOrWhiteSpace(TxtAutor.Text))
            {
                LblStatus.Content = "Debe llenar todos los campos";
                TxtAutor.Focus();
                return false;
            }
            if (TxtAutor.Text.Length < 2)
            {
                LblStatus.Content = "Debe tener mínimo 2 caracteres";
                TxtAutor.Focus();
                return false;
            }
            if (string.IsNullOrWhiteSpace(TxtCategoria.Text))
            {
                LblStatus.Content = "Debe llenar todos los campos";
                TxtCategoria.Focus();
                return false;
            }
            if (TxtCategoria.Text.Length < 4)
            {
                LblStatus.Content = "Debe tener mínimo 4 caracteres";
                TxtCategoria.Focus();
                return false;
            }
            if (string.IsNullOrWhiteSpace(TxtArgumento.Text))
            {
                LblStatus.Content = "Debe llenar todos los campos";
                TxtArgumento.Focus();
                return false;
            }
            if (TxtArgumento.Text.Length < 10)
            {
                LblStatus.Content = "Debe tener mínimo 10 caracteres";
                TxtArgumento.Focus();
                return false;
            }
            if (string.IsNullOrWhiteSpace(TxtUbicacion.Text))
            {
                LblStatus.Content = "Debe llenar todos los campos";
                TxtUbicacion.Focus();
                return false;
            }
            if (TxtUbicacion.Text.Length < 2)
            {
                LblStatus.Content = "Debe tener mínimo 2 caracteres";
                TxtUbicacion.Focus();
                return false;
            }
            if (string.IsNullOrWhiteSpace(TxtEditorial.Text))
            {
                LblStatus.Content = "Debe llenar todos los campos";
                TxtEditorial.Focus();
                return false;
            }
            if (TxtEditorial.Text.Length < 2)
            {
                LblStatus.Content = "Debe tener mínimo 2 caracteres";
                TxtEditorial.Focus();
                return false;
            }
            if (CmbTipo.SelectedItem == null)
            {
                LblStatus.Content = "Debe llenar todos los campos";
                CmbTipo.Focus();
                return false;
            }
            if (string.IsNullOrWhiteSpace(TxtNroPag.Text))
            {
                LblStatus.Content = "Debe llenar todos los campos";
                TxtNroPag.Focus();
                return false;
            }
            int i;
            if (int.TryParse(TxtNroPag.Text, out i))
            {
                if (i <= 0)
                {
                    LblStatus.Content = "El número de paginas debe ser mayor a 0";
                    TxtNroPag.Focus();
                    return false;
                }
            }
            else
            {
                LblStatus.Content = "Debe ingresar solo números";
                TxtNroPag.Focus();
                return false;
            }
            if (string.IsNullOrWhiteSpace(TxtNroCopias.Text))
            {
                LblStatus.Content = "Debe llenar todos los campos";
                TxtNroCopias.Focus();
                return false;
            }
            if (int.TryParse(TxtNroCopias.Text, out i))
            {
                if (_isAdd && i <= 0)
                {
                    LblStatus.Content = "El número de copias debe ser mayor a 0";
                    TxtNroCopias.Focus();
                    return false;
                }
                if (!_isAdd && i < 0)
                {
                    LblStatus.Content = "El número de copias debe ser mayor o igual a 0";
                    TxtNroCopias.Focus();
                    return false;
                }
            }
            else
            {
                LblStatus.Content = "Debe ingresar solo números";
                TxtNroCopias.Focus();
                return false;
            }
            LblStatus.Content = string.Empty;
            return true;
        }
        /// <summary>
        /// Calls to the Libro Controller, and executes the Insert or Update querys.
        /// In case of success, it's returned to the PanelAdmin Window.
        /// </summary>
        private void Execute()
        {
            if (App.Libros.TestConnection().Status)
            {
                var preload = App.Libros.PreloadLibro(TxtCodLibro.Text.ToUpper(), TxtTitulo.Text, TxtAutor.Text, TxtCategoria.Text, TxtArgumento.Text, TxtUbicacion.Text, TxtEditorial.Text, (int)CmbTipo.SelectedValue, int.Parse(TxtNroPag.Text), int.Parse(TxtNroCopias.Text));

                if (preload.Status)
                {
                    var result = _isAdd ? App.Libros.Insert() : App.Libros.Update();

                    if (result.Status)
                    {
                        new PanelAdmin(result).Show();
                        Close();
                    }
                    else
                        LblStatus.Content = result.Mensaje;
                }
                else
                    LblStatus.Content = preload.Mensaje;
            }
            else
            {
                ShowNormalDialog("Error", "Se ha perdido la conexión con el servidor. Intente nuevamente más tarde");
            }
        }
        /// <summary>
        /// Shows just an alert inside the Window
        /// </summary>
        /// <param name="title">Title of the alert</param>
        /// <param name="message">Message of the alert</param>
        private async void ShowNormalDialog(string title, string message)
        {
            await this.ShowMessageAsync(title, message);
        }
        #endregion
        #region Event Handlers
        /// <summary>
        /// Event that loads itself when the Window was loaded
        /// </summary>
        /// <param name="sender">The object that triggered this event</param>
        /// <param name="e">Parameters (optional)</param>
        private void LibroManager_OnLoaded(object sender, RoutedEventArgs e)
        {
            var test = App.Libros.TestConnection();
            if (test.Status)
            {
                CmbTipo.ItemsSource = App.Libros.FetchTipoLibros();
                CmbTipo.DisplayMemberPath = "NomTipo";
                CmbTipo.SelectedValuePath = "CodTipo";
                if (!_isAdd) SearchLibroDialog();
                else TxtCodLibro.Focus();
            }
            else
            {
                new PanelAdmin(test).Show();
                Close();
            }
        }
        /// <summary>
        /// Event that loads when user clicks on the Back button
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
            if (App.Libros.TestConnection().Status)
            {
                if (!Validation()) return;
                Execute();
            }
            else
                ShowNormalDialog("Error", "Se ha perdido la conexión con el servidor. Intente nuevamente más tarde");
        }
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
        #endregion
    }
}