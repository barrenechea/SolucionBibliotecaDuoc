using System.Collections.Generic;
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
        private List<Libro> _libros;
        private List<TipoLibro> _tipoLibros; 
        #endregion
        #region Constructor
        /// <summary>
        /// Generates a new Instance of Catalogo
        /// </summary>
        public Catalogo()
        {
            InitializeComponent();
        }
        #endregion
        #region Custom Methods
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
        #region Event Handlers
        /// <summary>
        /// Event that loads itself when the Window was loaded
        /// </summary>
        /// <param name="sender">The object that triggered this event</param>
        /// <param name="e">Parameters (optional)</param>
        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            var test = App.Admins.TestConnection();
            if (test.Status)
            {
                _libros = App.Libros.FetchAllAvailable();
                _tipoLibros = App.Libros.FetchTipoLibros();
                lstLibros.ItemsSource = _libros;
                lstLibros.Columns[4].Visibility = Visibility.Hidden;
                lstLibros.Columns[5].Visibility = Visibility.Hidden;
                lstLibros.Columns[7].Visibility = Visibility.Hidden;
                lstLibros.Columns[8].Visibility = Visibility.Hidden;
                lstLibros.Columns[0].Header = "CODIGO";
                lstLibros.Columns[9].Header = "COPIAS DISPONIBLES";
            }
            else
            {
                new Inicio(test).Show();
                Close();
            }
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
            new Inicio().Show();
            Close();
        }
        #endregion
    }
}