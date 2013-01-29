using System;
using System.Linq;
using System.Collections.ObjectModel;
using System.Collections.Generic;

namespace Irish_Cinema_Listings.Models
{
    public class ModelItem
    {
        public String Name { get; set; }
        public String Id { get; set; }
        public String Url { get; set; }

        public static Collection<Dictionary<String, String>> SortItems(Collection<Dictionary<String, String>> unsortedItems, String key)
        {
            var orderedlist = unsortedItems.OrderBy(k => k[key]);
            var sortedItems = new Collection<Dictionary<String, String>>();
            foreach (Dictionary<String, String> item in orderedlist)
            {
                sortedItems.Add(item);
            }
            return sortedItems;
        }
    }
}
