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
#endregion

public class RuntimeNetLogic3 : BaseNetLogic
{
    public override void Start()
    {
        // Insert code to be executed when the user-defined logic is started
        Label Message2 = (Label)LogicObject.Owner.Owner.Get("Label3");
        Message2.Text = "";
    }

    public override void Stop()
    {
        // Insert code to be executed when the user-defined logic is stopped
        Label Message2 = (Label)LogicObject.Owner.Owner.Get("Label3");
        Message2.Text = "";
    }

    [ExportMethod]

    public void restore(string selectedpath)
    {
        SQLiteStore db = (SQLiteStore)Project.Current.Get<Store>("DataStores/EDB_Batch_Report");
        Label Message2 = (Label)LogicObject.Owner.Owner.Get("Label3");
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
            Message2.Text = "Database is Restoring";
        }
        else
        {
            // Message2.Text = part1[1];
            AuditTrailLogging RecipeSavedLog = new AuditTrailLogging();
            RecipeSavedLog.LogIntoAudit("DB", "Batch DB Wrong Selection", Session.User.BrowseName, "Database Restore");
            Message2.Text = "Selected Backup Is Not Batch Database, Please Select Proper Database.";
            
        }
        Thread.Sleep(3000);
        Message2.Text = "";
        
    }

}
