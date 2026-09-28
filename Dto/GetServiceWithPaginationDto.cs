namespace ServiceAggregator.Api.Dto
{
    public class GetServiceWithPaginationDto
    {
        /// <summary>
        /// شناسه سرویس
        /// </summary>
        public Guid Id { get; set; }
        /// <summary>
        /// نام سرویس
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// url سرویس
        /// </summary>
        public string Url { get; set; }
        /// <summary>
        /// متد سرویس
        /// </summary>
        public string Method { get; set; }
        /// <summary>
        /// نوع سرویس
        /// </summary>
        public string Type { get; set; }
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
        /// <summary>
        /// نوع احراز هویت
        /// </summary>
        public string? AuthenticationMode { get; set; }
        /// <summary>
        /// فرمت احراز هویت
        /// </summary>
        public string? AuthenticationFormat { get; set; }
    }
}
