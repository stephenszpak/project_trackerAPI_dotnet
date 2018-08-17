using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectTracker.SolutionExtensions
{
    public static class StringConverterExtensions
    {

        public static long ToLong(this object value, long defaultValue = 0L)
        {

            long result = defaultValue;
            if (!long.TryParse(value.ToString().Trim(), out result))
            {
                result = defaultValue;
            }
            return result;
        }

        public static int ToInt(this object value, int defaultValue = 0)
        {
            int result = defaultValue;
            if (!int.TryParse(value.ToString().Trim(), out result))
            {
                result = defaultValue;
            }
            return result;
        }

        public static bool ToBool(this object value, bool defaultValue = false)
        {
            bool result = defaultValue;
            if (!bool.TryParse(value.ToString().Trim(), out result))
            {
                result = defaultValue;
            }

            return result;

        }
        public static decimal ToDecimal(this object value, decimal defaultValue = 0M)
        {
            decimal result = defaultValue;
            if (!decimal.TryParse(value.ToString().Trim(), out result))
            {
                result = defaultValue;
            }

            return result;

        }

        public static DateTime ToDateTime(this object value, DateTime defaultValue = default(DateTime))
        {
            DateTime result = defaultValue;
            if (value != null && !DateTime.TryParse(value.ToString().Trim(), out result))
            {
                result = defaultValue;
            }

            return result;


        }

        public static double ToDouble(this object value, double defaultValue = 0.0D)
        {
            double result = defaultValue;
            if (!double.TryParse(value.ToString().Trim(), out result))
            {
                result = defaultValue;
            }
            return result;
        }
    }
}
