using Plugin.Media;
using Plugin.Media.Abstractions;
using Swap.CustomViews;
using Swap.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using static Swap.Services.ItemFormServices;

namespace Swap.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ItemFormPage : ContentPage
    {
        public StackLayout m_booksFormStackLayout { get; set; }
        public StackLayout m_videoGamesFormStackLayout { get; set; }

        private bool m_bookFormHadBuilt = false;
        private bool m_videoGameFormHadBuilt = false;

        private CustomPicker m_bookGenrePicker;
        private CustomEntry m_numberOfPagesEntry, m_authorEntry;

        private CustomPicker m_platformPicker, m_videoGameGenrePicker;

        private CustomEntry m_description;
        private List<string> ImagesOfItem;
        internal readonly Dictionary<Xamarin.Forms.Image, string> m_imageToString = new Dictionary<Xamarin.Forms.Image, string>();
        private Item m_Item;

        private Button m_okButton;
        private StackLayout m_doneEditingStackLayout;
        private StackLayout m_deleteItemStackLayout;
        private MyItemsPage m_myItemsPage;

        private bool m_okButtonClickEnabled = true;
        private bool m_doneEditingButtonClickEnabled = true;
        private bool m_deleteItemButtonClickEnabled = true;

        //-----main methods-----//
        private void initializeCommonPickers()
        {

            foreach (string choice in state)
            {
                statePicker.Items.Add(choice);
            }
            foreach (string choice in typeOfItem)
            {
                typePicker.Items.Add(choice);
            }

        }

        private CustomEntry getDescriptionCustomEntry()
        {
            CustomEntry descriptionCustomEntry = new CustomEntry()
            {
                Placeholder = "פירוט נוסף",
                HorizontalTextAlignment = TextAlignment.Center,
                FontSize = 16,
                HorizontalOptions = LayoutOptions.Fill,
                VerticalOptions = LayoutOptions.Center,
                HeightRequest = 45


            };

            return descriptionCustomEntry;
        }

        private bool allRequiredFieldsHadBeenFilled()
        {
            if ((string.IsNullOrWhiteSpace(ItemName.Text) == true) ||

                 (statePicker.SelectedItem == null))
                return false;

            if ((string)typePicker.SelectedItem == "ספר")
            {
                if (m_bookGenrePicker.SelectedItem == null)
                    return false;
            }

            if ((string)typePicker.SelectedItem == "משחק וידאו")
            {
                if (m_platformPicker.SelectedItem == null ||
                   m_videoGameGenrePicker.SelectedItem == null)
                    return false;
            }

            return true;
        }

        private void updateItem()
        {
            if (m_Item is Book)
            {
                (m_Item as Book).Author = m_authorEntry.Text;
                if (m_numberOfPagesEntry.Text != null)
                {
                    (m_Item as Book).Pages = short.Parse(m_numberOfPagesEntry.Text);
                }
                m_Item.Genre = (string)m_bookGenrePicker.SelectedItem;
            }
            else
            {
                m_Item = new VideoGame();
                (m_Item as VideoGame).Genre = (string)m_videoGameGenrePicker.SelectedItem;
                (m_Item as VideoGame).Platform = StringToPlatform[(string)m_platformPicker.SelectedItem];
            }
        }

        private Item getItem()
        {
            Item item;
            string selectedTypeOfItem = (string)typePicker.SelectedItem;

            if (selectedTypeOfItem == "ספר" || m_Item is Book)
            {
                item = new Book();
                (item as Book).Author = m_authorEntry.Text;
                if (m_numberOfPagesEntry.Text != null)
                {
                    (item as Book).Pages = short.Parse(m_numberOfPagesEntry.Text);
                }
                item.Genre = (string)m_bookGenrePicker.SelectedItem;
            }
            else
            {
                item = new VideoGame();

                (item as VideoGame).Genre = (string)m_videoGameGenrePicker.SelectedItem;
                (item as VideoGame).Platform = StringToPlatform[(string)m_platformPicker.SelectedItem];
            }

            item.ImagesOfItem = new List<ItemFormServices.Image>(ImagesOfItem.Count);
            for (int i = 0; i < ImagesOfItem.Count; i++)
            {
                ItemFormServices.Image image = new ItemFormServices.Image
                {
                    BytesOfImage = string.Copy(ImagesOfItem[i])
                };
                item.ImagesOfItem.Add(image);
            }

            item.Name = ItemName.Text;
            item.IdCustomer = (Application.Current as App).UserId;
            item.Description = m_description.Text;
            item.Condition = StringToItemCondition[(string)statePicker.SelectedItem];
            item.Views = 0;

            if (m_Item != null)
            {
                item.Id = m_Item.Id;
            }

            return item;
        }

        private async void takePhoto_Button_Clicked(object sender, EventArgs e)
        {
            try
            {
                if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
                {
                    await DisplayAlert("No Camera", ":( No camera available.", "OK");
                    return;
                }

                if (ImagesOfItem.Count == 5)
                {
                    await DisplayAlert("הגבלת מ'ס תמונות לפריט", "באפשרותך להוסיף עד 5 תמונות לפריט!", "OK");
                    return;
                }

                var photo = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions()
                {
                    CompressionQuality = 30,
                    CustomPhotoSize = 50,
                    DefaultCamera = CameraDevice.Rear,
                });

                if (photo != null)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        photo.GetStream().CopyTo(memoryStream);
                        ImagesOfItem.Add(Convert.ToBase64String(memoryStream.ToArray()));

                        MR.Gestures.Image image = new MR.Gestures.Image()
                        {
                            Source = ImageSource.FromStream(() => photo.GetStream()),
                            HeightRequest = 180,
                            WidthRequest = 100,
                            Aspect = Aspect.AspectFill,
                        };

                        m_imageToString[image] = Convert.ToBase64String(memoryStream.ToArray());
                        image.LongPressing += deleteImage_Button_LongPressing;
                        myImagesStackLayout.Children.Add(image);
                        image.Opacity = 0;

                        image.FadeTo(1, 2000);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        private async void pickPhoto_Button_Clicked(object sender, EventArgs e)
        {
            try
            {
                if (!CrossMedia.Current.IsPickPhotoSupported)
                {
                    await DisplayAlert("Photos Not Supported", ":( Permission not granted to photos.", "OK");
                    return;
                }
                if (ImagesOfItem.Count == 5)
                {
                    await DisplayAlert("הגבלת מ'ס תמונות לפריט", "באפשרותך להוסיף עד 5 תמונות לפריט!", "OK");
                    return;
                }

                var file = await Plugin.Media.CrossMedia.Current.PickPhotoAsync(new Plugin.Media.Abstractions.PickMediaOptions
                {
                    CompressionQuality = 30,
                    CustomPhotoSize = 50,
                });
                if (file == null)
                    return;

                using (var memoryStream = new MemoryStream())
                {
                    file.GetStream().CopyTo(memoryStream);
                    ImagesOfItem.Add(Convert.ToBase64String(memoryStream.ToArray()));

                    MR.Gestures.Image image = new MR.Gestures.Image()
                    {

                        Source = ImageSource.FromStream(() => file.GetStream()),
                        HeightRequest = 180,
                        WidthRequest = 100,
                        Aspect = Aspect.AspectFill,
                    };

                    m_imageToString[image] = Convert.ToBase64String(memoryStream.ToArray());
                    image.LongPressing += deleteImage_Button_LongPressing;

                    myImagesStackLayout.Children.Add(image);
                    image.Opacity = 0;
                    image.FadeTo(1, 2000);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        private async void deleteImage_Button_LongPressing(object sender, EventArgs e)
        {
            bool answer = await DisplayAlert("", "האם אתה בטוח שברצונך למחוק תמונה זו?", "כן", "לא");
            if (answer == false)
                return;

            Xamarin.Forms.Image currentImage = sender as Xamarin.Forms.Image;
            myImagesStackLayout.Children.Remove(currentImage);
            ImagesOfItem.Remove(m_imageToString[currentImage]);
        }

        //----------Edit Item Page methods----------//
        public ItemFormPage(string i_edit, MyItemsPage i_page, Item i_Item)
        {
            m_Item = i_Item;
            InitializeComponent();
            initializeCommonPickers();
            m_booksFormStackLayout = new StackLayout();
            m_videoGamesFormStackLayout = new StackLayout();
            m_myItemsPage = i_page;

            ImagesOfItem = new List<string>();
            typePicker.IsEnabled = false;

            m_description = getDescriptionCustomEntry();
            m_doneEditingStackLayout = getDoneEditingStackLayout();
            m_deleteItemStackLayout = getDeleteItemStackLayout();

            Grid doneEditAndDeleteButtonsGrid = new Grid();
            doneEditAndDeleteButtonsGrid.Children.Add(m_doneEditingStackLayout, 0, 0);
            doneEditAndDeleteButtonsGrid.Children.Add(m_deleteItemStackLayout, 1, 0);

            doneEditAndDeleteButtonsGrid.ColumnDefinitions.Add(new ColumnDefinition
            {
                Width = new GridLength(1, GridUnitType.Star)
            });

            doneEditAndDeleteButtonsGrid.ColumnDefinitions.Add(new ColumnDefinition
            {
                Width = new GridLength(1, GridUnitType.Star)
            });

            if (m_Item is Book)
            {
                MakeBookForm(this, ref m_bookGenrePicker, ref m_authorEntry, ref m_numberOfPagesEntry);
                mainStackLayout.Children.Add(m_booksFormStackLayout);
                m_booksFormStackLayout.Children.Add(m_description);
                m_booksFormStackLayout.Children.Add(doneEditAndDeleteButtonsGrid);
            }
            else if (m_Item is VideoGame)
            {
                MakeVideoGameForm(this, ref m_platformPicker, ref m_videoGameGenrePicker);
                mainStackLayout.Children.Add(m_videoGamesFormStackLayout);
                m_videoGamesFormStackLayout.Children.Add(m_description);
                m_videoGamesFormStackLayout.Children.Add(doneEditAndDeleteButtonsGrid);
            }

            fillFormWithItemDetails(m_Item);
        }

        private void fillFormWithItemDetails(Item i_item)
        {
            typePicker.IsVisible = false;
            ItemName.Text = i_item.Name;

            ItemCondition condition = i_item.Condition;
            string state = StringToItemCondition.FirstOrDefault(x => x.Value == condition).Key;
            statePicker.SelectedItem = state;
            if (i_item is Book)
            {
                BookLable.IsVisible = true;
                m_authorEntry.Text = (i_item as Book).Author;
                m_numberOfPagesEntry.Text = ((i_item as Book).Pages).ToString();

                m_bookGenrePicker.SelectedItem = i_item.Genre;
            }

            if (i_item is VideoGame)
            {
                videoGameLable.IsVisible = true;
                m_videoGameGenrePicker.SelectedItem = m_videoGameGenrePicker.SelectedItem;

                GamingPlatform platform = (i_item as VideoGame).Platform;
                string console = StringToPlatform.FirstOrDefault(x => x.Value == platform).Key;

                m_platformPicker.SelectedItem = console;
                m_videoGameGenrePicker.SelectedItem = i_item.Genre;
            }
            m_description.Text = i_item.Description;

            for (int i = 0; i < i_item.ImagesOfItem.Count; i++)
            {
                var byteArray = Convert.FromBase64String(i_item.ImagesOfItem[i].BytesOfImage);
                var imageSource = ImageSource.FromStream(() => new MemoryStream(byteArray));

                MR.Gestures.Image image = new MR.Gestures.Image()
                {
                    Source = imageSource,
                    HeightRequest = 180,
                    WidthRequest = 100,
                    Aspect = Aspect.AspectFill,

                };
                ImagesOfItem.Add(i_item.ImagesOfItem[i].BytesOfImage);

                m_imageToString[image] = i_item.ImagesOfItem[i].BytesOfImage;
                image.LongPressing += deleteImage_Button_LongPressing;

                myImagesStackLayout.Children.Add(image);
                image.Opacity = 0;
                image.FadeTo(1, 2000);
            }
        }

        private StackLayout getDeleteItemStackLayout()
        {
            StackLayout result = new StackLayout();
            Label deleteItemLabel = (new Label()
            {
                Text = "מחק",
                FontAttributes = FontAttributes.Bold,
                HorizontalOptions = LayoutOptions.Center

            });
            ImageButton deleteItemImageButton = new ImageButton()
            {
                Source = "delete_item.png",
                BackgroundColor = Color.White,
                HorizontalOptions = LayoutOptions.Center

            };

            deleteItemImageButton.Clicked += deleteItem_Button_clicked;
            result.Children.Add(deleteItemImageButton);
            result.Children.Add(deleteItemLabel);

            return result;
        }

        private StackLayout getDoneEditingStackLayout()
        {
            StackLayout result = new StackLayout();

            Label doneEditingLabel = (new Label()
            {
                Text = "השלם עריכה",
                FontAttributes = FontAttributes.Bold,
                HorizontalOptions = LayoutOptions.Center
            });
            ImageButton doneEditingImageButton = new ImageButton()
            {
                Source = "edit_item.png",
                BackgroundColor = Color.White,
                HorizontalOptions = LayoutOptions.Center

            };

            doneEditingImageButton.Clicked += doneEditing_Button_clicked;
            result.Children.Add(doneEditingImageButton);
            result.Children.Add(doneEditingLabel);
            return result;
        }

        private async void deleteItem_Button_clicked(object sender, EventArgs e)
        {
            bool answer = await DisplayAlert("", "האם אתה בטוח שברצונך למחוק פריט זה?", "כן", "לא");
            if (answer == false)
                return;
            if (m_deleteItemButtonClickEnabled == false)
                return;
            m_deleteItemButtonClickEnabled = false;
            m_myItemsPage.removeItemFromStackLayout(m_Item);

            try
            {
                await ServerFacade.Items.DeleteItem(m_Item);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                await Shell.Current.DisplayAlert("שגיאת מערכת", "נסה מאוחר יותר", "אישור");
            }

            await Navigation.PushAsync(new WaitingPage());
            await DisplayAlert("", "הפריט נמחק בהצלחה!", "OK");

            m_deleteItemButtonClickEnabled = true;
            await Navigation.PopToRootAsync();
        }

        private async void doneEditing_Button_clicked(object sender, EventArgs e)
        {
            if (m_doneEditingButtonClickEnabled == false)
                return;
            m_doneEditingButtonClickEnabled = false;

            if (!allRequiredFieldsHadBeenFilled())
            {
                await DisplayAlert(" ישנם שדות חובה שלא מולאו", "מלא את כל השדות המופיעים ב (*)", "OK");
                m_doneEditingButtonClickEnabled = true;
                return;
            }

            m_myItemsPage.removeItemFromStackLayout(m_Item);
            Item item = getItem();
            try
            {
                await ServerFacade.Items.EditItem(item);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                await Shell.Current.DisplayAlert("שגיאת מערכת", "נסה מאוחר יותר", "אישור");
            }
            await Navigation.PushAsync(new WaitingPage());
            m_myItemsPage.addItemToStackLayout(item);
            await DisplayAlert("", "הפריט נערך בהצלחה!", "OK");
            m_doneEditingButtonClickEnabled = true;
            await Navigation.PopToRootAsync();
        }


        //----------Add Item Page methods----------//
        public ItemFormPage(string i_add, MyItemsPage i_page)
        {
            InitializeComponent();
            initializeCommonPickers();
            m_booksFormStackLayout = new StackLayout();
            m_videoGamesFormStackLayout = new StackLayout();
            ImagesOfItem = new List<string>();
            m_myItemsPage = i_page;
        }

        private Button getOkButton()
        {
            Button okButton = (new Button()
            {
                Text = "אישור",
                FontAttributes = FontAttributes.Bold,
                BackgroundColor = Color.SpringGreen

            });
            okButton.Clicked += ok_Button_clicked;

            return okButton;
        }

        private void TypeOfItemPicker_SelectedIndexChanged(object sender, EventArgs e)
        {
            if ((string)typePicker.SelectedItem == "ספר")
            {
                if (m_bookFormHadBuilt == false)
                {
                    MakeBookForm(this, ref m_bookGenrePicker, ref m_authorEntry, ref m_numberOfPagesEntry);
                    m_description = getDescriptionCustomEntry();
                    m_okButton = getOkButton();
                    m_booksFormStackLayout.Children.Add(m_description);
                    m_booksFormStackLayout.Children.Add(m_okButton);

                    m_bookFormHadBuilt = true;
                }
                mainStackLayout.Children.Remove(m_videoGamesFormStackLayout);
                mainStackLayout.Children.Add(m_booksFormStackLayout);
            }
            else if ((string)typePicker.SelectedItem == "משחק וידאו")
            {
                if (m_videoGameFormHadBuilt == false)
                {
                    MakeVideoGameForm(this, ref m_platformPicker, ref m_videoGameGenrePicker);
                    m_description = getDescriptionCustomEntry();
                    m_okButton = getOkButton();
                    m_videoGamesFormStackLayout.Children.Add(m_description);
                    m_videoGamesFormStackLayout.Children.Add(m_okButton);

                    m_videoGameFormHadBuilt = true;
                }
                mainStackLayout.Children.Remove(m_booksFormStackLayout);
                mainStackLayout.Children.Add(m_videoGamesFormStackLayout);
            }
        }

        private async void ok_Button_clicked(object sender, EventArgs e)
        {
            if (m_okButtonClickEnabled == false)
                return;
            m_okButtonClickEnabled = false;

            if (!allRequiredFieldsHadBeenFilled())
            {
                await DisplayAlert(" ישנם שדות חובה שלא מולאו", "מלא את כל השדות המופיעים ב (*)", "OK");
                m_okButtonClickEnabled = true;
                return;
            }

            Item item = getItem();
            try
            {
                await ServerFacade.Items.AddItem(item);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                await Shell.Current.DisplayAlert("שגיאת מערכת", "נסה מאוחר יותר", "אישור");
            }
            int currentIdCustomer = (Application.Current as App).UserId;

            await m_myItemsPage.DisplayItemsOfUser(currentIdCustomer, "Last Uploaded");

            await DisplayAlert("", "הפריט נוסף בהצלחה!", "OK");
            m_okButtonClickEnabled = true;
            await Navigation.PopToRootAsync();

        }
        //EOF
    }
}