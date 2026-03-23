using Domain.Services.Email;
using Domain.Services.Email.Models;
using System.Text.RegularExpressions;

namespace Application.Common.Services.EmailManager
{
    public class EmailParserService : IEmailParserService
    {
        public ParsedEmail Parse(string text)
        {
            text = Normalize(text);

            var address = ParseAddress(text);

            return new ParsedEmail
            {
                OrderNumber = ParseOrderNumber(text),
                CustomerName = address?.Name,
                Email = ParseEmail(text),
                ShippingAddress = address,
                Products = ParseProducts(text)
            };
        }

        string Normalize(string input)
        {
            return input
                .Replace("\r", "")
                .Replace("\t", " ")
                .Trim();
        }

        string ParseOrderNumber(string text)
        {
            return Regex.Match(
                text,
                @"Rendelésszám:\s*(\d+)",
                RegexOptions.IgnoreCase)
                .Groups[1]
                .Value;
        }

        string ParseEmail(string text)
        {
            return Regex.Match(
                text,
                @"Email cím:\s*([\w\.-]+@[\w\.-]+\.\w+)",
                RegexOptions.IgnoreCase)
                .Groups[1]
                .Value;
        }

        ParsedAddress ParseAddress(string text)
        {
            var match = Regex.Match(
                text,
                @"Szállítási cím\s+Számlázási cím\s+(.*?)\s+Termék",
                RegexOptions.Singleline | RegexOptions.IgnoreCase);

            if (!match.Success)
            {
                return new ParsedAddress();
            }

            var lines = match.Groups[1].Value
                .Split('\n', StringSplitOptions.RemoveEmptyEntries)
                .Select(x => RemoveLinks(x.Trim()))
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .ToList();

            var zipCityLine = lines.ElementAtOrDefault(3) ?? "";

            return new ParsedAddress
            {
                Name = lines.ElementAtOrDefault(0),
                Street = lines.ElementAtOrDefault(1),
                FloorDoor = lines.ElementAtOrDefault(2),
                Zip = Regex.Match(zipCityLine, @"\d{4}").Value,
                City = Regex.Replace(zipCityLine, @"\d{4}", "").Trim(),
                Country = lines.FirstOrDefault(x =>
                    x.Contains("Magyarország", StringComparison.OrdinalIgnoreCase))
            };
        }

        List<ParsedProduct> ParseProducts(string text)
        {
            var result = new List<ParsedProduct>();

            var match = Regex.Match(
                text,
                @"Termék\s+Cikkszám\s+Ár\s+Mennyiség\s+Összesen\s+(.*?)\s+Nettó részösszeg",
                RegexOptions.Singleline | RegexOptions.IgnoreCase);

            if (!match.Success)
            {
                return result;
            }

            var lines = match.Groups[1].Value
                .Split('\n', StringSplitOptions.RemoveEmptyEntries)
                .Select(x => RemoveLinks(x.Trim()))
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .ToList();

            for (int i = 0; i < lines.Count; i += 4)
            {
                if (i + 3 >= lines.Count)
                    break;

                result.Add(new ParsedProduct
                {
                    Name = lines[i],
                    Sku = lines[i + 1],
                    Quantity = lines[i + 3]
                });
            }

            return result;
        }

        static string RemoveLinks(string text)
        {
            return Regex
                .Replace(text, "<.*?>", "")
                .Trim();
        }
    }
}
