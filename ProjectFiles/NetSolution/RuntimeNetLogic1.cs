#region Using directives
using System;
using UAManagedCore;
using OpcUa = UAManagedCore.OpcUa;
using FTOptix.HMIProject;
using FTOptix.NetLogic;
using FTOptix.CoreBase;
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
using FTOptix.Alarm;
using FTOptix.DataLogger;
#endregion

public class RuntimeNetLogic1 : BaseNetLogic
{
    public override void Start()
    {
        // Insert code to be executed when the user-defined logic is started
        periodicTask = new PeriodicTask(run, 100, LogicObject);
        periodicTask.Start();
        int level = Project.Current.GetVariable("UI/Model/Automode/Level").Value;
        var automode = Project.Current.GetVariable("UI/Model/Setting/AutoMode").Value;

    }

    public override void Stop()
    {
        // Insert code to be executed when the user-defined logic is stopped
        periodicTask.Dispose();
        periodicTask = null;
    }

    private void run()
    {
        int level= Project.Current.GetVariable("UI/Model/Automode/Level").Value;
        var automode = Project.Current.GetVariable("UI/Model/Setting/AutoMode").Value;
        if (automode = true)
        {
            for (level = 100; level >= 0; level--) 
            {
                if (level == 0)
                {
                    level = 100;
                }
            }
        }
        Project.Current.GetVariable("Model/Automode/Level").Value = level;
    }
 
    private PeriodicTask periodicTask;
}
