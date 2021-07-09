using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Peer.Models
{
    public interface IPRModel
    {
        String Title { get; set; }
        String Actor { get; set; }
        DateTime Date { get; set; }
    }
}
