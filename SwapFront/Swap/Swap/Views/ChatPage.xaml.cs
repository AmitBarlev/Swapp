using Swap.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Swap.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class ChatPage : ContentPage
	{
       
       // public Contact Contact { get; set; }


        public ChatPage ()
		{
	
		}

        public ChatPage(Contact i_contact)
        {
           // this.Contact = i_contact;
            InitializeComponent();
            ViewModel = new ChatViewModel(i_contact);
        }


        public ChatViewModel ViewModel
        {
            get { return (BindingContext as ChatViewModel); }
            set { BindingContext = value; }
        }
    }
}