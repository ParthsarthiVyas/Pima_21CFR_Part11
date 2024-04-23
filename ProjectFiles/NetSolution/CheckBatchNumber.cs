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
#endregion

public class CheckBatchNumber : BaseNetLogic
{

    /*
    private Button _confirmButton;
    private UANode msglabel;
    
    public override void Start()
    {
       _confirmButton = Owner.Get<Button>("Confirm");
       msglabel = Project.Current.Get<Label>("UI/Screens/08Reports/BatchNumberDialog/LabelMsg");
    }
    
    private void BatchNoVar_VariableChange(object sender, VariableChangeEventArgs e)
    {
        CheckEnteredBacthNumber();
    }
    */
    [ExportMethod]
    public void CheckEnteredBacthNumber(string CheckBatchName)
    {
        //Label MsgLabel = Project.Current.Get<Label>("UI/Screens/08Reports/BatchNumberDialog/LabelMsg");
        //Button ownerbtn = Owner.Get<Button>("Confirm");
        //DialogType BatchNumCheckResultDialog = Project.Current.Get<DialogType>("UI/Screens/08Reports/BatchNumberDialog");
        string EnterdBacthNum = CheckBatchName;
        if (string.IsNullOrEmpty(EnterdBacthNum))
        {
            ShowMessage("Please enter a valid batch number");
            //Button BatchMsg = Project.Current.Get<Button>("UI/Screens/08Reports/BatchReportGenerate/Button1");
            //BatchMsg.Text = "Please enter a valid batch number";
            //BatchMsg.Visible = false;
        }
        else
        {
            var myStore = Project.Current.Get<Store>("DataStores/EDB_Batch_Report");
            string SetSQLQuery = LogicObject.GetVariable("SQLQuery").Value;
            myStore.Query(SetSQLQuery, out string[] header, out object[,] resultSet);
            int lastval = resultSet.GetLength(0) - 1;
            if (resultSet.GetLength(0) > 0)
            {
                for (int i = 0; i < resultSet.GetLength(0); i++)
                {
                    string BatchNum = resultSet[i,0].ToString();
                    if (BatchNum.Equals(EnterdBacthNum, StringComparison.OrdinalIgnoreCase))
                    {
                        ShowMessage("Batch number already exist, please enter new one");
                        BatchAudit BatchStart = new BatchAudit();
                        BatchStart.LogIntoAudit("Batch Start", "Batch Already Exist, Batch No. " + CheckBatchName, Session.User.BrowseName, "BatchStart");
                        goto exitloop;
                    }
                    else if (i == lastval)
                    {

                        StartBatch();
                        ShowMessage("");
                        BatchDataLogging();
                    }
                }
            }
            else
            {
                StartBatch();
                ShowMessage("");
                BatchDataLogging();
            }

            exitloop:
            ;
        }
    }

    private void StartBatch()
    {
        Project.Current.GetVariable("Model/Batch/Batch_Object/Start_date_time").Value = DateTime.Now;
        Project.Current.GetVariable("Model/Batch/Batch_Object/Stop_date_time").Value = DateTime.Now;
        Project.Current.GetVariable("Model/Batch/Batch_Object/Batch_Running_Status").Value = true;
        Project.Current.GetVariable("Model/Batch/Batch_Object/Total_Count").Value = 0;
        Project.Current.GetVariable("Model/Batch/Batch_Object/Rejected_Count").Value = 0;
        Project.Current.GetVariable("Model/Batch/Batch_Object/Good_Count").Value = 0;
    }

    
    private void ShowMessage(string message)
	{
		//Label BatchMsg = Project.Current.Get<Label>("UI/Screens/08Reports/BatchReportGenerate/Backgroung/ReportControlBorder/Rectangle2/Label1");
        //BatchMsg.Text = message;
        //BatchMsg.Visible = true;
        Label BatchMsg = Project.Current.Get<Label>("UI/Screens/Batch/Batch_screen/MessageText");
        
        BatchMsg.Text = message;
        BatchMsg.Visible = true;
		if (delayedTask != null)
			delayedTask?.Dispose();
		
		delayedTask = new DelayedTask(DelayedAction, 5000, LogicObject);
		delayedTask.Start();
	}

    public void BatchDataLogging()
    {
         //string batchno = Project.Current.GetVariable("Model/Batch/Batch_Object/Batch_name").Value;
         Project.Current.GetVariable("Model/Batch/Batch_Object/Stop_date_time").Value = DateTime.Now;
        Project.Current.GetVariable("Model/Batch/Batch_Object/Start_date_time").Value = DateTime.Now;

        Store myStore = Project.Current.Get<Store>("DataStores/EDB_Batch_Report");

          Table myTable = myStore.Tables.Get<Table>("BatchInfo");


          string batchno = Project.Current.GetVariable("Model/Batch/Batch_Object/Batch_name").Value;
          DateTime batchstart = Project.Current.GetVariable("Model/Batch/Batch_Object/Start_date_time").Value;
          DateTime batchstop = Project.Current.GetVariable("Model/Batch/Batch_Object/Stop_date_time").Value;
          string prodname = Project.Current.GetVariable("Model/Batch/Batch_Object/Product_name").Value;
          String Username = Project.Current.GetVariable("Model/Batch/Batch_Object/User_name").Value;
          int totalcount = Project.Current.GetVariable("Model/Batch/Batch_Object/Total_Count").Value;
          int rejectcount = Project.Current.GetVariable("Model/Batch/Batch_Object/Rejected_Count").Value;
          int goodcount = Project.Current.GetVariable("Model/Batch/Batch_Object/Good_Count").Value;
          int Companyname = Project.Current.GetVariable("Model/Batch/Batch_Object/Company_name").Value;
          int Batchsize = Project.Current.GetVariable("Model/Batch/Batch_Object/Batch_Size").Value;
          String Recipename = Project.Current.GetVariable("Model/Batch/Batch_Object/RecipeName").Value;
          String EquipmentName = Project.Current.GetVariable("Model/Batch/Batch_Object/Equipment_Name").Value;
          String EquipmentId = Project.Current.GetVariable("Model/Batch/Batch_Object/Equipment_Id").Value;





          object[,] rawValues = new object[1, 14]; // [Raw, Column]; Column = number columns in Table of Audit event logger  database 
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


          string[] columns = new string[14] { "LocalTimeStamp", "BatchNumber", "BatchStartTime", "BatchStopTime", "ProductName", "Username", "TotalCounts", "RejectCounts", "GoodCounts", "BatchSize", "Recipename", "Companyname", "EquipmentName", "EquipmentId"};
          myTable.Insert(columns, rawValues);

          

        BatchAudit BatchStart = new BatchAudit();
        BatchStart.LogIntoAudit("Batch Start", "Batch Start, Batch No. " + batchno, Session.User.BrowseName, "BatchStart");

    }

    private void DelayedAction(DelayedTask task)
	{
		if (task.IsCancellationRequested)
			return;
        
        //Label BatchMsg = Project.Current.Get<Label>("UI/Screens/08Reports/BatchReportGenerate/Backgroung/ReportControlBorder/Rectangle2/Label1");
        //BatchMsg.Text = "";
        //BatchMsg.Visible = false;
        Label BatchMsg = Project.Current.Get<Label>("UI/Screens/Batch/Batch_screen/MessageText");
        BatchMsg.Text = "";
        BatchMsg.Visible = false;
		delayedTask?.Dispose();
	}

    private DelayedTask delayedTask;
    //private IUAVariable BatchNoVar;
    
}
