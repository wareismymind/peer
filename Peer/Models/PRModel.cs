using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Peer.Models
{
    class PRModel : IPRModel
    {
        public string Title { get => Title; set => Title = value; }
        public string Actor { get => Actor; set => Actor = value; }
        public DateTime Date { get => Date; set => Date = value; }
    }
}
