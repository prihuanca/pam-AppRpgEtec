using AppRpgEtec.Models;
using AppRpgEtec.Services.Armas;
using AppRpgEtec.Services.Personagens;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace AppRpgEtec.ViewModels.Armas
{
    [QueryProperty("ArmaSelecionadaId", "aId")]
    public class CadastroArmaViewModel : BaseViewModel
    {
        private ArmaService aService;
        private PersonagemService pService;

        public ICommand SalvarCommand { get; }
        public ICommand CancelarCommand { get; }

        private string armaSelecionadaId;

        public string ArmaSelecionadaId
        {
            set
            {
                if (value != null)
                {
                    armaSelecionadaId = Uri.UnescapeDataString(value);
                    CarregarArma();
                }
            }
        }

        public CadastroArmaViewModel()
        {
            string token = Preferences.Get("UsuarioToken", string.Empty);
            aService = new ArmaService(token);
            pService = new PersonagemService(token);

            SalvarCommand = new Command(async () => await SalvarArma());
            CancelarCommand = new Command(async () => await CancelarCadastro());

            _ = ObterPersonagens();
        }

        private async Task CancelarCadastro()
        {
            await Shell.Current.GoToAsync("..");
        }

        private int id;
        private string nome;
        private int dano;

        public int Id
        {
            get => id;
            set
            {
                id = value;
                OnPropertyChanged();
            }
        }

        public string Nome
        {
            get => nome;
            set
            {
                nome = value;
                OnPropertyChanged();
            }
        }

        public int Dano
        {
            get => dano;
            set
            {
                dano = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<Personagem> personagens;
        public ObservableCollection<Personagem> Personagens
        {
            get => personagens;
            set
            {
                personagens = value;
                OnPropertyChanged();
            }
        }

        private Personagem personagemSelecionado;
        public Personagem PersonagemSelecionado
        {
            get => personagemSelecionado;
            set
            {
                personagemSelecionado = value;
                OnPropertyChanged();
            }
        }

        public async Task ObterPersonagens()
        {
            try
            {
                Personagens = await pService.GetPersonagensAsync();
                OnPropertyChanged(nameof(Personagens));
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Ops", ex.Message + " Detalhes: " + ex.InnerException, "Ok");
            }
        }

        public async void CarregarArma()
        {
            try
            {
                Arma a = await aService.GetArmaAsync(int.Parse(armaSelecionadaId));
                this.Id = a.Id;
                this.Nome = a.Nome;
                this.Dano = a.Dano;

                PersonagemSelecionado = Personagens.FirstOrDefault(p => p.Id == a.PersonagemId);
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Ops", ex.Message + " Detalhes: " + ex.InnerException, "Ok");
            }
        }

        public async Task SalvarArma()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(Nome) || Dano <= 0 || PersonagemSelecionado == null)
                {
                    await Application.Current.MainPage.DisplayAlert("Atenção", "Preencha todos os campos corretamente.", "Ok");
                    return;
                }

                Arma model = new Arma()
                {
                    Id = this.Id,
                    Nome = this.Nome,
                    Dano = this.Dano,
                    PersonagemId = this.PersonagemSelecionado.Id
                };

                if (model.Id == 0)
                    await aService.PostArmaAsync(model);
                else
                    await aService.PutArmaAsync(model);

                await Application.Current.MainPage.DisplayAlert("Mensagem", "Dados salvos com sucesso!", "Ok");
                await Shell.Current.GoToAsync("..");
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Ops", ex.Message + " Detalhes: " + ex.InnerException, "Ok");
            }
        }
    }
}
