using FFImageLoading.Forms;
using Newtonsoft.Json;
using Swap.Models;
using Swap.Services;
using Swap.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Swap.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MyItemsPage : ContentPage
    {
        private List<ItemFormServices.Item> items;
        private List<ItemFormServices.Book> Bookitems = new List<ItemFormServices.Book>();
        private List<ItemFormServices.VideoGame> VideoGameitems = new List<ItemFormServices.VideoGame>();
        private ItemFormServices.Item LastItem;
        App app = Application.Current as App;

        public enum View
        {
            myItems,
            otherUserItems
        }

        private View m_currentPageView;
        public MyItemsPage(View i_viewOption)
        {
            InitializeComponent();
            m_currentPageView = i_viewOption;
        }

        public MyItemsPage()
        {
            InitializeComponent();
            m_currentPageView = View.myItems;
        }

        protected override async void OnAppearing()
        {
            if (app.UserId == 0)
            {
                ClearAllSL();
                app.RefreshRequired = true;
            }
            else if (app.RefreshRequired == true)
            {
                await DisplayItemsOfUser(app.UserId, "All");
            }
        }

        internal Grid makeGridImageContainer(ItemFormServices.Item i_item, int i_GridWidth, int i_imageHeight, int i_titleHeight)
        {
            Grid result = null;
            ItemFormServices.Image mainImage = null;
            ImageSource imageSource = null;
            if (i_item.ImagesOfItem.Count > 0)
            {
                mainImage = i_item.ImagesOfItem[0];
                var byteArray = Convert.FromBase64String(mainImage.BytesOfImage);
                imageSource = ImageSource.FromStream(() => new MemoryStream(byteArray));
            }
            else  //if item has no images
            {

                imageSource = "noimage.jpg";
            }

            Grid imageContainer = new Grid()
            {
                WidthRequest = i_GridWidth

            };

            Label imageNumber_i_Title = new Label()
            {
                Text = i_item.Name,
                HorizontalTextAlignment = TextAlignment.Center,
                VerticalTextAlignment = TextAlignment.Center,

            };

            CachedImage image = new CachedImage()
            {
                Aspect = Aspect.AspectFill,
                Source = imageSource,
            };

            image.GestureRecognizers.Add(new TapGestureRecognizer
            {
                Command = new Command(async () =>
                {
                    if (m_currentPageView == View.myItems)
                    {

                        await Navigation.PushAsync(new ItemFormPage("Edit Item", this, i_item));
                    }
                    else if (m_currentPageView == View.otherUserItems)
                    {
                        LoginUserResult user = null;
                        try
                        {
                            user = await ServerFacade.Users.GetUserInfoAsync(i_item.IdCustomer);
                        }
                        catch (Exception)
                        {

                        }
                        await Navigation.PushAsync(new ItemPage(i_item, user));
                    }
                })
            });

            imageContainer.Children.Add(image, 0, 0);
            imageContainer.Children.Add(imageNumber_i_Title, 0, 1);
            imageContainer.Opacity = 0;
            imageContainer.FadeTo(1, 2000);

            imageContainer.RowDefinitions.Add(new RowDefinition
            {
                Height = new GridLength(i_imageHeight, GridUnitType.Absolute)
            });
            imageContainer.RowDefinitions.Add(new RowDefinition
            {
                Height = new GridLength(i_titleHeight, GridUnitType.Absolute)
            });

            result = imageContainer;

            return result;
        }

        internal readonly Dictionary<ItemFormServices.Item, Grid> m_itemToGrid = new Dictionary<ItemFormServices.Item, Grid>();

        internal void addItemToStackLayout(ItemFormServices.Item i_item)
        {
            Grid imageContainer = makeGridImageContainer(i_item, 100, 180, 20);
            m_itemToGrid.Add(i_item, imageContainer);

            if (i_item is ItemFormServices.Book)
            {
                myBooksSL.Children.Add(imageContainer);
                Bookitems.Add(i_item as ItemFormServices.Book);

            }
            else if (i_item is ItemFormServices.VideoGame)
            {
                myVideoGamesSL.Children.Add(imageContainer);
                VideoGameitems.Add(i_item as ItemFormServices.VideoGame);

            }
        }

        internal void removeItemFromStackLayout(ItemFormServices.Item m_itemToEdit)
        {
            Grid imageContainer = m_itemToGrid[m_itemToEdit];

            if (m_itemToEdit is ItemFormServices.Book)
            {
                myBooksSL.Children.Remove(imageContainer);
                Bookitems.Remove(m_itemToEdit as ItemFormServices.Book);
            }
            else if (m_itemToEdit is ItemFormServices.VideoGame)
            {
                myVideoGamesSL.Children.Remove(imageContainer);
                VideoGameitems.Remove(m_itemToEdit as ItemFormServices.VideoGame);
            }

            m_itemToGrid.Remove(m_itemToEdit);
        }

        internal async Task DisplayItemsOfUser(int i_userId, string i_option)
        {
            if (i_userId != app.UserId)
            {
                addItemButton.IsVisible = false;
            }

            await Navigation.PushAsync(new WaitingPage());

            app.RefreshRequired = false;
            // int currentIdCustomer = (Application.Current as App).UserId;
            var client = new HttpClient();
            var data = string.Format("id={0}", i_userId);
            var httpMethod = HttpMethod.Get;
            string Token = "Bearer " + (Application.Current as App).Token;

            string UrlAllItems = "http://Vmedu184.mtacloud.co.il/item/getItems" + "?" + data;
            string UrlLastItem = "http://Vmedu184.mtacloud.co.il/item/getlastItem" + "?" + data;

            var request = new HttpRequestMessage()
            {
                Method = httpMethod,
            };

            if (i_option == "All")
            {
                ClearAllSL();
                request.RequestUri = new Uri(UrlAllItems);
            }
            else if (i_option == "Last Uploaded")
            {
                request.RequestUri = new Uri(UrlLastItem);
            }

            request.Headers.Add("Authorization", Token);
            HttpResponseMessage httpResponse = await client.SendAsync(request);
            httpResponse.EnsureSuccessStatusCode();
            string responseContent = await httpResponse.Content.ReadAsStringAsync();

            if (i_option == "All")
            {
                items = JsonConvert.DeserializeObject<List<ItemFormServices.Item>>(responseContent);
                for (int i = 0; i < items.Count; i++)
                {
                    addItemToStackLayout(items[i]);
                }
            }

            else if (i_option == "Last Uploaded")
            {
                LastItem = JsonConvert.DeserializeObject<ItemFormServices.Item>(responseContent);
                addItemToStackLayout(LastItem);
            }

            await Navigation.PopAsync();
        }

        private async void addItem_Button_Clicked(object sender, EventArgs e)
        {
            if ((Application.Current as App).UserId == 0)
            {
                await DisplayAlert("גישה לא מורשת", "עלייך להתחבר על מנת להוסיף פריטים לספריה שלך!", "אישור");
                return;
            }

            await Navigation.PushAsync(new ItemFormPage("Add Item", this));
        }

        private async void viewAllBookItems_Button_Clicked(object sender, EventArgs e)
        {
            if ((Application.Current as App).UserId == 0)
            {
                await DisplayAlert("גישה לא מורשת", "עלייך ראשית להתחבר!", "אישור");
                return;
            }

            List<ItemFormServices.Book> booksListcopy = Bookitems.ToList();
            await Navigation.PushAsync(new MyItemsSecondPage(this, "All", booksListcopy));
        }

        private async void viewAllVideoGameItems_Button_Clicked(object sender, EventArgs e)
        {
            if ((Application.Current as App).UserId == 0)
            {
                await DisplayAlert("גישה לא מורשת", "עלייך ראשית להתחבר!", "OK");
                return;
            }
            List<ItemFormServices.VideoGame> videoGamesListcopy = VideoGameitems.ToList();
            await Navigation.PushAsync(new MyItemsSecondPage(this, "All", videoGamesListcopy));
        }
        public void ClearAllSL()
        {
            myBooksSL.Children.Clear();
            myVideoGamesSL.Children.Clear();
        }
    }
}