#region Using directives
using System;
using FTOptix.HMIProject;
using UAManagedCore;
using OpcUa = UAManagedCore.OpcUa;
using FTOptix.NetLogic;
using FTOptix.UI;
using FTOptix.WebUI;
using System.Linq;
using FTOptix.Recipe;
using System.Collections.Generic;
using System.Text;
using FTOptix.MicroController;
using FTOptix.System;
using FTOptix.DataLogger;
using FTOptix.Alarm;
#endregion

public class EditUserDetailPanelLogic : BaseNetLogic
{
    [ExportMethod]
    public void SaveUser(string username, string password, string locale, out NodeId result)
    {
        result = NodeId.Empty;

        if (string.IsNullOrEmpty(username))
        {
            ShowMessage(1);
            Log.Error("EditUserDetailPanelLogic", "Cannot create user with empty username");
            return;
        }

        result = ApplyUser(username, password, locale);
    }

    private NodeId ApplyUser(string username, string password, string locale)
    {
        var users = GetUsers();
        if (users == null)
        {
            ShowMessage(2);
            Log.Error("EditUserDetailPanelLogic", "Unable to get users");
            return NodeId.Empty;
        }

        var user = users.Get<User_21CFR>(username);
        if (user == null)
        {
            ShowMessage(3);
            Log.Error("EditUserDetailPanelLogic", "User not found");
            return NodeId.Empty;
        }

        //Apply LocaleId
        if (!string.IsNullOrEmpty(locale))
            user.LocaleId = locale;

        //-----------Customized Logic Start-----------------
        // Block/Unblock User
        CheckBox BlockedCheckBox = Owner.Get<CheckBox>("HorizontalLayout1/Details/VerticalLayout1/HorizontalLayoutUserBlocked/Blocked/UserStatus");
        if (user.User_Blocked != BlockedCheckBox.Checked)
        {
            if ((user.BrowseName == Session.User.BrowseName) && (BlockedCheckBox.Checked))
            {
                ShowMessage(21);
                Log.Error("EditUserDetailPanelLogic", "LoggedIn user can not be blocked");
                return NodeId.Empty;
            }
            user.User_Blocked = BlockedCheckBox.Checked;
            // User Status Change Activity Logging into Audit Database
            AuditTrailLogging UserStatusChg = new AuditTrailLogging();
            string userstate = "";
            if (user.User_Blocked)
            {
                userstate = "blocked";
            }
            else
            {
                userstate = "unblocked";
            }
            UserStatusChg.LogIntoAudit("User modified", "'" + username + "'" + " " + userstate, Session.User.BrowseName, "UserStatusChangeEvent");
        }
        //-----------Customized Logic End-------------------

        //-----------Customized Logic Start-----------------
        // User Active/Inactive
        CheckBox ActiveCheckBox = Owner.Get<CheckBox>("HorizontalLayout1/Details/VerticalLayout1/HorizontalLayoutUserBlocked/Blocked/UserActive");
        if (user.User_Active != ActiveCheckBox.Checked)
        {
            if ((user.BrowseName == Session.User.BrowseName) && (!ActiveCheckBox.Checked))
            {
                ShowMessage(22);
                Log.Error("EditUserDetailPanelLogic", "LoggedIn user can not be set to inactive");
                return NodeId.Empty;
            }
            user.User_Active = ActiveCheckBox.Checked;
            // User Status Change Activity Logging into Audit Database
            AuditTrailLogging UserStatusChg2 = new AuditTrailLogging();
            string userActivestate = "";
            if (user.User_Active)
            {
                userActivestate = "Active";
            }
            else
            {
                userActivestate = "Inactive";
            }
            UserStatusChg2.LogIntoAudit("User modified", "'" + username + "'" + " status changed to " + userActivestate, Session.User.BrowseName, "UserStatusChangeEvent");
        }
        //-----------Customized Logic End-------------------

        //Apply groups
        ApplyGroups(user);


        //-----------Customized Logic Start-----------------
        // User Password Strength Check
        if (!string.IsNullOrEmpty(password))
        {
            CheckPasswordStrength UserPassCheck = new CheckPasswordStrength();
            bool PassStrengthCheck = false;
            PassStrengthCheck = UserPassCheck.CheckPassword(password);
            if (!PassStrengthCheck)
            {
                ShowMessage(20);
                Log.Error("EditUserDetailPanelLogic", "Entered Password is not Complex");
                return NodeId.Empty;
            }
        }

        //-----------Customized Logic End-------------------


        //Apply password
        if (!string.IsNullOrEmpty(password))
        {
            var result = Session.ChangePasswordInternal(username, password);

            switch (result.ResultCode)
            {
                case FTOptix.Core.ChangePasswordResultCode.Success:
                    //-----------Customized Logic Start-----------------
                    // User Password Change Activity Logging into Audit Database
                    user.Password_Creation_Date = DateTime.Now;
                    user.Invalid_Login_Attempts = 0;
                    AuditTrailLogging UserPassChange = new AuditTrailLogging();
                    UserPassChange.LogIntoAudit("User modified", "'" + username + "'" + " password changed", Session.User.BrowseName, "UserPasswordChangeEvent");
                    //-----------Customized Logic End-------------------
                    var editPasswordTextboxPtr = LogicObject.GetVariable("PasswordTextbox");
                    if (editPasswordTextboxPtr == null)
                        Log.Error("EditUserDetailPanelLogic", "PasswordTextbox variable not found");

                    var nodeId = (NodeId)editPasswordTextboxPtr.Value;
                    if (nodeId == null)
                        Log.Error("EditUserDetailPanelLogic", "PasswordTextbox not set");

                    var editPasswordTextbox = (TextBox)InformationModel.Get(nodeId);
                    if (editPasswordTextbox == null)
                        Log.Error("EditUserDetailPanelLogic", "EditPasswordTextbox not found");

                    editPasswordTextbox.Text = string.Empty;
                    break;
                case FTOptix.Core.ChangePasswordResultCode.WrongOldPassword:
                    //Not applicable
                    break;
                case FTOptix.Core.ChangePasswordResultCode.PasswordAlreadyUsed:
                    ShowMessage(4);
                    return NodeId.Empty;
                case FTOptix.Core.ChangePasswordResultCode.PasswordChangedTooRecently:
                    ShowMessage(5);
                    return NodeId.Empty;
                case FTOptix.Core.ChangePasswordResultCode.PasswordTooShort:
                    ShowMessage(6);
                    return NodeId.Empty;
                case FTOptix.Core.ChangePasswordResultCode.UserNotFound:
                    ShowMessage(7);
                    return NodeId.Empty;
                case FTOptix.Core.ChangePasswordResultCode.UnsupportedOperation:
                    ShowMessage(8);
                    return NodeId.Empty;

            }
        }


        ShowMessage(9);
        return user.NodeId;
    }

