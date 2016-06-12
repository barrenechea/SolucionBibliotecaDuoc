﻿ using System;
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
                Close();
            }
        }

        private void LoadData()
        {
            if (_isExtend)
            {
                lblDevolucion.Visibility = Visibility.Visible;
                lblDevolucion.Content = App.Prestamo.DetPrestamoPersistenceList[0].FecDevolucion.ToString("dd-MM-yyyy");
            }
            lblNomUsuario.Content = string.Format("{0} {1}", App.Users.PersonaPersistence.Nombre, App.Users.PersonaPersistence.Apellido);
            foreach (var lib in App.Prestamo.InfoLibrosPersistence)
            {
                _dataGridList.Add(lib);
            }
            lstLibro.Items.Refresh();
            if (_isExtend) FixWindow();
        }

        private void FixWindow()
        {
            grdExtender.Visibility = Visibility.Visible;
            App.Prestamo.FetchPrestamo(_nroFicha);
            lblExtendido.Content = App.Prestamo.DetPrestamoPersistenceList[0].Renovacion;
            if (App.Prestamo.DetPrestamoPersistenceList[0].Renovacion >= 3)
            {
                btnExecute.IsEnabled = false;
                lblStatus.Content = "No se puede extender un libro más de tres veces";
            }
            if (App.Prestamo.DetPrestamoPersistenceList[0].FecDevolucion >= DateTime.Now) return;
            btnExecute.IsEnabled = false;
            lblStatus.Content = "El alumno tiene morosidad, no puede extender el libro";
        }

        private void BtnLogout_OnClick(object sender, RoutedEventArgs e)
        {
            App.Admins.Logout();
            new Inicio().Show();
            Close();
        }

        private void BtnVolver_Click(object sender, RoutedEventArgs e)
        {
            Clear();
            new PanelAdmin().Show();
            Close();
        }

        private void Clear()
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

        private void BtnExecute_Click(object sender, RoutedEventArgs e)
        {
            var codPrestamo = App.Prestamo.CodigoPrestamo(_nroFicha);
            if (_isExtend)
            {
                App.Prestamo.ExtenderPrestamo(codPrestamo.ToString(), App.Prestamo.DetPrestamoPersistenceList[0].CodLibro,
                    App.Prestamo.DetPrestamoPersistenceList[0].FecDevolucion);
                LoadData();
            }
            else
            {
                if (App.Prestamo.DetPrestamoPersistenceList[0].FecDevolucion < DateTime.Now)
                {
                    App.Prestamo.HojaDeMorosidad(_nroFicha, App.Prestamo.DiasAtraso(App.Prestamo.DetPrestamoPersistenceList[0].FecDevolucion));
                    lblStatus.Content = App.Prestamo.DiasAtraso(App.Prestamo.DetPrestamoPersistenceList[0].FecDevolucion);
                }
                var res = App.Prestamo.DevolverLibro(codPrestamo.ToString(), App.Prestamo.DetPrestamoPersistenceList[0].CodLibro,
                    App.Prestamo.DetPrestamoPersistenceList[0].FecDevolucion);
                new PanelAdmin(res).Show();
                Close();
            }
        }
    }
}