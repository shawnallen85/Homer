using System;
using System.Collections.ObjectModel;
using DynamicData;
using Homer.Models;
using Homer.Services;
using LibVLCSharp.Shared;

namespace Homer.ViewModels
{
    public class MainWindowViewModel : ViewModelBase, IDisposable
    {
        private readonly LibVLC _libVlc = new LibVLC();
        public MainWindowViewModel()
        {
            Lineups = new ObservableCollection<Lineup>();
            MediaPlayer = new MediaPlayer(_libVlc);
            HdHomeRunService hdHomeRunService = new HdHomeRunService();
            hdHomeRunService.StartScan();
            Hosts = hdHomeRunService.Hosts;
        }

        public ObservableCollection<Lineup> Lineups { get; private set; }
        public ObservableCollection<HdHomeRunHost> Hosts { get; }

        public MediaPlayer MediaPlayer { get; }

        private Media _media = null;

        private Lineup _selectedChannel;
        public Lineup SelectedChannel
        {
            get => _selectedChannel;
            set
            {
                _selectedChannel = value;
                if (MediaPlayer == null)
                    return;
                if (_selectedChannel == null)
                    return;
                if(MediaPlayer.IsPlaying)
                    MediaPlayer.Stop();
                _media?.Dispose();
                _media = new Media(_libVlc, new Uri(_selectedChannel.Url));
                MediaPlayer.Play(_media);
            }
        }
        
        private HdHomeRunHost _selectedHost;
        public HdHomeRunHost SelectedHost
        {
            get => _selectedHost;
            set
            {
                _selectedHost = value;
                Lineups.Clear();
                if (_selectedHost == null)
                    return;
                Lineups.AddRange(LineupService.GetItems(_selectedHost.RestClient));
            }
        }
        
        public void Dispose()
        {
            _libVlc?.Dispose();
        }
    }
}
