namespace OnixValidator.Views
{
    using System.Windows;
    using System.Windows.Input;

    using OnixValidator.ViewModels;

    public sealed partial class MainWindow
    {
        private MainViewModel _viewModel;

        public MainWindow()
        {
            Loaded += OnLoaded;
            InitializeComponent();
        }

        private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            _viewModel = DataContext as MainViewModel;
        }

        private void OnDragEnter(object sender, DragEventArgs e)
        {
            _viewModel.DragEnterCommand.Execute(e.Data);
        }

        private void OnDragLeave(object sender, DragEventArgs e)
        {
            _viewModel.DragLeaveCommand.Execute(null);
        }

        private void OnMouseLeave(object sender, MouseEventArgs e)
        {
            _viewModel.DragLeaveCommand.Execute(null);
        }

        private void OnDrop(object sender, DragEventArgs e)
        {
            _viewModel.DropCommand.Execute(e.Data);
        }
    }
}
