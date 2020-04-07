using System;
using System.Collections.Generic;
using System.Text;

namespace Rosalyn.Misc
{
    public static class Extensions
    {
        public static string ToHumanReadableString(this TimeSpan timeSpan)
        {
            List<string> pieces = new List<string>();
            if (timeSpan.Days >= 1d)
                pieces.Add($"{timeSpan.Days} days");
            if (timeSpan.Hours >= 1d)
                pieces.Add($"{timeSpan.Hours} hours");
            if (timeSpan.Minutes >= 1d)
                pieces.Add($"{timeSpan.Minutes} minutes");
            if (timeSpan.Seconds >= 1d)
                pieces.Add($"{timeSpan.Seconds} seconds");
            if (timeSpan.Milliseconds >= 1d)
                pieces.Add($"{timeSpan.Milliseconds} milliseconds");
            return String.Join(", ", pieces);
        }
    }
}