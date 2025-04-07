using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using MemoryGame.Models;
using MemoryGame.Services;

namespace MemoryGame.ViewModels
{
    public class StatisticsViewModel : BaseViewModel
    {
        private readonly StatisticsService statisticsService;
        public ObservableCollection<PlayerStatistics> PlayerStats { get; set; }

        public StatisticsViewModel()
        {
            statisticsService = new StatisticsService();
            var stats = statisticsService.GetAllStatistics();
            PlayerStats = new ObservableCollection<PlayerStatistics>(stats);
        }
    }
}
