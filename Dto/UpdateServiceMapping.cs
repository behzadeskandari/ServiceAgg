using ServiceAggregator.Api.Models;

namespace ServiceAggregator.Api.Dto
{
    public class UpdateServiceMapping
    {
        /// <summary>
        /// شناسه مپینگ سرویس
        /// </summary>
        public Guid Id { get; set; }
        /// <summary>
        /// شناسه مپینگ سرویس Parent
        /// </summary>
        public Guid? ParentId { get; set; }
        /// <summary>
        /// شناسه سرویس
        /// </summary>
        public Guid ServiceId { get; set; }
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
        /// <summary>
        /// نوع ورودی (1) یا خروجی (2)
        /// </summary>
        public MappingDirection Direction { get; set; }
    }
}
