#region Using directives
using System;
using UAManagedCore;
using OpcUa = UAManagedCore.OpcUa;
using FTOptix.HMIProject;
using FTOptix.NetLogic;
using FTOptix.CoreBase;
using FTOptix.NativeUI;
using FTOptix.UI;
using FTOptix.Store;
using FTOptix.Core;
using FTOptix.WebUI;
using FTOptix.Alarm;
#endregion

public class GetBatchInfo : BaseNetLogic
{
    public override void Start()
    {
        SelBatchNoVar = Project.Current.GetVariable("Model/Report/Batch_report/BatchName");
        SelBatchNoVar.VariableChange += SelBatchNoVar_VariableChange;

        LoadBatchInfo();
    }

    private void SelBatchNoVar_VariableChange(object sender, VariableChangeEventArgs e)
    {
        LoadBatchInfo();
    }

    private void LoadBatchInfo()
    {
        var myStore = Project.Current.Get<Store>("DataStores/EDB_Batch_Report");
        string batchsquery = Project.Current.GetVariable("Model/Report/Batch_report/Batch_Query").Value;
        //Log.Warning(batchsquery);
        myStore.Query(batchsquery, out string[] header, out object[,] resultSet);
        //int LastVal = resultSet.GetLength(0) - 1;
        //Log.Warning("Number Results = " + resultSet.GetLength(0));

        string batchno = "";
        DateTime batchstart = default;
        DateTime batchstop = default;
        string prodname = "";
        string Username = "";
        int totalcount = 0;
        int rejectcount = 0;
        int goodcount = 0;
        int Batchsize = 0;
        string Recipename = "";
        string Companyname = "";
        string EquipmentName = "";
        string EquipmentId = "";
        

        if (resultSet.GetLength(0) > 0)
        {
            batchno = resultSet[0,0].ToString();
            batchstart = DateTime.Parse(resultSet[0,1].ToString());
            batchstop = DateTime.Parse(resultSet[0,2].ToString());
            prodname = resultSet[0,3].ToString();
            Username = resultSet[0,4].ToString();
            totalcount = int.Parse(resultSet[0,5].ToString());
            rejectcount = int.Parse(resultSet[0,6].ToString());
            goodcount = int.Parse(resultSet[0,7].ToString());
            Batchsize = int.Parse(resultSet[0,8].ToString());
            Recipename = resultSet[0,9].ToString(); 
             Companyname = resultSet[0,10].ToString(); 
            EquipmentName = resultSet[0,11].ToString(); 
            EquipmentId = resultSet[0,12].ToString();
           


        }

         Project.Current.GetVariable("Model/Report/Batch_report/BatchName").Value = batchno;
         Project.Current.GetVariable("Model/Report/Batch_report/ProductName").Value = prodname;
         Project.Current.GetVariable("Model/Report/Batch_report/Username").Value = Username;
         Project.Current.GetVariable("Model/Report/Batch_report/TotalCounts").Value = totalcount;
         Project.Current.GetVariable("Model/Report/Batch_report/RejectCounts").Value = rejectcount;
         Project.Current.GetVariable("Model/Report/Batch_report/GoodCounts").Value = goodcount;
         Project.Current.GetVariable("Model/Report/Batch_report/Companyname").Value = Companyname;
         Project.Current.GetVariable("Model/Report/Batch_report/BatchSize").Value = Batchsize;
         Project.Current.GetVariable("Model/Report/Batch_report/Recipename").Value = Recipename;
         Project.Current.GetVariable("Model/Report/Batch_report/EquipmentName").Value = EquipmentName;
         Project.Current.GetVariable("Model/Report/Batch_report/EquipmentId").Value = EquipmentId;
         Project.Current.GetVariable("Model/Report/Batch_report/BatchStartTime").Value = batchstart;
        Project.Current.GetVariable("Model/Report/Batch_report/BatchStopTime").Value = batchstop;






    }

    private IUAVariable SelBatchNoVar;
}
