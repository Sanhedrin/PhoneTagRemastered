using PhoneTag.XamarinForms.Controls.MenuButtons;
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
        private Label m_ChatBox;
        private Entry m_ChatInput;

        protected void initializeChat()
        {
            m_ChatDialog = generateChatDialog();
            AbsoluteLayout.SetLayoutFlags(m_ChatDialog, AbsoluteLayoutFlags.All);
            AbsoluteLayout.SetLayoutBounds(m_ChatDialog, new Rectangle(-4, 0.5, 0.8, 0.8));

            m_ChatButton = generateChatButton();
            AbsoluteLayout.SetLayoutFlags(m_ChatButton, AbsoluteLayoutFlags.All);
            AbsoluteLayout.SetLayoutBounds(m_ChatButton, new Rectangle(-0.1, 0.5, 0.2, 0.2));
            (Content as AbsoluteLayout).Children.Add(m_ChatDialog);
            (Content as AbsoluteLayout).Children.Add(m_ChatButton);
        }

        private StackLayout generateChatDialog()
        {
            StackLayout chatDialog = new StackLayout()
            {
                BackgroundColor = Color.Black,
                VerticalOptions = LayoutOptions.FillAndExpand
            };

            ScrollView chatBox = generateChatBox();
            Entry inputBox = generateChatInput();

            chatDialog.Children.Add(chatBox);
            chatDialog.Children.Add(inputBox);

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
            ScrollView chatBox = new ScrollView();;

            chatBox.Content = m_ChatBox = new Label();

            return chatBox;
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
