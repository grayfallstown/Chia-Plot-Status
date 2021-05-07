using ChiaPlotStatus.Logic.Models;
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
            // property not found, sort by first property on A whatever it is
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
                string valueAStr = "";
                string valueBStr = "";
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
                    case TypeCode.DateTime:
                    case TypeCode.Object:
                        if (valueA is HealthIndicator)
                            sortIndex = ((HealthIndicator)valueA).SortIndex.CompareTo(((HealthIndicator)valueB).SortIndex);
                        else
                            sortIndex = compare((DateTime?)valueA, (DateTime?)valueB);
                        break;
                    case TypeCode.String:
                        valueAStr = "";
                        valueBStr = "";
                        if (valueA != null)
                            valueAStr = valueA.ToString().ToLower();
                        if (valueB != null)
                            valueBStr = valueB.ToString().ToLower();
                        sortIndex = string.Compare(valueAStr, valueBStr);
                        break;
                    default:
                        Debug.WriteLine("TypeCode " + typeCode);
                        break;
                }

                // keep empty values on the bottom of the table
                if (propertyB != null)
                {
                    // we sort over plotLog first but might hide useless information in plotLogReadable
                    // the sorting does not see this values as empty as it looks into plotLogs first since
                    // the data there is more easily sortable
                    // but on the table we stil see empty values on top of the table as they are hidden.
                    // That is why we sort on propertyB (plotLogReadable/item2) here
                    var valueAPropB = propertyB.GetValue(a.Item2);
                    var valueBPropB = propertyB.GetValue(b.Item2);
                    var valueAProbANullOrEmpty = valueA == null || string.IsNullOrEmpty("" + valueA);
                    var valueBProbANullOrEmpty = valueB == null || string.IsNullOrEmpty("" + valueB);
                    var valueAProbBNullOrEmpty = valueAPropB == null || string.IsNullOrEmpty("" + valueAPropB);
                    var valueBProbBNullOrEmpty = valueBPropB == null || string.IsNullOrEmpty("" + valueBPropB);

                    if ((!valueAProbANullOrEmpty && valueAProbBNullOrEmpty) || (!valueBProbANullOrEmpty && valueBProbBNullOrEmpty))
                    {
                        // this is to sort by propb when values are null in plotlogreadable but not in plotlog
                        if (valueAProbBNullOrEmpty != valueBProbBNullOrEmpty)
                            Debug.WriteLine("asd");
                        if (valueAProbBNullOrEmpty && !valueBProbBNullOrEmpty)
                        {
                            if (sortAsc)
                                sortIndex = 1;
                            else
                                sortIndex = -1;
                        }
                        if (!valueAProbBNullOrEmpty && valueBProbBNullOrEmpty)
                        {
                            if (sortAsc)
                                sortIndex = -1;
                            else
                                sortIndex = 1;
                        }
                    }
                    else
                    {
                        // this is to sort by propa when values are null in plotlog
                        if (valueAProbANullOrEmpty && !valueBProbANullOrEmpty)
                        {
                            if (sortAsc)
                                sortIndex = 1;
                            else
                                sortIndex = -1;
                        }
                        if (!valueAProbANullOrEmpty && valueBProbANullOrEmpty)
                        {
                            if (sortAsc)
                                sortIndex = -1;
                            else
                                sortIndex = 1;
                        }
                    }
                }


                // stabilize sorting so a refresh or sort does not put plot logs with equal values at random
                // positions causing visual "jumping" of table entries.
                // this way sorting stays consistent on equal values across the runtime of the program
                if (sortIndex == 0)
                {
                    sortIndex = a.Item1.GetHashCode().CompareTo(b.Item1.GetHashCode());
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

        private static int compare(DateTime? a, DateTime? b)
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
