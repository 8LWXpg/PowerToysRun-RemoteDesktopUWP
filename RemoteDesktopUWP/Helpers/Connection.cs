using System.IO;
using System.Xml.Serialization;
using Wox.Infrastructure;

namespace Community.PowerToys.Run.Plugin.RemoteDesktopUWP.Helpers;

[XmlRoot("SerializableModel", Namespace = "http://schemas.datacontract.org/2004/07/RdClient.Shared.Data")]
[XmlType("ConnectionArgsModel", Namespace = "http://schemas.datacontract.org/2004/07/RdClient.Shared.Models.Launch")]
public class Connection
{
    private static readonly XmlSerializer _serializer = new(typeof(Connection));

    public Guid ConnectionId { get; set; }
    public required string ConnectionType { get; set; }
    public required string Description { get; set; }
    public required string DisplayName { get; set; }
    public Guid WorkspaceId { get; set; }

    public static Connection Parse(string path)
    {
        using var stream = new FileStream(path, FileMode.Open);
        return _serializer.Deserialize(stream) as Connection ?? throw new InvalidOperationException($"Failed to deserialize {path}");
    }

    public bool Open() => Helper.OpenInShell("shell:AppsFolder\\Microsoft.RemoteDesktop_8wekyb3d8bbwe!App", $"{ConnectionId},{WorkspaceId},{ConnectionType},Pinned,{WorkspaceId}");
}