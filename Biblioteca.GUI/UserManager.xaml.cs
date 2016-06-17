using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Biblioteca.Entidad;
using MahApps.Metro.Controls.Dialogs;

namespace Biblioteca.GUI
{
    /// <summary>
    /// Interaction logic for UserManager.xaml
    /// </summary>
    public partial class UserManager
    {
        #region Attributes
        private readonly bool _isAdd;
        private string _nroFicha;
        private bool _isEstudiante;
        #endregion
        #region Constructor
        /// <summary>
        /// Generates a new instance of UserManager
        /// </summary>
        /// <param name="isAdd">Determines if the Window is going to be used to Add an Usuario or not</param>
        public UserManager(bool isAdd)
        {
            InitializeComponent();
            _isAdd = isAdd;

            LblTitulo.Content = BtnExecute.Content = _isAdd ? "Agregar Usuario" : "Modificar Usuario";
            CmbParentesco.ItemsSource = new [] { "Padre", "Madre", "Otro" };
            DateFechaNac.DisplayDateStart = DateTime.Now.AddYears(-100);
            DateFechaNac.DisplayDateEnd = DateTime.Now.AddYears(-3).AddDays(-1);
            if (_isAdd && App.Users.PersonaPersistence != null) LoadData();
        }
        #endregion
        #region Custom Methods
        /// <summary>
        /// Shows a Select Dialog, to select if it's going to add an Estudiante or Funcionario
        /// Only executed when Adding Usuario, not Modifying them.
        /// </summary>
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
        /// <summary>
        /// Opens an Input Dialog inside the Window and attempts to fetch an Usuario
        /// </summary>
        private async void SearchUserDialog()
        {
            TxtRun.IsEnabled = TxtRunApoderado.IsEnabled = false;
            var settings = new MetroDialogSettings
            {
                AffirmativeButtonText = "Buscar",
                NegativeButtonText = "Cancelar",
                ColorScheme = MetroDialogOptions.ColorScheme
            };

            var nroFicha = await this.ShowInputAsync("Buscar", "Ingrese un número de ficha", settings);

            var check = App.Users.CheckEmptySearchString(nroFicha);
            if (!check.Status)
            {
                if (check.Mensaje == null)
                {
                    new PanelAdmin().Show();
                    Close();
                }
                else
                {
                    new PanelAdmin(check).Show();
                    Close();
                }
                return;
            }

            var exists = App.Users.FetchUsuario(nroFicha);
            if (exists.Status)
            {
                _nroFicha = nroFicha;
                LoadData();
                SwitchEnabledAccount.Visibility = Visibility.Visible;
            }
            else
            {
                new PanelAdmin(exists).Show();
                Close();
            }
        }
        /// <summary>
        /// Load a Libro data onto the Window Controls
        /// </summary>
        private void LoadData()
        {
            _isEstudiante = App.Users.PersonaPersistence is Estudiante;

            TxtNombre.Text = App.Users.PersonaPersistence.Nombre;
            TxtApellido.Text = App.Users.PersonaPersistence.Apellido;
            TxtRun.Text = App.Users.PersonaPersistence.Run;
            TxtDireccion.Text = App.Users.PersonaPersistence.Direccion;
            CmbComuna.SelectedValue = App.Users.PersonaPersistence.CodComuna;
            TxtFonoFijo.Text = App.Users.PersonaPersistence.FonoFijo;
            TxtFonoCel.Text = App.Users.PersonaPersistence.FonoCel;
            DateFechaNac.SelectedDate = DateTime.Parse(((Usuario)App.Users.PersonaPersistence).FecNacimiento);

            if (_isEstudiante)
            {
                TxtRunApoderado.Text = App.Users.RunApoderado;
                CmbParentesco.SelectedValue = App.Users.Parentesco;
                TxtCurso.Text = ((Estudiante)App.Users.PersonaPersistence).Curso;
            }
            else
                TxtCargo.Text = ((Funcionario)App.Users.PersonaPersistence).Cargo;
            SwitchEnabledAccount.IsChecked = ((Usuario)App.Users.PersonaPersistence).Estado;
            FixWindow();
        }
        /// <summary>
        /// Modifies labels, titles and other stuff, based on the parameter received by the Constructor.
        /// </summary>
        private void FixWindow()
        {
            LblTitulo.Content = BtnExecute.Content = _isAdd
                ? (_isEstudiante ? ("Agregar Estudiante") : ("Agregar Funcionario"))
                : (_isEstudiante ? ("Modificar Estudiante") : ("Modificar Funcionario"));

            if (_isEstudiante)
            {
                TxtRunApoderado.Visibility = Visibility.Visible;
                CmbParentesco.Visibility = Visibility.Visible;
                TxtCurso.Visibility = Visibility.Visible;
            }
            else
            {
                TxtCargo.Visibility = Visibility.Visible;
            }

            SwitchEnabledAccount.Visibility = _isAdd ? Visibility.Collapsed : Visibility.Visible;

            App.Users.ClearPersistantData();
        }
        /// <summary>
        /// Method that validates the form inside the Window
        /// </summary>
        /// <returns>If the validation was successful or not</returns>
        private bool Validation()
        {
            if (string.IsNullOrWhiteSpace(TxtNombre.Text))
            {
                LblStatus.Content = "Debe llenar todos los campos";
                TxtNombre.Focus();
                return false;
            }
            if (TxtNombre.Text.Length < 2)
            {
                LblStatus.Content = "Debe tener mínimo 2 caracteres";
                TxtNombre.Focus();
                return false;
            }
            if (TxtNombre.Text.Any(char.IsDigit))
            {
                LblStatus.Content = "El campo no puede tener dígitos";
                TxtNombre.Focus();
                return false;
            }
            if (string.IsNullOrWhiteSpace(TxtApellido.Text))
            {
                LblStatus.Content = "Debe llenar todos los campos";
                TxtApellido.Focus();
                return false;
            }
            if (TxtApellido.Text.Length < 2)
            {
                LblStatus.Content = "Debe tener mínimo 2 caracteres";
                TxtApellido.Focus();
                return false;
            }
            if (TxtApellido.Text.Any(char.IsDigit))
            {
                LblStatus.Content = "El campo no puede tener dígitos";
                TxtApellido.Focus();
                return false;
            }
            if (string.IsNullOrWhiteSpace(TxtRun.Text))
            {
                LblStatus.Content = "Debe llenar todos los campos";
                TxtRun.Focus();
                return false;
            }
            if (!RUNValidate(TxtRun.Text))
            {
                LblStatus.Content = "Debe ingresar un RUN válido";
                TxtRun.Focus();
                return false;
            }
            if (DateFechaNac.SelectedDate == null)
            {
                LblStatus.Content = "Debe llenar todos los campos";
                DateFechaNac.Focus();
                return false;
            }
            if ((DateTime)DateFechaNac.SelectedDate > DateTime.Now)
            {
                LblStatus.Content = "Debe ingresar una fecha válida";
                DateFechaNac.Focus();
                return false;
            }
            if ((DateTime)DateFechaNac.SelectedDate > DateTime.Now.AddYears(-3))
            {
                LblStatus.Content = "No puede tener menos de 3 años";
                DateFechaNac.Focus();
                return false;
            }
            if ((DateTime)DateFechaNac.SelectedDate < DateTime.Now.AddYears(-100))
            {
                LblStatus.Content = "No puede tener más de 100 años";
                DateFechaNac.Focus();
                return false;
            }
            if (string.IsNullOrWhiteSpace(TxtDireccion.Text))
            {
                LblStatus.Content = "Debe llenar todos los campos";
                TxtDireccion.Focus();
                return false;
            }
            if (TxtDireccion.Text.Length < 5)
            {
                LblStatus.Content = "Debe tener mínimo 5 caracteres";
                TxtDireccion.Focus();
                return false;
            }
            if (!TxtDireccion.Text.Any(char.IsDigit))
            {
                LblStatus.Content = "El campo debe tener letras y números";
                TxtDireccion.Focus();
                return false;
            }
            if (!TxtDireccion.Text.Any(char.IsLetter))
            {
                LblStatus.Content = "El campo debe tener letras y números";
                TxtDireccion.Focus();
                return false;
            }
            if (CmbComuna.SelectedItem == null)
            {
                LblStatus.Content = "Debe llenar todos los campos";
                CmbComuna.Focus();
                return false;
            }
            if (string.IsNullOrWhiteSpace(TxtFonoFijo.Text))
            {
                LblStatus.Content = "Debe llenar todos los campos";
                TxtFonoFijo.Focus();
                return false;
            }
            int i;
            if (!int.TryParse(TxtFonoFijo.Text, out i))
            {
                LblStatus.Content = "El teléfono sólo debe tener números";
                TxtFonoFijo.Focus();
                return false;
            }
            if (TxtFonoFijo.Text.Length < 7)
            {
                LblStatus.Content = "Debe tener mínimo 7 caracteres";
                TxtFonoFijo.Focus();
                return false;
            }
            if (string.IsNullOrWhiteSpace(TxtFonoCel.Text))
            {
                LblStatus.Content = "Debe llenar todos los campos";
                TxtFonoCel.Focus();
                return false;
            }
            if (!int.TryParse(TxtFonoCel.Text, out i))
            {
                LblStatus.Content = "El teléfono sólo debe tener números";
                TxtFonoCel.Focus();
                return false;
            }
            if (TxtFonoCel.Text.Length < 8)
            {
                LblStatus.Content = "Debe tener mínimo 8 caracteres";
                TxtFonoCel.Focus();
                return false;
            }
            if (_isEstudiante)
            {
                if (string.IsNullOrWhiteSpace(TxtRunApoderado.Text))
                {
                    LblStatus.Content = "Debe llenar todos los campos";
                    TxtRunApoderado.Focus();
                    return false;
                }
                if (!RUNValidate(TxtRunApoderado.Text))
                {
                    LblStatus.Content = "Debe ingresar un RUN válido";
                    TxtRunApoderado.Focus();
                    return false;
                }
                if (TxtRunApoderado.Text == TxtRun.Text)
                {
                    LblStatus.Content = "Estudiante y Apoderado no pueden tener mismo RUN";
                    TxtRunApoderado.Focus();
                    return false;
                }
                if (App.Users.IsStudent(TxtRunApoderado.Text))
                {
                    LblStatus.Content = "El apoderado no puede ser un estudiante";
                    TxtRunApoderado.Focus();
                    return false;
                }
                if (CmbParentesco.SelectedIndex == -1)
                {
                    LblStatus.Content = "Debe seleccionar un parentesco";
                    CmbParentesco.Focus();
                    return false;
                }
                if (string.IsNullOrWhiteSpace(TxtCurso.Text))
                {
                    LblStatus.Content = "Debe llenar todos los campos";
                    TxtCurso.Focus();
                    return false;
                }
            }
            else
            {
                if (string.IsNullOrWhiteSpace(TxtCargo.Text))
                {
                    LblStatus.Content = "Debe llenar todos los campos";
                    TxtCargo.Focus();
                    return false;
                }
            }
            LblStatus.Content = string.Empty;
            return true;
        }
        /// <summary>
        /// Method that validates a chilean RUN
        /// </summary>
        /// <param name="run">RUN to validate</param>
        /// <returns>Boolean that indicates if the RUN is valid or not</returns>
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
        /// <summary>
        /// Calls to the Users Controller, and executes the Insert query.
        /// In case of success, it's returned to the PanelAdmin Window.
        /// </summary>
        private void ExecuteAdd()
        {
            if (_isEstudiante)
            {
                App.Users.PreloadPersona(TxtRun.Text, TxtNombre.Text, TxtApellido.Text, TxtDireccion.Text, (int)CmbComuna.SelectedValue, TxtFonoFijo.Text, TxtFonoCel.Text, (DateTime)DateFechaNac.SelectedDate, true, TxtCurso.Text, TxtRunApoderado.Text, CmbParentesco.SelectedValue.ToString());
                if (!App.Users.ExistsRun(TxtRunApoderado.Text).Status)
                {
                    GoCreateApoderado();
                    return;
                }
            }
            else
                App.Users.PreloadPersona(TxtRun.Text, TxtNombre.Text, TxtApellido.Text, TxtDireccion.Text, (int)CmbComuna.SelectedValue, TxtFonoFijo.Text, TxtFonoCel.Text, (DateTime)DateFechaNac.SelectedDate, true, TxtCargo.Text);

            var execute = App.Users.Insert();
            if (execute.Status)
            {
                if (_isEstudiante)
                {
                    var insertApoderado = App.Users.InsertApoderadoOnly(TxtRunApoderado.Text, App.Users.FetchNroFicha(TxtRun.Text), CmbParentesco.SelectedValue.ToString());
                    if (!insertApoderado.Status)
                    {
                        LblStatus.Content = insertApoderado.Mensaje + " (Apoderado)";
                        return;
                    }
                }
                App.Users.ClearPersistantData();
                new PanelAdmin(execute).Show();
                Close();
            }
            else
                LblStatus.Content = execute.Mensaje + " (Usuario)";
        }
        /// <summary>
        /// Calls to the Users Controller, and executes the Update query.
        /// In case of success, it's returned to the PanelAdmin Window.
        /// </summary>
        private void ExecuteUpdate()
        {
            if (_isEstudiante)
                App.Users.PreloadPersona(TxtRun.Text, TxtNombre.Text, TxtApellido.Text, TxtDireccion.Text, (int)CmbComuna.SelectedValue, TxtFonoFijo.Text, TxtFonoCel.Text, (DateTime)DateFechaNac.SelectedDate, SwitchEnabledAccount.IsChecked.Value, TxtCurso.Text, TxtRunApoderado.Text, CmbParentesco.SelectedValue.ToString(), int.Parse(_nroFicha));
            else
                App.Users.PreloadPersona(TxtRun.Text, TxtNombre.Text, TxtApellido.Text, TxtDireccion.Text, (int)CmbComuna.SelectedValue, TxtFonoFijo.Text, TxtFonoCel.Text, (DateTime)DateFechaNac.SelectedDate, SwitchEnabledAccount.IsChecked.Value, TxtCargo.Text, int.Parse(_nroFicha));

            var execute = App.Users.Update();

            if (execute.Status)
            {
                new PanelAdmin(execute).Show();
                Close();
            }
            else
                LblStatus.Content = execute.Mensaje + " (Usuario)";
        }
        /// <summary>
        /// Shows an alert dialog and redirects to an UserAddApoderado Window
        /// </summary>
        private async void GoCreateApoderado()
        {
            await this.ShowMessageAsync("Error", "No se ha encontrado el apoderado. Se redirigirá a la ventana de creación");
            new UserAddApoderado().Show();
            Close();
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
        private void WindowHasLoaded(object sender, RoutedEventArgs e)
        {
            var test = App.Users.TestConnection();
            if (test.Status)
            {
                CmbComuna.ItemsSource = App.Users.FetchComunas();
                CmbComuna.DisplayMemberPath = "NomComuna";
                CmbComuna.SelectedValuePath = "CodComuna";

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
        /// <summary>
        /// Event that loads when user clicks on the Logout button
        /// </summary>
        /// <param name="sender">The object that triggered this event</param>
        /// <param name="e">Parameters (optional)</param>
        private void BtnLogout_OnClick(object sender, RoutedEventArgs e)
        {
            App.Users.ClearPersistantData();
            App.Admins.Logout();
            new Inicio().Show();
            Close();
        }
        /// <summary>
        /// Event that loads when user clicks on the Back button
        /// </summary>
        /// <param name="sender">The object that triggered this event</param>
        /// <param name="e">Parameters (optional)</param>
        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            App.Users.ClearPersistantData();
            new PanelAdmin().Show();
            Close();
        }
        /// <summary>
        /// Event that loads when user clicks on the Execute button
        /// </summary>
        /// <param name="sender">The object that triggered this event</param>
        /// <param name="e">Parameters (optional)</param>
        private void BtnExecute_Click(object sender, RoutedEventArgs e)
        {
            if (App.Users.TestConnection().Status)
            {
                if (!Validation()) return;

                if (_isAdd) ExecuteAdd();
                else ExecuteUpdate();
            }
            else
                ShowNormalDialog("Error", "Se ha perdido la conexión con el servidor. Intente nuevamente más tarde");
        }
        /// <summary>
        /// Event triggered when user modifies the RUN fields.
        /// This will remove dot chars '.' from the field automatically.
        /// </summary>
        /// <param name="sender">The object that triggered this event</param>
        /// <param name="e">Parameters (optional)</param>
        private void txtRun_TextChanged(object sender, TextChangedEventArgs e)
        {
            TxtRun.Text = TxtRun.Text.Replace(".", "");
            if (TxtRun.IsFocused) TxtRun.CaretIndex = TxtRun.Text.Length;
            TxtRunApoderado.Text = TxtRunApoderado.Text.Replace(".", "");
            if (TxtRunApoderado.IsFocused) TxtRunApoderado.CaretIndex = TxtRunApoderado.Text.Length;
        }
        #endregion
    }
}