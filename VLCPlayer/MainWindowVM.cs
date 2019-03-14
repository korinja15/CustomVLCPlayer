using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using LibVLCSharp.Shared;
using Microsoft.Win32;
using Vlc.DotNet.Core;

namespace VLCPlayer
{
    class MainWindowVM : ViewModelBase
    {
        public ObservableCollection<Playlist> PlaylistCollection { get; set; } = new ObservableCollection<Playlist>();

        public Playlist CurrentPlaylist { get; set; }
        public int CurrentPlaylist_position { get; set; }

        public String PlayerTime { get; set; }

        public MediaPlayer Player { get; set; }

        public Page PanelView { get; set; }

        public bool Fullscreen { get; set; }

        public string NewFilePath { get; set; }

        public string CreatePlaylist_Name { get; set; }

        public int PlaylistView_SelectedIndex { get; set; }
        public int SelectPlaylist_SelectedIndex { get; set; }

        public RelayCommand OnBack10SecondsCommand { get; }
        public RelayCommand OnForward30SecondsCommand { get; }
        public RelayCommand OnCloseWindowCommand { get; }
        public RelayCommand OnAddPlaylistCommand { get; }
        public RelayCommand PreviousCommand { get; }
        public RelayCommand NextCommand { get; }
        public RelayCommand SwitchPlayCommand { get; }
        public RelayCommand SwitchFullscreenCommand { get; }

        public RelayCommand CreatePlaylist_Command { get; set; }
        public RelayCommand SelectPlaylist_Command { get; set; }
        public RelayCommand CreatePlaylist_AddNewPlaylistCommand { get; }
        public RelayCommand SelectPlaylist_AddToPlaylistCommand { get; }
        public RelayCommand<string> PlaylistView_PlayCommand { get; }

        public MainWindowVM()
        {
            CreatePlayer();

            OnBack10SecondsCommand = new RelayCommand(OnBack10Seconds);
            OnForward30SecondsCommand = new RelayCommand(OnForward30Seconds);
            OnCloseWindowCommand = new RelayCommand(OnCloseWindow);
            OnAddPlaylistCommand = new RelayCommand(AddPlaylist);
            PreviousCommand = new RelayCommand(Previous);
            NextCommand = new RelayCommand(Next);
            SwitchPlayCommand = new RelayCommand(SwitchPlay);
            SwitchFullscreenCommand = new RelayCommand(SwitchFullscreen);

            CreatePlaylist_Command = new RelayCommand(CreatePlaylist);
            SelectPlaylist_Command = new RelayCommand(SelectPlaylist);          
            CreatePlaylist_AddNewPlaylistCommand = new RelayCommand(CreatePlaylist_AddNewPlaylist);
            SelectPlaylist_AddToPlaylistCommand = new RelayCommand(SelectPlaylist_AddToPlaylist);
            PlaylistView_PlayCommand = new RelayCommand<string>(x => PlaylistView_Play(x));

            //PlaylistCollection.Add(new Playlist {Name = "Playlist 1", Items = new ObservableCollection<string> { "C:/Users/kubii/Downloads/bunny.avi" } });
        }

        void CreatePlayer()
        {
            var VlcLibDirectory = new DirectoryInfo(Path.Combine(Directory.GetCurrentDirectory(), "libvlc",
                IntPtr.Size == 4 ? "win-x86" : "win-x64"));
            
            MainWindow.Instance.MyControl.SourceProvider.CreatePlayer(VlcLibDirectory);
        }

        void OnBack10Seconds()
        {
            MainWindow.Instance.MyControl.SourceProvider.MediaPlayer.Time = MainWindow.Instance.MyControl.SourceProvider.MediaPlayer.Time - 10000;
        }

        void OnForward30Seconds()
        {
            MainWindow.Instance.MyControl.SourceProvider.MediaPlayer.Time = MainWindow.Instance.MyControl.SourceProvider.MediaPlayer.Time + 30000;
        }

        void OnCloseWindow()
        {
            MainWindow.Instance.Close();
        }

        void AddPlaylist()
        {
            var data = new OpenFileDialog();
            data.ShowDialog();
            NewFilePath = data.FileName;
            MainWindow.Instance.PanelView.Content = new ExistingNewPlaylist {DataContext = MainWindow.Instance.DataContext};
        }

        void Previous()
        {
            CurrentPlaylist_position--;
            MainWindow.Instance.MyControl.SourceProvider.MediaPlayer.Play(new FileInfo(CurrentPlaylist.Items[CurrentPlaylist_position]));
        }

        void Next()
        {
            CurrentPlaylist_position++;
            MainWindow.Instance.MyControl.SourceProvider.MediaPlayer.Play(new FileInfo(CurrentPlaylist.Items[CurrentPlaylist_position]));
        }

        void SwitchPlay()
        {
            if (MainWindow.Instance.MyControl.SourceProvider.MediaPlayer.IsPlaying())
            {
                MainWindow.Instance.MyControl.SourceProvider.MediaPlayer.Pause();
            }
            else
            {
                MainWindow.Instance.MyControl.SourceProvider.MediaPlayer.Play();
            }
        }

        void SwitchFullscreen()
        {
            MainWindow.Instance.MainWindowPage.WindowState = MainWindow.Instance.MainWindowPage.WindowState == WindowState.Maximized ? WindowState.Minimized : WindowState.Maximized;
        }

        void CreatePlaylist()
        {
            MainWindow.Instance.PanelView.Content = new CreatePlaylist {DataContext = MainWindow.Instance.DataContext};
        }

        void SelectPlaylist()
        {
            MainWindow.Instance.PanelView.Content = new SelectPlaylist {DataContext = MainWindow.Instance.DataContext};
        }

        void CreatePlaylist_AddNewPlaylist()
        {
            PlaylistCollection.Add(new Playlist {Name = CreatePlaylist_Name, Items = new ObservableCollection<string> {NewFilePath}});
            CreateSelectPlaylist_final();
        }

        void SelectPlaylist_AddToPlaylist()
        {
            PlaylistCollection[SelectPlaylist_SelectedIndex].Items.Add(NewFilePath);
            CreateSelectPlaylist_final();
        }

        void CreateSelectPlaylist_final()
        {
            MainWindow.Instance.PanelView.Content = new PlaylistView { DataContext = MainWindow.Instance.DataContext };
            CreatePlaylist_Name = "";
        }

        void PlaylistView_Play(string x)
        {
            var items = PlaylistCollection.Where(X => X.Name == x).First();
            CurrentPlaylist = items;
            CurrentPlaylist_position = 0;
            MainWindow.Instance.MyControl.SourceProvider.MediaPlayer.Play(new FileInfo(items.Items[CurrentPlaylist_position]));
        }
    }
}
