using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SmokeFree.Views.AppSettings
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class StyleAppSetting : ResourceDictionary
	{
		public StyleAppSetting ()
		{
			InitializeComponent ();
		}
	}
}