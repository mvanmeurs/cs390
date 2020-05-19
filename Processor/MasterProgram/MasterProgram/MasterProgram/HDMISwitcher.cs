// HDMI Switcher.cs implements a Generic HDMI Switcher for Touchpanel Control
// Written by Jguertin82018 on github
// https://github.com/Jguertin82018/SimplSharp---HD-MD4x1-4K-1E
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Crestron.SimplSharp;
using Crestron.SimplSharpPro;
using Crestron.SimplSharpPro.DM;
using Crestron.SimplSharpPro.DM.Endpoints.Transmitters;

namespace DMPS3_200Test
{
    public class HDMISwitcher
    {
        // Fields
        private HdMd4x14kE switcher;
        private string[] InputName = new string[] {"HDMI1", "HDMI2","HDMI3","HDMI4"};
        //Properties
        public bool isLocked { get { return switcher.FrontPanelLockOnFeedback.BoolValue; } }




        //Constructors
        public HDMISwitcher(uint ipId, string ipAddress, CrestronControlSystem device)
        {
            try
            {
                switcher = new HdMd4x14kE(ipId, ipAddress, device);
                //                 ^replace this line with the model of your switcher
                switcher.OnlineStatusChange += new OnlineStatusChangeEventHandler(switcher_OnlineStatusChange);
                switcher.DMSystemChange += new DMSystemEventHandler(switcher_DMSystemChange);
                switcher.DMInputChange += new DMInputEventHandler(switcher_DMInputChange);
                switcher.DMOutputChange += new DMOutputEventHandler(switcher_DMOutputChange);
                switcher.AutoModeOff();
                //switcher.VideoEnter.BoolValue = true;
                switcher.FrontPanelLockOn();
                switcher.Register();
                for (uint i = 1; i <= 4; i++)
                {
                    switcher.HdmiInputs[i].Name.StringValue = InputName[i - 1];
                }
                for (uint i = 1; i <= 4; i++)
                {
                    CrestronConsole.PrintLine("Switcher Input:{0} and the name is {1}", i, switcher.HdmiInputs[i].Name.StringValue); 
                }
                CrestronConsole.PrintLine("4x1 Constructor Switcher Registration status is: " + switcher.Registered);
            }
            catch (Exception e)
            {
                CrestronConsole.PrintLine("Error is the Switcher COnstructor is: " + e);
            }

        }

        // Delegates and events
        public delegate void SwitcherOnlineStatusChangeEvent(bool status);
        public event SwitcherOnlineStatusChangeEvent SwitcherOnlineEvent;

        public delegate void SwitcherFrontPanelLockChangeEvent(bool status);
        public event SwitcherFrontPanelLockChangeEvent SwitcherLockPanelEvent;

        public delegate void SwitcherOutputChangeEventHandler(uint source,string name);
        public event SwitcherOutputChangeEventHandler SwitcherOutputChangeEvent;

        public delegate void SwitcherInputChangeEventHandler(uint input, bool status);
        public event SwitcherInputChangeEventHandler SwitcherInputChangeEvent;
        void switcher_DMOutputChange(Switch device, DMOutputEventArgs args)
        {

            try
            {
                switch (args.EventId)
                {

                    case DMOutputEventIds.VideoOutEventId:
                        if (switcher.HdmiOutputs[1].VideoOutFeedback != null)
                        {
                            var input = Convert.ToUInt16(switcher.HdmiOutputs[1].VideoOutFeedback.Number);
                            SwitcherOutputChangeEvent(Convert.ToUInt16(switcher.HdmiOutputs[1].VideoOutFeedback.Number), switcher.HdmiInputs[input].Name.StringValue);
                        }
                        else
                            SwitcherOutputChangeEvent(0, "Nothing");
                        break;
                }
            }
            catch (Exception e)
            {
                CrestronConsole.PrintLine("Error in Switcher Output Chane is: " + e);
            }

        }

        void switcher_DMInputChange(Switch device, DMInputEventArgs args)
        {
            switch (args.EventId)
            {
                case DMInputEventIds.SourceSyncEventId:
                    SwitcherInputChangeEvent(args.Number, switcher.HdmiInputs[args.Number].VideoDetectedFeedback.BoolValue);
                    break;
            }
        }

        // Get Lock Status
        void switcher_DMSystemChange(Switch device, DMSystemEventArgs args)
        {
            switch (args.EventId)
            {
                case DMSystemEventIds.FrontPanelLockOnEventId:
                    SwitcherLockPanelEvent(isLocked);
                    break;
                case DMSystemEventIds.FrontPanelLockOffEventId:
                    SwitcherLockPanelEvent(isLocked);
                    break;
            }
        }
        void switcher_OnlineStatusChange(GenericBase currentDevice, OnlineOfflineEventArgs args)
        {
            SwitcherOnlineEvent(args.DeviceOnLine);
        }

        //Methods
        public void RouteVideo(uint video)
        {
            if (video > 0)
                switcher.HdmiOutputs[1].VideoOut = switcher.HdmiInputs[video];
            else
                switcher.HdmiOutputs[1].VideoOut = null;

        }

        public void LockPanel(bool status)
        {
            if (status)
                switcher.FrontPanelLockOn();
            else
                switcher.FrontPanelLockOff();
        }
    }
}

