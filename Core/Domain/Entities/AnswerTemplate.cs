using Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class AnswerTemplate : BaseEntity
    {
        public string Key { get; set; } = null!;

        public string Path { get; set; } = null!;

        public string? Subject { get; set; }

        public string? DefaultRecipients { get; set; }
    }
}
