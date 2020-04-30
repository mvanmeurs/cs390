using System;
using Crestron.SimplSharp;                          	// For Basic SIMPL# Classes
using Crestron.SimplSharpPro;                       	// For Basic SIMPL#Pro classes
using Crestron.SimplSharpPro.CrestronThread;        	// For Threading
using Crestron.SimplSharpPro.Diagnostics;		    	// For System Monitor Access
using Crestron.SimplSharpPro.DeviceSupport;         	// For Generic Device Support
using Crestron.SimplSharpPro.UI;
using Crestron.SimplSharpPro.CrestronConnected;
using Crestron.SimplSharpPro.DM.Endpoints.Receivers;
using Crestron.SimplSharpPro.Lighting;
using Crestron.SimplSharpPro.DM;


namespace HelloWorldTutorial
{

    public class ControlSystem : CrestronControlSystem
    {
        private Tsw760 userInterface;
        private ComPort comport;
        private Glpp1DimFlv3CnPm lighting;
        private HdMd8x1 hdmi_switcher;

        public const string PROJECTOR_DELIMITER = "\x0D";
        public const string PROJECTOR_VGA1 = "SOURCE 10";
        public const string PROJECTOR_VGA2 = "SOURCE 20";
        public const string PROJECTOR_HDMI = "SOURCE 30";
        public const string PROJECTOR_SVIDEO = "SOURCE 40";
        public const string PROJECTOR_POWER_ON = "PWR ON";
        public const string PROJECTOR_POWER_OFF = "PWR OFF";

        /// <summary>
        /// ControlSystem Constructor. Starting point for the SIMPL#Pro program.
        /// Use the constructor to:
        /// * Initialize the maximum number of threads (max = 400)
        /// * Register devices
        /// * Register event handlers
        /// * Add Console Commands
        /// 
        /// Please be aware that the constructor needs to exit quickly; if it doesn't
        /// exit in time, the SIMPL#Pro program will exit.
        /// 
        /// You cannot send / receive data in the constructor
        /// </summary>
        public ControlSystem()
            : base()


        {
            try
            {
                Thread.MaxNumberOfUserThreads = 20;

                userInterface = new Tsw760(03, this);
                userInterface.SigChange += new SigEventHandler(userInterface_Sigchange);
                userInterface.Register();

                //A4 is 164 in hexidecial notaion
                lighting = new Glpp1DimFlv3CnPm(164, this);
                lighting.Register();

                comport = ComPorts[1]; 
                comport.Register();
                comport.SetComPortSpec(Crestron.SimplSharpPro.ComPort.eComBaudRates.ComspecBaudRate9600,
                                        Crestron.SimplSharpPro.ComPort.eComDataBits.ComspecDataBits8,
                                        Crestron.SimplSharpPro.ComPort.eComParityType.ComspecParityNone,
                                        Crestron.SimplSharpPro.ComPort.eComStopBits.ComspecStopBits1,
                                        Crestron.SimplSharpPro.ComPort.eComProtocolType.ComspecProtocolRS232,
                                        Crestron.SimplSharpPro.ComPort.eComHardwareHandshakeType.ComspecHardwareHandshakeNone,
                                        Crestron.SimplSharpPro.ComPort.eComSoftwareHandshakeType.ComspecSoftwareHandshakeNone,
                                        false);

                //Subscribe to the controller events (System, Program, and Ethernet)
                CrestronEnvironment.SystemEventHandler += new SystemEventHandler(ControlSystem_ControllerSystemEventHandler);
                CrestronEnvironment.ProgramStatusEventHandler += new ProgramStatusEventHandler(ControlSystem_ControllerProgramEventHandler);
                CrestronEnvironment.EthernetEventHandler += new EthernetEventHandler(ControlSystem_ControllerEthernetEventHandler);
            }
            catch (Exception e)
            {
                ErrorLog.Error("Error in the constructor: {0}", e.Message);
            }
        }

