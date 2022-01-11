using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace GazChap
{
	class Program
	{
        const string program_url = "https://gazchap.com/wsl2-instance-ip/";
        private String hosts_file = System.IO.Path.Combine( Environment.SystemDirectory, "drivers\\etc\\hosts" );
        const String wsl_host_entry = "host.wsl2.internal";

        static void Main( string[] args ) {
            var program = new Program();
            String wsl_ip = program.getWSLIP();
            if ( wsl_ip != "" ) {
                program.updateHosts( wsl_ip );
            }
        }

        String getWSLIP() {
            try {
                string cmdPath = System.IO.Path.Combine( Environment.SystemDirectory, "wsl.exe" );

                System.Diagnostics.Process pProcess = new System.Diagnostics.Process();
                pProcess.StartInfo.FileName = cmdPath;
                pProcess.StartInfo.Arguments = "hostname -I";
                pProcess.StartInfo.UseShellExecute = false;
                pProcess.StartInfo.RedirectStandardOutput = true;
                pProcess.Start();
                string output = pProcess.StandardOutput.ReadToEnd();
                pProcess.WaitForExit();

                string wsl_ip = output.Trim();
                return wsl_ip;
            } catch( Exception e ) {
                errorToConsole( "Unable to start wsl process - is WSL2 installed and in your system path?", e );
                return "";
            }
        }

        void updateHosts( String wsl_ip ) {
            bool entry_exists = false;
            List<String> entries = new List<String>();
            String entry_to_update = wsl_ip + "\t" + wsl_host_entry + "\t# added by WSL2_Instance_IP (" + program_url + ")";
            if ( File.Exists( hosts_file ) ) {
                String[] lines = File.ReadLines( hosts_file ).ToArray();
                for ( int i = 0; i < lines.Length; i++ ) {
                    if ( lines[i].Contains( wsl_host_entry ) ) {
                        lines[i] = entry_to_update;
                        entry_exists = true;
                        break;
                    }
                }
                entries = lines.ToList();
            } else {
                File.Create( hosts_file ).Close();
            }
            if ( !entry_exists ) {
                entries.Add( entry_to_update );
            }
            try {
                File.WriteAllLines( hosts_file, entries );
            } catch ( System.UnauthorizedAccessException e ) {
                errorToConsole( "Access denied when updating hosts file.", e );
                return;
            }
        }

        void errorToConsole( string message, Exception e ) {
            TextWriter errorWriter = Console.Error;
            errorWriter.WriteLine( "WSL2_Instance_IP Error: " + message );
            errorWriter.WriteLine( "\t" + e.Message );
            errorWriter.Close();
        }
    }
}
