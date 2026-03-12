using Rockstar.Admin.WPF.Models;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace Rockstar.Admin.WPF.Views.Clients.Controls
{
    public partial class ClientCard : UserControl, INotifyPropertyChanged
    {
        public static readonly DependencyProperty ClientProperty =
            DependencyProperty.Register(nameof(Client), typeof(Client), typeof(ClientCard),
                new PropertyMetadata(null, OnClientChanged));

        public static readonly DependencyProperty EditCommandProperty =
            DependencyProperty.Register(nameof(EditCommand), typeof(ICommand), typeof(ClientCard),
                new PropertyMetadata(null));

        public static readonly DependencyProperty DeleteCommandProperty =
            DependencyProperty.Register(nameof(DeleteCommand), typeof(ICommand), typeof(ClientCard),
                new PropertyMetadata(null));

        public ClientCard()
        {
            InitializeComponent();
            this.Loaded += ClientCard_Loaded;
        }

        private void ClientCard_Loaded(object sender, RoutedEventArgs e)
        {
            if (Client != null)
            {
                Debug.WriteLine($"ClientCard Loaded: ID={Client.Id}, FirstName='{Client.FirstName}', LastName='{Client.LastName}', FullName='{FullName}'");
            }
        }

        public Client Client
        {
            get => (Client)GetValue(ClientProperty);
            set => SetValue(ClientProperty, value);
        }

        public ICommand EditCommand
        {
            get => (ICommand)GetValue(EditCommandProperty);
            set => SetValue(EditCommandProperty, value);
        }

        public ICommand DeleteCommand
        {
            get => (ICommand)GetValue(DeleteCommandProperty);
            set => SetValue(DeleteCommandProperty, value);
        }

        public string FullName
        {
            get
            {
                if (Client == null) return "Нет данных";

                var fullName = Client.FullName;
                Debug.WriteLine($"FullName getter: '{fullName}' for client {Client.Id}");
                return fullName;
            }
        }

        public string Email => Client?.Email ?? "Email не указан";

        public string PhoneText => !string.IsNullOrWhiteSpace(Client?.Phone) ? $"📞 {Client.Phone}" : "📞 Не указан";

        public string AgeText => Client?.Age != null ? $"📅 {Client.Age} лет" : "📅 Возраст не указан";

        public BitmapImage PhotoSource
        {
            get
            {
                if (Client?.Photo != null && Client.Photo.Length > 0)
                {
                    try
                    {
                        var image = new BitmapImage();
                        image.BeginInit();
                        image.StreamSource = new MemoryStream(Client.Photo);
                        image.CacheOption = BitmapCacheOption.OnLoad;
                        image.EndInit();
                        image.Freeze();
                        return image;
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"PhotoSource error: {ex.Message}");
                        return null;
                    }
                }
                return null;
            }
        }

        private static void OnClientChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ClientCard card)
            {
                card.OnPropertyChanged(nameof(FullName));
                card.OnPropertyChanged(nameof(Email));
                card.OnPropertyChanged(nameof(PhoneText));
                card.OnPropertyChanged(nameof(AgeText));
                card.OnPropertyChanged(nameof(PhotoSource));

                Debug.WriteLine($"Client changed: {card.Client?.Id}");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}