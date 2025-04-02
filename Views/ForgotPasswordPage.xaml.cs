using System.Threading.Tasks;

namespace Bocaito
{
    public partial class ForgotPasswordPage : ContentPage
    {

        public ForgotPasswordPage()
        {
            InitializeComponent();
        }

        async void OnSendCodeClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new ForgotPassword2Page());
        }

        async void OnBackClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("..", true);
        }
    }
}