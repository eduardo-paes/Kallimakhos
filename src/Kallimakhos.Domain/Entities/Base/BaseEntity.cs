using System.Diagnostics;

namespace Kallimakhos.Domain.Entities.Base
{
    public abstract class BaseEntity
    {
        /// <summary>
        /// Executes a process and waits for it to finish.
        /// </summary>
        /// <param name="fileName">The name of the file to execute.</param>
        /// <param name="command">The command to execute.</param>
        /// <exception cref="Exception">Thrown when the process returns an error.</exception>
        internal void ExecuteProcess(string fileName, string command)
        {
            // Create a process to execute the command
            Process process = new()
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = fileName,
                    Arguments = command,
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                }
            };

            // Start the process
            process.Start();

            // Wait for the process to finish
            process.WaitForExit();

            // Check the exit code to see if the process completed successfully
            int exitCode = process.ExitCode;
            if (exitCode != 0)
            {
                throw new Exception($"Error: {exitCode}");
            }

            // Close the process and release resources
            process.Close();
        }
    }
}