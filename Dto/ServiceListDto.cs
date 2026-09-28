namespace ServiceAggregator.Api.Dto
{
    public class ServiceListDto
    {
        /// <summary>
        /// شناسه سرویس
        /// </summary>
        public Guid Id { get; set; }
        /// <summary>
        /// عنوان سرویس
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// url سرویس
        /// </summary>
        public string Url { get; set; }
        /// <summary>
        /// متد سرویس
        /// </summary>
        public string Method { get; set; } = "POST"; // GET or POST
        /// <summary>
        /// نوع سرویس
        /// </summary>
        public string Type { get; set; } = "REST"; // SOAP
        /// <summary>
        /// وضعیت
        /// </summary>
        public bool Status { get; set; }
    }
}
