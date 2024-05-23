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
using System.Threading;
using FTOptix.RAEtherNetIP;
#endregion

public class RuntimeNetLogic3 : BaseNetLogic
{
    public override void Start()
    {
        // Insert code to be executed when the user-defined logic is started
        // Label Message2 = (Label)LogicObject.Owner.Owner.Get("Label3");
        //Message2.Text = "";
        string message1 = Project.Current.GetVariable("Model/RestoreDBMessage").Value;
        message1 = "";
    }

    public override void Stop()
    {
        // Insert code to be executed when the user-defined logic is stopped
        //Label Message2 = (Label)LogicObject.Owner.Owner.Get("Label3");
        //Message2.Text = "";
        string message1 = Project.Current.GetVariable("Model/RestoreDBMessage").Value;
        message1 = "";
    }

    [ExportMethod]

    public void restore(string selectedpath)
    {
        SQLiteStore db = (SQLiteStore)Project.Current.Get<Store>("DataStores/EDB_Batch_Report");
        //Label Message2 = (Label)LogicObject.Owner.Owner.Get("Label3");
        string message1 = Project.Current.GetVariable("Model/RestoreDBMessage").Value;
        string selectedpath1 = selectedpath;
        if (selectedpath.StartsWith("file:///"))
        {
            selectedpath = selectedpath.Substring("file:///".Length);
        }

            string[] parts = selectedpath.Split('_');
            string data = parts[0];
        string[] part1 = data.Split('/');
        if (part1.Length > 0 && part1[1]== "BatchDB") // Trim the extracted database name
        {
            db.Restore(selectedpath1);
            AuditTrailLogging RecipeSavedLog = new AuditTrailLogging();
            RecipeSavedLog.LogIntoAudit("DB", "Batch DB Restore", Session.User.BrowseName, "Database Restore");
            message1 = "Database is Restoring";
        }
        else
        {
            // Message2.Text = part1[1];
            AuditTrailLogging RecipeSavedLog = new AuditTrailLogging();
            RecipeSavedLog.LogIntoAudit("DB", "Batch DB Wrong Selection", Session.User.BrowseName, "Database Restore");
            message1 = "Selected Backup Is Not Batch Database, Please Select Proper Database.";
            
        }
        Project.Current.GetVariable("Model/RestoreDBMessage").Value = message1;
        Thread.Sleep(3000);
        //Message2.Text = "";
        Project.Current.GetVariable("Model/RestoreDBMessage").Value = "";

    }

}