    private void ApplyGroups(User_21CFR user)
    {
        Panel groupsPanel = Owner.Get<Panel>("HorizontalLayout1/GroupsPanel1");
        IUAVariable editable = groupsPanel.GetVariable("Editable");
        IUANode groups = groupsPanel.GetAlias("Groups");
        var panel = groupsPanel.Children.Get("ScrollView").Get("Container");

        if (editable.Value == false)
            return;

        if (user == null || groups == null || panel == null)
            return;

        var userNode = InformationModel.Get(user.NodeId);
        if (userNode == null)
            return;

        var groupCheckBoxes = panel.Refs.GetObjects(OpcUa.ReferenceTypes.HasOrderedComponent, false);
        //string usergroupsselected = "";
        List<string> _usergroupsselected = new List<string>();
        List<string> _usergroupsremoved = new List<string>();
        bool GroupChangeDetected = false;

        foreach (var groupCheckBoxNode in groupCheckBoxes)
        {
            var group = groups.Get(groupCheckBoxNode.BrowseName);
            if (group == null)
                return;

            bool userHasGroup = UserHasGroup(user, group.NodeId);

            if (groupCheckBoxNode.GetVariable("Checked").Value && !userHasGroup)
            {
                userNode.Refs.AddReference(FTOptix.Core.ReferenceTypes.HasGroup, group);
                GroupChangeDetected = true;
                _usergroupsselected.Add(group.BrowseName);
            }
            else if (!groupCheckBoxNode.GetVariable("Checked").Value && userHasGroup)
            {
                userNode.Refs.RemoveReference(FTOptix.Core.ReferenceTypes.HasGroup, group.NodeId, false);
                GroupChangeDetected = true;
                _usergroupsremoved.Add(group.BrowseName);
            }

            if (groupCheckBoxNode.GetVariable("Checked").Value)
            {
                // usergroupsselected = usergroupsselected + group.BrowseName + ",";

            }
        }

        //-----------Customized Logic Start-----------------
        if (GroupChangeDetected)
        {
            int NoOfGroups = Project.Current.GetVariable("UI/UserObjects/UserGroups/UserGroupCounts").Value;
            string[] UGroupName = Project.Current.GetVariable("UI/UserObjects/UserGroups/UserGroupName").Value;
            int cnt = NoOfGroups;
            int useraddedfirstcnt = 0;
            int userremovedfirstcnt = 0;


            //         while (cnt > 0)
            //{
            //	if (usergroupsselected.Contains(UGroupName[cnt-1]))
            //	{
            //		usergroupsselected = UGroupName[cnt-1];
            //		break;
            //	}
            //	else if (cnt == 1)
            //	{
            //		usergroupsselected = "not assigned";
            //		break;
            //	}
            //	cnt -= 1;
            //}
            StringBuilder usergroupadded = new StringBuilder("");
            StringBuilder usergroupremoved = new StringBuilder("");

            if (_usergroupsselected.Count > 0)
            {

                foreach (var group in _usergroupsselected)
                {
                    if (useraddedfirstcnt == 0)
                    {
                        usergroupadded.Append(group);
                    }
                    else if (useraddedfirstcnt > 0)
                    {
                        usergroupadded.Append("," + group);

                    }

                    useraddedfirstcnt++;


                }
                AuditTrailLogging GroupChange = new AuditTrailLogging();
                GroupChange.LogIntoAudit("User modified", "'" + user.BrowseName + "'" + " added to '" + usergroupadded + "'" + " group", Session.User.BrowseName, "UserGroupChangeEvent");

            }

            if (_usergroupsremoved.Count > 0)
            {

                foreach (var group in _usergroupsremoved)
                {
                    if (userremovedfirstcnt == 0)
                    {
                        usergroupremoved.Append(group);
                    }
                    else if (userremovedfirstcnt > 0)
                    {
                        usergroupremoved.Append("," + group);

                    }

                    userremovedfirstcnt++;


                }
                AuditTrailLogging GroupChange = new AuditTrailLogging();
                GroupChange.LogIntoAudit("User modified", "'" + user.BrowseName + "'" + " removed from '" + usergroupremoved + "'" + " group", Session.User.BrowseName, "UserGroupChangeEvent");

            }



            // User Group Change Activity Logging into Audit Database
            //         AuditTrailLogging GroupChange = new AuditTrailLogging();
            //GroupChange.LogIntoAudit("User modified", "'" + user.BrowseName + "'" + " group changed to '" + usergroupsselected + "'", Session.User.BrowseName, "UserGroupChangeEvent");

        }
        //-----------Customized Logic End-------------------

    }

