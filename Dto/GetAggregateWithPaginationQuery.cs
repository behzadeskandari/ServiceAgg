namespace ServiceAggregator.Api.Dto
{
    public class GetAggregateWithPaginationQuery
    {
        /// <summary>
        /// نام ترکیب
        /// </summary>
        public string? Name { get; set; }
        /// <summary>
        /// وضعیت
        /// </summary>
        public bool? Status { get; set; }
        /// <summary>
        /// تاریخ ایجاد
        /// </summary>
        public DateTime? CreateDate { get; set; }
        /// <summary>
        /// شماره صفحه
        /// </summary>
        public int PageIndex { get; set; } = 1;
        /// <summary>
        /// تعداد در هر صفحه
        /// </summary>
        public int PageSize { get; set; } = 10;
    }
}
