using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using Irish_Cinema_Listings.Models;
using System.Collections.Generic;

namespace Irish_Cinema_Listings.Utils
{
    public class JsonUtils
    {
        public static Collection<Dictionary<String, String>> GetItems(String jsonStr, String[] attributes)
        {
            var unsortedItems = new Collection<Dictionary<String, String>>();

            foreach (String itemStr in jsonStr.Split(new string[] { "},{" }, StringSplitOptions.RemoveEmptyEntries))
            {
                var modelItems = new Dictionary<String, String>();
                foreach (String attr in attributes)
                {
                    String jsonAttr = "\"" + attr + "\":\"";
                    string attrVal = GetAttr(itemStr, jsonAttr);
                    modelItems.Add(attr, attrVal);

                    // string url = HttpUtility.HtmlDecode(HttpUtility.HtmlDecode(countyStr.Substring(startPos, (endPos - startPos))));
                }
                unsortedItems.Add(modelItems);
            }

            //Collection<ModelItem> sortedItems = ModelItem.SortItems(unsortedItems);
            return unsortedItems;
        }

        public static String StripSlashes(String encodedStr)
        {
            return encodedStr.Replace("\\/", "/");
        }

        private static String GetAttr(String itemStr, String attr)
        {
            string value = "";
            if (itemStr.Contains(attr))
            {
                int startPos = itemStr.IndexOf(attr) + attr.Length;
                int endPos = itemStr.IndexOf("\"", startPos);
                value = itemStr.Substring(startPos, (endPos - startPos));
            }
            return value;
        }
    }
}
