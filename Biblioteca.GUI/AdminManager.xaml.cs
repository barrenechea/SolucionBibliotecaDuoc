using System.Linq;
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
        /// <summary>
        /// Generates a new instance of AdminManager
        /// </summary>
        /// <param name="isAdd">Determines if the Window is going to be used to Add an Administrator or not</param>
        public AdminManager(bool isAdd)
        {
            _isAdd = isAdd;
            InitializeComponent();
        }
        #endregion
        #region Custom Methods
        /// <summary>
        /// Opens an Input Dialog inside the Window and attempts to fetch an Administrador
        /// </summary>
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
        /// <summary>
        /// Load an Administrador data onto the Window Controls
        /// </summary>
        private void LoadData()
        {
            txtNombre.Text = App.Admins.AdminPersistence.Nombre;
            txtApellido.Text = App.Admins.AdminPersistence.Apellido;
            txtUsuario.Text = App.Admins.AdminPersistence.IdUsuario;
            switchEnabledAccount.IsChecked = App.Admins.AdminPersistence.Estado;
        }
        /// <summary>
        /// Modifies labels, titles and other stuff, based on the parameter received by the Constructor.
        /// </summary>
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
        /// <summary>
        /// Method that validates the form inside the Window
        /// </summary>
        /// <returns>If the validation was successful or not</returns>
        private bool Validation()
        {
            if (string.IsNullOrWhiteSpace(txtNombre.Text))
            {
                lblStatus.Content = "Debe llenar todos los campos";
                txtNombre.Focus();
                return false;
            }
            if (txtNombre.Text.Length < 2)
            {
                lblStatus.Content = "Debe tener mínimo 2 caracteres";
                txtNombre.Focus();
                return false;
            }
            if (txtNombre.Text.Any(char.IsDigit))
            {
                lblStatus.Content = "El campo no puede tener dígitos";
                txtNombre.Focus();
                return false;
            }
            if (string.IsNullOrWhiteSpace(txtApellido.Text))
            {
                lblStatus.Content = "Debe llenar todos los campos";
                txtApellido.Focus();
                return false;
            }
            if (txtApellido.Text.Length < 2)
            {
                lblStatus.Content = "Debe tener mínimo 2 caracteres";
                txtApellido.Focus();
                return false;
            }
            if (txtApellido.Text.Any(char.IsDigit))
            {
                lblStatus.Content = "El campo no puede tener dígitos";
                txtApellido.Focus();
                return false;
            }
            if (string.IsNullOrWhiteSpace(txtUsuario.Text))
            {
                lblStatus.Content = "Debe llenar todos los campos";
                txtUsuario.Focus();
                return false;
            }
            if (txtUsuario.Text.Length < 5)
            {
                lblStatus.Content = "Debe tener mínimo 5 caracteres";
                txtUsuario.Focus();
                return false;
            }
            if (txtUsuario.Text.Any(char.IsDigit))
            {
                lblStatus.Content = "El campo no puede tener dígitos";
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
            if (!string.IsNullOrWhiteSpace(txtPassword.Password))
            {
                if (txtPassword.Password.Length < 6)
                {
                    lblStatus.Content = "Debe tener mínimo 6 caracteres";
                    txtPassword.Focus();
                    return false;
                }
                if (!string.Equals(txtPassword.Password, txtPasswordCheck.Password))
                {
                    lblStatus.Content = "Las contraseñas no coinciden";
                    txtPassword.Focus();
                    return false;
                }
            }
            
            lblStatus.Content = string.Empty;
            return true;
        }
        /// <summary>
        /// Opens a Confirmation Dialog inside the Window.
        /// It's being used when an Administrador is being modified, but the password fields were left in blank.
        /// </summary>
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
        /// <summary>
        /// Calls to the Admin Controller, and executes the Insert or Update querys.
        /// In case of success, it's returned to the PanelAdmin Window.
        /// </summary>
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
        #region Event Handler Methods
        /// <summary>
        /// Event that loads itself when the Window was loaded
        /// </summary>
        /// <param name="sender">The object that triggered this event</param>
        /// <param name="e">Parameters (optional)</param>
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
        /// Event that loads when user clicks on the Execute button
        /// </summary>
        /// <param name="sender">The object that triggered this event</param>
        /// <param name="e">Parameters (optional)</param>
        private void BtnExecute_Click(object sender, RoutedEventArgs e)
        {
            if (App.Admins.TestConnection().Status)
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