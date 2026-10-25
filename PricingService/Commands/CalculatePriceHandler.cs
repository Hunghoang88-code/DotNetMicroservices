using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using PricingService.Api.Commands;
using PricingService.Domain;

namespace PricingService.Commands
{
    public class CalculatePriceHandler : IRequestHandler<CalculatePriceCommand, CalculatePriceResult>
    {
        private readonly IDataStore _dataStore;

        public CalculatePriceHandler(IDataStore dataStore)
        {
            _dataStore = dataStore;
        }

        public async Task<CalculatePriceResult> Handle(CalculatePriceCommand request, CancellationToken cancellationToken)
        {
            // Validate command
            var validator = new CalculatePriceCommandValidator();
            var validationResult = validator.Validate(request);
            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }

            var tariff = await _dataStore.Tariffs[request.ProductCode];
            if (tariff == null)
            {
                return CalculatePriceResult.Empty();
            }

            var subject = request.Answers.ToDictionary(answer => answer.QuestionCode, answer => answer.GetAnswer());
            var price = tariff.CalculatePrice(new Calculation(
                request.ProductCode,
                request.PolicyFrom,
                request.PolicyTo,
                request.SelectedCovers,
                subject));

            return new CalculatePriceResult
            {
                TotalPrice = price.TotalPremium,
                CoverPrices = price.Covers.ToDictionary(cover => cover.Key, cover => cover.Value.Price)
            };
        }
    }
}