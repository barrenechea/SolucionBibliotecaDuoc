using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Biblioteca.Entidad;
using MahApps.Metro.Controls.Dialogs;

namespace Biblioteca.GUI
{
    /// <summary>
    /// Interaction logic for PrestamoManager.xaml
    /// </summary>
    public partial class PrestamoManager
    {
        #region Attributes
        private readonly List<Libro> _dataGridList;
        private readonly bool _isExtend;
        private bool _isStudent;
        private int _nroFicha;
        #endregion
        #region Constructor
        /// <summary>
        /// Initializes a new PrestamoManager Window
        /// </summary>
        /// <param name="isExtend">Defines if this window is to Extend a Prestamo. Otherwise, is for Devolution</param>
        public PrestamoManager(bool isExtend)
        {

            InitializeComponent();

            _isExtend = isExtend;
            _dataGridList = new List<Libro>();
            lstLibro.ItemsSource = _dataGridList;

            lblTitulo.Content = btnExecute.Content = _isExtend ? "Extender prestamo" : "Devolver libro";
        }
        #endregion
        #region Custom Methods
        /// <summary>
        /// Opens a Search Dialog, in order to get a Numero de Ficha related to an Usuario
        /// </summary>
        private async void SearchPrestamoDialog()
        {
            var settings = new MetroDialogSettings
            {
                AffirmativeButtonText = "Buscar",
                NegativeButtonText = "Cancelar",
                ColorScheme = MetroDialogOptions.ColorScheme
            };

            var nroFicha = await this.ShowInputAsync("Buscar", "Ingrese el número de ficha del usuario", settings);

            if (string.IsNullOrWhiteSpace(nroFicha))
            {
                new PanelAdmin().Show();
                Close();
                return;
            }

            var exists = App.Users.FetchUsuario(nroFicha);
            if (exists.Status)
            {
                _nroFicha = int.Parse(nroFicha);
                WindowLogic();
            }
            else
            {
                new PanelAdmin(exists).Show();
                Close();
            }
        }
        /// <summary>
        /// Applies logic, checking if its for Extend or Devolution and fetch related data
        /// </summary>
        private void WindowLogic()
        {
            var fetch = App.Prestamo.FetchPrestamo(_nroFicha);
            if (fetch.Status)
            {
                fetch = App.Users.IsStudent(_nroFicha);
                _isStudent = fetch.Status;
                if (_isStudent || !_isExtend)
                {
                    LoadData();
                    return;
                }
            }

            new PanelAdmin(fetch).Show();
            Clear();
            Close();
        }
        /// <summary>
        /// Loads the fetched data into Window fields
        /// </summary>
        private void LoadData()
        {
            lblNomUsuario.Content = string.Format("{0} {1}", App.Users.PersonaPersistence.Nombre, App.Users.PersonaPersistence.Apellido);
            if (_isStudent)
            {
                lblDevolucion.Content = App.Prestamo.DetPrestamoPersistenceList[0].FecDevolucion.ToString("dd-MM-yyyy");
                lblRenovacion.Content = App.Prestamo.DetPrestamoPersistenceList[0].Renovacion;
            }
            else lblDevolucion.Content = "Fin del semestre";
            
            foreach (var lib in App.Prestamo.InfoLibrosPersistence) _dataGridList.Add(lib);

            lstLibro.Items.Refresh();
            FixWindow();
        }
        /// <summary>
        /// Shows or hide items of the Window, based on if it's for Extend, or Devolution
        /// </summary>
        private void FixWindow()
        {
            if (!_isExtend) return;

            grdRenovacion.Visibility = Visibility.Visible;
            if (App.Prestamo.DetPrestamoPersistenceList[0].Renovacion >= 3)
                lblStatus.Content = "No se puede extender un libro más de tres veces";

            if (App.Prestamo.DetPrestamoPersistenceList[0].FecDevolucion < DateTime.Now)
                lblStatus.Content = "El alumno tiene morosidad, no puede extender el libro";
        }
        /// <summary>
        /// Checks if the user can extend a Libro or not,
        /// based on previous renewals and current date.
        /// </summary>
        /// <returns>Boolean that indicates if can Extend a Libro</returns>
        private static bool ValidationExtend()
        {
            if (App.Prestamo.DetPrestamoPersistenceList[0].Renovacion >= 3) return false;
            return App.Prestamo.DetPrestamoPersistenceList[0].FecDevolucion >= DateTime.Now;
        }
        /// <summary>
        /// Execution logic based on if the Window is for Extend or Devolution
        /// </summary>
        private void Execute()
        {
            var codPrestamo = App.Prestamo.CodigoPrestamo(_nroFicha);
            if (_isExtend)
            {
                var res = App.Prestamo.ExtenderPrestamo(codPrestamo);
                
                new PanelAdmin(res).Show();
                Clear();
                Close();
            }
            else
            {
                var libro = (Libro)lstLibro.SelectedItem;
                codPrestamo = _isStudent ? App.Prestamo.CodigoPrestamo(_nroFicha) : App.Prestamo.CodigoPrestamo(_nroFicha, libro.CodLibro);
                var execute = App.Prestamo.DevolverLibro(codPrestamo, libro.CodLibro, _isStudent);
                App.Libros.Increase(libro.CodLibro);

                if (!execute.Status) App.Morosidad.Insert(_nroFicha, App.Prestamo.DiasAtraso(), libro.CodLibro);

                new PanelAdmin(execute).Show();
                Clear();
                Close();
            }
        }
        /// <summary>
        /// Method that clear persistant items on Controllers related to this View
        /// </summary>
        private static void Clear()
        {
            App.Prestamo.ClearPersistantData();
            App.Users.ClearPersistantData();
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
        /// Event that loads when user clicks on the Logout button
        /// </summary>
        /// <param name="sender">The object that triggered this event</param>
        /// <param name="e">Parameters (optional)</param>
        private void BtnLogout_OnClick(object sender, RoutedEventArgs e)
        {
            Clear();
            App.Admins.Logout();
            new Inicio().Show();
            Close();
        }
        /// <summary>
        /// Event that loads when user clicks on the Back button
        /// </summary>
        /// <param name="sender">The object that triggered this event</param>
        /// <param name="e">Parameters (optional)</param>
        private void BtnVolver_Click(object sender, RoutedEventArgs e)
        {
            new PanelAdmin().Show();
            Clear();
            Close();
        }
        /// <summary>
        /// Automatic event that's triggered when the Window has initialized
        /// </summary>
        /// <param name="sender">The object that triggered this event</param>
        /// <param name="e">Parameters (optional)</param>
        private void WindowHasLoaded(object sender, RoutedEventArgs e)
        {
            SearchPrestamoDialog();

            for (var i = 3; i < 10; i++)
            {
                lstLibro.Columns[i].Visibility = Visibility.Hidden;
            }

            lstLibro.Columns[0].Header = "CODIGO";
        }
        /// <summary>
        /// Triggered event when the User clicks on something at the DataGrid
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lstLibros_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_isExtend)
            {
                if (ValidationExtend()) btnExecute.IsEnabled = lstLibro.SelectedIndex != -1;
            }
            else
                btnExecute.IsEnabled = lstLibro.SelectedIndex != -1;
        }
        /// <summary>
        /// Event that loads when user clicks on the Execute button
        /// </summary>
        /// <param name="sender">The object that triggered this event</param>
        /// <param name="e">Parameters (optional)</param>
        private void BtnExecute_Click(object sender, RoutedEventArgs e)
        {
            if (App.Prestamo.TestConnection().Status)
            {
                Execute();
            }
            else
                ShowNormalDialog("Error", "Se ha perdido la conexión con el servidor. Intente nuevamente más tarde");
        }
        #endregion
    }
}