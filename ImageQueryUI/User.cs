using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace ImageQueryUI
{
    public class User
    {
        public int id;
        public string name { get; }
        public List<Image> images;

        public User(int id, string name)
        {
            this.id = Convert.ToInt32(id);
            this.name = name;
            this.images = new List<Image>();
        }
    }
}
