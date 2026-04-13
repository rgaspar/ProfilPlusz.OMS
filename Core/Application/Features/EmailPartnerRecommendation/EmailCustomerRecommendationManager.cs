using Application.Common.DTOs.Email;
using Application.Common.Repositories;
using Application.Common.Services.AnswerTemplateManager;
using Application.Common.Services.EmailManager;
using Application.Common.Services.Location;
using Application.Features.CustomerManager.Queries;
using Application.Features.EmailCustomerRecommendation.Settings;
using Application.Features.EmailPartnerRecommendation;
using Domain.Entities;
using Domain.Enums;
using Domain.Services.Email;
using MediatR;
using Microsoft.Extensions.Options;

namespace Application.Features.EmailCustomerRecommendation
{
    public class EmailCustomerRecommendationManager : IEmailCustomerRecommendationManager
    {
        private readonly IEmailReaderService _emailReaderService;
        private readonly IEmailWriterService _emailWriterService;
        private readonly IEmailParserService _emailParser;
        private readonly IAddressResolverService _addressResolverService;
        private readonly IStateService _stateService;
        private readonly ICommandRepository<Customer> _customerRepository;
        private readonly IAnswerTemplateService _answerTemplateService;
        private readonly EmailCustomerRecommendationSettings _settings;
        private readonly IProcessedEmailRepository _processedEmailRepository;
        private readonly ISender _sender;

        public EmailCustomerRecommendationManager(IEmailReaderService emailReaderService, IEmailWriterService emailWriterService, IEmailParserService emailParser, IAddressResolverService addressResolverService, IStateService stateService, IAnswerTemplateService answerTemplateService, IOptions<EmailCustomerRecommendationSettings> settings, IProcessedEmailRepository processedEmailRepository, ISender sender)
        {
            _emailReaderService = emailReaderService;
            _emailWriterService = emailWriterService;
            _emailParser = emailParser;
            _addressResolverService = addressResolverService;
            _stateService = stateService;
            _answerTemplateService = answerTemplateService;
            _settings = settings.Value;
            _processedEmailRepository = processedEmailRepository;
            _sender = sender;
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

                    var customerExistsResult = await _sender.Send(new CustomerExistsByEmailRequest { Email = parsedEmail.Email, IsDeleted = false }, cancellationToken);

                    var customerExists = customerExistsResult.Exists;

                    log.CustomerExists = customerExists;

                    if (customerExists)
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

                    if (string.IsNullOrWhiteSpace(addressInfo?.State))
                    {
                        log.Status = EmailProcessingStatus.Skipped;
                        continue;
                    }

                    var normalizedState = _stateService.Normalize(addressInfo.State);

                    log.State = normalizedState;

                    var states = _stateService.GetWithNeighbours(normalizedState);

                    var result = await _sender.Send(
                                new GetCustomersByCountiesRequest
                                {
                                    States = states
                                },
                                cancellationToken);

                    var customers = result.Data;

                    if (!customers.Any())
                    {
                        log.Status = EmailProcessingStatus.Skipped;
                        continue;
                    }

                    var body =
                        await _answerTemplateService.RenderAsync(
                            "EMAIL_CUSTOMER_RECOMMENDATION",
                            new
                            {
                                CustomerName = parsedEmail.CustomerName,
                                OrderNumber = parsedEmail.OrderNumber,
                                Customers = customers
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
