using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageQueryUI
{
    public class ImageType
    {
        public int id;
        public string name;

        public ImageType(int id, string name)
        {
            this.id = Convert.ToInt32(id);
            this.name = name;
        }
    }
}
