using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static RouletteWpfApp.MainWindow;

namespace RouletteWpfApp
{
    public class ResponseMessage
    {
        public string Qualifier { get; set; } = default!;
        public ResponseData Data { get; set; } = default!;
    }
}
