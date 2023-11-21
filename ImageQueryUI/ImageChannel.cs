using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageQueryUI
{
    public class ImageChannel
    {
        public int id;
        public string sensorType;
        public string channelType;

        public ImageChannel(int id, string sensorType, string channelType)
        {
            this.id = id;
            this.sensorType = sensorType;
            this.channelType = channelType;
        }
    }
}
