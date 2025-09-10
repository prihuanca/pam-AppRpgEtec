using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using AppRpgEtec.Models;
using AppRpgEtec.Services.Usuarios;
using AppRpgEtec.Views.Personagens;
using AppRpgEtec.Views.Usuarios;

namespace AppRpgEtec.ViewModels.Usuarios
{
    public class UsuarioViewModel : BaseViewModel
    {

        public UsuarioViewModel()
        {
            uService = new UsuarioService();
            InicializarCommands();
        }

        public void InicializarCommands()
        {
            AutenticarCommand = new Command(async () => await AutenticarUsuario());
            RegistrarCommand = new Command(async () => await RegistrarUsuario());
            DirecionarCadastoCommand = new Command(async () => await DirecionarParaCadastro());

        }



        private UsuarioService uService;
        public ICommand AutenticarCommand { get; set; }
        public ICommand RegistrarCommand { get; set; }
        public ICommand DirecionarCadastoCommand { get; set; }


        #region AtributosPropiedades

        private string login = string.Empty;
        private string senha = string.Empty;

        public string Login 
        { 
            get => login;
            set
            {
                login = value;
                OnPropertyChanged();
            }  
        }
        public string Senha 
        {
            get => senha;
            set
            {
                senha = value;
                OnPropertyChanged();
            }  
        }

        #endregion



        #region Metodos
        public async Task AutenticarUsuario()
        {
            try
            {
                Usuario u = new Usuario();
                u.Username = login;
                u.PasswordString = senha;

                Usuario uAutenticado = await uService.PostAutenticarUsuarioAsync(u);

                if(!string.IsNullOrEmpty(uAutenticado.Token))
                {
                    string mensagem = $"Bem-Vindo(a) { uAutenticado.Username}";

                    //Guardando dados para uso  futuro
                    Preferences.Set("UsuarioId", uAutenticado.Id);
                    Preferences.Set("UsuarioUsername", uAutenticado.Username);
                    Preferences.Set("UsuarioPerfil", uAutenticado.Perfil);
                    Preferences.Set("UsuarioToken", uAutenticado.Token);

                    await Application.Current.MainPage.DisplayAlert("Informação", mensagem, "Ok");

                    Application.Current.MainPage = new AppShell();
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("Informação", "Dados Incorretos 😑", "Ok");
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Informação", ex.Message + ex.InnerException, "Ok");
            }    
           
        }
        public async Task RegistrarUsuario()//metodo para registrar um usuario
        {
            try
            {
                Usuario u = new Usuario();
                u.Username = Login;
                u.PasswordString = senha;

                Usuario uRegistrado = await uService.PostRegistrarUsuarioAsync(u);

                if (uRegistrado.Id != 0)
                {
                    string mensagem = $"Usuario Id {uRegistrado.Id} registrado com sucesso";
                    await Application.Current.MainPage.DisplayAlert("Informação", mensagem, "Ok");

                    await Application.Current.MainPage.Navigation.PopAsync();//Remove a pagina da pilha de visualização
                }
            }
            catch (Exception)
            {

                throw;
            }

        }

        public async Task DirecionarParaCadastro()//Metodo para exibição da view de cadastro
        {
            try
            {
                await Application.Current.MainPage.Navigation.PushAsync(new CadastroView());
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Informação", ex.Message + "Detaslhes" + ex.InnerException, "Ok");
            }
        }
        #endregion
    }
}
