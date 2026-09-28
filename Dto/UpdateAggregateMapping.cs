namespace ServiceAggregator.Api.Dto
{
    public class UpdateAggregateMapping
    {
        /// <summary>
        /// شناسه مپینگ ترکیب
        /// </summary>
        public Guid Id { get; set; }
        /// <summary>
        /// شناسه مپینگ سرویس Parent
        /// </summary>
        public Guid? ParentId { get; set; }
        /// <summary>
        /// شناسه ترکیب
        /// </summary>
        public Guid AggregateId { get; set; }
        /// <summary>
        /// عنوان فیلد
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// نوع فیلد
        /// </summary>
        public string Type { get; set; }
        /// <summary>
        /// شرح
        /// </summary>
        public string Description { get; set; }
    }
}
