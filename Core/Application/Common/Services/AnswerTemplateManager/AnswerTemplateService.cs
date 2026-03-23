using Application.Common.Configuration;
using Application.Common.Repositories;
using Microsoft.Extensions.Options;

namespace Application.Common.Services.AnswerTemplateManager
{
    public class AnswerTemplateService : IAnswerTemplateService
    {
        private readonly IAnswerTemplateRepository _repository;
        private readonly TemplateSettings _settings;

        public AnswerTemplateService(
            IAnswerTemplateRepository repository,
            IOptions<TemplateSettings> options)
        {
            _repository = repository;
            _settings = options.Value;
        }

        public async Task<string> RenderAsync(string key, object? model = null)
        {
            var template = await _repository.GetByKeyAsync(key)
                ?? throw new Exception($"Template not found: {key}");

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
            var template = await _repository.GetByKeyAsync(key)
                ?? throw new Exception($"Template not found: {key}");

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
                var value = prop.GetValue(model)?.ToString() ?? "";
                template = template.Replace($"{{{{{prop.Name}}}}}", value);
            }

            return template;
        }
    }
}
