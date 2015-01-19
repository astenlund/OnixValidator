using System;

namespace OnixValidator
{
    using System.Threading;

    public sealed class Model
    {
        private Status _currentStatus;

        public event EventHandler<StatusChangedEventArgs> StatusChanged;

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
                OnStatusChanged(value);
            }
        }

        public string ValidateOnix(string filePath, CancellationToken ct, out bool success)
        {
            CurrentStatus = Status.Working;

            success = false;

            // TODO: Replace with real work
            for (var i = 0; i < 200; i++)
            {
                if (ct.IsCancellationRequested)
                {
                    CurrentStatus = Status.Idle;
                    return string.Empty;
                }

                Thread.Sleep(10);
            }

            success = true;

            CurrentStatus = Status.Idle;
            
            // TODO: Replace with real return value
            return filePath;
        }

        private void OnStatusChanged(Status status)
        {
            var handler = StatusChanged;
            if (handler != null) handler(this, new StatusChangedEventArgs(status));
        }
    }
}
