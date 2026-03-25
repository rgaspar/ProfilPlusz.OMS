using Application.Common.DTOs.Email;
using Application.Common.Repositories;
using Application.Common.Services.AnswerTemplateManager;
using Application.Common.Services.EmailManager;
using Application.Common.Services.Location;
using Application.Features.EmailPartnerRecommendation.Settings;
using Domain.Entities;
using Domain.Enums;
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

        private readonly IProcessedEmailRepository _processedEmailRepository;

        public EmailPartnerRecommendationManager(IEmailReaderService emailReaderService, IEmailWriterService emailWriterService, IEmailParserService emailParser, IAddressResolverService addressResolverService, ICountyService countyService, IPartnerRepository partnerRepository, IAnswerTemplateService answerTemplateService, IOptions<EmailPartnerRecommendationSettings> settings, IProcessedEmailRepository processedEmailRepository)
        {
            _emailReaderService = emailReaderService;
            _emailWriterService = emailWriterService;
            _emailParser = emailParser;
            _addressResolverService = addressResolverService;
            _countyService = countyService;
            _partnerRepository = partnerRepository;
            _answerTemplateService = answerTemplateService;
            _settings = settings.Value;
            _processedEmailRepository = processedEmailRepository;
        }

        public async Task ProcessUnreadEmailsAsync(CancellationToken cancellationToken = default)
        {
            var emails = await _emailReaderService.GetUnreadEmailsAsync();

            foreach (var email in emails)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var log = new ProcessedEmail
                {
                    ExternalId = email.ExternalId,
                    Subject = email.Subject,
                    ReceivedAt = email.ReceivedAt,
                    ProcessedAt = DateTime.UtcNow,
                    Status = EmailProcessingStatus.Skipped
                };

                try
                {
                    var parsedEmail = _emailParser.Parse(email.Body);

                    log.FromEmail = parsedEmail.Email;
                    log.OrderNumber = parsedEmail.OrderNumber;
                    log.CustomerName = parsedEmail.CustomerName;

                    if (string.IsNullOrWhiteSpace(parsedEmail.Email))
                    {
                        log.Status = EmailProcessingStatus.Skipped;
                        continue;
                    }

                    var partnerExists = await _partnerRepository.ExistsByEmailAsync(parsedEmail.Email, cancellationToken);

                    log.PartnerExists = partnerExists;

                    if (partnerExists)
                    {
                        log.Status = EmailProcessingStatus.Skipped;
                        continue;
                    }

                    if (parsedEmail.ShippingAddress == null || string.IsNullOrWhiteSpace(parsedEmail.ShippingAddress.Street))
                    {
                        log.Status = EmailProcessingStatus.Skipped;
                        continue;
                    }

                    var addressInfo = await _addressResolverService.ResolveAsync(parsedEmail.ShippingAddress);

                    if (string.IsNullOrWhiteSpace(addressInfo?.County))
                    {
                        log.Status = EmailProcessingStatus.Skipped;
                        continue;
                    }

                    var normalizedCounty = _countyService.Normalize(addressInfo.County);

                    log.County = normalizedCounty;

                    var counties = _countyService.GetWithNeighbours(normalizedCounty);

                    var partners = await _partnerRepository.GetByCountiesAsync(counties, cancellationToken);

                    if (!partners.Any())
                    {
                        log.Status = EmailProcessingStatus.Skipped;
                        continue;
                    }

                    var body =
                        await _answerTemplateService.RenderAsync(
                            "EMAIL_PARTNER_RECOMMENDATION",
                            new
                            {
                                CustomerName = parsedEmail.CustomerName,
                                OrderNumber = parsedEmail.OrderNumber,
                                Partners = partners
                            });

                    await _emailWriterService.CreateDraftAsync(
                        new DraftEmailDto
                        {
                            ToName = _settings.DefaultToName,
                            ToEmail = _settings.DefaultToEmail,
                            Subject = _settings.Subject,
                            Body = body
                        },
                        cancellationToken);

                    log.RecommendationSent = true;
                    log.Status = EmailProcessingStatus.Processed;
                }
                catch (Exception ex)
                {
                    log.Status = EmailProcessingStatus.Error;
                    log.ErrorMessage = ex.Message;
                }
                finally
                {
                    await _processedEmailRepository.SaveAsync(log, cancellationToken);
                }
            }
        }
    }
}
