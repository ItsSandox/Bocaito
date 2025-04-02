using System.Threading.Tasks;

namespace Bocaito
{
    public partial class ForgotPassword2Page : ContentPage
    {

        public ForgotPassword2Page()
        {
            InitializeComponent();
        }

        async void OnVerifyCodeClicked(object sender, EventArgs e)
        {
            Console.WriteLine("Verificar código");
        }

        async void OnBackClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("..", true);
        }
    }
}