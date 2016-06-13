 using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Biblioteca.Entidad;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;

namespace Biblioteca.GUI
{
    /// <summary>
    /// Interaction logic for PrestamoExtender.xaml
    /// </summary>
    public partial class PrestamoExtender
    {
        private int _nroFicha;
        private readonly bool _isExtend;
        private List<Libro> _dataGridList = new List<Libro>();
        private bool _isStudent;
        
        public PrestamoExtender(bool isExtend)
        {
            
            InitializeComponent();
            _isExtend = isExtend;
            lstLibro.ItemsSource = _dataGridList;
            lblTitulo.Content = btnExecute.Content = _isExtend ? "Extender prestamo" : "Devolver libro";
        }

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
                Clear();
                return;
            }
            _nroFicha = int.Parse(nroFicha);
            var exists = App.Users.FetchUsuario(nroFicha);
            if (exists.Status)
            {
                exists = App.Prestamo.FetchPrestamo(_nroFicha);
                if (!exists.Status)
                {
                    new PanelAdmin(exists).Show();
                    Clear();
                    Close();
                    return;
                }
                exists = App.Users.IsStudent(int.Parse(nroFicha));
                if (exists.Status)
                {
                    _nroFicha = int.Parse(nroFicha);
                    LoadData();
                }
                else
                {
                    if (_isExtend)
                    {
                        new PanelAdmin(exists).Show();
                        Clear();
                        Close();
                    }
                    else
                    {
                        _nroFicha = int.Parse(nroFicha);
                        LoadData();
                    }
                    
                }
            }
            else
            {
                new PanelAdmin(exists).Show();
                Clear();
                Close();
            }
        }

        private void LoadData()
        {
            _isStudent = App.Users.IsStudent(_nroFicha).Status;
            lblNomUsuario.Content = string.Format("{0} {1}", App.Users.PersonaPersistence.Nombre, App.Users.PersonaPersistence.Apellido);
            if (_isStudent)
            {
                lblDevolucion.Content = App.Prestamo.DetPrestamoPersistenceList[0].FecDevolucion.ToString("dd-MM-yyyy");
                lblRenovacion.Content = App.Prestamo.DetPrestamoPersistenceList[0].Renovacion;
            }
            else
            {
                lblDevolucion.Content = "Fin del semestre";
            }
            foreach (var lib in App.Prestamo.InfoLibrosPersistence)
                _dataGridList.Add(lib);

            lstLibro.Items.Refresh();
            FixWindow();
        }

        private void FixWindow()
        {
            
            if(_isExtend)
            { 
                grdRenovacion.Visibility = Visibility.Visible;
                if (App.Prestamo.DetPrestamoPersistenceList[0].Renovacion >= 3)
                {
                    btnExecute.IsEnabled = false;
                    lblStatus.Content = "No se puede extender un libro más de tres veces";
                }
                if (App.Prestamo.DetPrestamoPersistenceList[0].FecDevolucion >= DateTime.Now) return;
                btnExecute.IsEnabled = false;
                lblStatus.Content = "El alumno tiene morosidad, no puede extender el libro";
            }
        }

        private void BtnLogout_OnClick(object sender, RoutedEventArgs e)
        {
            App.Admins.Logout();
            new Inicio().Show();
            Close();
        }

        private void BtnVolver_Click(object sender, RoutedEventArgs e)
        {
            new PanelAdmin().Show();
            Clear();
            Close();
        }

        private static void Clear()
        {
            App.Prestamo.ClearPersistantData();
            App.Users.ClearPersistantData();
        }

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
            btnExecute.IsEnabled = lstLibro.SelectedIndex != -1;
        }

        private void BtnExecute_Click(object sender, RoutedEventArgs e)
        {
            var codPrestamo = App.Prestamo.CodigoPrestamo(_nroFicha);
            if (_isExtend)
            {
                var res = App.Prestamo.ExtenderPrestamo(codPrestamo, App.Prestamo.DetPrestamoPersistenceList[0].CodLibro,
                    App.Prestamo.DetPrestamoPersistenceList[0].FecDevolucion);
                new PanelAdmin(res).Show();
                Clear();
                Close();
            }
            else
            {
                var libro = (Libro)lstLibro.SelectedItem;
                codPrestamo = !App.Users.IsStudent(_nroFicha).Status ? App.Prestamo.CodigoPrestamoLibro(_nroFicha, libro.CodLibro) : App.Prestamo.CodigoPrestamo(_nroFicha);
                var res = App.Prestamo.DevolverLibro(codPrestamo, libro.CodLibro,
                    App.Prestamo.DetPrestamoPersistenceList[0].FecDevolucion, App.Users.IsStudent(_nroFicha).Status);
                if (App.Prestamo.DetPrestamoPersistenceList[0].FecDevolucion < DateTime.Now)
                    App.Prestamo.HojaDeMorosidad(_nroFicha, App.Prestamo.DiasAtraso(App.Prestamo.DetPrestamoPersistenceList[0].FecDevolucion),libro.CodLibro);
                
                new PanelAdmin(res).Show();
                Clear();
                Close();
            }
        }
    }
}