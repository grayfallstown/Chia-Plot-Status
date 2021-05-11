using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ChiaPlotStatus.Logic.Utils
{
    public static class SearchFilter
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


        public static List<(A, B)> Search<A, B>(string? searchString, List<(A, B)> items)
        {
            if (items.Count == 0 || string.IsNullOrEmpty(searchString))
            {
                return items;
            }
            List<A> itemsA = new();
            List<B> itemsB = new();
            foreach (var item in items)
            {
                itemsA.Add(item.Item1);
                itemsB.Add(item.Item2);
            }
            itemsA = Search(searchString, itemsA);
            itemsB = Search(searchString, itemsB);
            List<(A, B)> result = new();
            foreach (var item in items)
                if (itemsA.Contains(item.Item1) || itemsB.Contains(item.Item2))
                    result.Add(item);
            return result;
        }

    }
}
