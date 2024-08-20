﻿using System.Collections.Generic;
using System;
using System.IO;
using System.Windows.Forms;
using System.Linq;

namespace QobuzDownloaderX.Shared
{
    public class DownloadLogger
    {
        public readonly string downloadErrorLogPath = Path.Combine(Globals.LoggingDir, "Download_Errors.log");
        public delegate void DownloadEnded();
        private readonly DownloadEnded updateUiOnDownloadEnd;

        public string DownloadLogPath { get; set; }

        public DownloadLogger(TextBox outputTextBox, DownloadEnded updateUiOnDownloadEnd)
        {
            this.updateUiOnDownloadEnd = updateUiOnDownloadEnd;
        }

        public void RemovePreviousErrorLog()
        {
            // Remove previous download error log
            if (System.IO.File.Exists(downloadErrorLogPath))
            {
                System.IO.File.Delete(downloadErrorLogPath);
            }
        }

        /// <summary>
        /// Add string to download log file, screen or both.<br/>
        /// When a line is written to the log file, a timestamp is added as prefix for non blank lines.<br/>
        /// For writing to log file, blank lines are filtered exept if the given string starts with a blank line.<br/>
        /// Use AddEmptyDownloadLogLine to create empty devider lines in the download log.
        /// </summary>
        /// <param name="logEntry">String to be logged</param>
        /// <param name="logToFile">Should string be logged to file?</param>
        /// <param name="logToScreen">Should string be logged to screen?</param>
        public void AddDownloadLogLine(string logEntry, bool logToFile, bool logToScreen = false)
        {
            if (string.IsNullOrEmpty(logEntry)) return;

            //if (logToScreen) ScreenOutputTextBox?.Invoke(new Action(() => ScreenOutputTextBox.AppendText(logEntry)));

            Console.WriteLine(logEntry);

            if (logToFile)
            {
                var logEntries = logEntry.Split(new[] { Environment.NewLine }, StringSplitOptions.None)
                    .Select(logLine => string.IsNullOrWhiteSpace(logLine) ? logLine : $"{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff} : {logLine}");

                // Filter out all empty lines exept if the logEntry started with an empty line to avoid blank lines for each newline in UI
                var filteredLogEntries = logEntries.Aggregate(new List<string>(), (accumulator, current) =>
                {
                    if (accumulator.Count == 0 || !string.IsNullOrWhiteSpace(current))
                    {
                        accumulator.Add(current);
                    }

                    return accumulator;
                });

                System.IO.File.AppendAllLines(DownloadLogPath, filteredLogEntries);
            }
        }

        /// <summary>
        /// Convenience method to add [ERROR] prefix to logged string before calling AddDownloadLogLine.
        /// </summary>
        /// <param name="logEntry">String to be logged</param>
        /// <param name="logToFile">Should string be logged to file?</param>
        /// <param name="logToScreen">Should string be logged to screen?</param>
        public void AddDownloadLogErrorLine(string logEntry, bool logToFile, bool logToScreen = false)
        {
            AddDownloadLogLine($"[ERROR] {logEntry}", logToFile, logToScreen);
        }

        /// <summary>
        /// Convenience method to add empty spacing line to log.
        /// </summary>
        /// <param name="logToFile">Should string be logged to file?</param>
        /// <param name="logToScreen">Should string be logged to screen?</param>
        public void AddEmptyDownloadLogLine(bool logToFile, bool logToScreen = false)
        {
            AddDownloadLogLine($"{Environment.NewLine}{Environment.NewLine}", logToFile, logToScreen);
        }

        public void AddDownloadErrorLogLines(IEnumerable<string> logEntries)
        {
            if (logEntries == null && !logEntries.Any()) return;

            System.IO.File.AppendAllLines(downloadErrorLogPath, logEntries);
        }

        public void AddDownloadErrorLogLine(string logEntry)
        {
            AddDownloadErrorLogLines(new string[] { logEntry });
        }

        /// <summary>
        /// Standardized logging when global download task fails.
        /// After logging, disabled controls are re-enabled.
        /// </summary>
        /// <param name="downloadTaskType">Name of the failed download task</param>
        /// <param name="downloadEx">Exception thrown by task</param>
        public void LogDownloadTaskException(string downloadTaskType, Exception downloadEx)
        {
            // If there is an issue trying to, or during the download, show error info.
            ClearUiLogComponent();
            AddDownloadLogErrorLine($"{downloadTaskType} Download Task ERROR. Details saved to error log.{Environment.NewLine}", true, true);

            AddDownloadErrorLogLine($"{downloadTaskType} Download Task ERROR.");
            AddDownloadErrorLogLine(downloadEx.ToString());
            AddDownloadErrorLogLine(Environment.NewLine);
            updateUiOnDownloadEnd?.Invoke();
        }

        public void LogFinishedDownloadJob(bool noErrorsOccured)
        {
            AddEmptyDownloadLogLine(true, true);

            // Say that downloading is completed.
            if (noErrorsOccured)
            {
                AddDownloadLogLine("Download job completed! All downloaded files will be located in your chosen path.", true, true);
            }
            else
            {
                AddDownloadLogLine("Download job completed with warnings and/or errors! Some or all files could be missing!", true, true);
            }

            updateUiOnDownloadEnd?.Invoke();
        }

        public void ClearUiLogComponent()
        {
            //ScreenOutputTextBox.Invoke(new Action(() => ScreenOutputTextBox.Text = String.Empty));
        }
    }
}