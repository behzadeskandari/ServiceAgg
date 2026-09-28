namespace ServiceAggregator.Api.Dto;
public class LoginWithCaptchaRequest
{
    /// <summary>
    /// نام کاربری
    /// </summary>
    public string Username { get; set; } = "";
    /// <summary>
    /// رمز عبور
    /// </summary>
    public string Password { get; set; } = "";
    /// <summary>
    /// عبارت امنیتی
    /// </summary>
    public string? Captcha { get; set; }
    /// <summary>
    /// کد رهگیری
    /// </summary>
    public string? Key { get; set; }
}
