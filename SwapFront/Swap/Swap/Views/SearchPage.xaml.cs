using Newtonsoft.Json;
using Swap.ContentViews;
using Swap.CustomViews;
using Swap.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;


namespace Swap.Views
{

    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SearchPage : ContentPage
    {
        private App app = Application.Current as App;

        public enum ItemType
        {
            Item,
            Book,
            Videogame
        };
        internal readonly static Dictionary<string, ItemType> StringToItemType = new Dictionary<string, ItemType>()
          {
            { "ספר", ItemType.Book },
            { "משחק וידאו",ItemType.Videogame}
          };

        public StackLayout m_booksStackLayout { get; set; }
        public StackLayout m_videoGamesStackLayout { get; set; }

        private bool m_bookFormHadBuilt = false;
        private bool m_videoGameFormHadBuilt = false;




        private CustomPicker m_bookTypePicker, m_platformPicker, m_genrePicker;
        private CustomEntry m_numberOfPagesEntry, m_authorEntry;
        private bool m_searchButtonClickEnabled = true;


        public SearchPage()
        {
            InitializeComponent();
            initializeCommonPickers();
            m_booksStackLayout = new StackLayout();
            m_videoGamesStackLayout = new StackLayout();


        }

        private void initializeCommonPickers()
        {

            statePicker.Items.Add("הכל");
            statePicker.SelectedItem = "הכל";
            foreach (string choice in ItemFormServices.state)
            {
                statePicker.Items.Add(choice);
            }

            typePicker.Items.Add("הכל");
            typePicker.SelectedItem = "הכל";

            foreach (string choice in ItemFormServices.typeOfItem)
            {
                typePicker.Items.Add(choice);
            }


        }

        private void TypeOfItemPicker_SelectedIndexChanged(object sender, EventArgs e)
        {
            if ((string)typePicker.SelectedItem == "הכל")
            {
                if (m_booksStackLayout != null)
                {
                    mainStackLayout.Children.Remove(m_booksStackLayout);
                }
                if (m_videoGamesStackLayout != null)
                {
                    mainStackLayout.Children.Remove(m_videoGamesStackLayout);
                }

            }
            else if ((string)typePicker.SelectedItem == "ספר")
            {
                if (m_bookFormHadBuilt == false)
                {
                    ItemFormServices.MakeBookForm(this, ref m_bookTypePicker, ref m_authorEntry, ref m_numberOfPagesEntry);
                    m_bookFormHadBuilt = true;
                }
                mainStackLayout.Children.Remove(m_videoGamesStackLayout);
                mainStackLayout.Children.Add(m_booksStackLayout);
            }
            else if ((string)typePicker.SelectedItem == "משחק וידאו")
            {
                if (m_videoGameFormHadBuilt == false)
                {
                    ItemFormServices.MakeVideoGameForm(this, ref m_platformPicker, ref m_genrePicker);
                    m_videoGameFormHadBuilt = true;
                }
                mainStackLayout.Children.Remove(m_booksStackLayout);
                mainStackLayout.Children.Add(m_videoGamesStackLayout);
            }
        }

        private string getParametersForSearch()
        {
            string result = null;
            if (radiusSwitch.IsToggled == true)
            {
                result += string.Format("rad={0}&", RadiusSlider.Value);
            }
            else if (citySwitch.IsToggled == true)
            {
                result += string.Format("city={0}&", cityEntry.Text);
            }

            if (ItemName.Text != null)
            {
                result += string.Format("name={0}&", ItemName.Text);
            }

            if (statePicker.SelectedItem != null && (string)statePicker.SelectedItem != "הכל")
            {
                result += string.Format("con={0}&", (int)ItemFormServices.StringToItemCondition[(string)statePicker.SelectedItem]);
            }
            if (typePicker.SelectedItem != null && (string)typePicker.SelectedItem != "הכל")
            {
                result += string.Format("t={0}&", (int)StringToItemType[(string)typePicker.SelectedItem]);
            }
            if ((string)typePicker.SelectedItem == "ספר")
            {

                if (m_bookTypePicker.SelectedItem != null && (string)m_bookTypePicker.SelectedItem != "הכל")
                {
                    result += string.Format("gen={0}&", (string)m_bookTypePicker.SelectedItem);
                }

                if (m_authorEntry.Text != null)
                {
                    result += string.Format("au={0}&", m_authorEntry.Text);
                }

            }
            if ((string)typePicker.SelectedItem == "משחק וידאו")
            {

                if (m_genrePicker.SelectedItem != null && (string)m_genrePicker.SelectedItem != "הכל")
                {
                    result += string.Format("gen={0}&", (string)m_genrePicker.SelectedItem);
                }
                if (m_platformPicker.SelectedItem != null && (string)m_platformPicker.SelectedItem != "הכל")
                {
                    result += string.Format("plat={0}&", (int)ItemFormServices.StringToPlatform[(string)m_platformPicker.SelectedItem]);
                }
            }

            if (result != null)
            {

                if (result.EndsWith("&") == true)
                {
                    result = result.Remove(result.Length - 1);
                }
            }

            return result;
        }

        async void search_Button_Clicked(object sender, EventArgs e)
        {
            if (m_searchButtonClickEnabled == false)
                return;

            m_searchButtonClickEnabled = false;

            string parameters = getParametersForSearch();
            try
            {
                if (string.IsNullOrWhiteSpace(app.Token) == true)
                {
                    await Shell.Current.DisplayAlert("גישה לא מורשת", "עלייך ראשית להתחבר!", "אישור");
                    return;
                }
                List<ItemFormServices.Item> items = await ServerFacade.Items.SearchItemsAsync(parameters);
                await Navigation.PushAsync(new SearchResultsPage(items));
                m_searchButtonClickEnabled = true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            finally
            {
                m_searchButtonClickEnabled = true;
            }
        }

        private void radius_Switch_Toggled_(object sender, ToggledEventArgs e)
        {
            if (e.Value)
            {
                if (citySwitch.IsToggled)
                {
                    citySwitch.IsToggled = false;
                }
            }
        }

        private void city_Switch_Toggled_(object sender, ToggledEventArgs e)
        {
            if (e.Value)
            {
                if (radiusSwitch.IsToggled)
                {
                    radiusSwitch.IsToggled = false;
                }
            }
        }
    }
}