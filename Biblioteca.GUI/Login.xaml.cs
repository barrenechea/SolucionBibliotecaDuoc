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
        /// <summary>
        /// Generates a new instance of Login
        /// </summary>
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
        /// <summary>
        /// What the BackgroundWorker does while it's running the task
        /// </summary>
        /// <param name="sender">The object that triggered this event</param>
        /// <param name="e">Parameters (optional)</param>
        private void Login_DoWork(object sender, DoWorkEventArgs e)
        {
            _mensaje = App.Admins.Login(_user, _pass);
        }
        /// <summary>
        /// What the BackgroundWorker does when has finished running the task
        /// </summary>
        /// <param name="sender">The object that triggered this event</param>
        /// <param name="e">Parameters (optional)</param>
        private async void Login_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            await _controller.CloseAsync();
            if (_mensaje.Status)
            {
                new PanelAdmin().Show();
                Close();
            }
            else ShowNormalDialog("Error", _mensaje.Mensaje);
        }
        #endregion
        #region Other methods
        /// <summary>
        /// Method that validates the form inside the Window
        /// </summary>
        /// <returns>If the validation was successful or not</returns>
        private bool Validate()
        {
            if (!string.IsNullOrWhiteSpace(txtUsuario.Text) && !string.IsNullOrWhiteSpace(txtPassword.Password)) return true;
            ShowNormalDialog("Error", "Debe llenar todos los campos");
            return false;
        }
        /// <summary>
        /// Method that calls an Async method, and attempts to Login
        /// </summary>
        private async void LoginDb()
        {
            _controller = await this.ShowProgressAsync("Por favor espere", "Iniciando sesión...");
            _login.RunWorkerAsync();
            _controller.SetIndeterminate();
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
        #endregion
        #region Event Handlers
        /// <summary>
        /// Event that loads when user clicks on the Back button
        /// </summary>
        /// <param name="sender">The object that triggered this event</param>
        /// <param name="e">Parameters (optional)</param>
        private void BtnVolver_Click(object sender, RoutedEventArgs e)
        {
            new Inicio().Show();
            Close();
        }
        /// <summary>
        /// Event that loads when user clicks on the Iniciar Sesion button
        /// </summary>
        /// <param name="sender">The object that triggered this event</param>
        /// <param name="e">Parameters (optional)</param>
        private void BtnIniciarSesion_Click(object sender, RoutedEventArgs e)
        {
            var checkConnectivity = App.Admins.TestConnection();
            if (checkConnectivity.Status)
            {
                if (!Validate()) return;

                _user = txtUsuario.Text;
                _pass = txtPassword.Password;
                LoginDb();
            }
            else
            {
                new Inicio(checkConnectivity).Show();
                Close();
            }
        }
        /// <summary>
        /// Method that detects if the user pressed "Enter" when at
        /// Login form and calls the Iniciar Sesion button event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MetroWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                BtnIniciarSesion_Click(sender, e);
        }
        #endregion
    }
}