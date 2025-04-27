using System.Diagnostics;
using System.Text.Json;


namespace nBanks.Application
{    public class PythonRunner
    {
        public static async Task<string> RunPythonScriptAsync(string scriptPath, List<string> fileIds, string question)
        {
            var fileIdsJson = JsonSerializer.Serialize(fileIds);

            var psi = new ProcessStartInfo
            {
                FileName = "python",
                Arguments = $"\"{scriptPath}\" \"{fileIdsJson}\" \"{question}\"",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            var process = new Process
            {
                StartInfo = psi
            };

            process.Start();

            string output = await process.StandardOutput.ReadToEndAsync();
            string errors = await process.StandardError.ReadToEndAsync();

            process.WaitForExit();

            if (!string.IsNullOrEmpty(errors))
            {
                throw new Exception($"Python execution error: {errors}");
            }

            return output.Trim();
        }

        public static async Task EmbedAndStoreDocumentAsync(string scriptPath, string fileId)
        {
            var psi = new ProcessStartInfo
            {
                FileName = "python",
                Arguments = $"\"{scriptPath}\" \"{fileId}\"",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            var process = new Process
            {
                StartInfo = psi
            };

            process.Start();

            string output = await process.StandardOutput.ReadToEndAsync();
            string errors = await process.StandardError.ReadToEndAsync();

            process.WaitForExit();

            if (!string.IsNullOrEmpty(errors))
            {
                throw new Exception($"Python execution error: {errors}");
            }

            Console.WriteLine($"Python script output: {output}");
        }

    }
}