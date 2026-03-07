using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Rockstar.Admin.WPF.Models;

namespace Rockstar.Admin.WPF.Views.Directions.Controls
{
    public partial class ServiceCardControl : UserControl, INotifyPropertyChanged
    {
        public static readonly DependencyProperty ServiceProperty =
            DependencyProperty.Register(nameof(Service), typeof(Service), typeof(ServiceCardControl),
                new PropertyMetadata(null, OnServiceChanged));

        public static readonly DependencyProperty EditCommandProperty =
            DependencyProperty.Register(nameof(EditCommand), typeof(ICommand), typeof(ServiceCardControl));

        public static readonly DependencyProperty DeleteCommandProperty =
            DependencyProperty.Register(nameof(DeleteCommand), typeof(ICommand), typeof(ServiceCardControl));

        public ServiceCardControl()
        {
            InitializeComponent();
        }

        public Service? Service
        {
            get => (Service?)GetValue(ServiceProperty);
            set => SetValue(ServiceProperty, value);
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

        public string ServiceName => Service?.Name ?? string.Empty;

        public string PriceText => Service != null ? $"{Service.Price:C}" : string.Empty;

        public string SessionsText => Service?.SessionsCount > 1
            ? $"{Service.SessionsCount} занятий"
            : "Разовое посещение";

        public string DurationText => Service?.DurationMinutes.HasValue == true
            ? $"Длительность: {Service.DurationMinutes} мин"
            : string.Empty;

        public string ServiceDescription => Service?.Description ?? string.Empty;

        private static void OnServiceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ServiceCardControl control)
            {
                control.OnPropertyChanged(nameof(ServiceName));
                control.OnPropertyChanged(nameof(PriceText));
                control.OnPropertyChanged(nameof(SessionsText));
                control.OnPropertyChanged(nameof(DurationText));
                control.OnPropertyChanged(nameof(ServiceDescription));
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}