﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppRpgEtec.Services.Personagens;
using AppRpgEtec.Models;
using System.Collections.ObjectModel;
using AppRpgEtec.Models.Enuns;
using System.Windows.Input;
using System.Linq;
namespace AppRpgEtec.ViewModels.Personagens
{
    [QueryProperty("PersonagemSelecionadoId", "pId")]
    public class CadastroPersonagemViewModel : BaseViewModel
    {
        private PersonagemService pService;

        public ICommand SalvarCommand { get; }
        public ICommand CancelarCommand { get; set; }

        public async void CarregarPersonagem()
        {
            try
            {
                Personagem p = await
                    pService.GetPersonagemAsync(int.Parse(personagemSelecionadoId));
                this.Nome = p.Nome;
                this.PontosVida = p.PontosVida;
                this.Defesa = p.Defesa;
                this.Derrotas = p.Derrotas;
                this.Disputas = p.Disputas;
                this.Forca = p.Forca;
                this.Inteligencia = p.Inteligencia;
                this.Vitorias = p.Vitorias;
                this.Id = p.Id;

                TipoClasseSelecionado = this.ListaTipoClasse
                 .FirstOrDefault(tClasse => tClasse.Id == (int)p.Classe);
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("ops", ex.Message + "Detalhes:" + ex.InnerException, "ok");
            }
        }

        //
        private string personagemSelecionadoId; 
        
        public string PersonagemSelecionadoId
        {
            set
            {
                if (value != null)
                {
                    personagemSelecionadoId = Uri.UnescapeDataString(value);
                    CarregarPersonagem();
                }
            }
        }

        public CadastroPersonagemViewModel()
        {
            string token = Preferences.Get("UsuarioToken", string.Empty);
            pService = new PersonagemService(token);
            _ = ObterClasse();

            SalvarCommand = new Command(async () => { await SalvarPersonagem(); });
            CancelarCommand = new Command(async => CancelarCadastro());
        }

        private async void CancelarCadastro()
        {
            await Shell.Current.GoToAsync("..");
        }

        private int id;
        private string nome;
        private int pontosVida;
        private int forca;
        private int defesa;
        private int inteligencia;
        private int disputas;
        private int vitorias;
        private int derrotas;

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
        { get => nome;
            set
            {
                 nome = value;
                OnPropertyChanged();
            }
        }
        public int PontosVida 
        { get => pontosVida;
            set
            {
                pontosVida = value;
                OnPropertyChanged();
            }
                
        }

        public int Forca 
        { get => forca;
            set
            {
                forca = value;
                OnPropertyChanged();
            }
        }

        public int Defesa 
        { get => defesa;
            set
            {
                defesa = value;
                OnPropertyChanged();
            }
        }
        public int Inteligencia 
        { 
            get => inteligencia;
            set
            {
                inteligencia = value;
                OnPropertyChanged();
            }
        }
        public int Disputas 
        { 
            get => disputas;
            set
            {
                disputas = value;
                OnPropertyChanged();
            }
        }
        public int Vitorias 
        {
            get => vitorias;
            set
            {
                vitorias = value;
                OnPropertyChanged();
            }
        }
        public int Derrotas
        {
            get => derrotas;
            set 
            { 
                derrotas = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<TipoClasse> listaTipoClasse;
        public ObservableCollection<TipoClasse> ListaTipoClasse
        {
            get {  return listaTipoClasse; }
            set
            {
                if(value != null)
                {
                    listaTipoClasse = value;
                    OnPropertyChanged();
                }
            }
        }

        public async Task ObterClasse()
        {
            try
            {
                ListaTipoClasse = new ObservableCollection<TipoClasse>();
                ListaTipoClasse.Add(new TipoClasse() { Id = 1, Descricao = "Cavaleiro" });
                ListaTipoClasse.Add(new TipoClasse() { Id = 2, Descricao = "Mago" });
                ListaTipoClasse.Add(new TipoClasse() { Id = 3, Descricao = "Cleigo" });
                OnPropertyChanged(nameof(ListaTipoClasse));
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage
                    .DisplayAlert("Ops", ex.Message + "Detalhes: " + ex.InnerException, "Ok");
            }
        }
        private TipoClasse tipoClasseSelecionado;
        public TipoClasse TipoClasseSelecionado
        {
            get { return tipoClasseSelecionado; }
            set
            {
                if (value != null)
                {
                    tipoClasseSelecionado = value;
                    OnPropertyChanged();
                }
            }
        }

        public async Task SalvarPersonagem()
        {
            try
            {
                Personagem model = new Personagem()
                {
                    Nome = this.nome,
                    PontosVida = this.pontosVida,
                    Defesa = this.defesa,
                    Derrotas = this.derrotas,
                    Forca = this.forca,
                    Inteligencia = this.inteligencia,
                    Vitorias = this.vitorias,
                    Id = this.id,



                    Classe = (classeEnum)tipoClasseSelecionado.Id
                };
                if (model.Id == 0)
                    await pService.PostPersonagemAsync(model);
                else
                    await pService.PutPersonagemAsync (model);
                    await Application.Current.MainPage.DisplayAlert("Mensagem", "Dados salvos com sucesso!", "Ok");
                await Shell.Current.GoToAsync(".."); //Remove a pagina atual da pilha de paginas
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Ops", ex.Message + "Detalhes" + ex.InnerException, "Ok");
            }
        }
    }

}
