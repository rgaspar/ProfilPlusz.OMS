using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.Services.Location
{
    public interface IStateService
    {
        IReadOnlyList<string> GetAll();

        bool Exists(string county);

        IReadOnlyList<string> GetNeighbours(string county);

        IReadOnlyList<string> GetWithNeighbours(string county);

        string Normalize(string county);
    }
}
