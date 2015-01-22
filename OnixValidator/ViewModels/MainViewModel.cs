namespace OnixValidator.ViewModels
{
    using System;
    using System.ComponentModel;
    using System.IO;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Input;

    using GalaSoft.MvvmLight.Command;

    using OnixValidator.Annotations;
    using OnixValidator.Extensions;
    using OnixValidator.Models;

    public class MainViewModel : INotifyPropertyChanged
    {
        private const string DefaultHeading = "DROP ONIX FILE HERE";
        private const string DefaultResult = "";

        private readonly Model _model = new Model();

        private string _heading = DefaultHeading;
        private string _result = DefaultResult;
        private bool _isPathValid;
        private bool _isResultPersistant;

        private CancellationTokenSource _cts;

        private bool _allowDrop;

        public MainViewModel()
        {
            _model.StatusChanged += Model_OnStatusChanged;
            _model.ProgressChanged += Model_OnProgressChanged;

            AllowDrop = true;

            DropCommand = new RelayCommand<IDataObject>(data =>
                {
                    if (IsWorking || _isPathValid == false)
                    {
                        return;
                    }

                    Heading = "PROCESSING...";

                    _cts = new CancellationTokenSource();

                    var path = data.GetPaths().Single();
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
                });

            DragEnterCommand = new RelayCommand<IDataObject>(data =>
                {
                    if (IsWorking)
                    {
                        return;
                    }

                    _isPathValid = false;

                    var paths = data.GetPaths();
                    if (paths.Length > 1)
                    {
                        Result = "Only a single file can be dropped here";
                        return;
                    }

                    var path = paths.Single();

                    if (Directory.Exists(path))
                    {
                        Result = "Directories are not supported";
                        return;
                    }

                    if (Path.GetExtension(path) != ".xml")
                    {
                        Result = "Only XML files are supported";
                        return;
                    }

                    _isPathValid = true;
                    Result = DefaultResult;
                });

            DragLeaveCommand = new RelayCommand(() =>
                {
                    if (IsWorking)
                    {
                        return;
                    }

                    Result = DefaultResult;
                });

            CancelCommand = new RelayCommand(() =>
                {
                    _cts.Cancel();
                });
        }

        public bool IsWorking
        {
            get { return _model.CurrentStatus == Model.Status.Working; }
        }

        public double ProgressValue
        {
            get { return _model.PercentDone; }
        }

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

        public bool AllowDrop
        {
            get
            {
                return _allowDrop;
            }
            set
            {
                if (value.Equals(_allowDrop))
                {
                    return;
                }
                _allowDrop = value;
                OnPropertyChanged();
            }
        }

        public RelayCommand<IDataObject> DropCommand { get; private set; }

        public RelayCommand<IDataObject> DragEnterCommand { get; private set; }

        public RelayCommand DragLeaveCommand { get; private set; }

        public ICommand CancelCommand { get; private set; }

        private void Finish()
        {
            AllowDrop = false;
            _isResultPersistant = true;
        }

        private void Model_OnStatusChanged(object sender, EventArgs statusChangedEventArgs)
        {
            OnPropertyChanged("IsWorking");
        }

        private void Model_OnProgressChanged(object sender, EventArgs eventArgs)
        {
            OnPropertyChanged("ProgressValue");
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
