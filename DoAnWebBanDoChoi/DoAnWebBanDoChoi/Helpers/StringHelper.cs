using System.Text;
using System.Text.RegularExpressions;

namespace DoAnWebBanDoChoi.Helpers
{
    public static class StringHelper
    {
        public static string ToUnsign(string input)
        {
            if (string.IsNullOrEmpty(input)) return "";

            string formD = input.Normalize(NormalizationForm.FormD);
            StringBuilder sb = new StringBuilder();

            foreach (char ch in formD)
            {
                if (System.Globalization.CharUnicodeInfo.GetUnicodeCategory(ch) != System.Globalization.UnicodeCategory.NonSpacingMark)
                    sb.Append(ch);
            }

            return Regex.Replace(sb.ToString().Normalize(NormalizationForm.FormC), @"[^0-9a-zA-Z\s]", "").ToLower();
        }
    }
}
