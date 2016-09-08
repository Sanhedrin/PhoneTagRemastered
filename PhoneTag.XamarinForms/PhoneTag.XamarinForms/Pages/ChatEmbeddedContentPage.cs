using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PhoneTag.SharedCodebase.Events.GameEvents;
using PhoneTag.XamarinForms.Controls.MenuButtons;
using Xamarin.Forms;
using PhoneTag.SharedCodebase.Views;

namespace PhoneTag.XamarinForms.Pages
{
    public partial class ChatEmbeddedContentPage : TrailableContentPage
    {
        private bool m_ChatDialogOpen = false;
        private bool m_NewMessage = false;
        private bool m_MessageBlink = false;

        private ImageButton m_ChatButton;
        private Grid m_ChatDialog;
        
        private void triggerChatBox()
        {
            if (m_ChatDialogOpen)
            {
                m_ChatDialog.TranslateTo(0, 0, 250, Easing.Linear);
                m_ChatButton.TranslateTo(0, 0, 250, Easing.Linear);
                m_ChatBorder.TranslateTo(0, 0, 250, Easing.Linear);
            }
            else
            {
                m_ChatDialog.TranslateTo(-m_ChatBorder.Width, 0, 250, Easing.Linear);
                m_ChatButton.TranslateTo(-m_ChatBorder.Width, 0, 250, Easing.Linear);
                m_ChatBorder.TranslateTo(-m_ChatBorder.Width, 0, 250, Easing.Linear);

                m_ChatButton.Source = "chat_button.png";
                m_NewMessage = false;
            }

            m_ChatDialogOpen = !m_ChatDialogOpen;
        }
        
        private void ChatInput_Completed(object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(m_ChatInput.Text))
            {
                if (!String.IsNullOrEmpty(m_ChatBoxText.Text))
                {
                    m_ChatBoxText.Text += Environment.NewLine;
                }

                sendMessage(m_ChatInput.Text);

                m_ChatInput.Text = String.Empty;
            }
        }

        private void addMessageToBox(ChatMessageEvent i_EventDetails)
        {
            if (!m_NewMessage && !m_ChatDialogOpen)
            {
                newMessageBlink();
            }

            if (i_EventDetails.Message.StartsWith(Environment.NewLine))
            {
                i_EventDetails.Message = i_EventDetails.Message.Substring(1);
            }

            m_ChatBoxText.Text += String.Format("{0}: {1}", i_EventDetails.PlayerName, i_EventDetails.Message);

            if (!m_ChatBoxText.Text.EndsWith(Environment.NewLine))
            {
                m_ChatBoxText.Text += Environment.NewLine;
            }

            m_ChatBoxText.Text = m_ChatBoxText.Text.Replace(Environment.NewLine + Environment.NewLine, Environment.NewLine);

            m_ChatBoxScrollView.ScrollToAsync(m_ChatBoxScrollView.ScrollX, m_ChatBoxText.Height, false);
        }

        private async Task newMessageBlink()
        {
            m_NewMessage = true;

            while (m_NewMessage)
            {
                m_ChatButton.Source = m_MessageBlink ? "chat_button.png" : "chat_button_new_message.png";

                m_MessageBlink = !m_MessageBlink;

                await Task.Delay(1000);
            }
        }

        private async Task sendMessage(string i_Message)
        {
            addMessageToBox(new ChatMessageEvent(UserView.Current.FBID, UserView.Current.Username, i_Message));

            GameRoomView room = await GameRoomView.GetRoom(UserView.Current.PlayingIn);

            room.SendMessage(i_Message);
        }

        public override void ParseEvent(Event i_EventDetails)
        {
            ChatMessageEvent messageEvent = i_EventDetails as ChatMessageEvent;

            if(messageEvent != null && !UserView.Current.FBID.Equals(messageEvent.PlayerFBID))
            {
                addMessageToBox(messageEvent);
            }
        }
    }
}
