using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ChiaPlottStatus.Logic.Utils
{
    public class SearchFilter
    {

        /**
         * Splits searchString into keywords and each keyword must be contained in at least one
         * property of an item for the item to be included in the return list.
         * Uses Reflection, which is more than fast enough for our usecase.
         */
        public static List<T> Search<T>(string? searchString, List<T> items)
        {
            if (items.Count == 0 || string.IsNullOrEmpty(searchString))
            {
                return items;
            }

            List<T> result = new();
            string[] keywords = searchString.ToLower().Split(" ");
            PropertyInfo[] properties = typeof(T).GetProperties();
            foreach (var item in items)
            {
                bool match = true;
                foreach (var keyword in keywords)
                {
                    bool keywordMatch = false;
                    foreach (PropertyInfo property in properties)
                    {
                        object? value = property.GetValue(item);
                        if (value != null)
                        {
                            string? valueStr = value.ToString().ToLower();
                            if (!string.IsNullOrEmpty(valueStr) && valueStr.Contains(keyword))
                            {
                                keywordMatch = true;
                                break;
                            }
                        }
                    }
                    if (!keywordMatch)
                    {
                        match = false;
                        break;
                    }
                }
                if (match)
                    result.Add(item);
            }
            return result;
        }
    }
}
