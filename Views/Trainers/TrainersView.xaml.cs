using Microsoft.Extensions.DependencyInjection;
using Rockstar.Admin.WPF.Services.Interfaces;
using Rockstar.Admin.WPF.ViewModels.Trainers;
using System;
using System.Windows.Controls;

namespace Rockstar.Admin.WPF.Views.Trainers
{
    public partial class TrainersView : Page
    {
        public TrainersView(Action<Page> navigate)
        {
            InitializeComponent();

            var trainerService = App.Services.GetRequiredService<ITrainerService>();
            var viewModel = new TrainersViewModel(trainerService, navigate);

            DataContext = viewModel;
        }
    }
}