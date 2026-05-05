namespace YANEDGE.EmailManagement.Authentication;

public class CurrentUserInfoDto
{
    public bool IsAuthenticated { get; set; }

    public string? UserId { get; set; }

    public string? UserName { get; set; }

    public string? AuthenticationType { get; set; }

    public List<string> Roles { get; set; } = [];

    public List<string> Permissions { get; set; } = [];

    public List<string> Scopes { get; set; } = [];
}
