namespace SecurityProvider;

public class SecurityOption
{
    public bool EnableSecurityProvider { get; set; } = true;
    public bool EnableRoleBasedAuthorization { get; set; } = true;
    public bool EnableTokenProvider { get; set; } = true;
}
