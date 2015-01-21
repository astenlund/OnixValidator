using System;

namespace OnixValidator
{
    using System.Threading;

    public sealed class Model
    {
        public event EventHandler<EventArgs> StatusChanged;
        public event EventHandler<EventArgs> ProgressChanged;

        private Status _currentStatus;

        private double _percentDone;

        public enum Status
        {
            Idle,
            Working
        }

        public Status CurrentStatus
        {
            get
            {
                return _currentStatus;
            }

            private set
            {
                _currentStatus = value;
                OnStatusChanged();
            }
        }

        public double PercentDone
        {
            get
            {
                return _percentDone;
            }

            private set
            {
                _percentDone = value;
                OnProgressChanged();
            }
        }

        public string ValidateOnix(string filePath, CancellationToken ct, out bool success)
        {
            // Set initial state
            {
                success = false;
                PercentDone = 0d;
                CurrentStatus = Status.Working;
            }

            // TODO: Replace with real work
            const int Upper = 100;
            for (var i = 0; i < Upper; i++)
            {
                if (ct.IsCancellationRequested)
                {
                    CurrentStatus = Status.Idle;
                    return string.Empty;
                }

                Thread.Sleep(20);

                // Update progress
                PercentDone = ((i + 1d) / Upper) * 100;
            }

            // Set finished state
            {
                success = true;
                PercentDone = 100d;
                CurrentStatus = Status.Idle;
            }
            
            // TODO: Replace with real return value
            return filePath;
        }

        private void OnStatusChanged()
        {
            var handler = StatusChanged;
            if (handler != null) handler(this, EventArgs.Empty);
        }

        private void OnProgressChanged()
        {
            var handler = ProgressChanged;
            if (handler != null) handler(this, EventArgs.Empty);
        }
    }
}
