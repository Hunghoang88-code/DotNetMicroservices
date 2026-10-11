namespace DashboardService.Api.Queries
{
    public enum TimeUnit { Day, Month, Year }

    public class GetTotalSalesQuery { public System.DateTime SalesDateFrom { get; set; } public System.DateTime SalesDateTo { get; set; } public string ProductCode { get; set; } }
    public class GetAgentsSalesQuery { public System.DateTime SalesDateFrom { get; set; } public System.DateTime SalesDateTo { get; set; } }
    public class GetSalesTrendsQuery { public System.DateTime SalesDateFrom { get; set; } public System.DateTime SalesDateTo { get; set; } public TimeUnit Unit { get; set; } }

    public class GetTotalSalesResult { public SalesSummaryDto Total { get; set; } public System.Collections.Generic.Dictionary<string, SalesDto> PerProductTotal { get; set; } }
    public class GetAgentsSalesResult { public System.Collections.Generic.Dictionary<string, SalesDto> PerAgentTotal { get; set; } }
    public class GetSalesTrendsResult { public System.Collections.Generic.IEnumerable<PeriodSalesDto> PeriodsSales { get; set; } }

    public class PeriodSalesDto { public string Period { get; set; } public SalesDto Sales { get; set; } }
    public class SalesDto { public decimal PremiumAmount { get; set; } public int PoliciesCount { get; set; } }
    public class SalesSummaryDto { public decimal PremiumAmount { get; set; } public int PoliciesCount { get; set; } }
}

namespace ProductService.Api.Queries.Dtos
{
    public class ProductDto
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public string Icon { get; set; }
        public System.Collections.Generic.IEnumerable<CoverDto> Covers { get; set; } = new System.Collections.Generic.List<CoverDto>();
        public System.Collections.Generic.IEnumerable<QuestionDto> Questions { get; set; } = new System.Collections.Generic.List<QuestionDto>();
    }

    public class CoverDto { public string Code { get; set; } public string Name { get; set; } public decimal SumInsured { get; set; } }

    public abstract class QuestionDto { public string QuestionCode { get; set; } public string Text { get; set; } }
    public class ChoiceQuestionDto : QuestionDto { public System.Collections.Generic.IList<ChoiceDto> Choices { get; set; } = new System.Collections.Generic.List<ChoiceDto>(); }
    public class NumericQuestionDto : QuestionDto { }
    public class ChoiceDto { public string Code { get; set; } public string Text { get; set; } public string Label => Text; }
}

namespace PolicyService.Api.Commands.Dtos
{
    public enum QuestionType { Numeric = 0, Choice = 1 }

    public class CreateOfferResult { public string OfferNumber { get; set; } public decimal Price { get; set; } public decimal TotalPrice { get; set; } }
    public class PersonDto { public string FirstName { get; set; } public string LastName { get; set; } public string TaxId { get; set; } }
    public class AddressDto { public string Street { get; set; } public string City { get; set; } public string Country { get; set; } public string ZipCode { get; set; } }
    public class CreatePolicyResult { public string PolicyNumber { get; set; } }

    // Question answer models used by the client
    public abstract class QuestionAnswer { public string QuestionCode { get; set; } public QuestionType QuestionType { get; set; } }
    public class ChoiceQuestionAnswer : QuestionAnswer { public string Answer { get; set; } public ChoiceQuestionAnswer() { QuestionType = QuestionType.Choice; } }
    public class NumericQuestionAnswer : QuestionAnswer { public decimal Answer { get; set; } public NumericQuestionAnswer() { QuestionType = QuestionType.Numeric; } }
}

namespace PolicyService.Api.Commands
{
    using System.Collections.Generic;
    using PolicyService.Api.Commands.Dtos;

    public class CreateOfferCommand
    {
        public string ProductCode { get; set; }
        public System.DateTime PolicyFrom { get; set; }
        public System.DateTime PolicyTo { get; set; }
        public List<string> SelectedCovers { get; set; }
        public List<QuestionAnswer> Answers { get; set; }
    }

    public class CreatePolicyCommand
    {
        public string OfferNumber { get; set; }
        public PersonDto PolicyHolder { get; set; }
        public AddressDto PolicyHolderAddress { get; set; }
    }
}

namespace PolicyService.Api.Queries
{
    using PolicyService.Api.Commands.Dtos;

    public class PolicyDto { public string Number { get; set; } public System.DateTime DateFrom { get; set; } public System.DateTime DateTo { get; set; } public string PolicyHolder { get; set; } public decimal TotalPremium { get; set; } public string ProductCode { get; set; } public System.Collections.Generic.List<string> Covers { get; set; } }

    public class GetPolicyDetailsQueryResult { public PolicyDto Policy { get; set; } }
}

namespace PolicySearchService.Api.Queries
{
    public class FindPolicyResult { public string PolicyNumber { get; set; } public string PolicyHolder { get; set; } public System.Collections.Generic.List<PolicySearchService.Api.Queries.Dtos.PolicyDto> Policies { get; set; } }
}

namespace PolicySearchService.Api.Queries.Dtos
{
    public class PolicyDto { public string PolicyNumber { get; set; } public string ProductCode { get; set; } public string PolicyStartDate { get; set; } public string PolicyEndDate { get; set; } public string PolicyHolder { get; set; } public decimal PremiumAmount { get; set; } }
}

namespace DashboardService.Api.Queries.Dtos
{
    // placeholder for dashboard DTOs
}
