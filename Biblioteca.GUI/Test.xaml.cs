using System.ComponentModel;
using System.Diagnostics;

namespace Biblioteca.GUI
{
    /// <summary>
    /// Lógica de interacción para Inicio1.xaml
    /// </summary>
    public partial class Test
    {
        #region Attributes
        private readonly BackgroundWorker _checkConnection = new BackgroundWorker();
        private readonly Stopwatch _tiempoInicial = new Stopwatch();
        private decimal _cantidad;
        #endregion
        #region Constructor
        public Test()
        {
            InitializeComponent();
            _checkConnection.DoWork += Conn_DoWork;
            _checkConnection.WorkerReportsProgress = true;
            _checkConnection.ProgressChanged += Conn_ProgressChanged;
            _checkConnection.RunWorkerCompleted += Conn_RunWorkerCompleted;
            _checkConnection.RunWorkerAsync();
        }
        #endregion
        #region Custom Methods
        private bool doWork = true;
        private void Conn_DoWork(object sender, DoWorkEventArgs e)
        {
            var count = 0;
            var timelapse = new Stopwatch();
            _tiempoInicial.Start();
            while (doWork)
            {
                timelapse.Start();
                App.Users.FetchComunas();
                timelapse.Stop();
                _cantidad = timelapse.ElapsedMilliseconds;
                timelapse.Reset();
                count++;
                ((BackgroundWorker)sender).ReportProgress(count);
            }
        }
        private void Conn_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            lblStatus.Content = string.Format("Querys: {0}", e.ProgressPercentage);
            lblSpeed.Content = string.Format("Delay: {0} milisec.", _cantidad);
            var tiempoInicial = (int)(_tiempoInicial.ElapsedMilliseconds / 1000);
            if (tiempoInicial > 0)
                lblPromedio.Content = string.Format("Promedio: {0} querys/seg", (e.ProgressPercentage / _tiempoInicial.Elapsed.Seconds));
            lblTiempoTranscurrido.Content = string.Format("Transcurridos {0} segs.", tiempoInicial);
        }
        private void Conn_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            _tiempoInicial.Stop();
        }
        #endregion

        private void MetroWindow_Closing(object sender, CancelEventArgs e)
        {
            doWork = false;
        }
    }
}