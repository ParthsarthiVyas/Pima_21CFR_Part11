#region Using directives
using System;
using UAManagedCore;
using OpcUa = UAManagedCore.OpcUa;
using FTOptix.HMIProject;
using FTOptix.NetLogic;
using FTOptix.CoreBase;
using FTOptix.Alarm;
using FTOptix.UI;
using FTOptix.NativeUI;
using FTOptix.Recipe;
using FTOptix.EventLogger;
using FTOptix.SQLiteStore;
using FTOptix.Store;
using FTOptix.ODBCStore;
using FTOptix.Report;
using FTOptix.Retentivity;
using FTOptix.AuditSigning;
using FTOptix.CommunicationDriver;
using FTOptix.Core;
#endregion

public class RuntimeNetLogic1 : BaseNetLogic
{
    public override void Start()
    {
        // Insert code to be executed when the user-defined logic is started
    }

    public override void Stop()
    {
        // Insert code to be executed when the user-defined logic is stopped
    }
    [ExportMethod]

    public void run()
    {
        float ppr = Project.Current.GetVariable("Model/Calibration/Encoder_Calibration/PPmm").Value;
        var resultant = ppr % 1000;
        int button = Project.Current.GetVariable("Model/Result").Value;
        if (resultant == 0) 
        { 
            button = 1;
        }
        else
        {
            button = 2;
        }
         Project.Current.GetVariable("Model/Result").Value = button;
    }
}
