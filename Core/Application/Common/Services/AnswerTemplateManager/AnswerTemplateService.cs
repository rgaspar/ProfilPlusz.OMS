using Application.Common.Configuration;
using Application.Common.Repositories;
using Domain.Entities;
using Microsoft.Extensions.Options;
using System.Collections;
using System.Reflection;

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

            var fullPath = Path.Combine(
                AppContext.BaseDirectory,
                _settings.BasePath,
                template.Path);

            if (!File.Exists(fullPath))
                throw new Exception($"Template file not found: {fullPath}");

            var content = await File.ReadAllTextAsync(fullPath);

            return Render(content, model);
        }

        private string Render(string template, object? model)
        {
            if (string.IsNullOrWhiteSpace(template) || model == null)
                return template;

            return RenderInternal(template, model, new HashSet<object>());
        }

        private string RenderInternal(
            string template,
            object model,
            HashSet<object> visited)
        {
            if (model == null)
                return template;

            var type = model.GetType();

            // végtelen ciklus védelem
            if (!IsSimpleType(type))
            {
                if (visited.Contains(model))
                    return template;

                visited.Add(model);
            }

            var properties = type
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.GetIndexParameters().Length == 0);

            foreach (var prop in properties)
            {
                var value = prop.GetValue(model);

                var key = prop.Name;

                // -------------------------
                // LISTA BLOKK
                // {{#Items}} ... {{/Items}}
                // -------------------------
                if (value is IEnumerable list && value is not string)
                {
                    var startTag = $"{{{{#{key}}}}}";
                    var endTag = $"{{{{/{key}}}}}";

                    while (true)
                    {
                        var startIndex = template.IndexOf(startTag, StringComparison.Ordinal);

                        if (startIndex < 0)
                            break;

                        var endIndex = template.IndexOf(endTag, startIndex, StringComparison.Ordinal);

                        if (endIndex < 0)
                            break;

                        var innerTemplate = template.Substring(
                            startIndex + startTag.Length,
                            endIndex - (startIndex + startTag.Length));

                        var renderedItems = "";

                        foreach (var item in list)
                        {
                            renderedItems += RenderInternal(
                                innerTemplate,
                                item!,
                                visited);
                        }

                        template =
                            template.Substring(0, startIndex)
                            + renderedItems
                            + template.Substring(endIndex + endTag.Length);
                    }
                }

                // -------------------------
                // KOMPLEX OBJECT
                // pl Address, Contact
                // -------------------------
                else if (value != null && !IsSimpleType(prop.PropertyType))
                {
                    template = RenderComplexObject(
                        template,
                        key,
                        value,
                        visited);
                }

                // -------------------------
                // SIMPLE VALUE
                // -------------------------
                else
                {
                    template = template.Replace(
                        $"{{{{{key}}}}}",
                        FormatValue(value),
                        StringComparison.Ordinal);
                }
            }

            return template;
        }

        private string RenderComplexObject(
            string template,
            string prefix,
            object value,
            HashSet<object> visited)
        {
            var properties = value
                .GetType()
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.GetIndexParameters().Length == 0);

            foreach (var prop in properties)
            {
                var propValue = prop.GetValue(value);

                var key = $"{prefix}.{prop.Name}";

                if (propValue != null && !IsSimpleType(prop.PropertyType))
                {
                    template = RenderComplexObject(
                        template,
                        key,
                        propValue,
                        visited);
                }
                else
                {
                    template = template.Replace(
                        $"{{{{{key}}}}}",
                        FormatValue(propValue),
                        StringComparison.Ordinal);
                }
            }

            return template;
        }

        private static bool IsSimpleType(Type type)
        {
            if (Nullable.GetUnderlyingType(type) is Type underlying)
                type = underlying;

            return
                type.IsPrimitive
                || type.IsEnum
                || type == typeof(string)
                || type == typeof(decimal)
                || type == typeof(DateTime)
                || type == typeof(DateTimeOffset)
                || type == typeof(TimeSpan)
                || type == typeof(Guid)
                || Convert.GetTypeCode(type) != TypeCode.Object;
        }

        private static string FormatValue(object? value)
        {
            if (value == null)
                return "";

            return value switch
            {
                DateTime dt => dt.ToString("yyyy.MM.dd HH:mm"),
                DateTimeOffset dto => dto.ToString("yyyy.MM.dd HH:mm"),
                bool b => b ? "igen" : "nem",
                _ => value.ToString() ?? ""
            };
        }

        public async Task<AnswerTemplate> GetTemplateAsync(string key)
        {
            if (_templateCache.TryGetValue(key, out var cached))
                return cached;

            var template =
                await _repository.GetByKeyAsync(key)
                ?? throw new Exception($"Template not found: {key}");

            _templateCache[key] = template;

            return template;
        }

        public async Task<string[]> GetDefaultRecipientsAsync(string key)
        {
            var template =
                await _repository.GetByKeyAsync(key)
                ?? throw new Exception($"Template not found: {key}");

            if (string.IsNullOrWhiteSpace(template.DefaultRecipients))
                return Array.Empty<string>();

            return template.DefaultRecipients
                .Split(';', StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.Trim())
                .ToArray();
        }
    }
}