#region Using directives
using System;
using UAManagedCore;
using OpcUa = UAManagedCore.OpcUa;
using FTOptix.HMIProject;
using FTOptix.NetLogic;
using FTOptix.CoreBase;
using FTOptix.NativeUI;
using FTOptix.UI;
using FTOptix.SQLiteStore;
using FTOptix.Store;
using FTOptix.Core;
using FTOptix.WebUI;
using FTOptix.DataLogger;
using System.Data;
using Microsoft.VisualBasic;
using System.Globalization;
using FTOptix.Alarm;
#endregion

public class LogBatchData : BaseNetLogic
{
    [ExportMethod]
    public void BatchDataLogging()
    {

        Project.Current.GetVariable("Model/Batch/Batch_Object/Stop_date_time").Value = DateTime.Now;


        Store myStore = Project.Current.Get<Store>("DataStores/EDB_Batch_Report");

        Table myTable = myStore.Tables.Get<Table>("BatchInfo");


        string batchno = Project.Current.GetVariable("Model/Batch/Batch_Object/Batch_name").Value;
        string prodname = Project.Current.GetVariable("Model/Batch/Batch_Object/Product_name").Value;
        string Username = Project.Current.GetVariable("Model/Batch/Batch_Object/User_name").Value;
        int totalcount = Project.Current.GetVariable("Model/Batch/Batch_Object/Total_Count").Value;
        int rejectcount = Project.Current.GetVariable("Model/Batch/Batch_Object/Rejected_Count").Value;
        int goodcount = Project.Current.GetVariable("Model/Batch/Batch_Object/Good_Count").Value;
        string Companyname = Project.Current.GetVariable("Model/Batch/Batch_Object/Company_name").Value;
        int Batchsize = Project.Current.GetVariable("Model/Batch/Batch_Object/Batch_Size").Value;
        string Recipename = Project.Current.GetVariable("Model/Batch/Batch_Object/RecipeName").Value;
        string EquipmentName = Project.Current.GetVariable("Model/Batch/Batch_Object/Equipment_Name").Value;
        string EquipmentId = Project.Current.GetVariable("Model/Batch/Batch_Object/Equipment_Id").Value;
        DateTime batchstart = Project.Current.GetVariable("Model/Batch/Batch_Object/Start_date_time").Value;
        DateTime batchstop = Project.Current.GetVariable("Model/Batch/Batch_Object/Stop_date_time").Value;
        string batchstop1 = batchstop.ToString("MMM d, yyyy, h:mm:ss g(en-US)");



        /* object[,] rawValues = new object[1, 14]; // [Raw, Column]; Column = number columns in Table of Audit event logger  database 
         rawValues[0, 0] = DateTime.Now;
         rawValues[0, 1] = batchno;
         rawValues[0, 2] = batchstart;
         rawValues[0, 3] = batchstop;
         rawValues[0, 4] = prodname;
         rawValues[0, 5] = Username;
         rawValues[0, 6] = totalcount;
         rawValues[0, 7] = rejectcount;
         rawValues[0, 8] = goodcount;
         rawValues[0, 9] = Batchsize;
         rawValues[0, 10] = Recipename;
         rawValues[0, 11] = Companyname;
         rawValues[0, 12] = EquipmentName;
         rawValues[0, 13] = EquipmentId;
        */
        string Selectquery = "UPDATE BatchInfo SET BatchStopTime = '" + batchstop.ToString("o", CultureInfo.InvariantCulture) + "', TotalCounts =" + totalcount + ", RejectCounts = " + rejectcount + ", GoodCounts =" + goodcount + " WHERE BatchNumber = '" + batchno + "'";
        //Object[,] ResultSet;
        //String[] Header;
        myStore.Query(Selectquery,out String[] Header, out Object[,] ResultSet);
        Log.Info("Batch,Batch Stored");


       // string[] columns = new string[14] { "LocalTimeStamp", "BatchNumber", "BatchStartTime", "BatchStopTime", "ProductName", "Username", "TotalCounts", "RejectCounts", "GoodCounts", "BatchSize", "Recipename", "Companyname", "EquipmentName", "EquipmentId" };
        //myTable.Insert(columns, rawValues);
        


        BatchAudit BatchStart = new BatchAudit();
        BatchStart.LogIntoAudit("Batch Stop", "Batch Stop, Batch No. " + batchno, Session.User.BrowseName, "BatchStop");
       
         Project.Current.GetVariable("Model/Batch/Batch_Object/Batch_Running_Status").Value = false;
       
    }


}
