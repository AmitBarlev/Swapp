using Newtonsoft.Json;
using Swap.Behaviors;
using Swap.Models;
using Swap.Services;
using Swap.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Xaml;
using static Swap.Services.ItemFormServices;

namespace Swap.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ItemPage : ContentPage
    {
        public Item Item { get; set; }
        public ImageSource Source { get; set; }
        public ItemViewModel ViewModel
        {
            get { return BindingContext as ItemViewModel; }
            set { BindingContext = value; }
        }
        
        public ItemPage(Item i_Item, LoginUserResult i_User)
        {
            Item = i_Item;
            ViewModel = new ItemViewModel(Item, i_User);
            (Application.Current as App).ItemViewModel = ViewModel;

            InitializeComponent();

            itemName.Behaviors.Add(new EnglishLabelTextAlignmentsBehavior());
            author.Behaviors.Add(new EnglishLabelTextAlignmentsBehavior());
            platform.Behaviors.Add(new EnglishLabelTextAlignmentsBehavior());
            sellerName.Behaviors.Add(new EnglishLabelTextAlignmentsBehavior());
            _ = ViewModel.IncrementItemViewsNumber(Item);
        }
    }


}