    private bool UserHasGroup(IUANode user, NodeId groupNodeId)
    {
        if (user == null)
            return false;
        var userGroups = user.Refs.GetObjects(FTOptix.Core.ReferenceTypes.HasGroup, false);
        foreach (var userGroup in userGroups)
        {
            if (userGroup.NodeId == groupNodeId)
                return true;
        }
        return false;
    }

    private IUANode GetUsers()
    {
        var pathResolverResult = LogicObject.Context.ResolvePath(LogicObject, "{Users}");
        if (pathResolverResult == null)
            return null;
        if (pathResolverResult.ResolvedNode == null)
            return null;

        return pathResolverResult.ResolvedNode;
    }

    private void ShowMessage(int message)
    {
        var errorMessageVariable = LogicObject.GetVariable("ErrorMessage");
        if (errorMessageVariable != null)
            errorMessageVariable.Value = message;

        if (delayedTask != null)
            delayedTask?.Dispose();

        delayedTask = new DelayedTask(DelayedAction, 5000, LogicObject);
        delayedTask.Start();
    }

    private void DelayedAction(DelayedTask task)
    {
        if (task.IsCancellationRequested)
            return;

        var errorMessageVariable = LogicObject.GetVariable("ErrorMessage");
        if (errorMessageVariable != null)
        {
            errorMessageVariable.Value = 0;
        }
        delayedTask?.Dispose();
    }

    private DelayedTask delayedTask;
}

