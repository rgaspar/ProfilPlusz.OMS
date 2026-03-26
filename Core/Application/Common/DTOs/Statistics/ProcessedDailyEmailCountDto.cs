using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.DTOs.Statistics
{
    public class ProcessedDailyEmailCountDto
    {
        public DateTime Date { get; set; }
        public int Count { get; set; }
    }
}
