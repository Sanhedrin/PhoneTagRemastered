using PhoneTag.XamarinForms.Controls.MapControl;
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
using Plugin.Settings;

namespace PhoneTag.XamarinForms.Pages
{
    /// <summary>
    /// The game page, 
    /// </summary>
    public partial class GamePage : ChatEmbeddedContentPage
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

        private async Task setupPage()
        {
            m_Camera.PictureReady += GamePage_PictureReady;
            startGeoLocationListening();

            bool displayTip = CrossSettings.Current.GetValueOrDefault("IconTips", true);

            if (displayTip)
            {
                bool understood = await DisplayAlert("Player Icons",
                    "On the game map, you can see icons representing the players in the game, tap any of them to get more information about the specific player", "Don't show again", "Ok");

                CrossSettings.Current.AddOrUpdateValue("IconTips", !understood);
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
                    isReady = await CrossGeolocator.Current.StartListeningAsync(1, 1, true);
                }

                if (!isReady)
                {
                    Application.Current.MainPage = new ErrorPage("GPS signal not found, please enable GPS");
                }
            }

            CrossGeolocator.Current.PositionChanged += GPS_PositionChanged;

            startPlayersLocationsPolling();

            Plugin.Geolocator.Abstractions.Position pos = await CrossGeolocator.Current.GetPositionAsync(3);
            UserView.Current.UpdatePosition(UserView.Current.CurrentLocation = new GeoPoint(pos.Latitude, pos.Longitude));
        }

        //Continuously polls the server for updated user locations.
        private async Task startPlayersLocationsPolling()
        {
            Dictionary<string, LocationUpdateInfo> playersLocations;

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
                    //If the player is cheating and trying to close the GPS during the game, we'll
                    //Kill them and kick them out.
                    if (!CrossGeolocator.Current.IsGeolocationEnabled)
                    {
                        m_GameRoomView.UndisputableKill(UserView.Current.FBID);
                        Application.Current.MainPage = new ErrorPage($"Tsk tsk!{Environment.NewLine}Turning off your GPS is cheating, you know.");
                        m_GpsCancellationToken = null;
                        break;
                    }

                    playersLocations = await m_GameRoomView.GetPlayersLocations();

                    m_GameMap.UpdateUAV(playersLocations, m_GameRoomView);

                    await Task.Delay(m_GameRoomView.GameDetails.GpsRefreshRate * 1000);
                }
            }
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
            ShotDisplayDialog shotDisplayDialog = new ShotDisplayDialog(i_PictureData);
            shotDisplayDialog.ShotCancelled += (o, e) =>
            {
                OnBackButtonPressed();
            };

            await showDialogSlideUp(shotDisplayDialog);
        }

        public void ShotTargetChosen()
        {
            OnBackButtonPressed();
        }

        //Closes the shot display button on back button press.
        protected override bool OnBackButtonPressed()
        {
            //The hardware back button can only be used to back out of the shot display page, so if
            //such is available, we'll pop it then put everything back.
            IEnumerable<View> shotDisplayDialogs = m_CurrentlyShowingDialogs.Where(view => view is ShotDisplayDialog);
            if (m_CurrentlyShowingDialogs.Count > 0 && shotDisplayDialogs.Count() > 0)
            {
                buttonShoot.IsEnabled = true;

                //We'll need to remove all the dialogs on top of this one, and then put them back.
                Queue<View> removedViews = new Queue<View>();

                while(!(m_CurrentlyShowingDialogs.Peek() is ShotDisplayDialog))
                {
                    removedViews.Enqueue(m_CurrentlyShowingDialogs.Pop());
                }

                //We don't await this as the pop on the item happens before the first await, and we don't
                //need to maintain the state of the stack until the animation completes.
                hideDialogSlideDown();

                while (removedViews.Count > 0)
                {
                    m_CurrentlyShowingDialogs.Push(removedViews.Dequeue());
                }

                return false;
            }
            else
            {
                return base.OnBackButtonPressed();
            }
        }

        private void ShootButton_Clicked()
        {
            if (!m_GameOver)
            {
                buttonShoot.IsEnabled = false;

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
            else if(i_EventDetails is GameEndedEvent)
            {
                handleGameEndedEvent(i_EventDetails as GameEndedEvent);
            }
            else if (i_EventDetails is OutOfBoundsEvent)
            {
                handleOutOfBoundsEvent(i_EventDetails as OutOfBoundsEvent);
            }
            else
            {
                base.ParseEvent(i_EventDetails);
            }
        }

        //Triggers when a warning is issued for the player being out of bounds.
        private void handleOutOfBoundsEvent(OutOfBoundsEvent i_OutOfBoundsEvent)
        {
            if (UserView.Current.FBID.Equals(i_OutOfBoundsEvent))
            {
                NotificationDialog dialog = new NotificationDialog(i_OutOfBoundsEvent.Message);

                dialog.Opened += NotificationDialog_Timeout;
                dialog.Timeout += NotificationDialog_Timeout;

                showDialogSlideDown(dialog);
            }
        }

        //Triggers when a win condition occured for one of the teams, ending the game.
        private void handleGameEndedEvent(GameEndedEvent i_GameEndedEvent)
        {
            endGame(i_GameEndedEvent.WinnerIds);
        }

        private async Task endGame(List<String> i_WinnerIds)
        {
            await hideDialogSlideDown();

            await transitionToGameEnd(i_WinnerIds);
        }

        //Triggers when a dispute has been made over a kill and players should vote for the result.
        private void handleKillDisputeEvent(KillDisputeEvent i_KillDisputeEvent)
        {
            DisputeDialog disputeDialog = new DisputeDialog(i_KillDisputeEvent);

            disputeDialog.Opened += DisputeDialog_Opened;
            disputeDialog.Timeout += NotificationDialog_Timeout;

            showDialogSlideDown(disputeDialog);
        }

        //When the dispute dialog times out.
        private void NotificationDialog_Timeout(object sender, EventArgs e)
        {
            if (m_CurrentlyShowingDialogs.Peek() is NotificationDialog)
            {
                hideDialogSlideUp();
            }
        }

        //Shows the voting menu for the dispute.
        private void DisputeDialog_Opened(object sender, EventArgs e)
        {
            showDisputeDialog(e as KillDisputeEventArgs);
        }

        private async Task showDisputeDialog(KillDisputeEventArgs e)
        {
            UserView user = await UserView.GetUser(e.AttackedId);

            if (user != null)
            {
                KillRequestEvent killRequest = new KillRequestEvent(e.RoomId, e.AttackerName, e.AttackerId, e.KillCamId, e.AttackedId);

                KillConfirmationDialog killConfirmationDialog = new KillConfirmationDialog(e.DisputeId, killRequest,
                    String.Format("Dispute!{0}Was {1} captured successfully?", Environment.NewLine, user.Username));

                killConfirmationDialog.KillConfirmed += DisputeDialog_VoteKill;
                killConfirmationDialog.KillDenied += DisputeDialog_VoteSpare;

                openDisputeDialog(killConfirmationDialog);

                bool displayTip = CrossSettings.Current.GetValueOrDefault("KillDispute", true);

                if (displayTip)
                {
                    bool understood = await TrailableContentPage.CurrentPage.DisplayAlert("Kill Dispute",
                        $"{user.Username} claims they have been wrongfully marked as dead.{Environment.NewLine}This dialog will be shown to all players, letting them vote whether they think the shot image justifies killing {user.Username} or not.{Environment.NewLine}The majority of the votes decides what happens; If the majority voted yes, {user.Username} will be killed, but if the majority voted no the player taking the shot will be killed instead as penalty.", "Don't show again", "Ok");

                    CrossSettings.Current.AddOrUpdateValue("KillDispute", !understood);
                }
            }
        }

        private async Task openDisputeDialog(KillConfirmationDialog i_KillConfirmationDialog)
        {
            await hideDialogSlideUp();
            await showDialogSlideUp(i_KillConfirmationDialog);
        }

        //Votes to spare the player who disputed a kill.
        private void DisputeDialog_VoteSpare(object sender, KillDisputeEventArgs e)
        {
            disputeVote(e, false);
        }

        //Votes to kill a player who disputed a kill.
        private void DisputeDialog_VoteKill(object sender, KillDisputeEventArgs e)
        {
            disputeVote(e, true);
        }

        private async Task disputeVote(KillDisputeEventArgs i_KillDisputeDetails, bool i_VoteToKill)
        {
            hideDialogSlideDown();

            DisputeView dispute = await DisputeView.GetDispute(i_KillDisputeDetails.DisputeId);

            if (dispute != null)
            {
                await dispute.Vote(i_VoteToKill);
            }
        }

        //Triggers when a player dies.
        private void handlePlayerKilledEvent(PlayerKilledEvent i_PlayerKilledEvent)
        {
            displayNotification(i_PlayerKilledEvent);

            if (i_PlayerKilledEvent.PlayerFBID.Equals(UserView.Current.FBID) && !m_GameOver)
            {
                playerKilled();
            }
        }

        //Triggers when another player issues a kill command on you.
        private async Task handleKillRequestEvent(KillRequestEvent i_KillRequestEvent)
        {
            if (i_KillRequestEvent.AttackedPlayerId.Equals(UserView.Current.FBID))
            {
                //If this is the initial request, we show the dialog with no picture.
                if (String.IsNullOrEmpty(i_KillRequestEvent.KillCamId))
                {
                    KillConfirmationDialog killConfirmationDialog = new KillConfirmationDialog(i_KillRequestEvent,
                        String.Format("You have been attacked by {0}", i_KillRequestEvent.RequestedBy));

                    killConfirmationDialog.KillConfirmed += KillConfirmationDialog_KillConfirmed;
                    killConfirmationDialog.KillDenied += KillConfirmationDialog_KillDenied;

                    showDialogSlideUp(killConfirmationDialog);
                }
                //If we have the killcam, we attach it to the kill request.
                else
                {
                    KillConfirmationDialog kcDialog = null;

                    foreach (View view in m_CurrentlyShowingDialogs)
                    {
                        if (view is KillConfirmationDialog)
                        {
                            kcDialog = view as KillConfirmationDialog;
                            await kcDialog.AttachImage(i_KillRequestEvent.KillCamId);
                            finishShowDialogSlideUp(kcDialog);
                            break;
                        }
                    }
                }
            }
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
            await hideDialogSlideDown();

            await m_GameRoomView.DisputeKill(i_KillDisputeDetails);
        }

        //Kill the current player and remove them from the game.
        private async Task playerKilled()
        {
            await hideDialogSlideDown();

            await transitionToSpectatorMode();

            await UserView.Current.Die();
        }
    }
}