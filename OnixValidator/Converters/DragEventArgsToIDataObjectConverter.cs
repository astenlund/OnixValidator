namespace OnixValidator.Converters
{
    using System.Windows;

    using GalaSoft.MvvmLight.Command;

    public class DragEventArgsToIDataObjectConverter : IEventArgsConverter
    {
        public object Convert(object value, object parameter)
        {
            return ((DragEventArgs)value).Data;
        }
    }
}
