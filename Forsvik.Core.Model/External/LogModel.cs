using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forsvik.Core.Model.External
{
    public class LogModel
    {
        public DateTime Created { get; set; }

        public string Text => $"{Created.ToString("yyyyMMdd HH:mm")}\t{Level}\t{Message}";

        public string Message { get; set; }

        public string Level { get; set; }
    }
}
