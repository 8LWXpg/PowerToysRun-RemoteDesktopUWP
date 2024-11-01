using System.Diagnostics;
using System.IO;

namespace Community.PowerToys.Run.Plugin.RemoteDesktopUWP.Helpers;

public static class ConnectionsProvider
{
	private static readonly string _connectionsPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\Packages\\Microsoft.RemoteDesktop_8wekyb3d8bbwe\\LocalState\\RemoteDesktopData\\JumpListConnectionArgs";
	private static readonly FileSystemWatcher _watcher;
	private static readonly object _lock = new();
	private static List<Connection>? _cachedConnections;
	public static List<Connection> Connections
	{
		get
		{
			lock (_lock)
			{
				_cachedConnections ??= Directory.GetFiles(_connectionsPath, "*.model").Select(Connection.Parse).ToList();
				return _cachedConnections;
			}
		}
	}

	static ConnectionsProvider()
	{
		_watcher = new()
		{
			Path = _connectionsPath,
			Filter = "*.model",
		};

		static void update(object _, FileSystemEventArgs e)
		{
			lock (_lock)
			{
				_cachedConnections = null;
			}
		}

		_watcher.Changed += update;
		_watcher.Created += update;
		_watcher.Deleted += update;
		_watcher.EnableRaisingEvents = true;
	}
}
