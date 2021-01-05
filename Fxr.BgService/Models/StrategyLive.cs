using System;
using System.Collections.Generic;

#nullable disable

namespace Fxr.BgService.Models
{
    public partial class StrategyLive
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Pair { get; set; }
        public string Trend { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
