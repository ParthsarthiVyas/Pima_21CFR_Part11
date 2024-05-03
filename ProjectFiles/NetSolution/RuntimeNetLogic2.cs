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
using System.IO;
using System.Threading;
using System.Xml.Linq;
#endregion

public class RuntimeNetLogic2 : BaseNetLogic
{
    public override void Start()
    {
        // Insert code to be executed when the user-defined logic is started
        Label Message = (Label)LogicObject.Owner.Owner.Get("Label2");
        Message.Text = "";
    }

    public override void Stop()
    {
        // Insert code to be executed when the user-defined logic is stopped
        Label Message = (Label)LogicObject.Owner.Owner.Get("Label2");
        Message.Text = "";
    }

    [ExportMethod]

    public void backup()
    {
        Label Message = (Label)LogicObject.Owner.Owner.Get("Label2");
            string filePath = Project.Current.GetVariable("Model/Databasepath/BachDB_Path").Value;

            if (File.Exists(filePath))
            {
               Console.WriteLine("File exists.");
             Message.Text = "Batch DB Backup Created Successfully";
                AuditTrailLogging RecipeSavedLog = new AuditTrailLogging();
                 RecipeSavedLog.LogIntoAudit("DB", "Batch DB Backup Created Successfully", Session.User.BrowseName, "Database Backup");
            }
            else
            {
                Console.WriteLine("File does not exist.");
              Message.Text = "Error!, Batch DB Backup Could Not Created.";
                AuditTrailLogging RecipeSavedLog = new AuditTrailLogging();
             RecipeSavedLog.LogIntoAudit("DB", "Error!, Batch DB Backup Could Not Created.", Session.User.BrowseName, "Database Backup");
            }
        Thread.Sleep(3000);
        Message .Text = "";
    }


}
