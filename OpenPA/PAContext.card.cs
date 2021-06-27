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


        public CardInfo? GetCardInfo(uint index)
        {
            CardInfo? cardInfo = default;

            Monitor.Enter(this);

            MainLoop.Instance.Lock();

            pa_card_info* card_info = null;

            pa_operation* op = pa_context_get_card_info_by_index(pa_Context, index, &CardInfoCallback, &card_info);

            while (pa_operation_get_state(op) == Enums.OperationState.RUNNING && card_info == null)
                MainLoop.Instance.Wait();

            if (card_info != null)
            {
                cardInfo = CardInfo.Convert(*card_info);

                MainLoop.Instance.Accept();

                MainLoop.Instance.Wait();
            }

            pa_operation_unref(op);

            MainLoop.Instance.Unlock();

            Monitor.Exit(this);

            return cardInfo;
        }

        public CardInfo? GetCardInfo(string card)
        {
            CardInfo? cardInfo = default;

            Monitor.Enter(this);

            IntPtr cardName = Marshal.StringToHGlobalAnsi(card);

            MainLoop.Instance.Lock();

            pa_card_info* card_info = null;

            pa_operation* op = pa_context_get_card_info_by_name(pa_Context, cardName, &CardInfoCallback, &card_info);

            while (pa_operation_get_state(op) == Enums.OperationState.RUNNING && card_info == null)
                MainLoop.Instance.Wait();

            if (card_info != null)
            {
                cardInfo = CardInfo.Convert(*card_info);

                MainLoop.Instance.Accept();

                MainLoop.Instance.Wait();
            }

            pa_operation_unref(op);

            MainLoop.Instance.Unlock();

            Marshal.FreeHGlobal(cardName);

            Monitor.Exit(this);

            return cardInfo;
        }

        public IReadOnlyList<CardInfo> GetCardInfoList()
        {
            List<CardInfo> cards = new();

            Monitor.Enter(this);

            MainLoop.Instance.Lock();

            pa_card_info* card_info = null;
           
            pa_operation* op = pa_context_get_card_info_list(pa_Context, &CardInfoCallback, &card_info);

            while (true)
            {
                while (pa_operation_get_state(op) == Enums.OperationState.RUNNING && card_info == null)
                    MainLoop.Instance.Wait();

                if (card_info != null)
                {
                    cards.Add(CardInfo.Convert(*card_info));

                    MainLoop.Instance.Accept();                    

                    card_info = null;
                }
                else
                {
                    break;
                }
                
            }

            pa_operation_unref(op);

            MainLoop.Instance.Unlock();

            Monitor.Exit(this);

            return cards;
        }

        public bool SetCardProfile(uint index, string profile)
        {
            bool success = false;

            Monitor.Enter(this);

            IntPtr profileName = Marshal.StringToHGlobalAnsi(profile);

            MainLoop.Instance.Lock();

            pa_operation* op = pa_context_set_card_profile_by_index(pa_Context, index, profileName, &SuccessCallback, &success);

            while (pa_operation_get_state(op) == Enums.OperationState.RUNNING)
                MainLoop.Instance.Wait();

            pa_operation_unref(op);

            MainLoop.Instance.Unlock();

            Marshal.FreeHGlobal(profileName);

            Monitor.Exit(this);

            return success;
        }

        public bool SetCardProfile(string card, string profile)
        {
            bool success = false;

            Monitor.Enter(this);

            IntPtr cardName = Marshal.StringToHGlobalAnsi(card);
            IntPtr profileName = Marshal.StringToHGlobalAnsi(profile);

            MainLoop.Instance.Lock();

            pa_operation* op = pa_context_set_card_profile_by_name(pa_Context, cardName, profileName, &SuccessCallback, &success);

            while (pa_operation_get_state(op) == Enums.OperationState.RUNNING)
                MainLoop.Instance.Wait();

            pa_operation_unref(op);

            MainLoop.Instance.Unlock();

            Marshal.FreeHGlobal(cardName);
            Marshal.FreeHGlobal(profileName);

            Monitor.Exit(this);

            return success;
        }

        public bool SetPortLatencyOffset(string card, string port, long offset)
        {
            bool success = false;

            Monitor.Enter(this);

            IntPtr cardName = Marshal.StringToHGlobalAnsi(card);
            IntPtr portName = Marshal.StringToHGlobalAnsi(port);

            MainLoop.Instance.Lock();

            pa_operation* op = pa_context_set_port_latency_offset(pa_Context, cardName, portName, offset, &SuccessCallback, &success);

            while (pa_operation_get_state(op) == Enums.OperationState.RUNNING)
                MainLoop.Instance.Wait();

            pa_operation_unref(op);

            MainLoop.Instance.Unlock();

            Marshal.FreeHGlobal(cardName);
            Marshal.FreeHGlobal(portName);

            Monitor.Exit(this);

            return success;
        }

    }
}