        void userInterface_Sigchange(BasicTriList currentDevice, SigEventArgs args)
        {
            bool PROJECTOR_POWER_ON_BUTTON = userInterface.BooleanInput[6].BoolValue;
            bool PROJECTOR_POWER_OFF_BUTTON = userInterface.BooleanInput[7].BoolValue;
            bool PROJECTOR_HDMI_BUTTON = userInterface.BooleanInput[8].BoolValue;
            bool PROJECTOR_VGA1_BUTTON = userInterface.BooleanInput[9].BoolValue;
            bool PROJECTOR_VGA2_BUTTON = userInterface.BooleanInput[10].BoolValue;
            bool PROJECTOR_SVIDEO_BUTTON = userInterface.BooleanInput[11].BoolValue;
            
            switch (args.Sig.Type)
            {
                case eSigType.Bool:
                    {
                        if (args.Sig.BoolValue)
                        {
                            //Turn on
                            if (args.Sig.Number == 6)
                            {
                                PROJECTOR_POWER_ON_BUTTON = true;
                                PROJECTOR_POWER_OFF_BUTTON = false;
                                comport.Send(PROJECTOR_POWER_ON + PROJECTOR_DELIMITER);
                            }
                            //Turn off
                            else if (args.Sig.Number == 7)
                            {
                                PROJECTOR_POWER_ON_BUTTON = false;
                                PROJECTOR_POWER_OFF_BUTTON = true;
                                comport.Send(PROJECTOR_POWER_OFF + PROJECTOR_DELIMITER);
                            }
                            //HDMI
                            else if (args.Sig.Number == 8)
                            {
                                setSourcesOff();
                                PROJECTOR_HDMI_BUTTON = true;
                                comport.Send(PROJECTOR_HDMI + PROJECTOR_DELIMITER);
                            }
                            //VGA1
                            else if (args.Sig.Number == 9)
                            {
                                setSourcesOff();
                                PROJECTOR_VGA1_BUTTON = true;
                                comport.Send(PROJECTOR_VGA1 + PROJECTOR_DELIMITER);
                            }
                            //VGA2
                            else if (args.Sig.Number == 10)
                            {
                                setSourcesOff();
                                PROJECTOR_VGA2_BUTTON = true;
                                comport.Send(PROJECTOR_VGA2 + PROJECTOR_DELIMITER);
                            }
                            //SVideo
                            else if (args.Sig.Number == 11)
                            {
                                setSourcesOff();
                                PROJECTOR_SVIDEO_BUTTON = true;
                                comport.Send(PROJECTOR_SVIDEO + PROJECTOR_DELIMITER);
                            }
                            //Lights On
                            else if (args.Sig.Number == 12)
                            {
                                lighting.SetLoadsFullOn();
                            }
                            //Lights Off
                            else if (args.Sig.Number == 13)
                            {
                                lighting.SetLoadsOff();
                            }
                        }
                        break;
                    }
                case eSigType.UShort:
                    {
                        break;
                    }
                case eSigType.String:
                    {
                        break;
                    }
            }
        }
        void setSourcesOff()
        {
            userInterface.BooleanInput[8].BoolValue = false;
            userInterface.BooleanInput[9].BoolValue = false;
            userInterface.BooleanInput[10].BoolValue = false;
            userInterface.BooleanInput[11].BoolValue = false;
        }

        /// <summary>
        /// InitializeSystem - this method gets called after the constructor 
        /// has finished. 
        /// 
        /// Use InitializeSystem to:
        /// * Start threads
        /// * Configure ports, such as serial and verisports
        /// * Start and initialize socket connections
        /// Send initial device configurations
        /// 
        /// Please be aware that InitializeSystem needs to exit quickly also; 
        /// if it doesn't exit in time, the SIMPL#Pro program will exit.
        /// </summary>
        public override void InitializeSystem()
        {
            try
            {

            }
            catch (Exception e)
            {
                ErrorLog.Error("Error in InitializeSystem: {0}", e.Message);
            }
        }

        /// <summary>
        /// Event Handler for Ethernet events: Link Up and Link Down. 
        /// Use these events to close / re-open sockets, etc. 
        /// </summary>
        /// <param name="ethernetEventArgs">This parameter holds the values 
        /// such as whether it's a Link Up or Link Down event. It will also indicate 
        /// wich Ethernet adapter this event belongs to.
        /// </param>
        void ControlSystem_ControllerEthernetEventHandler(EthernetEventArgs ethernetEventArgs)
        {
            switch (ethernetEventArgs.EthernetEventType)
            {//Determine the event type Link Up or Link Down
                case (eEthernetEventType.LinkDown):
                    //Next need to determine which adapter the event is for. 
                    //LAN is the adapter is the port connected to external networks.
                    if (ethernetEventArgs.EthernetAdapter == EthernetAdapterType.EthernetLANAdapter)
                    {
                        //
                    }
                    break;
                case (eEthernetEventType.LinkUp):
                    if (ethernetEventArgs.EthernetAdapter == EthernetAdapterType.EthernetLANAdapter)
                    {

                    }
                    break;
            }
        }

        /// <summary>
        /// Event Handler for Programmatic events: Stop, Pause, Resume.
        /// Use this event to clean up when a program is stopping, pausing, and resuming.
        /// This event only applies to this SIMPL#Pro program, it doesn't receive events
        /// for other programs stopping
        /// </summary>
        /// <param name="programStatusEventType"></param>
        void ControlSystem_ControllerProgramEventHandler(eProgramStatusEventType programStatusEventType)
        {
            switch (programStatusEventType)
            {
                case (eProgramStatusEventType.Paused):
                    //The program has been paused.  Pause all user threads/timers as needed.
                    break;
                case (eProgramStatusEventType.Resumed):
                    //The program has been resumed. Resume all the user threads/timers as needed.
                    break;
                case (eProgramStatusEventType.Stopping):
                    //The program has been stopped.
                    //Close all threads. 
                    //Shutdown all Client/Servers in the system.
                    //General cleanup.
                    //Unsubscribe to all System Monitor events
                    break;
            }

        }

        /// <summary>
        /// Event Handler for system events, Disk Inserted/Ejected, and Reboot
        /// Use this event to clean up when someone types in reboot, or when your SD /USB
        /// removable media is ejected / re-inserted.
        /// </summary>
        /// <param name="systemEventType"></param>
        void ControlSystem_ControllerSystemEventHandler(eSystemEventType systemEventType)
        {
            switch (systemEventType)
            {
                case (eSystemEventType.DiskInserted):
                    //Removable media was detected on the system
                    break;
                case (eSystemEventType.DiskRemoved):
                    //Removable media was detached from the system
                    break;
                case (eSystemEventType.Rebooting):
                    //The system is rebooting. 
                    //Very limited time to preform clean up and save any settings to disk.
                    break;
            }

        }
    }
}