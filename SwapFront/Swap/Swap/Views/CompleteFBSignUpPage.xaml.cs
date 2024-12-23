using Swap.ViewModels;
using System.Collections.Generic;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Swap.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class CompleteFBSignUpPage : ContentPage
	{
        public CompleteFBSignUpPageViewModel ViewModel
        {
            get { return (BindingContext as CompleteFBSignUpPageViewModel); }
            set { BindingContext = value; }
        }

        public CompleteFBSignUpPage(string i_FacebookId)
        {
            InitializeComponent();
            ViewModel = new CompleteFBSignUpPageViewModel(i_FacebookId);
            piker.ItemsSource = new List<string>
            {
                "asdasd",
                "asdasd",
                "asdasd",
                "asdasd",
            };
        }
    }
}