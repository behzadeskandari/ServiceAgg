using ServiceAggregator.Api.Models;

namespace ServiceAggregator.Api.Dto
{
    public record GetAggregateWithPaginationDto
    {
        /// <summary>
        /// شناسه رکورد
        /// </summary>
        public Guid Id { get; set; }
        /// <summary>
        /// شناسه اولین مرحله
        /// </summary>
        public Guid? FirstStepId { get; set; }
        /// <summary>
        /// نام ترکیب
        /// </summary>
        public string Name { get; set; } = "";
        /// <summary>
        /// شرح ترکیب
        /// </summary>
        public string Description { get; set; } = "";
        /// <summary>
        /// وضعیت
        /// </summary>
        public bool Status { get; set; } = true;
        /// <summary>
        /// تاریخ ایجاد
        /// </summary>
        public DateTime? CreateDate { get; set; }
        /// <summary>
        /// تاریخ تغییر
        /// </summary>
        public DateTime? UpdateDate { get; set; }


        public List<AggregateStepDto> Steps { get; set; } = [];
    }
    public class AggregateStepMappingDto
    {
        public Guid Id { get; set; }

        public Guid AggregateStepId { get; set; }

        public Guid? InputStepId { get; set; }

        public string MappingType { get; set; } = string.Empty;

        public string Source { get; set; } = string.Empty;

        public string? SourceField { get; set; }

        public string? TargetField { get; set; }

        public string? Value { get; set; }

        public string? ValueType { get; set; }

        public bool Status { get; set; }
    }

    public class AggregateStepDto
    {
        public Guid Id { get; set; }
        public Guid AggregateId { get; set; }
        public string StepName { get; set; } = string.Empty;

        public Guid? ServiceId { get; set; }

        public Guid? NextStepId { get; set; }
        public Guid? TrueStepId { get; set; }
        public Guid? FalseStepId { get; set; }

        public string? Condition { get; set; }
        public string? ConditionParameters { get; set; }

        public bool Status { get; set; }

        public List<AggregateStepMappingDto> Mappings { get; set; } = [];
    }
}

