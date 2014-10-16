﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace LauncherTwo
{
    public static class LaunchTools
    {
        private static string Arguments = string.Empty;
        public static readonly string INI_PATH = "-ini:UDKGame:DefaultPlayer.Name=";
        public static readonly string EXE_PATH = "\\Binaries\\Win32\\UDK.exe";
        static Process LastRunprocess;

        public static string GetArguments(string anIPAdress, string Username, string aPassword = "")
        {
            Arguments = anIPAdress;
            if (aPassword != "")
                Arguments += "?PASSWORD=" + aPassword;
            Arguments += " " + INI_PATH + Username;
            return Arguments;
        }

        public static async Task<bool> LaunchGameWithArgumentsAsync(string Arguments)
        {
            try
            {
                Process UDKProcess = new Process();
                LastRunprocess = UDKProcess;
                UDKProcess.StartInfo.FileName = GetPath();
                UDKProcess.StartInfo.Arguments = Arguments;
                TaskCompletionSource<object> tcs = new TaskCompletionSource<object>();
                UDKProcess.EnableRaisingEvents = true;
                UDKProcess.Exited += (sender, e) => { tcs.SetResult(null); };
                UDKProcess.Start();
                await tcs.Task;
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
            finally
            {
                LastRunprocess = null;
            }
        }

        public static async Task<bool> LaunchGameAsync(string Username)
        {
            return await LaunchGameWithArgumentsAsync(INI_PATH + Username);
        }

        public static async Task<bool> JoinServerAsync(string anIPAdress, string Username, string aPassword = "")
        {
            Arguments = GetArguments(anIPAdress, Username, aPassword);
            return await LaunchGameWithArgumentsAsync(Arguments);
        }

        public static bool LastRunStillRunning()
        {
            return LastRunprocess != null;
        }

        private static string GetPath()
        {
            string FileName = string.Empty; 
            // Exe location
            FileName += System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            // Go up one directory.
            FileName = Directory.GetParent(FileName).FullName;
            // Now into UDK.exe location.
            FileName += EXE_PATH;

            return FileName; 
        }
    }
}
