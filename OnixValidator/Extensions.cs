namespace OnixValidator
{
    using System;
    using System.Windows;

    public static class Extensions
    {
        public static string[] GetPaths(this IDataObject data)
        {
            var sourceArray = (string[])data.GetData(DataFormats.FileDrop);
            var destinationArray = new string[sourceArray.Length];

            Array.Copy(sourceArray, destinationArray, sourceArray.Length);

            return destinationArray;
        }
    }
}
