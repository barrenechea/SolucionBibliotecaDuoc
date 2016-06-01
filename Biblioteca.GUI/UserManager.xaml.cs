using System;
using System.Windows;
using System.Windows.Controls;
using Biblioteca.Entidad;
using MahApps.Metro.Controls.Dialogs;

namespace Biblioteca.GUI
{
    /// <summary>
    /// Interaction logic for NewUser.xaml
    /// </summary>
    public partial class UserManager
    {
        #region Attributes
        private readonly bool _isAdd;
        private bool _isEstudiante;
        #endregion
        #region Constructor
        public UserManager(bool isAdd)
        {
            InitializeComponent();
            _isAdd = isAdd;

            lblTitulo.Content = BtnExecute.Content = _isAdd ? "Agregar Usuario" : "Modificar Usuario";

            if (_isAdd && App.Users.PersonaPersistence != null) LoadData();
        }
        #endregion
        #region Custom Methods
        private async void SelectTypeDialog()
        {
            var settings = new MetroDialogSettings
            {
                AffirmativeButtonText = "Estudiante",
                NegativeButtonText = "Funcionario",
                ColorScheme = MetroDialogOptions.ColorScheme
            };

            var result = await this.ShowMessageAsync("¿Qué hacer?", "¿Qué tipo de usuario desea agregar?",
                MessageDialogStyle.AffirmativeAndNegative, settings);

            _isEstudiante = result == MessageDialogResult.Affirmative;
            FixWindow();
        }
        private async void SearchUserDialog()
        {
            txtRun.IsEnabled = txtRunApoderado.IsEnabled = false;
            var settings = new MetroDialogSettings
            {
                AffirmativeButtonText = "Buscar",
                NegativeButtonText = "Cancelar",
                ColorScheme = MetroDialogOptions.ColorScheme
            };

            var nroFicha = await this.ShowInputAsync("Buscar", "Ingrese un código de usuario a buscar", settings);

            if (string.IsNullOrWhiteSpace(nroFicha))
            {
                new PanelAdmin().Show();
                Close();
                return;
            }

            var exists = App.Users.FetchUsuario(nroFicha);
            if (exists.Status)
            {
                LoadData();
                switchEnabledAccount.Visibility = Visibility.Visible;
            }
            else
            {
                new PanelAdmin(exists).Show();
                Close();
            }
        }
        private void LoadData()
        {
            _isEstudiante = App.Users.PersonaPersistence is Estudiante;

            txtNombre.Text = App.Users.PersonaPersistence.Nombre;
            txtApellido.Text = App.Users.PersonaPersistence.Apellido;
            txtRun.Text = App.Users.PersonaPersistence.Run;
            txtDireccion.Text = App.Users.PersonaPersistence.Direccion;
            cmbComuna.SelectedValue = App.Users.PersonaPersistence.CodComuna;
            txtFonoFijo.Text = App.Users.PersonaPersistence.FonoFijo;
            txtFonoCel.Text = App.Users.PersonaPersistence.FonoCel;
            dateFechaNac.SelectedDate = DateTime.Parse(((Usuario)App.Users.PersonaPersistence).FecNacimiento);
            if (_isEstudiante)
            {
                txtRunApoderado.Text = App.Users.RunApoderado;
                txtParentesco.Text = App.Users.Parentesco;
                txtCurso.Text = ((Estudiante)App.Users.PersonaPersistence).Curso;
            }
            else
                txtCargo.Text = ((Funcionario)App.Users.PersonaPersistence).Cargo;
            switchEnabledAccount.IsChecked = ((Usuario)App.Users.PersonaPersistence).Estado;
            FixWindow();
        }
        private void FixWindow()
        {
            lblTitulo.Content = BtnExecute.Content = _isAdd
                ? (_isEstudiante ? ("Agregar Estudiante") : ("Agregar Funcionario"))
                : (_isEstudiante ? ("Modificar Estudiante") : ("Modificar Funcionario"));

            if (_isEstudiante) GridEstudiante.Visibility = Visibility.Visible;
            else GridFuncionario.Visibility = Visibility.Visible;
            if (!_isAdd) switchEnabledAccount.Visibility = Visibility.Visible;

            App.Users.ClearPersistantData();
        }
        private bool Validation()
        {
            if (txtNombre.Text == string.Empty)
            {
                lblStatus.Content = "Debe llenar todos los campos";
                txtNombre.Focus();
                return false;
            }
            if (txtApellido.Text == string.Empty)
            {
                lblStatus.Content = "Debe llenar todos los campos";
                txtApellido.Focus();
                return false;
            }
            if (txtRun.Text == string.Empty)
            {
                lblStatus.Content = "Debe llenar todos los campos";
                txtRun.Focus();
                return false;
            }
            if (!RUNValidate(txtRun.Text))
            {
                lblStatus.Content = "Debe ingresar un RUN válido";
                txtRun.Focus();
                return false;
            }
            if (dateFechaNac.SelectedDate == null)
            {
                lblStatus.Content = "Debe llenar todos los campos";
                dateFechaNac.Focus();
                return false;
            }
            if ((DateTime)dateFechaNac.SelectedDate > DateTime.Now)
            {
                lblStatus.Content = "Debe ingresar una fecha válida";
                dateFechaNac.Focus();
                return false;
            }
            if ((DateTime)dateFechaNac.SelectedDate > DateTime.Now.AddYears(-3))
            {
                lblStatus.Content = "No puede tener menos de 3 años";
                dateFechaNac.Focus();
                return false;
            }
            if ((DateTime)dateFechaNac.SelectedDate < DateTime.Now.AddYears(-100))
            {
                lblStatus.Content = "No puede tener más de 100 años";
                dateFechaNac.Focus();
                return false;
            }
            if (txtDireccion.Text == string.Empty)
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
            if (txtFonoFijo.Text == string.Empty)
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
            if (txtFonoCel.Text == string.Empty)
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
            if (_isEstudiante)
            {
                if (txtRunApoderado.Text == string.Empty)
                {
                    lblStatus.Content = "Debe llenar todos los campos";
                    txtRunApoderado.Focus();
                    return false;
                }
                if (!RUNValidate(txtRunApoderado.Text))
                {
                    lblStatus.Content = "Debe ingresar un RUN válido";
                    txtRunApoderado.Focus();
                    return false;
                }
                if (txtRunApoderado.Text == txtRun.Text)
                {
                    lblStatus.Content = "Estudiante y Apoderado no pueden tener mismo RUN";
                    txtRunApoderado.Focus();
                    return false;
                }
                if (App.Users.IsStudent(txtRunApoderado.Text))
                {
                    lblStatus.Content = "El apoderado no puede ser un estudiante";
                    txtRunApoderado.Focus();
                    return false;
                }
                if (txtParentesco.Text == string.Empty)
                {
                    lblStatus.Content = "Debe llenar todos los campos";
                    txtParentesco.Focus();
                    return false;
                }
                if (txtCurso.Text == string.Empty)
                {
                    lblStatus.Content = "Debe llenar todos los campos";
                    txtCurso.Focus();
                    return false;
                }

            }
            else
            {
                if (txtCargo.Text == string.Empty)
                {
                    lblStatus.Content = "Debe llenar todos los campos";
                    txtCargo.Focus();
                    return false;
                }
            }
            lblStatus.Content = string.Empty;
            return true;
        }
        private bool RUNValidate(string run)
        {
            try
            {
                run = run.ToUpper();
                run = run.Replace(" ", "");
                run = run.Replace(".", "");
                run = run.Replace("-", "");
                var rutAux = int.Parse(run.Substring(0, run.Length - 1));
                var dv = char.Parse(run.Substring(run.Length - 1, 1));

                var m = 0;
                var s = 1;

                for (; rutAux != 0; rutAux /= 10)
                {
                    s = (s + rutAux % 10 * (9 - m++ % 6)) % 11;
                }

                return dv == (char)(s != 0 ? s + 47 : 75);
            }
            catch
            {
                return false;
            }
        }
        private void ExecuteAdd()
        {
            if (_isEstudiante)
            {
                App.Users.PreloadPersona(txtRun.Text, txtNombre.Text, txtApellido.Text, txtDireccion.Text, (int)cmbComuna.SelectedValue, txtFonoFijo.Text, txtFonoCel.Text, (DateTime)dateFechaNac.SelectedDate, true, txtCurso.Text, txtRunApoderado.Text, txtParentesco.Text);
                if (!App.Users.ExistsRun(txtRunApoderado.Text).Status)
                {
                    GoCreateApoderado();
                    return;
                }
            }
            else
            {
                App.Users.PreloadPersona(txtRun.Text, txtNombre.Text, txtApellido.Text, txtDireccion.Text, (int)cmbComuna.SelectedValue, txtFonoFijo.Text, txtFonoCel.Text, (DateTime)dateFechaNac.SelectedDate, true, txtCargo.Text);
            }

            var execute = App.Users.Insert();
            if (execute.Status)
            {
                if (_isEstudiante)
                {
                    var insertApoderado = App.Users.InsertApoderadoOnly(txtRunApoderado.Text, App.Users.FetchLastNroFicha(), txtParentesco.Text);
                    if (!insertApoderado.Status)
                    {
                        lblStatus.Content = insertApoderado.Mensaje + " (Apoderado)";
                        return;
                    }
                }
                App.Users.ClearPersistantData();
                new PanelAdmin(execute).Show();
                Close();
            }
            else
                lblStatus.Content = execute.Mensaje + " (Usuario)";
        }
        private void ExecuteUpdate()
        {
            //ToDo Implementation
        }
        private async void GoCreateApoderado()
        {
            await this.ShowMessageAsync("Error", "No se ha encontrado el apoderado. Se redirigirá a la ventana de creación");
            new UserAddApoderado().Show();
            Close();
        }
        private async void ShowNormalDialog(string title, string message)
        {
            await this.ShowMessageAsync(title, message);
        }
        #endregion
        #region Event Handlers
        private void WindowHasLoaded(object sender, RoutedEventArgs e)
        {
            var test = App.Libros.TestConnection();
            if (test.Status)
            {
                cmbComuna.ItemsSource = App.Users.FetchComunas();
                cmbComuna.DisplayMemberPath = "NomComuna";
                cmbComuna.SelectedValuePath = "CodComuna";

                if (_isAdd)
                    SelectTypeDialog();
                else
                    SearchUserDialog();
            }
            else
            {
                new PanelAdmin(test).Show();
                Close();
            }
        }
        private void BtnLogout_OnClick(object sender, RoutedEventArgs e)
        {
            App.Users.ClearPersistantData();
            App.Login.Logout();
            new Inicio().Show();
            Close();
        }
        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            App.Users.ClearPersistantData();
            new PanelAdmin().Show();
            Close();
        }
        private void BtnExecute_Click(object sender, RoutedEventArgs e)
        {
            if (!Validation()) return;

            if (App.Libros.TestConnection().Status)
            {
                if (_isAdd) ExecuteAdd();
                else ExecuteUpdate();
            }
            else
                ShowNormalDialog("Error", "Se ha perdido la conexión con el servidor. Intente nuevamente más tarde");
        }
        private void txtRun_TextChanged(object sender, TextChangedEventArgs e)
        {
            txtRun.Text = txtRun.Text.Replace(".", "");
            if (txtRun.IsFocused) txtRun.CaretIndex = txtRun.Text.Length;
            txtRunApoderado.Text = txtRunApoderado.Text.Replace(".", "");
            if (txtRunApoderado.IsFocused) txtRunApoderado.CaretIndex = txtRunApoderado.Text.Length;
        }
        #endregion
    }
}