using PhoneTag.XamarinForms.Controls.MenuButtons;
using Plugin.XamJam.Screen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace PhoneTag.XamarinForms.Pages
{
    public partial class ChatEmbeddedContentPage : TrailableContentPage
    {
        private bool m_IsInitialized = false;
        private Label m_ChatBoxText;
        private Entry m_ChatInput;
        private ScrollView m_ChatBoxScrollView;
        private Image m_ChatBorder;

        protected void initializeChat()
        {
            if (!m_IsInitialized)
            {
                m_IsInitialized = true;

                m_ChatDialog = generateChatDialog();
                AbsoluteLayout.SetLayoutFlags(m_ChatDialog, AbsoluteLayoutFlags.All);
                AbsoluteLayout.SetLayoutBounds(m_ChatDialog, new Rectangle(3, 0.25, 0.64, 0.45));

                m_ChatBorder = generateChatBorder();
                AbsoluteLayout.SetLayoutFlags(m_ChatBorder, AbsoluteLayoutFlags.All);
                AbsoluteLayout.SetLayoutBounds(m_ChatBorder, new Rectangle(5, 0.225, 0.8, 0.5));

                m_ChatButton = generateChatButton();
                AbsoluteLayout.SetLayoutFlags(m_ChatButton, AbsoluteLayoutFlags.All);
                AbsoluteLayout.SetLayoutBounds(m_ChatButton, new Rectangle(1.125, 0.45, 0.2, 0.2));
            }

            (Content as AbsoluteLayout).Children.Add(m_ChatButton);
            (Content as AbsoluteLayout).Children.Add(m_ChatDialog);
            (Content as AbsoluteLayout).Children.Add(m_ChatBorder);
        }

        private Image generateChatBorder()
        {
            Image image = new Image
            {
                Source = "killcam_frame.png",
                Aspect = Aspect.Fill
            };

            return image;
        }

        private Grid generateChatDialog()
        {
            ScrollView chatBox = generateChatBox();
            Entry inputBox = generateChatInput();

            Grid chatDialog = new Grid()
            {
                BackgroundColor = Color.Black,
                VerticalOptions = LayoutOptions.FillAndExpand,

                RowDefinitions =
                {
                    new RowDefinition { Height = new GridLength(0.25, GridUnitType.Star) },
                    new RowDefinition { Height = new GridLength(0.05, GridUnitType.Star) }
                }
            };

            chatDialog.Children.Add(chatBox, 0, 0);
            chatDialog.Children.Add(inputBox, 0, 1);

            return chatDialog;
        }

        private Entry generateChatInput()
        {
            m_ChatInput = new Entry()
            {
                BackgroundColor = Color.White,
                TextColor = Color.Black
            };

            m_ChatInput.Completed += ChatInput_Completed;

            return m_ChatInput;
        }

        private ScrollView generateChatBox()
        {
            m_ChatBoxScrollView = new ScrollView();;

            m_ChatBoxScrollView.Content = m_ChatBoxText = new Label() { FontSize = 12 } ;
            m_ChatBoxScrollView.BackgroundColor = Color.Black;
            m_ChatBoxText.TextColor = Color.White;

            return m_ChatBoxScrollView;
        }

        private ImageButton generateChatButton()
        {
            ImageButton chatButton = new ImageButton()
            {
                Source = "chat_button.png",
                ClickAction = () => {
                    triggerChatBox();
                }
            };

            return chatButton;
        }
    }
}
