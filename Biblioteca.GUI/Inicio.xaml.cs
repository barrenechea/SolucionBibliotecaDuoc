using System.ComponentModel;
using System.Threading;
using System.Windows;
using Biblioteca.Entidad;
using MahApps.Metro.Controls.Dialogs;

namespace Biblioteca.GUI
{
    /// <summary>
    /// Lógica de interacción para Inicio.xaml
    /// </summary>
    public partial class Inicio
    {
        #region Attributes
        private readonly BackgroundWorker _checkConnection = new BackgroundWorker();
        private readonly BackgroundWorker _login = new BackgroundWorker();
        private ProgressDialogController _controller;
        private readonly Message _constructorMessage;
        private Message _connectionMessage;
        private LoginDialogData _loginData;

        #endregion
        #region Constructors
        /// <summary>
        /// Generates a new instance of Inicio
        /// </summary>
        public Inicio()
        {
            InitializeComponent();
            _constructorMessage = null;
            _checkConnection.DoWork += Conn_DoWork;
            _checkConnection.WorkerReportsProgress = true;
            _checkConnection.ProgressChanged += Conn_ProgressChanged;
            _checkConnection.RunWorkerCompleted += Conn_RunWorkerCompleted;
            _login.DoWork += Login_DoWork;
            _login.RunWorkerCompleted += Login_RunWorkerCompleted;
            _checkConnection.RunWorkerAsync();
        }
        /// <summary>
        /// Generates a new instance of Inicio
        /// </summary>
        /// <param name="msg">Message to be shown when the Window loads</param>
        public Inicio(Message msg)
        {
            InitializeComponent();
            _constructorMessage = msg;
            _checkConnection.DoWork += Conn_DoWork;
            _checkConnection.WorkerReportsProgress = true;
            _checkConnection.ProgressChanged += Conn_ProgressChanged;
            _checkConnection.RunWorkerCompleted += Conn_RunWorkerCompleted;
            _login.DoWork += Login_DoWork;
            _login.RunWorkerCompleted += Login_RunWorkerCompleted;
            _checkConnection.RunWorkerAsync();
        }
        #endregion
        #region Test Connection BackgroundWorker Methods
        /// <summary>
        /// What the BackgroundWorker does while it's running the task
        /// </summary>
        /// <param name="sender">The object that triggered this event</param>
        /// <param name="e">Parameters (optional)</param>
        private void Conn_DoWork(object sender, DoWorkEventArgs e)
        {
            var count = 0;
            while (true)
            {
                if (_connectionMessage != null)
                {
                    if (_connectionMessage.Status) break;
                    Thread.Sleep(5000);
                }
                _connectionMessage = App.Admins.TestConnection();
                count++;
                if (_connectionMessage.Status) break;

                ((BackgroundWorker)sender).ReportProgress(count);
            }
        }
        /// <summary>
        /// What the BackgroundWorker updates on the Window when progress has changed
        /// </summary>
        /// <param name="sender">The object that triggered this event</param>
        /// <param name="e">Parameters (optional)</param>
        private void Conn_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            lblStatus.Content = string.Format("Estado: {0} (Intento {1})", _connectionMessage.Mensaje, e.ProgressPercentage);
        }
        /// <summary>
        /// What the BackgroundWorker does when has finished running the task
        /// </summary>
        /// <param name="sender">The object that triggered this event</param>
        /// <param name="e">Parameters (optional)</param>
        private void Conn_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            lblStatus.Content = "Estado: Conectado al servidor";
            if (_connectionMessage.Status)
                btnCatalogo.IsEnabled = btnLogin.IsEnabled = btnBenchmark.IsEnabled = true;
        }
        #endregion
        #region Login BackgrounWorker Methods
        /// <summary>
        /// What the BackgroundWorker does while it's running the task
        /// </summary>
        /// <param name="sender">The object that triggered this event</param>
        /// <param name="e">Parameters (optional)</param>
        private void Login_DoWork(object sender, DoWorkEventArgs e)
        {
            _connectionMessage = App.Admins.TestConnection();
            if (_connectionMessage.Status)
                _connectionMessage = App.Admins.Login(_loginData.Username, _loginData.Password);
        }
        /// <summary>
        /// What the BackgroundWorker does when has finished running the task
        /// </summary>
        /// <param name="sender">The object that triggered this event</param>
        /// <param name="e">Parameters (optional)</param>
        private async void Login_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            await _controller.CloseAsync();
            if (_connectionMessage.Status)
            {
                new PanelAdmin().Show();
                Close();
            }
            else ShowNormalDialog("Error", _connectionMessage.Mensaje);
        }
        #endregion
        #region Custom Methods
        /// <summary>
        /// Generates an async dialog to fetch Login credentials
        /// </summary>
        private async void LoginWindow()
        {
            var aux = new LoginDialogSettings
            {
                EnablePasswordPreview = true,
                NegativeButtonVisibility = Visibility.Visible,
                UsernameWatermark = "Nombre de Usuario",
                PasswordWatermark = "Contraseña",
                AffirmativeButtonText = "Iniciar sesión",
                NegativeButtonText = "Cancelar"
            };
            var loginData = await this.ShowLoginAsync("Acceder", "Ingrese sus credenciales", aux);

            if (loginData == null) return;

            if (string.IsNullOrWhiteSpace(loginData.Username)) 
                ShowNormalDialog("Error", "Debe ingresar un usuario");
            else if (string.IsNullOrWhiteSpace(loginData.Password))
                ShowNormalDialog("Error", "Debe ingresar una contraseña");
            else
            {
                _loginData = loginData;
                LoggingIn();
            }
            
        }
        /// <summary>
        /// Genetares an async Logging In dialog
        /// </summary>
        private async void LoggingIn()
        {
            _controller = await this.ShowProgressAsync("Por favor espere", "Iniciando sesión...");
            _controller.SetIndeterminate();
            _login.RunWorkerAsync();
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
        private void Inicio_OnLoaded(object sender, RoutedEventArgs e)
        {
            if (_constructorMessage != null)
                ShowNormalDialog("Información", _constructorMessage.Mensaje);
        }
        /// <summary>
        /// Event that loads when user clicks on the Login button
        /// </summary>
        /// <param name="sender">The object that triggered this event</param>
        /// <param name="e">Parameters (optional)</param>
        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            LoginWindow();
        }
        /// <summary>
        /// Event that loads when user clicks on the Ver Catalogo button
        /// </summary>
        /// <param name="sender">The object that triggered this event</param>
        /// <param name="e">Parameters (optional)</param>
        private void btnCatalogo_Click(object sender, RoutedEventArgs e)
        {
            new Catalogo().Show();
            Close();
        }
        /// <summary>
        /// Event that loads when user clicks on the Benchmark button
        /// </summary>
        /// <param name="sender">The object that triggered this event</param>
        /// <param name="e">Parameters (optional)</param>
        private void BtnBenchmark_OnClicknchmark_Click(object sender, RoutedEventArgs e)
        {
            // ToDo DELETE THIS METHOD WHEN RELEASING THE FINAL APP!
            new Test().Show();
        }
        #endregion
    }
}