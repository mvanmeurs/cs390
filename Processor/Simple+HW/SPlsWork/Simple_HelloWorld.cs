using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Linq;
using Crestron;
using Crestron.Logos.SplusLibrary;
using Crestron.Logos.SplusObjects;
using Crestron.SimplSharp;

namespace UserModule_SIMPLE_HELLOWORLD
{
    public class UserModuleClass_SIMPLE_HELLOWORLD : SplusObject
    {
        static CCriticalSection g_criticalSection = new CCriticalSection();
        
        Crestron.Logos.SplusObjects.DigitalInput HELLOWORLDDIG;
        Crestron.Logos.SplusObjects.DigitalOutput DIGITALOUT;
        Crestron.Logos.SplusObjects.AnalogOutput ANALOGOUT;
        Crestron.Logos.SplusObjects.StringOutput STRINGOUT;
        object HELLOWORLDDIG_OnPush_0 ( Object __EventInfo__ )
        
            { 
            Crestron.Logos.SplusObjects.SignalEventArgs __SignalEventArg__ = (Crestron.Logos.SplusObjects.SignalEventArgs)__EventInfo__;
            try
            {
                SplusExecutionContext __context__ = SplusThreadStartCode(__SignalEventArg__);
                
                __context__.SourceCodeLine = 165;
                DIGITALOUT  .Value = (ushort) ( 1 ) ; 
                __context__.SourceCodeLine = 166;
                ANALOGOUT  .Value = (ushort) ( 999 ) ; 
                __context__.SourceCodeLine = 167;
                STRINGOUT  .UpdateValue ( "hello world!"  ) ; 
                
                
            }
            catch(Exception e) { ObjectCatchHandler(e); }
            finally { ObjectFinallyHandler( __SignalEventArg__ ); }
            return this;
            
        }
        
    
    public override void LogosSplusInitialize()
    {
        SocketInfo __socketinfo__ = new SocketInfo( 1, this );
        InitialParametersClass.ResolveHostName = __socketinfo__.ResolveHostName;
        _SplusNVRAM = new SplusNVRAM( this );
        
        HELLOWORLDDIG = new Crestron.Logos.SplusObjects.DigitalInput( HELLOWORLDDIG__DigitalInput__, this );
        m_DigitalInputList.Add( HELLOWORLDDIG__DigitalInput__, HELLOWORLDDIG );
        
        DIGITALOUT = new Crestron.Logos.SplusObjects.DigitalOutput( DIGITALOUT__DigitalOutput__, this );
        m_DigitalOutputList.Add( DIGITALOUT__DigitalOutput__, DIGITALOUT );
        
        ANALOGOUT = new Crestron.Logos.SplusObjects.AnalogOutput( ANALOGOUT__AnalogSerialOutput__, this );
        m_AnalogOutputList.Add( ANALOGOUT__AnalogSerialOutput__, ANALOGOUT );
        
        STRINGOUT = new Crestron.Logos.SplusObjects.StringOutput( STRINGOUT__AnalogSerialOutput__, this );
        m_StringOutputList.Add( STRINGOUT__AnalogSerialOutput__, STRINGOUT );
        
        
        HELLOWORLDDIG.OnDigitalPush.Add( new InputChangeHandlerWrapper( HELLOWORLDDIG_OnPush_0, false ) );
        
        _SplusNVRAM.PopulateCustomAttributeList( true );
        
        NVRAM = _SplusNVRAM;
        
    }
    
    public override void LogosSimplSharpInitialize()
    {
        
        
    }
    
    public UserModuleClass_SIMPLE_HELLOWORLD ( string InstanceName, string ReferenceID, Crestron.Logos.SplusObjects.CrestronStringEncoding nEncodingType ) : base( InstanceName, ReferenceID, nEncodingType ) {}
    
    
    
    
    const uint HELLOWORLDDIG__DigitalInput__ = 0;
    const uint DIGITALOUT__DigitalOutput__ = 0;
    const uint ANALOGOUT__AnalogSerialOutput__ = 0;
    const uint STRINGOUT__AnalogSerialOutput__ = 1;
    
    [SplusStructAttribute(-1, true, false)]
    public class SplusNVRAM : SplusStructureBase
    {
    
        public SplusNVRAM( SplusObject __caller__ ) : base( __caller__ ) {}
        
        
    }
    
    SplusNVRAM _SplusNVRAM = null;
    
    public class __CEvent__ : CEvent
    {
        public __CEvent__() {}
        public void Close() { base.Close(); }
        public int Reset() { return base.Reset() ? 1 : 0; }
        public int Set() { return base.Set() ? 1 : 0; }
        public int Wait( int timeOutInMs ) { return base.Wait( timeOutInMs ) ? 1 : 0; }
    }
    public class __CMutex__ : CMutex
    {
        public __CMutex__() {}
        public void Close() { base.Close(); }
        public void ReleaseMutex() { base.ReleaseMutex(); }
        public int WaitForMutex() { return base.WaitForMutex() ? 1 : 0; }
    }
     public int IsNull( object obj ){ return (obj == null) ? 1 : 0; }
}


}
