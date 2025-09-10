using AppRpgEtec.ViewModels.Usuarios;

namespace AppRpgEtec.Views.Usuarios;

public partial class LoginView : ContentPage
{
	UsuarioViewModel UsuarioViewModel;
	public LoginView()
	{

		InitializeComponent();

		UsuarioViewModel = new UsuarioViewModel();
		BindingContext = UsuarioViewModel;
	}
}