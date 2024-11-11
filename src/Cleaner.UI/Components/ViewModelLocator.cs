using Cleaner.UI.Viewers;

using CommunityToolkit.Mvvm.DependencyInjection;

namespace Cleaner.UI.Components
{
    public sealed class ViewModelLocator
    {
        public ShellViewModel? ShellViewModel
            => Ioc.Default.GetService<ShellViewModel>();

        public SettingViewModel? SettingViewModel
            => Ioc.Default.GetService<SettingViewModel>();
    }
}