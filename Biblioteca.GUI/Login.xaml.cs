using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using Biblioteca.Entidad;
using MahApps.Metro.Controls.Dialogs;

namespace Biblioteca.GUI
{
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class Login
    {
        #region Attributes
        readonly BackgroundWorker _login = new BackgroundWorker();
        private ProgressDialogController _controller;
        private string _user, _pass;
        private Message _mensaje;
        #endregion
        #region Constructor
        public Login()
        {
            InitializeComponent();
            _login.DoWork += Login_DoWork;
            _login.RunWorkerCompleted += Login_RunWorkerCompleted;
            txtUsuario.Focus();
        }
        #endregion
        #region Custom Methods
        #region BackgroundWorker methods
        private void Login_DoWork(object sender, DoWorkEventArgs e)
        {
            _mensaje = App.Login.Login(_user, _pass);
        }
        private async void Login_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            await _controller.CloseAsync();
            if (!_mensaje.Status)
                ShowNormalDialog("Error", _mensaje.Mensaje);
            else
            {
                new PanelAdmin().Show();
                Close();
            }
        }
        #endregion
        #region Other methods
        private bool Validate()
        {
            if (!string.IsNullOrWhiteSpace(txtUsuario.Text) && !string.IsNullOrWhiteSpace(txtPassword.Password)) return true;
            ShowNormalDialog("Error", "Debe llenar todos los campos");
            return false;
        }
        private async void LoginDb()
        {
            _controller = await this.ShowProgressAsync("Por favor espere", "Iniciando sesión...");
            _login.RunWorkerAsync();
            _controller.SetIndeterminate();
        }
        private async void ShowNormalDialog(string title, string message)
        {
            await this.ShowMessageAsync(title, message);
        }
        #endregion
        #endregion
        #region Event Handlers
        private void BtnVolver_Click(object sender, RoutedEventArgs e)
        {
            new Inicio().Show();
            Close();
        }
        private void BtnIniciarSesion_Click(object sender, RoutedEventArgs e)
        {
            if (!Validate()) return;

            var test = App.Login.TestConnection();
            if (test.Status)
            {
                _user = txtUsuario.Text;
                _pass = txtPassword.Password;
                LoginDb();
            }
            else
            {
                new Inicio(test).Show();
                Close();
            }
        }
        private void MetroWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                BtnIniciarSesion_Click(sender, e);
        }
        #endregion
    }
}