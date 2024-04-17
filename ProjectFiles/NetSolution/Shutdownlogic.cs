#region Using directives
using System;
using UAManagedCore;
using OpcUa = UAManagedCore.OpcUa;
using FTOptix.EventLogger;
using FTOptix.HMIProject;
using FTOptix.Retentivity;
using FTOptix.NetLogic;
using FTOptix.UI;
using FTOptix.NativeUI;
using FTOptix.Store;
using FTOptix.ODBCStore;
using FTOptix.CoreBase;
using FTOptix.AuditSigning;
using FTOptix.CommunicationDriver;
using FTOptix.Core;
using FTOptix.Alarm;
using System.Diagnostics;
using FTOptix.DataLogger;
using FTOptix.Report;
using FTOptix.Recipe;
using FTOptix.SQLiteStore;
using FTOptix.System;
#endregion

public class Shutdownlogic : BaseNetLogic
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
    public void shutdownipc()
    {

        try
        {
            // Create a new process to run the shutdown command
            Process process = new Process();
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = "shutdown",
                Arguments = "/s /f /t 0", // /f forces running applications to close, /t sets the delay to 0 seconds
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };
            process.StartInfo = startInfo;

            // Start the process
            process.Start();

            // Wait for the process to exit
            process.WaitForExit();

            // Optionally, you can read the output and error streams
            string output = process.StandardOutput.ReadToEnd();
            string error = process.StandardError.ReadToEnd();

            // Display the output and error messages if needed
            Console.WriteLine($"Output: {output}");
            Console.WriteLine($"Error: {error}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }
    }
}


