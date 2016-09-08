﻿using PhoneTag.SharedCodebase.Views;
using PhoneTag.XamarinForms.Pages;
using Plugin.XamJam.Screen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace PhoneTag.XamarinForms.Controls.SocialMenu.PlayerDetailTiles
{
    public class ShotSuggestionDetailsTile : PlayerDetailsTile
    {
        public ShotSuggestionDetailsTile(UserView i_UserView) : base(i_UserView)
        {
            setupTile();
        }

        protected override void setupTile()
        {
            Grid detailsGrid = generateDetailGrid();

            Children.Add(detailsGrid,
                Constraint.RelativeToParent((parent) => { return parent.Width / 8; }),
                Constraint.RelativeToParent((parent) => { return parent.Height / 5; }));
            
            initializeComponent();
        }

        private Grid generateDetailGrid()
        {
            Grid layout = new Grid()
            {
                ColumnSpacing = 10,
                BackgroundColor = Color.Transparent,
                ColumnDefinitions =
                {
                    new ColumnDefinition { Width = new GridLength(0.1, GridUnitType.Star) },
                    new ColumnDefinition { Width = new GridLength(0.1, GridUnitType.Star) },
                    new ColumnDefinition { Width = new GridLength(0.25, GridUnitType.Star) }
                }
            };

            View profilePic = generateProfilePicture();
            View nameLabel = generateUserNameLabel();
            Button chooseTargetButton = generateChooseTargetButton();
            
            layout.Children.Add(profilePic, 0, 0);
            layout.Children.Add(nameLabel, 1, 0);
            layout.Children.Add(chooseTargetButton, 2, 0);

            return layout;
        }

        private StackLayout generateActionStack()
        {
            StackLayout layout = new StackLayout();

            layout.Orientation = StackOrientation.Horizontal;
            layout.HorizontalOptions = new LayoutOptions() { Alignment = LayoutAlignment.End };

            Button chooseTargetButton = generateChooseTargetButton();

            layout.Children.Add(chooseTargetButton);

            return layout;
        }

        private Button generateChooseTargetButton()
        {
            Button chooseTargetButton = new Button();

            chooseTargetButton.Text = "Choose target";
            chooseTargetButton.BackgroundColor = Color.Silver;
            chooseTargetButton.TextColor = Color.Black;
            chooseTargetButton.Command = new Command(() => 
            {
                UserView.Current?.TryKill(UserView.FBID, ShotDisplayDialog.LastKillCam);

                (TrailableContentPage.CurrentPage as GamePage).ShotTargetChosen();
            });

            return chooseTargetButton;
        }

        //Should never be called, there shouldn't be any refresh worthy information in the shot suggestions.
        public override void Refresh(PlayerDetailsTile i_PlayerDetails)
        {
            throw new NotImplementedException();
        }
    }
}
