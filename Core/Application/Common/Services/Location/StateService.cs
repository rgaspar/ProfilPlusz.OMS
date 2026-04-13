using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.Services.Location
{
    public class StateService : IStateService
    {
        private static readonly Dictionary<string, string[]> _neighbours =
        new(StringComparer.OrdinalIgnoreCase)
        {
            ["Budapest"] = ["Pest vármegye"],

            ["Pest vármegye"] =
            [
                "Budapest",
                "Nógrád vármegye",
                "Heves vármegye",
                "Jász-Nagykun-Szolnok vármegye",
                "Bács-Kiskun vármegye",
                "Fejér vármegye",
                "Komárom-Esztergom vármegye"
            ],
            ["Fejér vármegye"] =
            [
                "Komárom-Esztergom vármegye",
                "Pest vármegye",
                "Bács-Kiskun vármegye",
                "Tolna vármegye",
                "Veszprém vármegye"
            ],
            ["Komárom-Esztergom vármegye"] =
            [
                "Győr-Moson-Sopron vármegye",
                "Veszprém vármegye",
                "Fejér vármegye",
                "Pest vármegye"
            ],
            ["Győr-Moson-Sopron vármegye"] =
            [
                "Komárom-Esztergom vármegye",
                "Veszprém vármegye",
                "Vas vármegye"
            ],
            ["Vas vármegye"] =
            [
                "Győr-Moson-Sopron vármegye",
                "Veszprém vármegye",
                "Zala vármegye"
            ],
            ["Veszprém vármegye"] =
            [
                "Győr-Moson-Sopron vármegye",
                "Komárom-Esztergom vármegye",
                "Fejér vármegye",
                "Tolna vármegye",
                "Somogy vármegye",
                "Zala vármegye",
                "Vas vármegye"
            ],
            ["Zala vármegye"] =
            [
                "Vas vármegye",
                "Veszprém vármegye",
                "Somogy vármegye"
            ],
            ["Somogy vármegye"] =
            [
                "Zala vármegye",
                "Veszprém vármegye",
                "Tolna vármegye",
                "Baranya vármegye"
            ],
            ["Tolna vármegye"] =
            [
                "Fejér vármegye",
                "Bács-Kiskun vármegye",
                "Baranya vármegye",
                "Somogy vármegye",
                "Veszprém vármegye"
            ],
            ["Baranya vármegye"] =
            [
                "Somogy vármegye",
                "Tolna vármegye",
                "Bács-Kiskun vármegye"
            ],
            ["Bács-Kiskun vármegye"] =
            [
                "Pest vármegye",
                "Jász-Nagykun-Szolnok vármegye",
                "Csongrád-Csanád vármegye",
                "Baranya vármegye",
                "Tolna vármegye",
                "Fejér vármegye"
            ],
            ["Csongrád-Csanád vármegye"] =
            [
                "Bács-Kiskun vármegye",
                "Jász-Nagykun-Szolnok vármegye",
                "Békés vármegye"
            ],
            ["Békés vármegye"] =
            [
                "Jász-Nagykun-Szolnok vármegye",
                "Csongrád-Csanád vármegye",
                "Hajdú-Bihar vármegye"
            ],
            ["Jász-Nagykun-Szolnok vármegye"] =
            [
                "Pest vármegye",
                "Heves vármegye",
                "Hajdú-Bihar vármegye",
                "Békés vármegye",
                "Csongrád-Csanád vármegye",
                "Bács-Kiskun vármegye"
            ],
            ["Heves vármegye"] =
            [
                "Nógrád vármegye",
                "Borsod-Abaúj-Zemplén vármegye",
                "Hajdú-Bihar vármegye",
                "Jász-Nagykun-Szolnok vármegye",
                "Pest vármegye"
            ],
            ["Nógrád vármegye"] =
            [
                "Borsod-Abaúj-Zemplén vármegye",
                "Heves vármegye",
                "Pest vármegye"
            ],
            ["Borsod-Abaúj-Zemplén vármegye"] =
            [
                "Nógrád vármegye",
                "Heves vármegye",
                "Hajdú-Bihar vármegye",
                "Szabolcs-Szatmár-Bereg vármegye"
            ],
            ["Hajdú-Bihar vármegye"] =
            [
                "Szabolcs-Szatmár-Bereg vármegye",
                "Borsod-Abaúj-Zemplén vármegye",
                "Heves vármegye",
                "Jász-Nagykun-Szolnok vármegye",
                "Békés vármegye"
            ],
            ["Szabolcs-Szatmár-Bereg vármegye"] =
            [
                "Borsod-Abaúj-Zemplén vármegye",
                "Hajdú-Bihar vármegye"
            ]
        };

        public IReadOnlyList<string> GetAll()
            => _neighbours.Keys.ToList();

        public bool Exists(string county)
            => _neighbours.ContainsKey(county);

        public IReadOnlyList<string> GetNeighbours(string county)
        {
            if (!_neighbours.TryGetValue(county, out var neighbours))
                return [];

            return neighbours;
        }

        public IReadOnlyList<string> GetWithNeighbours(string county)
        {
            var result = new List<string> { county };

            if (_neighbours.TryGetValue(county, out var neighbours))
                result.AddRange(neighbours);

            return result;
        }

        public string Normalize(string county)
        {
            var match = _neighbours.Keys
                .FirstOrDefault(x =>
                    x.Equals(county, StringComparison.OrdinalIgnoreCase));

            return match ?? county;
        }
    }
}
