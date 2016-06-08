using System.Windows;
using Biblioteca.Entidad;
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
        public LibroManager(bool isAdd)
        {
            InitializeComponent();
            _isAdd = isAdd;

            lblTitulo.Content = BtnExecute.Content = _isAdd ? "Agregar Libro" : "Modificar Libro";
            txtCodLibro.IsEnabled = _isAdd;
        }
        #endregion
        #region Custom Methods
        private async void SearchLibroDialog()
        {
            var settings = new MetroDialogSettings
            {
                AffirmativeButtonText = "Buscar",
                NegativeButtonText = "Cancelar",
                ColorScheme = MetroDialogOptions.ColorScheme
            };

            var result = await this.ShowInputAsync("Buscar", "Ingrese un código de libro a buscar", settings);
            if (result == null || result.Equals(string.Empty))
            {
                new PanelAdmin().Show();
                Close();
            }
            else
            {
                SearchLibro(result);
            }
        }
        private void SearchLibro(string codLibro)
        {
            var exists = App.Libros.FetchLibro(codLibro);
            if (exists.Status)
            {
                txtCodLibro.Text = App.Libros.LibroPersistence.CodLibro;
                txtTitulo.Text = App.Libros.LibroPersistence.Titulo;
                txtAutor.Text = App.Libros.LibroPersistence.Autor;
                txtCategoria.Text = App.Libros.LibroPersistence.Categoria;
                txtArgumento.Text = App.Libros.LibroPersistence.Argumento;
                txtUbicacion.Text = App.Libros.LibroPersistence.Ubicacion;
                txtEditorial.Text = App.Libros.LibroPersistence.Editorial;
                cmbTipo.SelectedValue = App.Libros.LibroPersistence.CodTipo;
                txtNroPag.Text = App.Libros.LibroPersistence.NroPaginas.ToString();
                txtNroCopias.Text = App.Libros.LibroPersistence.NroCopias.ToString();

                App.Libros.ClearPersistantData();
            }
            else
            {
                new PanelAdmin(exists).Show();
                Close();
            }
        }
        private bool Validation()
        {
            if (string.IsNullOrWhiteSpace(txtCodLibro.Text))
            {
                lblStatus.Content = "Debe llenar todos los campos";
                txtCodLibro.Focus();
                return false;
            }
            if (string.IsNullOrWhiteSpace(txtTitulo.Text))
            {
                lblStatus.Content = "Debe llenar todos los campos";
                txtTitulo.Focus();
                return false;
            }
            if (string.IsNullOrWhiteSpace(txtAutor.Text))
            {
                lblStatus.Content = "Debe llenar todos los campos";
                txtAutor.Focus();
                return false;
            }
            if (string.IsNullOrWhiteSpace(txtCategoria.Text))
            {
                lblStatus.Content = "Debe llenar todos los campos";
                txtCategoria.Focus();
                return false;
            }
            if (string.IsNullOrWhiteSpace(txtArgumento.Text))
            {
                lblStatus.Content = "Debe llenar todos los campos";
                txtArgumento.Focus();
                return false;
            }
            if (string.IsNullOrWhiteSpace(txtUbicacion.Text))
            {
                lblStatus.Content = "Debe llenar todos los campos";
                txtUbicacion.Focus();
                return false;
            }
            if (string.IsNullOrWhiteSpace(txtEditorial.Text))
            {
                lblStatus.Content = "Debe llenar todos los campos";
                txtEditorial.Focus();
                return false;
            }
            if (cmbTipo.SelectedItem == null)
            {
                lblStatus.Content = "Debe llenar todos los campos";
                cmbTipo.Focus();
                return false;
            }
            if (string.IsNullOrWhiteSpace(txtNroPag.Text))
            {
                lblStatus.Content = "Debe llenar todos los campos";
                txtNroPag.Focus();
                return false;
            }
            int i;
            if (int.TryParse(txtNroPag.Text, out i))
            {
                if (i <= 0)
                {
                    lblStatus.Content = "El número de paginas debe ser mayor a 0";
                    txtNroPag.Focus();
                    return false;
                }
            }
            else
            {
                lblStatus.Content = "Debe ingresar solo números";
                txtNroPag.Focus();
                return false;
            }
            if (string.IsNullOrWhiteSpace(txtNroCopias.Text))
            {
                lblStatus.Content = "Debe llenar todos los campos";
                txtNroCopias.Focus();
                return false;
            }
            if (int.TryParse(txtNroCopias.Text, out i))
            {
                if (_isAdd && i <= 0)
                {
                    lblStatus.Content = "El número de copias debe ser mayor a 0";
                    txtNroCopias.Focus();
                    return false;
                }
            }
            else
            {
                lblStatus.Content = "Debe ingresar solo números";
                txtNroCopias.Focus();
                return false;
            }
            lblStatus.Content = string.Empty;
            return true;
        }
        private void Execute()
        {
            if (App.Libros.TestConnection().Status)
            {
                var preload = App.Libros.PreloadLibro(txtCodLibro.Text.ToUpper(), txtTitulo.Text, txtAutor.Text, txtCategoria.Text, txtArgumento.Text, txtUbicacion.Text, txtEditorial.Text, (int)cmbTipo.SelectedValue, int.Parse(txtNroPag.Text), int.Parse(txtNroCopias.Text));

                if (preload.Status)
                {
                    var result = _isAdd ? App.Libros.Insert() : App.Libros.Update();

                    if (result.Status)
                    {
                        new PanelAdmin(result).Show();
                        Close();
                    }
                    else
                        lblStatus.Content = result.Mensaje;
                }
                else
                    lblStatus.Content = preload.Mensaje;
            }
            else
            {
                ShowNormalDialog("Error", "Se ha perdido la conexión con el servidor. Intente nuevamente más tarde");
            }
        }
        private async void ShowNormalDialog(string title, string message)
        {
            await this.ShowMessageAsync(title, message);
        }
        #endregion
        #region Event Handlers
        private void LibroManager_OnLoaded(object sender, RoutedEventArgs e)
        {
            var test = App.Libros.TestConnection();
            if (test.Status)
            {
                cmbTipo.ItemsSource = App.Libros.FetchTipoLibros();
                cmbTipo.DisplayMemberPath = "NomTipo";
                cmbTipo.SelectedValuePath = "CodTipo";
                if (!_isAdd) SearchLibroDialog();
                else txtCodLibro.Focus();
            }
            else
            {
                new PanelAdmin(test).Show();
                Close();
            }
        }
        private void BtnLogout_OnClick(object sender, RoutedEventArgs e)
        {
            App.Admins.Logout();
            new Inicio().Show();
            Close();
        }
        private void BtnExecute_Click(object sender, RoutedEventArgs e)
        {
            if (!Validation()) return;
            Execute();
        }
        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            new PanelAdmin().Show();
            Close();
        }
        #endregion
    }
}
