﻿using PhoneTag.XamarinForms.Controls.AnimatedImageControl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace PhoneTag.XamarinForms.Pages
{
    public partial class LoginPage : TrailableContentPage
    {
        private void initializeComponent()
        {
            Title = "Loading";
            Padding = new Thickness(0, 20, 0, 0);
            BackgroundColor = Color.Transparent;
            Content = new StackLayout
            {
                VerticalOptions = new LayoutOptions
                {
                    Alignment = LayoutAlignment.Center
                },
                Children = {
                    new AnimatedImage()
                    {
                        ImageName = "loading_logo",
                        Animate = true
                    },
                    new Label
                    {
                        Text = "Loading...",
                        TextColor = Color.White,
                        HorizontalTextAlignment = TextAlignment.Center
                    }
                }
            };
        }
    }
}
