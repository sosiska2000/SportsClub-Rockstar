using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Rockstar.Admin.WPF.Models;

namespace Rockstar.Admin.WPF.Views.Subscriptions.Controls
{
    public partial class SubscriptionCardControl : UserControl, INotifyPropertyChanged
    {
        public static readonly DependencyProperty SubscriptionProperty =
            DependencyProperty.Register(nameof(Subscription), typeof(Subscription), typeof(SubscriptionCardControl),
                new PropertyMetadata(null, OnSubscriptionChanged));

        public static readonly DependencyProperty EditCommandProperty =
            DependencyProperty.Register(nameof(EditCommand), typeof(ICommand), typeof(SubscriptionCardControl));

        public static readonly DependencyProperty DeleteCommandProperty =
            DependencyProperty.Register(nameof(DeleteCommand), typeof(ICommand), typeof(SubscriptionCardControl));

        public SubscriptionCardControl()
        {
            InitializeComponent();
        }

        public Subscription? Subscription
        {
            get => (Subscription?)GetValue(SubscriptionProperty);
            set => SetValue(SubscriptionProperty, value);
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

        public string SubscriptionName => Subscription?.Name ?? string.Empty;

        public string DirectionName => Subscription?.DirectionDisplay ?? string.Empty;

        public string DirectionColor => Subscription?.DirectionColor ?? "#9C27B0";

        public string PriceText => Subscription != null ? $"{Subscription.Price:C}" : string.Empty;

        public string SessionsText => Subscription?.SessionsDisplay ?? string.Empty;

        public string SubscriptionDescription => Subscription?.Description ?? string.Empty;

        private static void OnSubscriptionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is SubscriptionCardControl control)
            {
                control.OnPropertyChanged(nameof(SubscriptionName));
                control.OnPropertyChanged(nameof(DirectionName));
                control.OnPropertyChanged(nameof(DirectionColor));
                control.OnPropertyChanged(nameof(PriceText));
                control.OnPropertyChanged(nameof(SessionsText));
                control.OnPropertyChanged(nameof(SubscriptionDescription));
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}