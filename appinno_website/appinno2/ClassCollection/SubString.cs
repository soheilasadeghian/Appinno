using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace appinno2.ClassCollection
{
    public class SubString
    {
    public static string SubStringHtml(object InputHtml, object StartIndex, object Length)
    {
        return SubStringText(GetPlainTextFromHtml(InputHtml.ToString()), StartIndex, Length);
    }

    public static string SubStringText(object InputText, object StartIndex, object Length)
    {
        string StrText = InputText.ToString();
        int StrLenght = Convert.ToInt32(Length);
        if (StrText.Length > StrLenght)
        {
            return StrText.Substring(Convert.ToInt32(StartIndex), StrLenght) + "...";
        }
        else
        {
            return StrText;
        }
    }

    public static string GetPlainTextFromHtml(string Html)
    {
        try
        {
            return System.Text.RegularExpressions.Regex.Replace(Html, "<[^>]*>", string.Empty);
        }
        catch { return ""; }
    }
    public static string GetTag(string value, string tag)
    {
        Match m = Regex.Match(value, "<" + tag + ">" + @"\s*(.+?)\s*" + "</" + tag + ">");
        if (m.Success)
        {
            return m.Groups[1].Value;
        }
        else
        {
            return "";
        }
    }

    }

}