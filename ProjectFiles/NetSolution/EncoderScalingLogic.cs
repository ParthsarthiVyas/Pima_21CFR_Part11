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

public class EncoderScalingLogic : BaseNetLogic
{
    private IUAVariable buttonVariable;
    private IUAVariable ppmmVariable;
    private IUAVariable resultVariable;
    private PeriodicTask periodicTask;

    public override void Start()
    {
        // Insert code to be executed when the user-defined logic is started
        var owner = (EncoderScaling)LogicObject.Owner;
        buttonVariable = owner.ButtonVariable;
        ppmmVariable = owner.PPmmVariable;
        resultVariable = owner.ResultVariable;
        periodicTask = new PeriodicTask(ChandanSuthar, 1000, LogicObject);
    }

    public override void Stop()
    {
        // Insert code to be executed when the user-defined logic is stopped
        periodicTask.Dispose();
        periodicTask =null;
    }
    public void ChandanSuthar()
    {
        bool button = buttonVariable.Value;
        float ppmm = ppmmVariable.Value;
        bool result = resultVariable.Value;

       
       
        float remainder = ppmm % 1000;

            if (remainder == 0)
            {
                result = true;
            }
            else
            {
                result = false;
            }
   
        resultVariable.Value = result;

    }
}
