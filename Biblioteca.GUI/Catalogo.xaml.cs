using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using Biblioteca.Entidad;
using MahApps.Metro.Controls.Dialogs;

namespace Biblioteca.GUI
{
    /// <summary>
    /// Lógica de interacción para Catalogo.xaml
    /// </summary>
    public partial class Catalogo
    {
        #region Attributes
        private readonly BackgroundWorker _fetch = new BackgroundWorker();
        private List<Libro> _libros;
        private ProgressDialogController _controller;
        private List<TipoLibro> _tipoLibros;
        private Message _connectionMessage;
        #endregion
        #region Constructor
        /// <summary>
        /// Generates a new Instance of Catalogo
        /// </summary>
        public Catalogo()
        {
            InitializeComponent();
            _libros = new List<Libro>();
            _tipoLibros = new List<TipoLibro>();
            _fetch.DoWork += Fetch_DoWork;
            _fetch.RunWorkerCompleted += Fetch_RunWorkerCompleted;
        }
        #endregion
        #region Custom Methods
        /// <summary>
        /// Async method that initializes the Fetch Dialog
        /// </summary>
        private async void InitializeFetchDialog()
        {
            _controller = await this.ShowProgressAsync("Por favor espere", "Obteniendo listado de libros disponibles...");
            _controller.SetIndeterminate();
            _fetch.RunWorkerAsync();
        }
        /// <summary>
        /// Shows all the details of a selected Libro from the DataGrid as a Dialog inside the Window
        /// </summary>
        private void ShowDetail()
        {
            var libro = (Libro)lstLibros.SelectedItem;
            var nomTipo = string.Empty;
            foreach (var tipo in _tipoLibros.Where(tipo => tipo.CodTipo == libro.CodTipo))
            {
                nomTipo = tipo.NomTipo;
            }
            var sb = new StringBuilder();
            sb.AppendLine(string.Format("Nombre de Libro: {0}", libro.Titulo));
            sb.AppendLine(string.Format("Autor: {0}", libro.Autor));
            sb.AppendLine(string.Format("Páginas: {0}", libro.NroPaginas));
            sb.AppendLine(string.Format("Tipo: {0}", nomTipo));
            sb.AppendLine();
            sb.AppendLine("Argumento:");
            sb.AppendLine(libro.Argumento);
            sb.AppendLine();
            sb.AppendLine(string.Format("Solicita este libro con tu número de ficha, y dando el código: {0}", libro.CodLibro));

            ShowNormalDialog("Detalle de Libro", sb.ToString());
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
        #region Fetch BackgrounWorker Methods
        /// <summary>
        /// What the BackgroundWorker does while it's running the task
        /// </summary>
        /// <param name="sender">The object that triggered this event</param>
        /// <param name="e">Parameters (optional)</param>
        private void Fetch_DoWork(object sender, DoWorkEventArgs e)
        {
            _connectionMessage = App.Admins.TestConnection();
            if (_connectionMessage.Status)
            {
                var libros = App.Libros.FetchAllAvailable();
                foreach (var lib in libros) _libros.Add(lib);
                _tipoLibros = App.Libros.FetchTipoLibros();
            }
        }
        /// <summary>
        /// What the BackgroundWorker does when has finished running the task
        /// </summary>
        /// <param name="sender">The object that triggered this event</param>
        /// <param name="e">Parameters (optional)</param>
        private async void Fetch_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            await _controller.CloseAsync();
            if (_connectionMessage.Status)
                lstLibros.Items.Refresh();
            else
            {
                new Inicio(_connectionMessage).Show();
                Close();
            }
        }

        #endregion
        #region Event Handlers
        /// <summary>
        /// Event that loads itself when the Window was loaded
        /// </summary>
        /// <param name="sender">The object that triggered this event</param>
        /// <param name="e">Parameters (optional)</param>
        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            lstLibros.ItemsSource = _libros;
            lstLibros.Columns[4].Visibility = Visibility.Hidden;
            lstLibros.Columns[5].Visibility = Visibility.Hidden;
            lstLibros.Columns[7].Visibility = Visibility.Hidden;
            lstLibros.Columns[8].Visibility = Visibility.Hidden;
            lstLibros.Columns[0].Header = "CODIGO";
            lstLibros.Columns[9].Header = "COPIAS DISPONIBLES";

            InitializeFetchDialog();
        }
        /// <summary>
        /// Triggered event when the User clicks on something at the DataGrid
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lstLibros_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            BtnVerDetalle.IsEnabled = lstLibros.SelectedIndex != -1;
        }
        /// <summary>
        /// Event that loads when the user clicks on the Ver Detalle button
        /// </summary>
        /// <param name="sender">The object that triggered this event</param>
        /// <param name="e">Parameters (optional)</param>
        private void BtnVerDetalle_Click(object sender, RoutedEventArgs e)
        {
            ShowDetail();
        }
        /// <summary>
        /// Event that loads when user clicks on the Back button
        /// </summary>
        /// <param name="sender">The object that triggered this event</param>
        /// <param name="e">Parameters (optional)</param>
        private void BtnVolver_Click(object sender, RoutedEventArgs e)
        {
            _libros = null;
            new Inicio().Show();
            Close();
        }
        #endregion
    }
}