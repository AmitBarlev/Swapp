using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Swap.Services;
namespace Swap.Views
{
    internal enum ViewOptions
    {
        ThreeImagesPerRow = 0,
        FourImagesPerRow = 1
    };

    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MyItemsSecondPage : ContentPage
    {
        private MyItemsPage m_itemsPage;
        private static List<ItemFormServices.Book> m_bookitems = new List<ItemFormServices.Book>();
        private static List<ItemFormServices.VideoGame> m_videoGameitems = new List<ItemFormServices.VideoGame>();
        private List<Grid> threeItemsArowGrids = new List<Grid>();
        private List<Grid> fourItemsArowGrids = new List<Grid>();

        internal readonly Dictionary<string, StackLayout> VideoGameGenreToStackLayout = new Dictionary<string, StackLayout>();
        internal readonly Dictionary<string, StackLayout> VideoGamePlatformToStackLayout = new Dictionary<string, StackLayout>();
        internal readonly Dictionary<string, StackLayout> BooksGenreToStackLayout = new Dictionary<string, StackLayout>();
        private ViewOptions m_imagesView = ViewOptions.ThreeImagesPerRow;
        private bool m_pageAlreadyBeenSortedByVideoGameGenre = false;
        private bool m_pageAlreadyBeenSortedByBooksGenre = false;
        private bool m_pageAlreadyBeenSortedByPlatform = false;

        private string m_category;
        private bool m_pageHasAppeared = false;
        private string m_currentPage;

        protected override void OnAppearing()
        {

            if (m_pageHasAppeared == true)
                return;
            m_pageHasAppeared = true;


            if (m_category == "All")
            {
                if (m_currentPage == "Video Game")
                {
                    VideoGamesViewOptionsGrid.IsVisible = true;
                    InitializeGrids("VideoGame");
                }
                else if (m_currentPage == "Book")
                {
                    BooksViewOptionsGrid.IsVisible = true;
                    InitializeGrids("Book");

                }
            }
            else
            {
                if (m_currentPage == "Video Game")
                {
                    List<ItemFormServices.VideoGame> originalList = m_videoGameitems.ToList();
                    removeAllItemsOutOfCategory("Video Game", m_category);
                    InitializeGrids("VideoGame");
                    m_videoGameitems = originalList;

                }
                else if (m_currentPage == "Book")
                {

                    List<ItemFormServices.Book> originalList2 = m_bookitems.ToList();
                    removeAllItemsOutOfCategory("Book", m_category);
                    InitializeGrids("Book");
                    m_bookitems = originalList2;
                }
            }


        }

        //------------------constructors---------------------//

        internal MyItemsSecondPage(MyItemsPage i_myItemsPage, string i_categoryOption, List<ItemFormServices.Book> i_bookitems)
        {
            InitializeComponent();
            m_bookitems = i_bookitems;
            m_itemsPage = i_myItemsPage;
            m_category = i_categoryOption;
            m_currentPage = "Book";
        }

        internal MyItemsSecondPage(MyItemsPage i_myItemsPage, string i_categoryOption, List<ItemFormServices.VideoGame> i_videoGameitems)
        {
            InitializeComponent();
            m_videoGameitems = i_videoGameitems;
            m_itemsPage = i_myItemsPage;
            m_category = i_categoryOption;
            m_currentPage = "Video Game";
        }
        //-----------------------------------------------------//

        private void removeAllItemsOutOfCategory(string i_type, string i_categoryOption)
        {
            switch (i_type)
            {
                case "Book":
                    foreach (ItemFormServices.Book bookItem in m_bookitems.ToList())
                    {
                        if (bookItem.Genre != i_categoryOption)
                        {
                            m_bookitems.Remove(bookItem);
                        }
                    }
                    break;
                case "Video Game":

                    foreach (ItemFormServices.VideoGame videoGameitem in m_videoGameitems.ToList())
                    {

                        ItemFormServices.GamingPlatform gamingPlatform = videoGameitem.Platform;
                        string platform = ItemFormServices.StringToPlatform.FirstOrDefault(x => x.Value == gamingPlatform).Key;
                        if ((videoGameitem.Genre != i_categoryOption) && (platform != i_categoryOption))
                        {
                            m_videoGameitems.Remove(videoGameitem);

                        }
                    }
                    break;
            }
        }

        private void InitializeGrids(string i_type)
        {

            int listOfItemsLength = 0;
            if (i_type == "Book")
            {
                listOfItemsLength = m_bookitems.Count;
            }
            else if (i_type == "VideoGame")
            {
                listOfItemsLength = m_videoGameitems.Count;

            }
            for (int i = 0; i < listOfItemsLength; i++)
            {


                threeItemsArowGrids.Add(new Grid());
                fourItemsArowGrids.Add(new Grid());

                threeItemsArowGrids[i].ColumnDefinitions.Add(new ColumnDefinition
                {
                    Width = new GridLength(1, GridUnitType.Star)
                });
                threeItemsArowGrids[i].ColumnDefinitions.Add(new ColumnDefinition
                {
                    Width = new GridLength(1, GridUnitType.Star)
                });
                threeItemsArowGrids[i].ColumnDefinitions.Add(new ColumnDefinition
                {
                    Width = new GridLength(1, GridUnitType.Star)
                });

                fourItemsArowGrids[i].ColumnDefinitions.Add(new ColumnDefinition
                {
                    Width = new GridLength(1, GridUnitType.Star)
                });
                fourItemsArowGrids[i].ColumnDefinitions.Add(new ColumnDefinition
                {
                    Width = new GridLength(1, GridUnitType.Star)
                });
                fourItemsArowGrids[i].ColumnDefinitions.Add(new ColumnDefinition
                {
                    Width = new GridLength(1, GridUnitType.Star)
                });
                fourItemsArowGrids[i].ColumnDefinitions.Add(new ColumnDefinition
                {
                    Width = new GridLength(1, GridUnitType.Star)
                });


                Grid bigImageContainer = null;
                Grid smallImageContainer = null;
                if (i_type == "Book")
                {
                    smallImageContainer = m_itemsPage.makeGridImageContainer(m_bookitems[i], 100, 180, 20);
                    bigImageContainer = m_itemsPage.makeGridImageContainer(m_bookitems[i], 130, 250, 20);
                }
                else if (i_type == "VideoGame")
                {
                    smallImageContainer = m_itemsPage.makeGridImageContainer(m_videoGameitems[i], 100, 180, 20);
                    bigImageContainer = m_itemsPage.makeGridImageContainer(m_videoGameitems[i], 130, 250, 20);

                }

                threeItemsArowGrids[i / 3].FlowDirection = FlowDirection.RightToLeft;
                threeItemsArowGrids[i / 3].Children.Add(bigImageContainer, i % 3, 0);

                fourItemsArowGrids[i / 4].FlowDirection = FlowDirection.RightToLeft;
                fourItemsArowGrids[i / 4].Children.Add(smallImageContainer, i % 4, 0);
            }
            for (int i = 0; i < threeItemsArowGrids.Count; i++)
            {
                threeInArowMainStackLayout.Children.Add(threeItemsArowGrids[i]);
            }
            for (int i = 0; i < fourItemsArowGrids.Count; i++)
            {
                fourInArowMainStackLayout.Children.Add(fourItemsArowGrids[i]);
            }

        }

        private void changeView_Button_Clicked(object sender, EventArgs e)
        {

            if (m_imagesView == ViewOptions.ThreeImagesPerRow)
            {

                threeInArowMainStackLayout.IsVisible = false;
                fourInArowMainStackLayout.IsVisible = true;
                m_imagesView = ViewOptions.FourImagesPerRow;

                threeUpButton.IsVisible = true;
                fourUpButton.IsVisible = false;

            }
            else if (m_imagesView == ViewOptions.FourImagesPerRow)
            {
                fourInArowMainStackLayout.IsVisible = false;
                threeInArowMainStackLayout.IsVisible = true;
                m_imagesView = ViewOptions.ThreeImagesPerRow;
                fourUpButton.IsVisible = true;
                threeUpButton.IsVisible = false;

            }


        }
        //---------------Video Games Event Handlers---------------//

        private void viewAllVideoGames_Button_Clicked(object sender, EventArgs e)
        {
            viewAllMainStackLayout.IsVisible = true;
            viewByVideoGameGenreMainStackLayout.IsVisible = false;
            viewByPlatformMainStackLayout.IsVisible = false;

            viewAllVideoGamesButton.FontAttributes = FontAttributes.Bold;
            viewAllVideoGamesButton.BorderWidth = 1;
            viewByVideoGameGenreButton.BorderWidth = viewByPlatformButton.BorderWidth = 0;
            viewByVideoGameGenreButton.FontAttributes = viewByPlatformButton.FontAttributes = FontAttributes.None;
        }

        private void viewVideoGamesByGenre_Button_clicked(object sender, EventArgs e)
        {
            viewByVideoGameGenreMainStackLayout.IsVisible = true;
            viewAllMainStackLayout.IsVisible = false;
            viewByPlatformMainStackLayout.IsVisible = false;

            viewByVideoGameGenreButton.FontAttributes = FontAttributes.Bold;
            viewByVideoGameGenreButton.BorderWidth = 1;
            viewAllVideoGamesButton.BorderWidth = viewByPlatformButton.BorderWidth = 0;
            viewAllVideoGamesButton.FontAttributes = viewByPlatformButton.FontAttributes = FontAttributes.None;

            if (m_pageAlreadyBeenSortedByVideoGameGenre == false)
            {
                viewPageSortedBy("VideoGame Genre");
                m_pageAlreadyBeenSortedByVideoGameGenre = true;
            }


        }

        private void viewByPlatform_Button_clicked(object sender, EventArgs e)
        {
            viewByPlatformMainStackLayout.IsVisible = true;
            viewByVideoGameGenreMainStackLayout.IsVisible = false;
            viewAllMainStackLayout.IsVisible = false;

            viewByPlatformButton.FontAttributes = FontAttributes.Bold;
            viewByPlatformButton.BorderWidth = 1;
            viewByVideoGameGenreButton.BorderWidth = viewAllVideoGamesButton.BorderWidth = 0;
            viewByVideoGameGenreButton.FontAttributes = viewAllVideoGamesButton.FontAttributes = FontAttributes.None;


            if (m_pageAlreadyBeenSortedByPlatform == false)
            {
                viewPageSortedBy("Platform");
                m_pageAlreadyBeenSortedByPlatform = true;
            }


        }
        //---------------Books View Event Handlers---------------//

        private void viewAllBooks_Button_Clicked(object sender, EventArgs e)
        {
            viewAllMainStackLayout.IsVisible = true;
            viewByBooksGenreMainStackLayout.IsVisible = false;

            viewAllBooksButton.FontAttributes = FontAttributes.Bold;
            viewAllBooksButton.BorderWidth = 1;

            viewByBooksGenreButton.BorderWidth = 0;
            viewByBooksGenreButton.FontAttributes = FontAttributes.None;
        }

        private void viewBooksByGenre_Button_clicked(object sender, EventArgs e)
        {
            viewByBooksGenreMainStackLayout.IsVisible = true;
            viewAllMainStackLayout.IsVisible = false;

            viewByBooksGenreButton.FontAttributes = FontAttributes.Bold;
            viewByBooksGenreButton.BorderWidth = 1;
            viewAllBooksButton.BorderWidth = 0;
            viewAllBooksButton.FontAttributes = FontAttributes.None;

            if (m_pageAlreadyBeenSortedByBooksGenre == false)
            {
                viewPageSortedBy("Book Genre");
                m_pageAlreadyBeenSortedByBooksGenre = true;
            }


        }
        //-----------------------------------------------------//
        private void viewPageSortedBy(string i_sortingCategory)
        {
            if (i_sortingCategory == "VideoGame Genre")
            {

                for (int i = 0; i < m_videoGameitems.Count; i++)
                {
                    string genre = m_videoGameitems[i].Genre;
                    sortBy("Video Game", i, genre, VideoGameGenreToStackLayout, viewByVideoGameGenreMainStackLayout);

                }
            }
            else if (i_sortingCategory == "Platform")
            {
                for (int i = 0; i < m_videoGameitems.Count; i++)
                {
                    ItemFormServices.GamingPlatform gamingPlatform = m_videoGameitems[i].Platform;
                    string platform = ItemFormServices.StringToPlatform.FirstOrDefault(x => x.Value == gamingPlatform).Key;
                    sortBy("Video Game", i, platform, VideoGamePlatformToStackLayout, viewByPlatformMainStackLayout);


                }
            }
            else if (i_sortingCategory == "Book Genre")
            {
                for (int i = 0; i < m_bookitems.Count; i++)
                {
                    string genre = m_bookitems[i].Genre;
                    sortBy("Book", i, genre, BooksGenreToStackLayout, viewByBooksGenreMainStackLayout);


                }
            }
        }

        private void sortBy(string i_type, int i, string i_category, Dictionary<string, StackLayout> i_dictionary, StackLayout i_mainStackLayout)
        {
            if (i_dictionary.ContainsKey(i_category) == false)
            {

                StackLayout stackLayout = new StackLayout()
                {
                    HeightRequest = 200,
                    Orientation = StackOrientation.Horizontal
                };
                Frame frame = new Frame()
                {
                    BorderColor = Color.WhiteSmoke,
                    Content = stackLayout

                };
                ScrollView result = new ScrollView()
                {
                    FlowDirection = FlowDirection.RightToLeft,
                    Orientation = ScrollOrientation.Horizontal,
                    HorizontalScrollBarVisibility = ScrollBarVisibility.Never,
                    Content = frame
                };
                i_dictionary.Add(i_category, stackLayout);

                Button button = new Button()
                {
                    Text = "הצג הכל",
                    FontSize = 12,
                    TextColor = Color.DarkOrange,
                    BackgroundColor = Color.White,
                    FontAttributes = FontAttributes.Bold,
                    HorizontalOptions = LayoutOptions.Start

                };

                button.Clicked += async (sender, e) =>
                {
                    if (i_type == "Book")
                    {

                        await Navigation.PushAsync(new MyItemsSecondPage(m_itemsPage, i_category, m_bookitems));


                    }
                    else if (i_type == "Video Game")
                    {

                        await Navigation.PushAsync(new MyItemsSecondPage(m_itemsPage, i_category, m_videoGameitems));

                    }

                };
                Label label = ItemFormServices.SetNewLabel(i_category);
                label.HorizontalOptions = LayoutOptions.EndAndExpand;
                label.VerticalOptions = LayoutOptions.EndAndExpand;

                StackLayout titleStackLayout = new StackLayout()
                {
                    Orientation = StackOrientation.Horizontal
                };
                titleStackLayout.Children.Add(button);
                titleStackLayout.Children.Add(label);

                i_mainStackLayout.Children.Add(titleStackLayout);
                i_mainStackLayout.Children.Add(result);

            }

            Grid imageContainer = null;
            if (i_type == "Book")
            {
                imageContainer = m_itemsPage.makeGridImageContainer(m_bookitems[i], 100, 180, 20);

            }
            else if (i_type == "Video Game")
            {

                imageContainer = m_itemsPage.makeGridImageContainer(m_videoGameitems[i], 100, 180, 20);
            }
            i_dictionary[i_category].Children.Add(imageContainer);
        }
    }
}

