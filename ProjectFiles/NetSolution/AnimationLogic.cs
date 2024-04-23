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
using FTOptix.SQLiteStore;
using FTOptix.Store;
using FTOptix.ODBCStore;
using FTOptix.Report;
using FTOptix.Retentivity;
using FTOptix.AuditSigning;
using FTOptix.EventLogger;
using FTOptix.CommunicationDriver;
using FTOptix.Core;

using FTOptix.Alarm;
#endregion

public class AnimationLogic : BaseNetLogic
{
    private IUAVariable buttonVariable;
    private IUAVariable levelVariable;
    private PeriodicTask periodicTask;
    public override void Start()
    {
        // Insert code to be executed when the user-defined logic is started
        var owner = (Animation)LogicObject.Owner;
        buttonVariable = owner.ButtonVariable;
        levelVariable = owner.LevelVariable;

        periodicTask = new PeriodicTask(RunTask, 500, LogicObject);
        periodicTask.Start();
    }

    public override void Stop()
    {
        periodicTask.Dispose();
        periodicTask=null;
    }

    public void RunTask() 
    {
        bool button = buttonVariable.Value;
        int level = levelVariable.Value;


        if (button == true && level > 0)
        {
            level = level - 1;

        }

        if (button == true && level == 0)
        {
            level = 100;
        }
        levelVariable.Value = level;
    }
}
