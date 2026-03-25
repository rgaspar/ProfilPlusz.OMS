using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.Repositories
{
    public interface IProcessedEmailRepository
    {
        Task<bool> ExistsByExternalIdAsync(string externalId, CancellationToken cancellationToken);

        Task SaveAsync(ProcessedEmail email, CancellationToken cancellationToken);
    }
}
