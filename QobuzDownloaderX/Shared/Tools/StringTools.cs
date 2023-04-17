﻿using System;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;

namespace QobuzDownloaderX.Shared
{
    internal static class StringTools
    {
        /// <summary>
        /// Decodes the encoded non ascii characters.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The decoded string.</returns>
        public static string DecodeEncodedNonAsciiCharacters(string value)
        {
            if (value != null)
            {
                return Regex.Replace(
                    value,
                    @"\\u(?<Value>[a-zA-Z0-9]{4})",
                    m => ((char)int.Parse(m.Groups["Value"].Value, NumberStyles.HexNumber)).ToString());
            }
            else
            {
                return null;
            }
        }

        // For converting illegal filename characters to an underscore.
        public static string GetSafeFilename(string filename)
        {
            if (filename != null)
            {
                string result = filename.Trim().TrimEnd('.');
                return string.Join("_", result.Split(Path.GetInvalidFileNameChars()));
            }
            else
            {
                return null;
            }
        }

        // Nullsafe trimming of string to given max length with removal of leading and trailing spaces.
        public static string TrimToMaxLength(string text, int maxLength = 36)
        {
            if (text != null)
            {
                string result = text.Trim();
                return result.Substring(0, Math.Min(result.Length, maxLength)).Trim();
            }
            else
            {
                return "";
            }
        }

        public static string FormatDateTimeOffset(DateTimeOffset? dateTimeOffset)
        {
            if (dateTimeOffset != null)
            {
                return dateTimeOffset.GetValueOrDefault().ToString("yyyy-MM-dd");
            }
            else
            {
                return "";
            }
        }

        public static string FormatDurationInSeconds(double durationInSeconds)
        {
            TimeSpan duration = TimeSpan.FromSeconds(durationInSeconds);

            return duration.TotalHours < 1 ?
                $"{duration:mm\\:ss}" :
                $"{duration:hh\\:mm\\:ss}";

        }
    }
}