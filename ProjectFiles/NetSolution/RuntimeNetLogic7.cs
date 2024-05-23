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
using FTOptix.RAEtherNetIP;

#endregion

public class RuntimeNetLogic7 : BaseNetLogic
{
    public override void Start()
    {
        // AUTO BACKUP //
        //Project.Current.GetVariable("Model/Autobackupdate").Value = DateTime.Today.Date;
        DateTime givendate = Project.Current.GetVariable("Model/Autobackup/Autobackupdate").Value; // Change karjo apeli tarikh
        int mydats = Project.Current.GetVariable("Model/Autobackup/Automaticbackupdays").Value;
        ResourceUri path = "file:///" + Project.Current.GetVariable("Model/Autobackup/Autobackuppathbatch").Value;
        ResourceUri path1 = "file:///" + Project.Current.GetVariable("Model/Autobackup/AutobackuppathAudit").Value;
        ResourceUri path2 = "file:///" + Project.Current.GetVariable("Model/Autobackup/AutobackuppathAlarm").Value;
        ResourceUri path3 = "file:///" + Project.Current.GetVariable("Model/Autobackup/AutobackuppathRecipe").Value;
        // Today's tarikh
        DateTime todaydate = DateTime.Today;

        // Divas count karo
        int daydifference = (int)(todaydate - givendate).TotalDays;

        // Message generate karo
        if (daydifference >= mydats)
        {
            SQLiteStore db = (SQLiteStore)Project.Current.Get<Store>("DataStores/EDB_Batch_Report");
            db.Backup(path);
            SQLiteStore db1 = (SQLiteStore)Project.Current.Get<Store>("DataStores/EDB_AuditTrail");
            db.Backup(path1);
            SQLiteStore db2 = (SQLiteStore)Project.Current.Get<Store>("DataStores/EDB_Alarm");
            db.Backup(path2);
            SQLiteStore db3 = (SQLiteStore)Project.Current.Get<Store>("DataStores/Recipe_Db");
            db.Backup(path3);
            AuditTrailLogging RecipeSavedLog = new AuditTrailLogging();
            RecipeSavedLog.LogIntoAudit("DB", "Auto Backup: Audit DB", Session.User.BrowseName, "Database Backup");
            AuditTrailLogging backupcreate = new AuditTrailLogging();
            backupcreate.LogIntoAudit("DB", "Auto Backup: Batch DB", Session.User.BrowseName, "Database Backup");
            AuditTrailLogging backupcreate1 = new AuditTrailLogging();
            backupcreate1.LogIntoAudit("DB", "Auto Backup: Alarm DB", Session.User.BrowseName, "Database Backup");
            AuditTrailLogging backupcreate2 = new AuditTrailLogging();
            backupcreate2.LogIntoAudit("DB", "Auto Backup: Recipe DB", Session.User.BrowseName, "Database Backup");
            Project.Current.GetVariable("Model/Autobackup/Autobackupdate").Value = DateTime.Today;
        }
    }

    public override void Stop()
    {
        // Insert code to be executed when the user-defined logic is stopped
    }
}
