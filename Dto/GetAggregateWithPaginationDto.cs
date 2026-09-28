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
    }
}
