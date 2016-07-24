﻿using PhoneTag.XamarinForms.Controls.MapControl;
using PhoneTag.XamarinForms.Controls.CameraControl;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Maps;
using PhoneTag.SharedCodebase.Views;
using PhoneTag.SharedCodebase.Events.GameEvents;
using Plugin.Geolocator;
using PhoneTag.XamarinForms.Controls.KillDisputeResolver;
using PositionEventArgs = Plugin.Geolocator.Abstractions.PositionEventArgs;
using PhoneTag.SharedCodebase.Utils;
using PhoneTag.SharedCodebase.POCOs;
using System.Threading;

namespace PhoneTag.XamarinForms.Pages
{
    /// <summary>
    /// The game page, 
    /// </summary>
    public partial class GamePage : TrailableContentPage
    {
        private const double k_DefaultGameRadius = 0.5;
        private const double k_DefaultGameZoom = 1;
        private const bool k_IsSetUpView = false;
        private int k_InitialGpsDelay = 3000;

        private GameMapInteractive m_GameMap;
        private CameraPreview m_Camera;
        private GameRoomView m_GameRoomView;
        private CancellationTokenSource m_GpsCancellationToken;
        
        public GamePage(GameRoomView i_GameRoomView) : base()
        {
            m_GameRoomView = i_GameRoomView;
            Position startLocation = new Position(m_GameRoomView.GameDetails.StartLocation.Latitude, m_GameRoomView.GameDetails.StartLocation.Longitude);
            m_GameMap = new GameMapInteractive(startLocation, m_GameRoomView.GameDetails.GameRadius, m_GameRoomView.GameDetails.GameRadius * 2);

            initializeComponent();

            setupPage();
        }

        private void setupPage()
        {
            m_Camera.PictureReady += GamePage_PictureReady;
            startPlayersLocationsPolling();
            startGeoLocationListening();
        }

        private async Task startPlayersLocationsPolling()
        {
            Dictionary<string, GeoPoint> playersLocations;
            if (m_GpsCancellationToken == null)
            {
                m_GpsCancellationToken = new CancellationTokenSource();

                await Task.Delay(k_InitialGpsDelay);

                for (;;)
                {
                    if (m_GpsCancellationToken.IsCancellationRequested)
                    {
                        m_GpsCancellationToken = null;
                        break;
                    }

                    playersLocations = await m_GameRoomView.GetPlayersLocations();
                    
                    await Task.Delay(m_GameRoomView.GameDetails.GpsRefreshRate * 1000);
                }
            }
        }

        //Starts listening to the geolocator, while looking for errors.
        private async Task startGeoLocationListening()
        {
            if (!CrossGeolocator.Current.IsListening)
            {
                bool isReady = false;

                if (CrossGeolocator.Current.IsGeolocationAvailable && CrossGeolocator.Current.IsGeolocationEnabled)
                {
                    isReady = await CrossGeolocator.Current.StartListeningAsync(1, 1);
                }

                if (!isReady)
                {
                    Application.Current.MainPage = new ErrorPage("GPS signal not found, please enable GPS");
                }
            }

            CrossGeolocator.Current.PositionChanged += GPS_PositionChanged;
        }

        //When our position changes, we should inform the server about it.
        private void GPS_PositionChanged(object sender, PositionEventArgs e)
        {
            GeoPoint position = new GeoPoint(e.Position.Latitude, e.Position.Longitude);
            UserView.Current.UpdatePosition(position);
        }

        private void GamePage_PictureReady(object sender, PictureReadyEventArgs e)
        {
            pictureReady(e.PictureBuffer);
        }

        private async Task pictureReady(byte[] i_PictureData)
        {
            ShotDisplayPage shotDisplayPage = new ShotDisplayPage(i_PictureData);
            shotDisplayPage.ShotCancelled += (o, e) =>
            {
                buttonShoot.IsEnabled = true;
                buttonShoot.Text = "Shoot!";
            };

            await Navigation.PushAsync(shotDisplayPage);
        }

        private void ShootButton_Clicked(object sender, EventArgs e)
        {
            if (buttonShoot.Text.Equals("Shoot!"))
            {
                buttonShoot.IsEnabled = false;
                buttonShoot.Text = "Processing shot...";

                m_Camera.TakePicture();
            }
            else
            {
                Application.Current.MainPage = new NavigationPage(new MainMenuPage());
                UserView.Current.LeaveGame();
            }
        }

