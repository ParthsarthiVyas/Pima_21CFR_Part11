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
    private IUAVariable level1Variable;
    private IUAVariable Motor11Variable;
    private IUAVariable Motor12Variable;
    private IUAVariable Motor13Variable;
    private PeriodicTask periodicTask;
    private IUAVariable SoVariable;
    private IUAVariable tap1Variable;
    private IUAVariable tap2Variable;
    private IUAVariable tap3Variable;

    public override void Start()
    {
        // Insert code to be executed when the user-defined logic is started
        var owner = (Animation)LogicObject.Owner;
        buttonVariable = owner.ButtonVariable;
        levelVariable = owner.LevelVariable;
        level1Variable = owner.Level1Variable;
        Motor11Variable = owner.Motor1Variable;
        Motor12Variable = owner.Motor2Variable;
        Motor13Variable = owner.Motor3Variable;
        SoVariable = owner.soVariable;
        tap1Variable = owner.Tap1Variable;
        tap2Variable = owner.Tap2Variable;
        tap3Variable = owner.Tap3Variable;
        periodicTask = new PeriodicTask(RunTask, 200, LogicObject);
        periodicTask.Start();
    }

    public override void Stop()
    {
        periodicTask.Dispose();
        periodicTask = null;
    }

    public void RunTask()
    {
        bool button = buttonVariable.Value;
        int level = levelVariable.Value;
        int level1 = level1Variable.Value;
        int Motor1 = Motor11Variable.Value;
        int Motor2 = Motor12Variable.Value;
        int Motor3 = Motor13Variable.Value;
        bool so = SoVariable.Value;
        int tap1 = tap1Variable.Value;
        int tap2 = tap2Variable.Value;
        int tap3 = tap3Variable.Value;
        if (button)
        {
            if (level1 < 100 && so == false)
            {
                for (int i = 0; i < 1; i++)
                {
                    if (level1 % 20 == 0)
                    {
                        Motor1 = 1; Motor2 = 0; Motor3 = 0;
                    }
                    else if (level1 % 20 >= 10)
                    {
                        Motor1 = 0; Motor2 = 1; Motor3 = 0;
                    }
                    else
                    {
                        Motor1 = 1; Motor2 = 0; Motor3 = 0;
                    }
                    level1 = level1 + 1;
                    tap1 = 1;
                    so = false;
                }
            }


            if (level1 >= 100 && level < 100 && so == false)
            {
                for (int i = 0; i < 1; i++)
                {
                    if (level % 20 == 0)
                    {
                        Motor1 = 1; Motor2 = 0; Motor3 = 0;
                    }
                    else if (level % 20 >= 10)
                    {
                        Motor1 = 0; Motor2 = 1; Motor3 = 0;
                    }
                    else
                    {
                        Motor1 = 1; Motor2 = 0; Motor3 = 0;
                    }
                    level = level + 1;
                    tap1 = 0;
                    tap2 = 1;
                    so = false;
                }
            }

            if (level1 == 100 && level == 100 && so == false)
            {
                Motor1 = 0; Motor2 = 0; Motor3 = 1;
                so = true;
                tap1 = 0;
                tap2 = 0;
            }

            if (level1 >= 0 && level == 100 && so == true)
            {
                Motor1 = 0; Motor2 = 0; Motor3 = 1;
                for (int i = 0; i < 1; i++)
                {
                    level1 = level1 - 1;

                }
            }

            if (level1 == 0 && level > 0 && so == true)
            {
                Motor1 = 0; Motor2 = 0; Motor3 = 0;
                for (int i = 0; i < 1; i++)
                {
                    level = level - 1;
                    tap3 = 1;
                }
            }

            if (level1 == 0 && level == 00 && so == true)
            {
                so = false;
                tap3 = 0;
            }

            levelVariable.Value = level;
            level1Variable.Value = level1;
            Motor11Variable.Value = Motor1;
            Motor12Variable.Value = Motor2;
            Motor13Variable.Value = Motor3;
            SoVariable.Value = so;
            tap1Variable.Value = tap1;
            tap2Variable.Value = tap2;
            tap3Variable.Value = tap3;
        }
    }
}