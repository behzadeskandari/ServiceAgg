namespace ServiceAggregator.Api.Dto
{
    public class GetServiceWithPaginationQuery
    {
        /// <summary>
        /// نام سرویس
        /// </summary>
        public string? Name { get; set; }
        /// <summary>
        /// Url سرویس
        /// </summary>
        public string? Url { get; set; }
        /// <summary>
        /// وضعیت
        /// </summary>
        public bool? Status { get; set; }


        public List<Inputs> Inputs { get; set; } = new List<Inputs>();
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


    public class Inputs
    {
        public Guid Id { get; set; }
        public Guid? ParentId { get; set; }
        public Guid? ServiceId { get; set; }
        public string? Name { get; set; }
        public string? Type { get; set; }
        public string? Description { get; set; }
        public string? Direction { get; set; }
        public string? CreatedDate { get; set; }
        public string? CreateDate { get; set; }
        public string? UpdateDate { get; set; }
    }

}
