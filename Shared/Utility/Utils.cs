using Microsoft.CSharp.RuntimeBinder;

namespace GestionProyectos.Shared.Utility
{
    public class Utils
    {
        public static List<T> ConvertDynamicToList<T>(dynamic dynamicList)
        {
            try
            {
                return (List<T>)dynamicList;
            }
            catch (RuntimeBinderException)
            {
                return new List<T>();
            }
        }
        public static dynamic GetFormatedValueForDb(Type dbValueType, dynamic dbValue)
        {
            if (dbValueType.Equals(typeof(long)))
                return (long)dbValue;
            else if (dbValueType.Equals(typeof(int)))
                return (int)dbValue;
            else if (dbValueType.Equals(typeof(short)))
                return (short)dbValue;
            else if (dbValueType.Equals(typeof(decimal)))
                return (decimal)dbValue;
            else if (dbValueType.Equals(typeof(bool)))
                return (bool)dbValue;
            else if (dbValueType.Equals(typeof(byte)))
                return (byte)dbValue;
            else if (dbValueType.Equals(typeof(DateTime)))
                return $"'{dbValue.ToString("yyyy-MM-dd HH:mm:ss.fff")}'";
            else
                return $"'{dbValue}'";
        }
    }
}
