using PhoneTag.SharedCodebase;
using PhoneTag.SharedCodebase.StaticInfo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace PhoneTag.XamarinForms.Pages
{
    /// <summary>
    /// The game creation page.
    /// </summary>
    public partial class CreateGamePage : ContentPage
    {
        private Entry textBoxGameName = new Entry();
        private Picker pickerGameMode = new Picker();

        public CreateGamePage()
        {
            initializeGameModeList();
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
        }

        //Opens the area chooser page to choose the game's area.
        private async void SetGameAreaButton_Clicked()
        {
            await Navigation.PushAsync(new GameAreaChooserPage());
        }

        //Creates the game using the chosen parameters.
        private void CreateGameButton_Clicked()
        {
            throw new NotImplementedException();
        }

        //Updates the selected game mode.
        private void PickerGameMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
