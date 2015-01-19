using System;

namespace OnixValidator
{
    public class StatusChangedEventArgs : EventArgs
    {
        public StatusChangedEventArgs(Model.Status newStatus)
        {
            NewStatus = newStatus;
        }

        public Model.Status NewStatus { get; private set; }
    }
}
