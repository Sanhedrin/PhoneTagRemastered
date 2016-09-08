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
using PhoneTag.SharedCodebase.Events.GameEvents;
using Plugin.Settings.Abstractions;
using Plugin.Settings;

namespace PhoneTag.XamarinForms.Pages
{
    /// <summary>
    /// The game creation page.
    /// </summary>
    public partial class CreateGamePage : TrailableContentPage
    {
        private Entry textBoxGameName = new Entry();
        private Picker pickerGameMode = new Picker();
        private Button buttonCreateGame = new Button();
        private StackLayout stackLayoutGameDetails = new StackLayout();

        private GameAreaChooserPage m_AreaChooserPage = new GameAreaChooserPage();

        private GameDetailsView m_GameDetails = null;

        public CreateGamePage() : base()
        {
            initializeGameModeList();

            pickerGameMode.SelectedIndexChanged += PickerGameMode_SelectedIndexChanged;
            buttonCreateGame.Clicked += CreateGameButton_Clicked;
            textBoxGameName.TextChanged += TextBoxGameName_TextChanged;
            
            initializeComponent();
        }

        //Collects the supported game modes from the server and updates the picker with them.
        private async Task initializeGameModeList()
        {
            pickerGameMode.IsEnabled = false;

            List<String> gameModes = await PhoneTagInfo.GetGameModeList();

            if (gameModes != null)
            {
                foreach (String gameMode in gameModes)
                {
                    pickerGameMode.Items.Add(gameMode);
                }
            }

            pickerGameMode.IsEnabled = true;
        }

        //Opens the area chooser page to choose the game's area.
        private async Task SetGameAreaButton_Clicked()
        {
            if (m_GameDetails != null)
            {
                await Navigation.PushAsync(m_AreaChooserPage);
            }
            else
            {
                await DisplayAlert("Error", "Please choose game mode before setting the game area", "Ok");
            }
        }

        //Creates the game using the chosen parameters.
        private void CreateGameButton_Clicked(object sender, EventArgs e)
        {
            buttonCreateGame.IsEnabled = false;
            initializeComponent();
            createGame();
        }

        private async Task createGame()
        {
            if (m_AreaChooserPage != null && m_AreaChooserPage.ChosenPosition != null)
            {
                while (m_AreaChooserPage.ChosenPosition.Latitude == 0 && m_AreaChooserPage.ChosenPosition.Longitude == 0)
                {
                    await Task.Delay(10);
                }

                m_GameDetails.StartLocation = new GeoPoint(m_AreaChooserPage.ChosenPosition.Latitude, m_AreaChooserPage.ChosenPosition.Longitude);
                m_GameDetails.GameRadius = m_AreaChooserPage.ChosenRadius;

                String gameRoomId = await GameRoomView.CreateRoom(m_GameDetails);
                Navigation.InsertPageBefore(new GameLobbyPage(gameRoomId), this);
                Navigation.PopAsync();
            }
        }

        //Updates the selected game mode.
        private void PickerGameMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            checkDisplayModeTypeTips();
            validateData();
            updateGameModeBox();
        }

        private async Task checkDisplayModeTypeTips()
        {
            String modeName = pickerGameMode.Items[pickerGameMode.SelectedIndex];
            bool displayTip = CrossSettings.Current.GetValueOrDefault(modeName, true);

            if (displayTip)
            {
                bool understood = await DisplayAlert(String.Format("Game Mode Information: {0}", modeName),
                    GameModeFactory.GetDescriptionForMode(modeName), "Don't show again", "Ok");

                CrossSettings.Current.AddOrUpdateValue(modeName, !understood);
            }
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
            if (m_GameDetails != null)
            {
                m_GameDetails.Name = textBoxGameName.Text;
            }
            buttonCreateGame.IsEnabled = pickerGameMode.SelectedIndex >= 0 && !String.IsNullOrEmpty(textBoxGameName.Text);
        }

        //Adds the details of the game mode for editting.
        private void updateGameModeBox()
        {
            String gameModeName = pickerGameMode.Items[pickerGameMode.SelectedIndex];

            m_GameDetails = PhoneTagInfo.GetGameDetailsForMode(gameModeName);
            stackLayoutGameDetails = m_GameDetails.GetViewPresenter();

            initializeComponent();
            pickerGameMode.IsEnabled = true;
        }

        public override void ParseEvent(Event i_EventDetails)
        {
        }
    }
}
