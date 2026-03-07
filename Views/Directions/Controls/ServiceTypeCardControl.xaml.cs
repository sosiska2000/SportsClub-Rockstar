using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Rockstar.Admin.WPF.Models;

namespace Rockstar.Admin.WPF.Views.Directions.Controls
{
    public partial class ServiceTypeCardControl : UserControl, INotifyPropertyChanged
    {
        public static readonly DependencyProperty ServiceTypeProperty =
            DependencyProperty.Register(nameof(ServiceType), typeof(ServiceType), typeof(ServiceTypeCardControl),
                new PropertyMetadata(null, OnServiceTypeChanged));

        public static readonly DependencyProperty EditCommandProperty =
            DependencyProperty.Register(nameof(EditCommand), typeof(ICommand), typeof(ServiceTypeCardControl));

        public static readonly DependencyProperty DeleteCommandProperty =
            DependencyProperty.Register(nameof(DeleteCommand), typeof(ICommand), typeof(ServiceTypeCardControl));

        public ServiceTypeCardControl()
        {
            InitializeComponent();
        }

        public ServiceType? ServiceType
        {
            get => (ServiceType?)GetValue(ServiceTypeProperty);
            set => SetValue(ServiceTypeProperty, value);
        }

        public ICommand? EditCommand
        {
            get => (ICommand?)GetValue(EditCommandProperty);
            set => SetValue(EditCommandProperty, value);
        }

        public ICommand? DeleteCommand
        {
            get => (ICommand?)GetValue(DeleteCommandProperty);
            set => SetValue(DeleteCommandProperty, value);
        }

        public string TypeName => ServiceType?.Name ?? string.Empty;

        public string TypeDescription => ServiceType?.Description ?? string.Empty;

        public string DurationText => ServiceType != null
            ? $"По умолчанию: {ServiceType.DefaultDuration} мин"
            : string.Empty;

        private static void OnServiceTypeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ServiceTypeCardControl control)
            {
                control.OnPropertyChanged(nameof(TypeName));
                control.OnPropertyChanged(nameof(TypeDescription));
                control.OnPropertyChanged(nameof(DurationText));
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}