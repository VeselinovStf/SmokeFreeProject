﻿
using SmokeFree.Abstraction.Utility.Wrappers;
using SmokeFree.Bootstrap;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SmokeFree.Views.Test
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CreateTestView : ContentPage
    {
        public CreateTestView()
        {
            InitializeComponent();
        }


        /// <summary>
        /// On Slected Item from Picker. Change Is Visible Prop. of Start Test Buton. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnSelectedIndex_Change(object sender, EventArgs e)
        {
            // If item is selected display start test button
            if (picker.SelectedIndex > -1)
            {
                StartTestButton.IsVisible = true;
            }
            else
            {
                StartTestButton.IsVisible = false;
            }
        }

        /// <summary>
        /// Closes Description And Opens Goal DateTime
        /// </summary>
        private void OnCloseDescriptionButton_Clicked(object sender, EventArgs e)
        {
            var dateTimeService = AppContainer.Resolve<IDateTimeWrapper>();

            this.GoalDateTime.Focus();
            this.GoalDateTime.MinimumDate = dateTimeService.Now().AddDays(Globals.MinChallengeDays);
            this.GoalDateTime.MaximumDate = dateTimeService.Now().AddYears(1);


            this.Description.IsVisible = false;
            this.ViewContent.IsVisible = true;
        }

        /// <summary>
        /// Goal Date Clicked
        /// </summary>
        private void GoalDate_Clicked(object sender, EventArgs e)
        {
            this.GoalDateTime.Focus();
        }
    }
}