using System.IO;
using System.Xml.Serialization;

namespace Community.PowerToys.Run.Plugin.RemoteDesktopUWP.Helpers;

public static class ConnectionsProvider
{
	private static readonly string _connectionsPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\Packages\\Microsoft.RemoteDesktop_8wekyb3d8bbwe\\LocalState\\RemoteDesktopData\\JumpListConnectionArgs";
	private static readonly FileSystemWatcher _watcher;
	private static readonly object _lock = new();
	private static IEnumerable<Connection>? _cachedConnections;
	public static IEnumerable<Connection> Connections
	{
		get
		{
			lock (_lock)
			{
				_cachedConnections ??= GetConnections();
				return _cachedConnections;
			}
		}
	}

	static ConnectionsProvider()
	{
		_watcher = new(_connectionsPath);

		_watcher.Changed += (_, _) =>
		{
			lock (_lock)
			{
				_cachedConnections = null;
			}
		};
		_watcher.EnableRaisingEvents = true;
	}

	private static IEnumerable<Connection> GetConnections()
	{
		return Directory.GetFiles(_connectionsPath, "*.model").Select(Connection.Parse);
	}
}
