
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SmokeFree.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SmokeFreeNavigationView : NavigationPage
    {
        public SmokeFreeNavigationView() : base()
        {
            InitializeComponent();
        }

        public SmokeFreeNavigationView(Page root) : base(root)
        {
            InitializeComponent();
        }
    }
}