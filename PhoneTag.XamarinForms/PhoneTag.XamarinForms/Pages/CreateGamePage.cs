using PhoneTag.SharedCodebase;
using PhoneTag.SharedCodebase.StaticInfo;
using PhoneTag.SharedCodebase.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using PhoneTag.XamarinForms.Extensions;
using PhoneTag.SharedCodebase.Utils;

namespace PhoneTag.XamarinForms.Pages
{
    /// <summary>
    /// The game creation page.
    /// </summary>
    public partial class CreateGamePage : ContentPage
    {
        private Entry textBoxGameName = new Entry();
        private Picker pickerGameMode = new Picker();
        private Button buttonCreateGame = new Button();
        private StackLayout stackLayoutGameDetails = new StackLayout();

        private GameAreaChooserPage m_AreaChooserPage = new GameAreaChooserPage();

        private GameDetailsView m_GameDetails = null;

        public CreateGamePage()
        {
            initializeGameModeList();

            pickerGameMode.SelectedIndexChanged += PickerGameMode_SelectedIndexChanged;
            buttonCreateGame.Clicked += CreateGameButton_Clicked;
            textBoxGameName.TextChanged += TextBoxGameName_TextChanged;
            
            initializeComponent();
        }

        //Collects the supported game modes from the server and updates the picker with them.
        private async void initializeGameModeList()
        {
            List<String> gameModes = await PhoneTagInfo.GetGameModeList();

            foreach (String gameMode in gameModes)
            {
                pickerGameMode.Items.Add(gameMode);
            }

            pickerGameMode.IsEnabled = true;
        }

        //Opens the area chooser page to choose the game's area.
        private async void SetGameAreaButton_Clicked()
        {
            await Navigation.PushAsync(m_AreaChooserPage);
        }

        //Creates the game using the chosen parameters.
        private async void CreateGameButton_Clicked(object sender, EventArgs e)
        {
            m_GameDetails.StartLocation = new GeoPoint(m_AreaChooserPage.ChosenPosition.Latitude, m_AreaChooserPage.ChosenPosition.Longitude);
            m_GameDetails.GameRadius = m_AreaChooserPage.ChosenRadius;

            String gameRoomId = await GameRoomView.CreateRoom(m_GameDetails);
            await Navigation.PushAsync(new GameLobbyPage(gameRoomId));
        }

        //Updates the selected game mode.
        private void PickerGameMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            validateData();
            updateGameModeBox();
        }

        //Makes sure that the data is valid
        private void TextBoxGameName_TextChanged(object sender, TextChangedEventArgs e)
        {
            validateData();
        }

        //Checks if all data on the page is valid before enabling the creation button.
        //It's needed to be done in this way because multibinding is not yet supported in Xamarin.
        private void validateData()
        {
            buttonCreateGame.IsEnabled = pickerGameMode.SelectedIndex >= 0 && !String.IsNullOrEmpty(textBoxGameName.Text);
        }

        private void updateGameModeBox()
        {
            String gameModeName = pickerGameMode.Items[pickerGameMode.SelectedIndex];

            m_GameDetails = PhoneTagInfo.GetGameDetailsForMode(gameModeName);
            stackLayoutGameDetails = m_GameDetails.GetViewPresenter();

            initializeComponent();
            pickerGameMode.IsEnabled = true;
        }
    }
}
