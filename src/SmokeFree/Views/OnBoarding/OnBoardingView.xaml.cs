
using SmokeFree.ViewModels.OnBoarding;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SmokeFree.Views.OnBoarding
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class OnBoardingView : ContentPage
    {
        public OnBoardingView()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Changes Corousel Item On Button Click - Next
        /// </summary>
        private void OnChangeCarouselItem_Clicked(object sender, EventArgs e)
        {
            // Get Binding Context Carousel Items Collection
            var bindingContext = this.BindingContext as OnBoardingViewModel;
            var totalCarouselElements = bindingContext.OnBoardingItems.Count;

            // Check if element is not the last
            var newPosition = CarouselView.Position + 1;
            if (newPosition != totalCarouselElements)
            {
                // Increment and display next element 
                CarouselView.Position++;
            }
        }
    }
}