#region Using directives
using System;
using UAManagedCore;
using OpcUa = UAManagedCore.OpcUa;
using FTOptix.HMIProject;
using FTOptix.NetLogic;
using FTOptix.CoreBase;
using FTOptix.NativeUI;
using FTOptix.UI;
using FTOptix.WebUI;
using FTOptix.Alarm;
using FTOptix.Recipe;
using FTOptix.EventLogger;
using FTOptix.DataLogger;
using FTOptix.SQLiteStore;
using FTOptix.Store;
using FTOptix.ODBCStore;
using FTOptix.Report;
using FTOptix.RAEtherNetIP;
using FTOptix.Modbus;
using FTOptix.MelsecFX3U;
using FTOptix.Retentivity;
using FTOptix.AuditSigning;
using FTOptix.CommunicationDriver;
using FTOptix.Core;
using FTOptix.MicroController;
using FTOptix.System;
#endregion

public class Confirmlogic : BaseNetLogic
{

    
    public override void Start()
    {


    
        // Insert code to be executed when the user-defined logic is started
    }

    public override void Stop()
    {
        // Insert code to be executed when the user-defined logic is stopped
    }

    [ExportMethod]
    public void ConfirmLogic()
    {
            IUAVariable _AckVar = Project.Current.GetVariable("UI/Screens/03Alarms/PresentAlarms/Backgroung/AlarmGrid1/Variable1");
//_AckVar.Value = 0;

        Dialog  _CommentBox = (Dialog) LogicObject.Owner.Owner;
        IUAVariable _AlarmName = (IUAVariable)_CommentBox.GetAlias("AlarmName");
        TextBox _Comment = (TextBox)LogicObject.Owner.Owner.Get("TextBox1");
        Label _warning = (Label)LogicObject.Owner.Owner.Get("Label1");

        if (_Comment.Text  == null || _Comment.Text == "")
        {
            _warning.Text = "Please Enter Comment";
        return;
        }

//IUAVariable _AckVar = Project.Current.GetVariable("Model/ACKSTATUS");

_AckVar.Value = 1;

       
    AuditTrailLogging Ackcomment = new AuditTrailLogging();
		Ackcomment.LogIntoAudit("Alarm Acknowledged" + _AlarmName.Value,_Comment.Text, Session.User.BrowseName, "UserCreateEvent");
        _warning.Text = "Success";
        _CommentBox.Close();


    }

}

