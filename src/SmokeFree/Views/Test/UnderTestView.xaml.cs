using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SmokeFree.Views.Test
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class UnderTestView : ContentPage
    {
        public UnderTestView()
        {
            InitializeComponent();

        }

        protected override void OnAppearing()
        {
            MessagingCenter.Send<UnderTestView>(this, "UnderTestViewAppearing");
            base.OnAppearing();
        }

        protected override void OnDisappearing()
        {
            MessagingCenter.Send<UnderTestView>(this, "UnderTestViewDisappearing");
            base.OnDisappearing();
        }

    }
}