using System.IO;

namespace Community.PowerToys.Run.Plugin.RemoteDesktopUWP.Helpers;

public static class ConnectionsProvider
{
	private static readonly string ConnectionsPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\Packages\\Microsoft.RemoteDesktop_8wekyb3d8bbwe\\LocalState\\RemoteDesktopData\\JumpListConnectionArgs";
	private static readonly FileSystemWatcher Watcher;
	private static readonly object Lock = new();
	private static List<Connection>? CachedConnections;
	public static List<Connection> Connections
	{
		get
		{
			lock (Lock)
			{
				CachedConnections ??= Directory.GetFiles(ConnectionsPath, "*.model").Select(Connection.Parse).ToList();
				return CachedConnections;
			}
		}
	}

	static ConnectionsProvider()
	{
		Watcher = new()
		{
			Path = ConnectionsPath,
			Filter = "*.model",
		};

		static void update(object _, FileSystemEventArgs e)
		{
			lock (Lock)
			{
				CachedConnections = null;
			}
		}

		Watcher.Changed += update;
		Watcher.Created += update;
		Watcher.Deleted += update;
		Watcher.EnableRaisingEvents = true;
	}
}
