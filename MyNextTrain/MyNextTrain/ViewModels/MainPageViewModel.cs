using IrishRailApi;
using MyNextTrain.Models;
using Plugin.LocalNotifications;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace MyNextTrain.ViewModels
{
    public class MainPageViewModel : ViewModelBase
    {
        private readonly string _defaultStationCode = "BROCK";
        private readonly IIrishRailApiService _irishRailApiService;
        private readonly IPageDialogService _pageDialogService;

        private int _notificationId = 100;
        public ObservableCollection<ArrayOfObjStationObjStation> Stations { get; set; }
        public ArrayOfObjStationObjStation SelectedStation { get; set; }
        public Direction Direction { get; set; }
        public ICommand FindTrainCommand { get; set; }
        public bool Processing { get; set; }

        public List<string> Directions
        {
            get
            {
                return Enum.GetNames(typeof(Direction)).ToList();
            }
        }

        public MainPageViewModel(INavigationService navigationService, IPageDialogService dialogService, IIrishRailApiService irishRailApiService)
            : base(navigationService)
        {
            Title = "My Next Train";
            Stations = new ObservableCollection<ArrayOfObjStationObjStation>();
            _irishRailApiService = irishRailApiService;
            _pageDialogService = dialogService;
            FindTrainCommand = new DelegateCommand(DoFindTrain);
            Processing = true;
        }

        private async void DoFindTrain()
        {
            Processing = true;

            try
            {
                var stationData = await _irishRailApiService.GetStationDataByStationCodeAsync(SelectedStation.StationCode);

                var nextTrain = GetNextTrain(stationData);

                if (nextTrain == null)
                {
                    await _pageDialogService.DisplayAlertAsync("Sorry!",
                        $"No train is leaving from {SelectedStation.StationDesc} going {Direction} in the next 20 minutes.",
                        "Ok");
                }
                else
                {
                    var delay = (DateTime.ParseExact(nextTrain.Expdepart, "HH:mm", CultureInfo.InvariantCulture).AddMinutes(-10)) - DateTime.Now;

                    CrossLocalNotifications.Current.Show("Your next train!",
                        $"Your next train from {SelectedStation.StationDesc} going {Direction} is departing in 10 minutes, at {nextTrain.Expdepart}!", ++_notificationId, DateTime.Now.Add(delay));
                    await _pageDialogService.DisplayAlertAsync("Found it!",
                        $"I'll notify you 10 minutes before departure.",
                        "Ok");
                }
            }
            catch
            {
                await _pageDialogService.DisplayAlertAsync("Error",
                       $"There was a problem finding the next train. Please try again later.",
                       "Ok");
            }

            Processing = false;

        }

        public override async void OnNavigatedTo(NavigationParameters parameters)
        {
            await GetStations();
            base.OnNavigatedTo(parameters);
        }

        public async Task GetStations()
        {
            Processing = true;

            try
            {
                var stations = await _irishRailApiService.GetAllStationsWithTypeAsync("D");

                Stations = new ObservableCollection<ArrayOfObjStationObjStation>(stations.Items.OrderBy(m => m.StationDesc).ToList());
                if (Stations.Any())
                {
                    SelectedStation = Stations.FirstOrDefault(station => station.StationCode == _defaultStationCode);
                }
            }
            catch
            {
                await _pageDialogService.DisplayAlertAsync("Error",
                                      $"There was a problem initializing the app. Please try again later.",
                                      "Ok");
                await GetStations();
            }

            Processing = false;
        }

        private ArrayOfObjStationDataObjStationData GetNextTrain(ArrayOfObjStationData stationData)
        {
            var nextTrain = stationData.Items.Where(m => m.Direction == Direction.ToString())
                   .Where(m => DateTime.ParseExact(m.Expdepart, "HH:mm", CultureInfo.InvariantCulture) - DateTime.Now >= TimeSpan.FromMinutes(10))
                   .Where(m => DateTime.ParseExact(m.Expdepart, "HH:mm", CultureInfo.InvariantCulture) - DateTime.Now <= TimeSpan.FromMinutes(20))
                   .OrderBy(m => DateTime.ParseExact(m.Expdepart, "HH:mm", CultureInfo.InvariantCulture).TimeOfDay)
                   .FirstOrDefault();

            return nextTrain;
        }
    }
}
