using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SmokeFree.Views.ErrorAndEmpty
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SomethingWentWrongView : ContentPage
    {
        public SomethingWentWrongView()
        {
            InitializeComponent();
        }
    }
}