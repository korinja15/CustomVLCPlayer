using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VLCPlayer
{
    public class Playlist
    {
        public string Name { get; set; }
        public ObservableCollection<string> Items { get; set; }
    }
}
