using System;

namespace OnixValidator
{
    using System.ComponentModel;
    using System.IO;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Input;

    using Annotations;

    public sealed partial class MainWindow : INotifyPropertyChanged
    {
        private const string DefaultHeading = "DROP ONIX FILE HERE";
        private const string DefaultResult = "";

        private readonly Model _model = new Model();

        private string _heading = DefaultHeading;
        private string _result = DefaultResult;
        private bool _isPathValid;
        private bool _isResultPersistant;

        private CancellationTokenSource _cts;

        public MainWindow()
        {
            _model.StatusChanged += Model_OnStatusChanged;
            InitializeComponent();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public string Heading
        {
            get
            {
                return _heading;
            }
            set
            {
                if (value == _heading)
                {
                    return;
                }
                _heading = value;
                OnPropertyChanged();
            }
        }

        public string Result
        {
            get
            {
                return _result;
            }
            set
            {
                if (value == _result || _isResultPersistant)
                {
                    return;
                }
                _result = value;
                OnPropertyChanged();
            }
        }

        public bool IsWorking
        {
            get { return _model.CurrentStatus == Model.Status.Working; }
        }

        private void Finish()
        {
            AllowDrop = false;
            _isResultPersistant = true;
        }

        private void MainWindow_OnDragEnter(object sender, DragEventArgs e)
        {
            if (IsWorking)
            {
                return;
            }

            _isPathValid = false;

            var paths = e.Data.GetPaths();
            if (paths.Length > 1)
            {
                Result = "Only a single file can be dropped here.";
                return;
            }

            var path = paths.Single();

            if (Directory.Exists(path))
            {
                Result = "Directories are not supported.";
                return;
            }

            if (Path.GetExtension(path) != ".xml")
            {
                Result = "Only XML files are supported.";
                return;
            }

            _isPathValid = true;
            Result = DefaultResult;
        }

        private void Model_OnStatusChanged(object sender, EventArgs statusChangedEventArgs)
        {
            OnPropertyChanged("IsWorking");
        }

        private void MainWindow_OnDragLeave(object sender, DragEventArgs e)
        {
            if (IsWorking)
            {
                return;
            }

            Result = DefaultResult;
        }

        private void MainWindow_OnMouseLeave(object sender, MouseEventArgs e)
        {
            if (IsWorking)
            {
                return;
            }

            Result = DefaultResult;
        }

        private void MainWindow_OnDrop(object sender, DragEventArgs e)
        {
            if (IsWorking || _isPathValid == false)
            {
                return;
            }

            Heading = "PROCESSING...";

            _cts = new CancellationTokenSource();

            var path = e.Data.GetPaths().Single();
            var success = false;

            Task.Run(() => _model.ValidateOnix(path, _cts.Token, out success)).ContinueWith(
                task =>
                    {
                        if (_cts.IsCancellationRequested)
                        {
                            Heading = DefaultHeading;
                            Result = DefaultResult;
                        }
                        else
                        {
                            Result = task.Result;

                            if (success)
                            {
                                Heading = "VALIDATION RESULT";
                                Finish();
                            }
                            else
                            {
                                Heading = DefaultHeading;
                            }
                        }
                    },
                TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void CancelButton_OnClick(object sender, RoutedEventArgs e)
        {
            _cts.Cancel();
        }

        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
