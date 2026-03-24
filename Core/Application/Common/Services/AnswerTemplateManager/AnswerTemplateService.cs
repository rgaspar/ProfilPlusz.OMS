using Application.Common.Configuration;
using Application.Common.Repositories;
using Domain.Entities;
using Microsoft.Extensions.Options;

namespace Application.Common.Services.AnswerTemplateManager
{
    public class AnswerTemplateService : IAnswerTemplateService
    {
        private readonly IAnswerTemplateRepository _repository;
        private readonly TemplateSettings _settings;
        private readonly Dictionary<string, AnswerTemplate> _templateCache = new();

        public AnswerTemplateService(
            IAnswerTemplateRepository repository,
            IOptions<TemplateSettings> options)
        {
            _repository = repository;
            _settings = options.Value;
        }

        public async Task<string> RenderAsync(string key, object? model = null)
        {
            var template = await GetTemplateAsync(key);

            var fullPath = Path.Combine(AppContext.BaseDirectory, _settings.BasePath, template.Path);

            if (!File.Exists(fullPath))
            {
                throw new Exception($"Template file not found: {fullPath}");
            }

            var content = await File.ReadAllTextAsync(fullPath);

            return Render(content, model);
        }

        public async Task<string[]> GetDefaultRecipientsAsync(string key)
        {
            var template = await _repository.GetByKeyAsync(key) ?? throw new Exception($"Template not found: {key}");

            if (string.IsNullOrWhiteSpace(template.DefaultRecipients))
                return [];

            return template.DefaultRecipients
                .Split(';', StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.Trim())
                .ToArray();
        }

        private string Render(string template, object? model)
        {
            if (model == null)
                return template;

            foreach (var prop in model.GetType().GetProperties())
            {
                var value = prop.GetValue(model);

                // LISTA kezelés
                if (value is IEnumerable<object> list && !(value is string))
                {
                    var startTag = $"{{{{#{prop.Name}}}}}";
                    var endTag = $"{{{{/{prop.Name}}}}}";

                    var startIndex = template.IndexOf(startTag);
                    var endIndex = template.IndexOf(endTag);

                    if (startIndex >= 0 && endIndex > startIndex)
                    {
                        var innerTemplate = template.Substring(
                            startIndex + startTag.Length,
                            endIndex - (startIndex + startTag.Length));

                        var renderedItems = "";

                        foreach (var item in list)
                        {
                            renderedItems += Render(innerTemplate, item);
                        }

                        template =
                            template.Substring(0, startIndex)
                            + renderedItems
                            + template.Substring(endIndex + endTag.Length);
                    }
                }
                else
                {
                    var stringValue = value?.ToString() ?? "";

                    template = template.Replace(
                        $"{{{{{prop.Name}}}}}",
                        stringValue);
                }
            }

            return template;
        }

        public async Task<AnswerTemplate> GetTemplateAsync(string key)
        {
            if (_templateCache.TryGetValue(key, out var cached))
            {
                return cached;
            }

            var template = await _repository.GetByKeyAsync(key)
                ?? throw new Exception($"Template not found: {key}");

            _templateCache[key] = template;

            return template;
        }
    }
}
