namespace TDining.Api.Options;

public sealed class ApplicationOptions
{
    public const string SectionName = "Application";

    public string ProductName { get; init; } = "T Dining";

    public string SupportEmail { get; init; } = "support@example.com";

    public string Currency { get; init; } = "VND";

    public string TimeZone { get; init; } = "Asia/Ho_Chi_Minh";
}
