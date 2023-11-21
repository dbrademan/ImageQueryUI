using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageQueryUI
{
    public class ImageDatabaseObject
    {
        public SqliteConnection DatabaseConnection;
        public Dictionary<int, User> users;
        public Dictionary<int, Image> allImages;
        public Dictionary<int, ImageType> imageTypes;
        public Dictionary<int, ImageChannel> imageChannels;

        public ImageDatabaseObject() 
        { 
            this.users = new Dictionary<int, User>();
            this.allImages = new Dictionary<int, Image>();
            this.imageTypes = new Dictionary<int, ImageType>();
            this.imageChannels = new Dictionary<int, ImageChannel>();
        }
    }
}
