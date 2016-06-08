using System.ComponentModel;
using System.Threading;
using System.Windows;
using System.Windows.Media;
using Biblioteca.Entidad;
using MahApps.Metro.Controls.Dialogs;

namespace Biblioteca.GUI
{
    /// <summary>
    /// Lógica de interacción para Inicio1.xaml
    /// </summary>
    public partial class Inicio
    {
        #region Attributes
        private readonly BackgroundWorker _checkConnection = new BackgroundWorker();
        private readonly Message _constructorMessage;
        private Message _connectionMessage;
        #endregion
        #region Constructors
        public Inicio()
        {
            InitializeComponent();
            _constructorMessage = null;
            _checkConnection.DoWork += Conn_DoWork;
            _checkConnection.WorkerReportsProgress = true;
            _checkConnection.ProgressChanged += Conn_ProgressChanged;
            _checkConnection.RunWorkerCompleted += Conn_RunWorkerCompleted;
            _checkConnection.RunWorkerAsync();
        }
        public Inicio(Message msg)
        {
            InitializeComponent();
            _constructorMessage = msg;
            _checkConnection.DoWork += Conn_DoWork;
            _checkConnection.WorkerReportsProgress = true;
            _checkConnection.ProgressChanged += Conn_ProgressChanged;
            _checkConnection.RunWorkerCompleted += Conn_RunWorkerCompleted;
            _checkConnection.RunWorkerAsync();
        }
        #endregion
        #region Custom Methods
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
        private void Conn_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            lblStatus.Content = string.Format("Estado: {0} (Intento {1})", _connectionMessage.Mensaje, e.ProgressPercentage);
            lblStatus.Foreground = _connectionMessage.Status ? Brushes.Green : Brushes.Red;
        }
        private void Conn_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            lblStatus.Content = "Estado: Conectado al servidor";
            lblStatus.Foreground = _connectionMessage.Status ? Brushes.Green : Brushes.Red;
            if (_connectionMessage.Status)
                btnCatalogo.IsEnabled = btnLogin.IsEnabled = btnBenchmark.IsEnabled = true;
        }
        private async void ShowNormalDialog(string title, string message)
        {
            await this.ShowMessageAsync(title, message);
        }
        #endregion
        #region Event Handlers
        private void Inicio_OnLoaded(object sender, RoutedEventArgs e)
        {
            if (_constructorMessage != null)
                ShowNormalDialog("Información", _constructorMessage.Mensaje);
        }
        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            new Login().Show();
            Close();
        }
        private void btnCatalogo_Click(object sender, RoutedEventArgs e)
        {
            new Catalogo().Show();
            Close();
        }
        #endregion


        private void BtnBenchmark_OnClicknchmark_Click(object sender, RoutedEventArgs e)
        {
            new Test().Show();
        }
    }
}