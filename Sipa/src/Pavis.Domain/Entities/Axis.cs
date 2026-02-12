using Pavis.Domain.Entities;

namespace Pavis.Domain.Entities;

public class Axis : BaseEntity
{
    public string Code { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public string Status { get; private set; } = "ACTIVE";

    protected Axis() { }

    public Axis(string code, string name, string status = "ACTIVE")
    {
        Code = code;
        Name = name;
        Status = status;
    }

    public void Update(string name, string? status = null)
    {
        Name = name;
        if (status != null) Status = status;
        UpdateTimestamp();
    }
}