        public override void ParseEvent(Event i_EventDetails)
        {
            if(i_EventDetails is KillRequestEvent)
            {
                handleKillRequestEvent(i_EventDetails as KillRequestEvent);
            }
            else if(i_EventDetails is PlayerKilledEvent)
            {
                handlePlayerKilledEvent(i_EventDetails as PlayerKilledEvent);
            }
            else if(i_EventDetails is KillDisputeEvent)
            {
                handleKillDisputeEvent(i_EventDetails as KillDisputeEvent);
            }
        }

        //Triggers when a dispute has been made over a kill and players should vote for the result.
        private void handleKillDisputeEvent(KillDisputeEvent i_KillDisputeEvent)
        {
            DisputeDialog disputeDialog = new DisputeDialog(i_KillDisputeEvent);

            disputeDialog.Opened += DisputeDialog_Opened;
            disputeDialog.Timeout += DisputeDialog_Timeout;

            showDialog(disputeDialog);
        }

        //When the dispute dialog times out.
        private void DisputeDialog_Timeout(object sender, EventArgs e)
        {
            hideDialog();
        }

        //Shows the voting menu for the dispute.
        private void DisputeDialog_Opened(object sender, KillDisputeEventArgs e)
        {
            KillRequestEvent killRequest = new KillRequestEvent(e.RoomId, e.AttackerFBID, e.KillCamId);
            KillConfirmationDialog killConfirmationDialog = new KillConfirmationDialog(e.DisputeId, killRequest);

            killConfirmationDialog.KillConfirmed += DisputeDialog_VoteKill;
            killConfirmationDialog.KillDenied += DisputeDialog_VoteSpare;
            
            openDisputeDialog(killConfirmationDialog);
        }

        private async Task openDisputeDialog(KillConfirmationDialog i_KillConfirmationDialog)
        {
            await hideDialog();
            await showDialog(i_KillConfirmationDialog);
        }

        //Votes to spare the player who disputed a kill.
        private void DisputeDialog_VoteSpare(object sender, KillDisputeEventArgs e)
        {
            disputeVote(e, false);
        }

        //Votes to kill a player who disputed a kill.
        private void DisputeDialog_VoteKill(object sender, KillDisputeEventArgs e)
        {
            disputeVote(e, false);
        }

        private async Task disputeVote(KillDisputeEventArgs i_KillDisputeDetails, bool i_VoteToKill)
        {
            DisputeView dispute = await DisputeView.GetDispute(i_KillDisputeDetails.DisputeId);

            await dispute.Vote(i_VoteToKill);
        }

        //Triggers when a player dies.
        private void handlePlayerKilledEvent(PlayerKilledEvent i_PlayerKilledEvent)
        {
            displayNotification(i_PlayerKilledEvent);
        }

        //Triggers when another player issues a kill command on you.
        private void handleKillRequestEvent(KillRequestEvent i_KillRequestEvent)
        {
            KillConfirmationDialog killConfirmationDialog = new KillConfirmationDialog(i_KillRequestEvent);

            killConfirmationDialog.KillConfirmed += KillConfirmationDialog_KillConfirmed;
            killConfirmationDialog.KillDenied += KillConfirmationDialog_KillDenied;

            showDialog(killConfirmationDialog);
        }
        
        private void KillConfirmationDialog_KillDenied(object sender, KillDisputeEventArgs e)
        {
            sendDispute(e);
        }

        private void KillConfirmationDialog_KillConfirmed(object sender, EventArgs e)
        {
            playerKilled();
        }

        //Sends a dispute for this kill.
        //What this means is that all players receive a notification that lets them vote on the matter.
        //The disqualification is determined by the result of the votes.
        private async Task sendDispute(KillDisputeEventArgs i_KillDisputeDetails)
        {
            await hideDialog();

            await m_GameRoomView.DisputeKill(i_KillDisputeDetails);
        }

        //Kill the current player and remove them from the game.
        private async Task playerKilled()
        {
            await hideDialog();

            await transitionToSpectatorMode();

            await UserView.Current.Die();
        }
    }
}