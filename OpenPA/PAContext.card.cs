using OpenPA.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static OpenPA.Native.pa_operation;
using static OpenPA.Native.pa_context;

namespace OpenPA
{
    public unsafe partial class PAContext
    {

        #region Callbacks
        [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) })]
        static void CardInfoCallback(pa_context* ctx, pa_card_info* info, int eol, void* userdata)
        { 
            if (eol == 0)
            {
                *((pa_card_info**)userdata) = info;

                MainLoop.Instance.Signal(1);
            }
            else
            {
                *((pa_card_info**)userdata) = null;

                MainLoop.Instance.Signal(0);
            }
        }
        #endregion

        /// <summary>
        /// Get information about a card by its index
        /// </summary>
        /// <param name="index">Index of card</param>
        /// <returns>Card information</returns>
        public Task<CardInfo?> GetCardInfoAsync(uint index) => Task.Run(() => GetCardInfo(index));

        /// <summary>
        /// Get information about a card by its index
        /// </summary>
        /// <param name="index">Index of card</param>
        /// <returns>Card information</returns>
        public CardInfo? GetCardInfo(uint index)
        {
            // Return variable
            CardInfo? cardInfo = default;

            // Hold a lock to remain thread-safe
            Monitor.Enter(this);

            // Lock the mainloop
            MainLoop.Instance.Lock();

            // Setup a variable for the return value
            pa_card_info* card_info = null;

            // Invoke the operation
            pa_operation* op = pa_context_get_card_info_by_index(pa_Context, index, &CardInfoCallback, &card_info);

            // Wait for the operation to complete
            while (pa_operation_get_state(op) == Enums.OperationState.RUNNING && card_info == null)
                MainLoop.Instance.Wait();

            // If the call returned something
            if (card_info != null)
            {
                // Get a managed object of the pa_card_info structure
                cardInfo = CardInfo.Convert(*card_info);

                // Allow pulse audio to free the structure
                MainLoop.Instance.Accept();
                
                // Signal the mainloop
                MainLoop.Instance.Wait();
            }

            // Dereference the pa_operation structure
            pa_operation_unref(op);

            // Unlock the mainloop
            MainLoop.Instance.Unlock();

            // Release the lock
            Monitor.Exit(this);

            // Return the result
            return cardInfo;
        }

        /// <summary>
        /// Get information about a card by its name
        /// </summary>
        /// <param name="card">Name of the card</param>
        /// <returns>Card information</returns>
        public Task<CardInfo?> GetCardInfoAsync(string card) => Task.Run(() => GetCardInfo(card));

        /// <summary>
        /// Get information about a card by its name
        /// </summary>
        /// <param name="card">Name of the card</param>
        /// <returns>Card information</returns>
        public CardInfo? GetCardInfo(string card)
        {
            // Return variable
            CardInfo? cardInfo = default;

            // Hold a lock to remain thread-safe
            Monitor.Enter(this);

            // Copy the card's name to unmanaged memory
            IntPtr cardName = Marshal.StringToHGlobalAnsi(card);

            // Lock the mainloop
            MainLoop.Instance.Lock();

            // Setup a return variable
            pa_card_info* card_info = null;

            // Invoke the operation
            pa_operation* op = pa_context_get_card_info_by_name(pa_Context, cardName, &CardInfoCallback, &card_info);

            // Wait for the operation to complete
            while (pa_operation_get_state(op) == Enums.OperationState.RUNNING && card_info == null)
                MainLoop.Instance.Wait();

            // If the call returned something
            if (card_info != null)
            {
                // Get a managed object of the pa_card_info structure
                cardInfo = CardInfo.Convert(*card_info);

                // Allow pulse audio to free the structure
                MainLoop.Instance.Accept();

                // Signal the mainloop
                MainLoop.Instance.Wait();
            }

            // Dereference the pa_operation structure
            pa_operation_unref(op);

            // Unlock the mainloop
            MainLoop.Instance.Unlock();

            // Free the unmanaged memory buffer
            Marshal.FreeHGlobal(cardName);

            // Release the lock
            Monitor.Exit(this);

            // Return the result
            return cardInfo;
        }

        /// <summary>
        /// Get the complete card list
        /// </summary>
        /// <returns>List of cards</returns>
        public Task<IReadOnlyList<CardInfo>> GetCardInfoListAsync() => Task.Run(GetCardInfoList);

        /// <summary>
        /// Get the complete card list
        /// </summary>
        /// <returns>List of cards</returns>
        public IReadOnlyList<CardInfo> GetCardInfoList()
        {
            // Create the list
            List<CardInfo> cards = new();

            // Hold a lock to remain thread-safe
            Monitor.Enter(this);

            // Lock the mainloop
            MainLoop.Instance.Lock();

            // Setup a return buffer
            pa_card_info* card_info = null;
           
            // Invoke the operation
            pa_operation* op = pa_context_get_card_info_list(pa_Context, &CardInfoCallback, &card_info);

            // Keep looping
            while (true)
            {
                // Wait for the operation to signal
                while (pa_operation_get_state(op) == Enums.OperationState.RUNNING && card_info == null)
                    MainLoop.Instance.Wait();

                // If the operation returned something
                if (card_info != null)
                {
                    // Convert the structure to an object and add it to the list
                    cards.Add(CardInfo.Convert(*card_info));

                    // Allow pulseaudio to free the pa_card_info structure
                    MainLoop.Instance.Accept();                    

                    // Set the pointer to null
                    card_info = null;
                }
                else
                {
                    // Break loop on end of list or error
                    break;
                }
                
            }

            // Dereference the pa_operation structure
            pa_operation_unref(op);

            // Unlock the mainloop
            MainLoop.Instance.Unlock();

            // Release the lock
            Monitor.Exit(this);

            // Return the list
            return cards;
        }

        /// <summary>
        /// Sets the card to a profile
        /// </summary>
        /// <param name="index">Index of card</param>
        /// <param name="profile">Name of profile</param>
        /// <returns>Success</returns>
        public Task<bool> SetCardProfileAsync(uint index, string profile) => Task.Run(() => SetCardProfile(index, profile));

        /// <summary>
        /// Sets the card to a profile
        /// </summary>
        /// <param name="index">Index of card</param>
        /// <param name="profile">Name of profile</param>
        /// <returns>Success</returns>
        public bool SetCardProfile(uint index, string profile)
        {
            // Setup return variable
            bool success = false;

            // Hold a lock to remain thread-safe
            Monitor.Enter(this);

            // Copy the profile name to unmanaged memory
            IntPtr profileName = Marshal.StringToHGlobalAnsi(profile);

            // Lock the mainloop
            MainLoop.Instance.Lock();

            // Invoke the operation
            pa_operation* op = pa_context_set_card_profile_by_index(pa_Context, index, profileName, &SuccessCallback, &success);

            // Wait for the operation to complete
            while (pa_operation_get_state(op) == Enums.OperationState.RUNNING)
                MainLoop.Instance.Wait();

            // Dereference the pa_operation structure
            pa_operation_unref(op);

            // Unlock the mainloop
            MainLoop.Instance.Unlock();

            // Free the unmanaged buffer
            Marshal.FreeHGlobal(profileName);

            // Release the lock
            Monitor.Exit(this);

            // Return the result
            return success;
        }

        /// <summary>
        /// Sets the card to a profile
        /// </summary>
        /// <param name="card">Card name</param>
        /// <param name="profile">Profile name</param>
        /// <returns>Success</returns>
        public Task<bool> SetCardProfileAsync(string card, string profile) => Task.Run(() => SetCardProfile(card, profile));

        /// <summary>
        /// Sets the card to a profile
        /// </summary>
        /// <param name="card">Card name</param>
        /// <param name="profile">Profile name</param>
        /// <returns>Success</returns>
        public bool SetCardProfile(string card, string profile)
        {
            // Setup return variable
            bool success = false;

            // Hold a lock to remain thread-safe
            Monitor.Enter(this);

            // Copy the card name to unmanaged memory
            IntPtr cardName = Marshal.StringToHGlobalAnsi(card);

            // Copy the profile name to unmanaged memory
            IntPtr profileName = Marshal.StringToHGlobalAnsi(profile);

            // Lock the mainloop
            MainLoop.Instance.Lock();

            // Invoke the operation
            pa_operation* op = pa_context_set_card_profile_by_name(pa_Context, cardName, profileName, &SuccessCallback, &success);

            // Wait for the operation to complete
            while (pa_operation_get_state(op) == Enums.OperationState.RUNNING)
                MainLoop.Instance.Wait();

            // Dereference the pa_operation structure
            pa_operation_unref(op);

            // Unlock the mainloop
            MainLoop.Instance.Unlock();

            // Free the unmanaged memory buffers
            Marshal.FreeHGlobal(cardName);
            Marshal.FreeHGlobal(profileName);

            // Release the lock
            Monitor.Exit(this);

            // Return the result
            return success;
        }

        /// <summary>
        /// Set the latency offset of a specific port on a card
        /// </summary>
        /// <param name="card">Card name</param>
        /// <param name="port">Port name</param>
        /// <param name="offset">Latency offset</param>
        /// <returns>Success</returns>
        public Task<bool> SetPortLatencyOffsetAsync(string card, string port, long offset) => Task.Run(() => SetPortLatencyOffset(card, port, offset));

        /// <summary>
        /// Set the latency offset of a specific port on a card
        /// </summary>
        /// <param name="card">Card name</param>
        /// <param name="port">Port name</param>
        /// <param name="offset">Latency offset</param>
        /// <returns>Success</returns>
        public bool SetPortLatencyOffset(string card, string port, long offset)
        {
            // Setup return variable
            bool success = false;

            // Hold a lock to remain thread-safe
            Monitor.Enter(this);

            // Copy the card name to unmanaged memory
            IntPtr cardName = Marshal.StringToHGlobalAnsi(card);

            // Copy the port name to unmanaged memory
            IntPtr portName = Marshal.StringToHGlobalAnsi(port);

            // Lock the mainloop
            MainLoop.Instance.Lock();

            // Invoke the operation
            pa_operation* op = pa_context_set_port_latency_offset(pa_Context, cardName, portName, offset, &SuccessCallback, &success);

            // Wait for the operation to complete
            while (pa_operation_get_state(op) == Enums.OperationState.RUNNING)
                MainLoop.Instance.Wait();

            // Dereference the pa_operation structure
            pa_operation_unref(op);

            // Unlock the mainloop
            MainLoop.Instance.Unlock();

            // Free the unmanaged memory buffers
            Marshal.FreeHGlobal(cardName);
            Marshal.FreeHGlobal(portName);

            // Release the lock
            Monitor.Exit(this);

            // Return the result
            return success;
        }

    }
}
