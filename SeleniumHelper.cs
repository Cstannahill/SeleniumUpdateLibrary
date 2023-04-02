using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Compression;
using System.IO;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace SeleniumUpdateLibrary
{
    public class SeleniumHelper
    {
        public static async Task<string> UpdateChromeDriverIfNeeded()
        {
            string versionInfo;
            bool isSuccessful = false;
            string currentVersion = await GetCurrentVersionAsync();
            Console.WriteLine($"{currentVersion} is your current version of chromeDriver.");
            string latestVersion = await GetLatestVersionAsync();
            Console.WriteLine($"{latestVersion} is the latest version of chromeDriver.");
            if (currentVersion != latestVersion)
            {
                try
                {
                    SeleniumHelper manager = new SeleniumHelper();
                    bool latest = await manager.DownloadLatestVersion(latestVersion);

                    isSuccessful = true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            else
            {
                Console.WriteLine("Your Driver is up to date.");
            }
            versionInfo = $"Current Version: {currentVersion}; Latest Version: {latestVersion}; Your updated was successful: {isSuccessful}";
            return versionInfo;
        }

        public static async Task<string> GetCurrentVersionAsync()
        {
            string driverPath = GetDriverPath();
            ProcessStartInfo psi = new ProcessStartInfo(driverPath)
            {
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true,
                Arguments = "--version"
            };

            Process process = new Process { StartInfo = psi };
            process.Start();

            string output = await process.StandardOutput.ReadToEndAsync();
            process.WaitForExit();

            string version = output.Split(' ')[1].Trim();
            return version;
        }

        private static async Task<string> GetLatestVersionAsync()
        {
            using (HttpClient httpClient = new HttpClient())
            {
                HttpResponseMessage response = await httpClient.GetAsync("https://chromedriver.storage.googleapis.com/LATEST_RELEASE");
                response.EnsureSuccessStatusCode();
                string latestVersion = await response.Content.ReadAsStringAsync();
                return latestVersion;
            }
        }

        private async Task<bool> DownloadLatestVersion(string latestVersion)
        {
            string url = $"https://chromedriver.storage.googleapis.com/{latestVersion}/chromedriver_win32.zip";
            string tempPath = Path.GetTempPath();
            string zipPath = Path.Combine(tempPath, "chromedriver_win32.zip");
            string extractPath = Path.Combine(tempPath, "chromedriver_win32");
            string newDriverPath = Path.Combine(extractPath, "chromedriver.exe");
            string currentDriverPath = GetDriverPath();

            // Download the latest ChromeDriver ZIP
            using (var httpClient = new HttpClient())
            {
                var response = await httpClient.GetAsync(url);
                using (var fileStream = new FileStream(zipPath, FileMode.Create, FileAccess.Write))
                {
                    await response.Content.CopyToAsync(fileStream);
                }
            }

            // Extract the ZIP
            ZipFile.ExtractToDirectory(zipPath, extractPath);

            // Delete the old ChromeDriver if it exists
            if (File.Exists(currentDriverPath))
            {
                File.Delete(currentDriverPath);
            }

            // Move the new ChromeDriver to the desired location
            File.Move(newDriverPath, currentDriverPath);

            // Cleanup: Delete the downloaded ZIP and extracted folder
            File.Delete(zipPath);
            Directory.Delete(extractPath, true);

            return File.Exists(currentDriverPath);
        }

        private static string GetDriverPath()
        {
            string currentPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string driverPath = Path.Combine(currentPath, "chromedriver.exe");
            return driverPath;
        }
        public async static void UpdateDriver()
        {
            try

            {
                string updateResult = Task.Run(() => SeleniumHelper.UpdateChromeDriverIfNeeded()).Result;
                string updatedVersion = Task.Run(() => SeleniumHelper.GetCurrentVersionAsync()).Result;
                Console.WriteLine($"Current Version after update is {updatedVersion}");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            //try to pop the browser using selenium to verify the update fixed the issue
            await TestSel();

        }

        private static async Task TestSel()
        {
            using (ChromeDriver driver = new ChromeDriver())
            {

                driver.Navigate().GoToUrl("https://www.google.com");
                await Task.Delay(3000);
                IWebElement searchBox = driver.FindElement(By.Name("q")); 
                await Task.Delay(1000);
                searchBox.SendKeys("Your Selenium Seems to be working. Cool. Cool Cool Cool");
                await Task.Delay(2000);
                searchBox.Submit();
                await Task.Delay(1000);
                driver.Quit();
            }
        }
    }
}
