using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ChiaPlotStatus.Logic.Utils
{
    public static class Sorter
    {

        public static void Sort<A, B>(string propertyName, bool sortAsc, List<(A, B)> items)
        {
            PropertyInfo[] propertiesA = typeof(A).GetProperties();
            PropertyInfo[] propertiesB = typeof(B).GetProperties();
            PropertyInfo? propertyA = null;
            PropertyInfo? propertyB = null;
            foreach (var prop in propertiesA)
                if (string.Equals(propertyName, prop.Name))
                    propertyA = prop;
            foreach (var prop in propertiesB)
                if (string.Equals(propertyName, prop.Name))
                    propertyB = prop;

            PropertyInfo? property = propertyA;
            bool sortOverLeftTupleItem = true;
            if (property == null)
            {
                property = propertyB;
                sortOverLeftTupleItem = false;
            }
            if (property == null)
                property = propertiesA[0];
            items.Sort((a, b) =>
            {
                int sortIndex = 0;
                object? valueA = null;
                object? valueB = null;
                if (sortOverLeftTupleItem)
                {
                    valueA = property.GetValue(a.Item1);
                    valueB = property.GetValue(b.Item1);
                }
                else
                {
                    valueA = property.GetValue(a.Item2);
                    valueB = property.GetValue(b.Item2);
                }
                TypeCode typeCode = Type.GetTypeCode(property.PropertyType);
                switch (typeCode)
                {
                    case TypeCode.Int32:
                        sortIndex = compare((int?)valueA, (int?)valueB);
                        break;
                    case TypeCode.Single:
                        sortIndex = compare((Single?)valueA, (Single?)valueB);
                        break;
                    case TypeCode.Decimal:
                    case TypeCode.Double:
                        sortIndex = compare((Decimal?)valueA, (Decimal?)valueB);
                        break;
                    case TypeCode.String:
                        string valueAStr = "";
                        string valueBStr = "";
                        if (valueA != null)
                            valueAStr = valueA.ToString().ToLower();
                        if (valueB != null)
                            valueBStr = valueB.ToString().ToLower();
                        sortIndex = string.Compare(valueAStr, valueBStr);
                        break;
                    default:
                        break;
                }
                if (!sortAsc)
                {
                    sortIndex *= -1;
                }
                return sortIndex;
            });
        }

        private static int compare(Single? a, Single? b)
        {
            if (a == null && b != null)
                return +1;
            if (a != null && b == null)
                return -1;
            if (a == null && b == null)
                return 0;
            if (a > b)
                return +1;
            if (a < b)
                return -1;
            return 0;
        }

        private static int compare(Decimal? a, Decimal? b)
        {
            if (a == null && b != null)
                return +1;
            if (a != null && b == null)
                return -1;
            if (a == null && b == null)
                return 0;
            if (a > b)
                return +1;
            if (a < b)
                return -1;
            return 0;
        }

        private static int compare(int? a, int? b)
        {
            if (a == null && b != null)
                return +1;
            if (a != null && b == null)
                return -1;
            if (a == null && b == null)
                return 0;
            if (a > b)
                return +1;
            if (a < b)
                return -1;
            return 0;
        }
    }
}