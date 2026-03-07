using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Rockstar.Admin.WPF.Models;

namespace Rockstar.Admin.WPF.Views.Trainers.Controls
{
    public partial class TrainerCardControl : UserControl, INotifyPropertyChanged
    {
        public static readonly DependencyProperty TrainerProperty =
            DependencyProperty.Register(nameof(Trainer), typeof(Trainer), typeof(TrainerCardControl),
                new PropertyMetadata(null, OnTrainerChanged));

        public static readonly DependencyProperty EditCommandProperty =
            DependencyProperty.Register(nameof(EditCommand), typeof(ICommand), typeof(TrainerCardControl));

        public static readonly DependencyProperty DeleteCommandProperty =
            DependencyProperty.Register(nameof(DeleteCommand), typeof(ICommand), typeof(TrainerCardControl));

        public TrainerCardControl()
        {
            InitializeComponent();
        }

        public Trainer? Trainer
        {
            get => (Trainer?)GetValue(TrainerProperty);
            set => SetValue(TrainerProperty, value);
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
                    try
                    {
                        var image = new BitmapImage();
                        image.BeginInit();
                        image.StreamSource = new System.IO.MemoryStream(Trainer.Photo);
                        image.CacheOption = BitmapCacheOption.OnLoad;
                        image.EndInit();
                        image.Freeze();
                        return image;
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"Error loading photo: {ex.Message}");
                        return null;
                    }
                }
                return null;
            }
        }

        private static void OnTrainerChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TrainerCardControl control && e.NewValue is Trainer trainer)
            {
                System.Diagnostics.Debug.WriteLine($"Card updated: {trainer.FullName}, Direction: {trainer.DirectionDisplayName}");
                control.OnPropertyChanged(nameof(FullName));
                control.OnPropertyChanged(nameof(DirectionDisplayName));
                control.OnPropertyChanged(nameof(ExperienceText));
                control.OnPropertyChanged(nameof(Description));
                control.OnPropertyChanged(nameof(PhotoSource));
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}