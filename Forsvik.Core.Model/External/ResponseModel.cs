using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forsvik.Core.Model.External
{
    public class ResponseModel<T>
    {
        public T Result { get; set; }
        public string Error { get; set; }
    }
}
