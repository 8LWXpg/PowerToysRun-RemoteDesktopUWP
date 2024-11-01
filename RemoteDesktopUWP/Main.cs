using Community.PowerToys.Run.Plugin.RemoteDesktopUWP.Helpers;
using ManagedCommon;
using System.Windows.Controls;
using Wox.Infrastructure;
using Wox.Plugin;

namespace Community.PowerToys.Run.Plugin.RemoteDesktopUWP;
public class Main : IPlugin, IPluginI18n, IReloadable, IDisposable
{
	private PluginInitContext? _context;
	private string? _iconPath;
	private bool _disposed;
	public string Name => Properties.Resources.plugin_name;
	public string Description => Properties.Resources.plugin_description;
	public static string PluginID => "1826ac5f67f74388b370f4182ac54dc7";

	public List<Result> Query(Query query)
	{
		ArgumentNullException.ThrowIfNull(query);

		var results = ConnectionsProvider.Connections.Select((c) =>
		{
			MatchResult match = StringMatcher.FuzzySearch(query.Search, c.DisplayName);
			return new Result
			{
				Title = c.DisplayName,
				SubTitle = c.Description,
				TitleHighlightData = match.MatchData,
				IcoPath = _iconPath,
				Score = match.Score,
				Action = _ => c.Open(),
			};
		}).ToList();

		if (!string.IsNullOrEmpty(query.Search))
		{
			_ = results.RemoveAll(x => x.Score <= 0);
		}

		return results;
	}

	public void Init(PluginInitContext context)
	{
		_context = context ?? throw new ArgumentNullException(nameof(context));
		_context.API.ThemeChanged += OnThemeChanged;
		UpdateIconPath(_context.API.GetCurrentTheme());
	}

	public string GetTranslatedPluginTitle() => Properties.Resources.plugin_name;

	public string GetTranslatedPluginDescription() => Properties.Resources.plugin_description;

	private void OnThemeChanged(Theme oldtheme, Theme newTheme) => UpdateIconPath(newTheme);

	private void UpdateIconPath(Theme theme)
	{
		_iconPath = theme is Theme.Light or Theme.HighContrastWhite
			? "Images/RemoteDesktopUWP.light.png"
			: "Images/RemoteDesktopUWP.dark.png";
	}

	public Control CreateSettingPanel() => throw new NotImplementedException();

	public void ReloadData()
	{
		if (_context is null)
		{
			return;
		}

		UpdateIconPath(_context.API.GetCurrentTheme());
	}

	public void Dispose()
	{
		Dispose(true);
		GC.SuppressFinalize(this);
	}

	protected virtual void Dispose(bool disposing)
	{
		if (!_disposed && disposing)
		{
			if (_context != null && _context.API != null)
			{
				_context.API.ThemeChanged -= OnThemeChanged;
			}

			_disposed = true;
		}
	}
}
