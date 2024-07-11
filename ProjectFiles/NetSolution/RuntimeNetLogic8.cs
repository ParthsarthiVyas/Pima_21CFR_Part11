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
using FTOptix.RAEtherNetIP;
using FTOptix.Retentivity;
using FTOptix.AuditSigning;
using FTOptix.CommunicationDriver;
using FTOptix.Core;
using System.Diagnostics;

#endregion

public class RuntimeNetLogic8 : BaseNetLogic
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
    public static void Main(string[] arg)
    {
        string pdfFilePath = Project.Current.GetVariable("Model/Variable1").Value; ;
        // string sumatraPDFPath = @"C:\SumatraPDF\SumatraPDF.exe";
        string sumatraPDFPath = @"C:\Program Files\Adobe\Acrobat DC\Acrobat\Acrobat.exe";

        PrintPDF(pdfFilePath, sumatraPDFPath);
    }

      static void PrintPDF(string filePath, string sumatraPDFPath)
    {
        filePath = Project.Current.GetVariable("Model/Variable1").Value;

        try
        {
            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = sumatraPDFPath,
               // Arguments = $"-print-to-default \"{filePath}\"",
                Arguments = $"/s /o /h /t \"{filePath}\"",
                CreateNoWindow = true,
                UseShellExecute = false
            };

            using (Process process = new Process { StartInfo = psi })
            {
                process.Start();
                process.WaitForExit();
            }

            Console.WriteLine("PDF printed successfully.");
            Project.Current.GetVariable("Model/Variable2").Value = "printing initiated successfully";
        }
        catch (Exception ex)
        {
            Project.Current.GetVariable("Model/Variable2").Value = ($"An error occurred while printing the PDF: {ex.Message}");
            Console.WriteLine($"An error occurred while printing the PDF: {ex.Message}");
        }
    }
}


