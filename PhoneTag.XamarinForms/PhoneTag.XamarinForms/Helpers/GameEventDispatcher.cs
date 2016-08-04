using PhoneTag.SharedCodebase.Events;
using PhoneTag.SharedCodebase.Events.GameEvents;
using PhoneTag.SharedCodebase.Views;
using PhoneTag.XamarinForms.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace PhoneTag.XamarinForms.Helpers
{
    /// <summary>
    /// A parser for the event push messages arriving from the server.
    /// </summary>
    public static class GameEventDispatcher
    {
        private static GameRoomView m_TrackedRoom = null;

        /// <summary>
        /// Starts listening for events on the given room.
        /// Can only listen to one room at a time, so starting listening on another room will stop listening
        /// on the current one.
        /// </summary>
        /// <param name="i_Room">Room to listen to events on.</param>
        public static void ListenToEventsOn(GameRoomView i_Room)
        {
            if (m_TrackedRoom != null)
            {
                m_TrackedRoom.CancelEventPolling();
                m_TrackedRoom.EventArrived -= GameRoom_EventArrived;
            }

            i_Room.EventArrived += GameRoom_EventArrived;
            i_Room.PollGameEvents();
            m_TrackedRoom = i_Room;
        }

        //When a new event arrives, we'll send it to the dispatcher.
        private static void GameRoom_EventArrived(object sender, GameEventArrivedArgs e)
        {
            parse(e.Event);
        }

        /// <summary>
        /// Takes the event acquired from the server, parses it and acts accordingly.
        /// Can be called either by events obtained from polling the room or through push notifications.
        /// </summary>
        private static void parse(Event i_Event)
        {
            Device.BeginInvokeOnMainThread(() => TrailableContentPage.CurrentPage.ParseEvent(i_Event));
        }
    }
}
