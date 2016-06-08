using System.Windows;
using MahApps.Metro.Controls.Dialogs;

namespace Biblioteca.GUI
{
    /// <summary>
    /// Interaction logic for AdminManager.xaml
    /// </summary>
    public partial class AdminManager
    {
        #region Attribute
        private readonly bool _isAdd;
        #endregion
        #region Constructor
        public AdminManager(bool isAdd)
        {
            _isAdd = isAdd;
            InitializeComponent();
        }
        #endregion
        #region Custom Methods
        private async void SearchAdminDialog()
        {
            txtUsuario.IsEnabled = false;
            var settings = new MetroDialogSettings
            {
                AffirmativeButtonText = "Buscar",
                NegativeButtonText = "Cancelar",
                ColorScheme = MetroDialogOptions.ColorScheme
            };

            var usuario = await this.ShowInputAsync("Buscar", "Ingrese un nombre de usuario a buscar", settings);

            if (string.IsNullOrWhiteSpace(usuario))
            {
                new PanelAdmin().Show();
                Close();
                return;
            }

            var fetch = App.Admins.FetchUsuario(usuario);
            if (fetch.Status)
            {
                LoadData();
                switchEnabledAccount.Visibility = Visibility.Visible;
            }
            else
            {
                new PanelAdmin(fetch).Show();
                Close();
            }
        }
        private void LoadData()
        {
            txtNombre.Text = App.Admins.AdminPersistence.Nombre;
            txtApellido.Text = App.Admins.AdminPersistence.Apellido;
            txtUsuario.Text = App.Admins.AdminPersistence.IdUsuario;
            switchEnabledAccount.IsChecked = App.Admins.AdminPersistence.Estado;
        }
        private void FixWindow()
        {
            lblTitulo.Content = _isAdd
                ? (App.Admins.AdminActive.TipoDeUsuarioChar == 'D' ? ("Agregar Jefe") : ("Agregar Bibliotecario"))
                : (App.Admins.AdminActive.TipoDeUsuarioChar == 'D' ? ("Modificar Jefe") : ("Modificar Bibliotecario"));

            BtnExecute.Content = _isAdd
                ? (App.Admins.AdminActive.TipoDeUsuarioChar == 'D' ? ("Agregar Jefe de Biblioteca") : ("Agregar Bibliotecario"))
                : (App.Admins.AdminActive.TipoDeUsuarioChar == 'D' ? ("Modificar Jefe de Biblioteca") : ("Modificar Bibliotecario"));

            if (!_isAdd) switchEnabledAccount.Visibility = Visibility.Visible;
        }
        private bool Validation()
        {
            if (string.IsNullOrWhiteSpace(txtNombre.Text))
            {
                lblStatus.Content = "Debe llenar todos los campos";
                txtNombre.Focus();
                return false;
            }
            if (string.IsNullOrWhiteSpace(txtApellido.Text))
            {
                lblStatus.Content = "Debe llenar todos los campos";
                txtApellido.Focus();
                return false;
            }
            if (string.IsNullOrWhiteSpace(txtUsuario.Text))
            {
                lblStatus.Content = "Debe llenar todos los campos";
                txtUsuario.Focus();
                return false;
            }

            if (_isAdd)
            {
                if (App.Admins.ExistsUsuario(txtUsuario.Text))
                {
                    lblStatus.Content = "Nombre de usuario ya existe";
                    txtUsuario.Focus();
                    return false;
                }
                if (string.IsNullOrWhiteSpace(txtPassword.Password))
                {
                    lblStatus.Content = "Debe llenar todos los campos";
                    txtPassword.Focus();
                    return false;
                }
                if (string.IsNullOrWhiteSpace(txtPasswordCheck.Password))
                {
                    lblStatus.Content = "Debe llenar todos los campos";
                    txtPasswordCheck.Focus();
                    return false;
                }
            }
            if (!string.Equals(txtPassword.Password, txtPasswordCheck.Password))
            {
                lblStatus.Content = "Las contraseñas no coinciden";
                txtPassword.Focus();
                return false;
            }
            lblStatus.Content = string.Empty;
            return true;
        }
        private async void ConfirmationDialog()
        {
            var settings = new MetroDialogSettings
            {
                AffirmativeButtonText = "Si",
                NegativeButtonText = "No",
                ColorScheme = MetroDialogOptions.ColorScheme
            };

            var confirmation = await this.ShowMessageAsync("Contraseña", "¿Está seguro que desea mantener la contraseña antigua?", MessageDialogStyle.AffirmativeAndNegative, settings);

            if (confirmation == MessageDialogResult.Affirmative)
            {
                Execute();
            }
        }
        private void Execute()
        {
            if (App.Admins.TestConnection().Status)
            {
                var preload = App.Admins.PreloadAdmin(txtNombre.Text, txtApellido.Text, txtUsuario.Text, txtPassword.Password, (bool)switchEnabledAccount.IsChecked);

                if (preload.Status)
                {
                    var result = _isAdd ? App.Admins.Insert() : App.Admins.Update();

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
        #region Event Handler Methods
        private void WindowHasLoaded(object sender, RoutedEventArgs e)
        {
            FixWindow();
            var test = App.Admins.TestConnection();
            if (test.Status)
            {
                if (!_isAdd) SearchAdminDialog();
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

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            new PanelAdmin().Show();
            Close();
        }

        private void BtnExecute_Click(object sender, RoutedEventArgs e)
        {
            if (App.Libros.TestConnection().Status)
            {
                if (!Validation()) return;

                if (_isAdd) Execute();

                else
                {
                    if(string.IsNullOrWhiteSpace(txtPassword.Password)) ConfirmationDialog();
                    else Execute();
                }
            }
            else
                ShowNormalDialog("Error", "Se ha perdido la conexión con el servidor. Intente nuevamente más tarde");
        }
        #endregion
    }
}