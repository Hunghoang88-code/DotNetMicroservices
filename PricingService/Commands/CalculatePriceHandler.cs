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

            // Logic tính giá dựa trên Tariff
            var tariff = await _dataStore.Tariffs[request.ProductCode];
            if (tariff == null)
            {
                return CalculatePriceResult.Empty();
            }

            // Trả về kết quả tính toán (placeholder implementation)
            var result = new CalculatePriceResult
            {
                TotalPrice = 100M,
                CoverPrices = new System.Collections.Generic.Dictionary<string, decimal>()
            };

            return result;
        }
    }
}