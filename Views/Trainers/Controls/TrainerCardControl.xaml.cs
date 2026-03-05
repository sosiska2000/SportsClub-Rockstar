using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Rockstar.Admin.WPF.Models;

namespace Rockstar.Admin.WPF.Views.Trainers.Controls
{
    // 🔑 Класс должен быть public
    public partial class TrainerCardControl : UserControl, INotifyPropertyChanged
    {
        public static readonly DependencyProperty TrainerProperty =
            DependencyProperty.Register(nameof(Trainer), typeof(Trainer), typeof(TrainerCardControl),
                new PropertyMetadata(null, OnTrainerChanged));

        public static readonly DependencyProperty EditCommandProperty =
            DependencyProperty.Register(nameof(EditCommand), typeof(System.Windows.Input.ICommand), typeof(TrainerCardControl));

        public static readonly DependencyProperty DeleteCommandProperty =
            DependencyProperty.Register(nameof(DeleteCommand), typeof(System.Windows.Input.ICommand), typeof(TrainerCardControl));

        public TrainerCardControl()
        {
            InitializeComponent();
            DataContext = this;
        }

        public Trainer? Trainer
        {
            get => (Trainer?)GetValue(TrainerProperty);
            set => SetValue(TrainerProperty, value);
        }

        public System.Windows.Input.ICommand? EditCommand
        {
            get => (System.Windows.Input.ICommand?)GetValue(EditCommandProperty);
            set => SetValue(EditCommandProperty, value);
        }

        public System.Windows.Input.ICommand? DeleteCommand
        {
            get => (System.Windows.Input.ICommand?)GetValue(DeleteCommandProperty);
            set => SetValue(DeleteCommandProperty, value);
        }

        public string FullName => Trainer?.FullName ?? string.Empty;
        public string DirectionDisplayName => Trainer?.DirectionDisplayName ?? string.Empty;
        public string ExperienceText => Trainer != null ? $"Стаж: {Trainer.Experience} лет" : string.Empty;
        public string Description => Trainer?.Description ?? string.Empty;
        
        public BitmapImage? PhotoSource
        {
            get
            {
                if (Trainer?.Photo != null && Trainer.Photo.Length > 0)
                {
                    var image = new BitmapImage();
                    image.BeginInit();
                    image.StreamSource = new MemoryStream(Trainer.Photo);
                    image.CacheOption = BitmapCacheOption.OnLoad;
                    image.EndInit();
                    image.Freeze();
                    return image;
                }
                return null;
            }
        }

        private static void OnTrainerChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TrainerCardControl control)
            {
                control.RaisePropertyChanged(nameof(FullName));
                control.RaisePropertyChanged(nameof(DirectionDisplayName));
                control.RaisePropertyChanged(nameof(ExperienceText));
                control.RaisePropertyChanged(nameof(Description));
                control.RaisePropertyChanged(nameof(PhotoSource));
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        
        protected virtual void RaisePropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}