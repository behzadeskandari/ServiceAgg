using Microsoft.AspNetCore.Mvc;
using ServiceAggregator.Api.Dto;
using ServiceAggregator.Api.Services;

namespace ServiceAggregator.Api.Controllers
{
    [ApiController]
    [Route("api/test")]
    public class TestController : ControllerBase
    {
        public TestController()
        {
        }
        [HttpPost("service1")]
        public IActionResult Service1(Service1Request service1request)
        {
            return Ok("{\"result\":{\"customerBriefDetailInfoBeans\":[{\"cif\":\"59628274\",\"standardName\":\"حميدرضا/طاهريان/داود\",\"name\":\"حميدرضا طاهريان\",\"corporationId\":null,\"gender\":\"MALE\",\"mobile\":\"+989124939480\",\"enabled\":true,\"firstName\":\"حميدرضا\",\"latinFirstName\":\"a\",\"lastName\":\"طاهريان\",\"latinLastName\":\"b\",\"fatherName\":\"داود\",\"latinFatherName\":\"c\",\"ssn\":\"0081115296\"}]}}");
        }
        [HttpPost("service2")]
        public IActionResult Service2(Service2Request service2request)
        {
            return Ok("{\"cif\":\"59628274\",\"standardName\":\"حميدرضا/طاهريان/داود\",\"name\":\"حميدرضا طاهريان\",\"corporationId\":null,\"gender\":\"MALE\",\"mobile\":\"+989124939480\",\"enabled\":true,\"firstName\":\"حميدرضا\",\"latinFirstName\":\"a\",\"lastName\":\"طاهريان\",\"latinLastName\":\"b\",\"fatherName\":\"داود\",\"latinFatherName\":\"c\",\"ssn\":\"0081115296\"}");
        }
    }
}
