using Realms;
using SmokeFree.Data.Models;
using System;
using System.Diagnostics;
using System.Linq;
using Xamarin.Forms;

namespace SmokeFree
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();

            var config = new RealmConfiguration
            {
                SchemaVersion = 3,
            };
            
            var _realm = Realm.GetInstance(config);
            var tests = _realm.All<Test>().ToList();

            //_realm.Write(() =>
            //{
            //    var entry = new Test { CreatedOn = DateTimeOffset.Now };
            //    _realm.Add(entry);
            //});

            Debug.WriteLine("HERE");
        }
    }
}
