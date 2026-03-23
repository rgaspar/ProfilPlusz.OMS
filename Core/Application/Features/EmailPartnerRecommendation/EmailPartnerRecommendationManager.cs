using Application.Common.Repositories;
using Application.Common.Services.AnswerTemplateManager;
using Application.Common.Services.EmailManager;
using Application.Common.Services.Location;
using Domain.Services.Email;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public EmailPartnerRecommendationManager(IEmailReaderService emailReaderService, IEmailWriterService emailWriterService, IEmailParserService emailParser, IAddressResolverService addressResolverService, ICountyService countyService, IPartnerRepository partnerRepository, IAnswerTemplateService answerTemplateService)
        {
            _emailReaderService = emailReaderService;
            _emailWriterService = emailWriterService;
            _emailParser = emailParser;
            _addressResolverService = addressResolverService;
            _countyService = countyService;
            _partnerRepository = partnerRepository;
            _answerTemplateService = answerTemplateService;
        }

        public async Task ProcessUnreadEmailsAsync(CancellationToken cancellationToken = default)
        {
            var emails = await _emailReaderService.GetLatestEmailsAsync(1);

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
                            Counties = counties,
                            Partners = partners
                        });

                /*
                //draft email létrehozása
                await _emailWriterService.CreateDraftAsync(
                    new DraftEmailDto
                    {
                        To = parsedEmail.SenderEmail,
                        Subject = "Ajánlott partnerek a környékeden",
                        Body = body
                    },
                    cancellationToken);
                */
            }
        }
    }
}
