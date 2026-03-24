using Application.Common.DTOs.Email;
using Application.Common.Repositories;
using Application.Common.Services.AnswerTemplateManager;
using Application.Common.Services.EmailManager;
using Application.Common.Services.Location;
using Application.Features.EmailPartnerRecommendation.Settings;
using Domain.Services.Email;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.EmailPartnerRecommendation
{
    public class EmailPartnerRecommendationManager : IEmailPartnerRecommendationManager
    {
        private readonly IEmailReaderService _emailReaderService;
        private readonly IEmailWriterService _emailWriterService;
        private readonly IEmailParserService _emailParser;

        private readonly IAddressResolverService _addressResolverService;
        private readonly ICountyService _countyService;

        private readonly IPartnerRepository _partnerRepository;
        private readonly IAnswerTemplateService _answerTemplateService;

        private readonly EmailPartnerRecommendationSettings _settings;

        public EmailPartnerRecommendationManager(IEmailReaderService emailReaderService, IEmailWriterService emailWriterService, IEmailParserService emailParser, IAddressResolverService addressResolverService, ICountyService countyService, IPartnerRepository partnerRepository, IAnswerTemplateService answerTemplateService, IOptions<EmailPartnerRecommendationSettings> settings)
        {
            _emailReaderService = emailReaderService;
            _emailWriterService = emailWriterService;
            _emailParser = emailParser;
            _addressResolverService = addressResolverService;
            _countyService = countyService;
            _partnerRepository = partnerRepository;
            _answerTemplateService = answerTemplateService;
            _settings = settings.Value;
        }

        public async Task ProcessUnreadEmailsAsync(CancellationToken cancellationToken = default)
        {
            var emails = await _emailReaderService.GetLatestEmailsAsync();

            foreach (var email in emails)
            {
                cancellationToken.ThrowIfCancellationRequested();

                //email feldolgozása
                var parsedEmail = _emailParser.Parse(email.Body);

                if (parsedEmail == null)
                {
                    continue;
                }

                if (string.IsNullOrWhiteSpace(parsedEmail.ShippingAddress.Street))
                {
                    continue;
                }

                //cím → megye
                var addressInfo = await _addressResolverService.ResolveAsync(parsedEmail.ShippingAddress);

                if (addressInfo == null)
                {
                    continue;
                }

                if (string.IsNullOrWhiteSpace(addressInfo.County))
                {
                    continue;
                }

                //megye normalizálása
                var normalizedCounty = _countyService.Normalize(addressInfo.County);

                //szomszédos megyék
                var counties = _countyService.GetWithNeighbours(normalizedCounty);

                //partnerek lekérdezése
                var partners = await _partnerRepository.GetByCountiesAsync(counties, cancellationToken);

                if (!partners.Any())
                {
                    continue;
                }

                //template kitöltése
                var body = await _answerTemplateService.RenderAsync(
                    "EMAIL_PARTNER_RECOMMENDATION",
                    new
                    {
                        CustomerName = parsedEmail.CustomerName,
                        OrderNumber = parsedEmail.OrderNumber,
                        Partners = partners
                    });

                //draft email létrehozása
                await _emailWriterService.CreateDraftAsync(
                    new DraftEmailDto
                    {
                        ToName = _settings.DefaultToName,
                        ToEmail = _settings.DefaultToEmail,

                        Subject = _settings.Subject,
                        Body = body
                    },
                    cancellationToken);
            }
        }
    }
}
