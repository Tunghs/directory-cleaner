using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;

namespace Cleaner.Common.Bases
{
    public abstract class ViewModelBase : ObservableObject
    {
        public ViewModelBase()
        { }

        public virtual void Cleanup()
        {
            WeakReferenceMessenger.Default.Cleanup();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                Cleanup();
            }
        }
    }
}