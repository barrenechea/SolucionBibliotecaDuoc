using System;
using System.Windows;
using Biblioteca.Entidad;

namespace Biblioteca.GUI
{
    /// <summary>
    /// Interaction logic for NewApoderado.xaml
    /// </summary>
    public partial class UserAddApoderado
    {
        #region Constructor
        public UserAddApoderado()
        {
            InitializeComponent();
            cmbComuna.ItemsSource = App.Users.FetchComunas();
            cmbComuna.DisplayMemberPath = "NomComuna";
            cmbComuna.SelectedValuePath = "CodComuna";
            txtRun.Text = App.Users.RunApoderado;
            txtParentesco.Text = App.Users.Parentesco;
        }
        #endregion
        #region Custom Methods
        private bool Validation()
        {
            if (string.IsNullOrWhiteSpace(txtNombre.Text))
            {
                lblStatus.Content = "Debe llenar todos los campos";
                txtNombre.Focus();
                return false;
            }
            if (string.IsNullOrWhiteSpace(txtApellido.Text))
            {
                lblStatus.Content = "Debe llenar todos los campos";
                txtApellido.Focus();
                return false;
            }
            if (string.IsNullOrWhiteSpace(txtDireccion.Text))
            {
                lblStatus.Content = "Debe llenar todos los campos";
                txtDireccion.Focus();
                return false;
            }
            if (cmbComuna.SelectedItem == null)
            {
                lblStatus.Content = "Debe llenar todos los campos";
                cmbComuna.Focus();
                return false;
            }
            if (string.IsNullOrWhiteSpace(txtFonoFijo.Text))
            {
                lblStatus.Content = "Debe llenar todos los campos";
                txtFonoFijo.Focus();
                return false;
            }
            int i;
            if (!int.TryParse(txtFonoFijo.Text, out i))
            {
                lblStatus.Content = "El teléfono sólo debe tener números";
                txtFonoFijo.Focus();
                return false;
            }
            if (string.IsNullOrWhiteSpace(txtFonoCel.Text))
            {
                lblStatus.Content = "Debe llenar todos los campos";
                txtFonoCel.Focus();
                return false;
            }
            if (!int.TryParse(txtFonoCel.Text, out i))
            {
                lblStatus.Content = "El teléfono sólo debe tener números";
                txtFonoCel.Focus();
                return false;
            }
            lblStatus.Content = string.Empty;
            return true;
        }
        private void AddApoderado()
        {
            var insertEstudiante = App.Users.Insert();
            if (insertEstudiante.Status)
            {
                var preload = App.Users.PreloadPersona(txtRun.Text, txtNombre.Text, txtApellido.Text, txtDireccion.Text,
                    (int) cmbComuna.SelectedValue, txtFonoFijo.Text, txtFonoCel.Text, App.Users.FetchLastNroFicha(),
                    txtParentesco.Text);
                if (preload.Status)
                {
                    var insertApoderado = App.Users.Insert();
                    if (insertApoderado.Status)
                    {
                        App.Users.ClearPersistantData();
                        new PanelAdmin(insertEstudiante).Show();
                        Close();
                        return;
                    }
                    lblStatus.Content = insertApoderado.Mensaje + " (Apoderado)";
                    return;
                }
                lblStatus.Content = preload.Mensaje + " (Apoderado)";
                return;
            }
            lblStatus.Content = insertEstudiante.Mensaje + " (Estudiante)";
        }
        #endregion
        #region Event Handlers
        private void BtnLogout_OnClick(object sender, RoutedEventArgs e)
        {
            App.Login.Logout();
            new Inicio().Show();
            Close();
        }
        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            new UserManager(true).Show();
            Close();
        }
        private void BtnAgregar_OnClick(object sender, RoutedEventArgs e)
        {
            if (!Validation()) return;
            AddApoderado();
        }
        #endregion
    }
}