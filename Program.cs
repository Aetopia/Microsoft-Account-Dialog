using System;
using System.Threading;
using System.Diagnostics;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Security.Credentials;
using Windows.UI.ApplicationSettings;
using Windows.Security.Authentication.Web.Core;
using System.Drawing;
using System.Reflection;

namespace Windows.UI.ApplicationSettings
{
	[Guid("D3EE12AD-3865-4362-9746-B75A682DF0E6")]
	[InterfaceType(ComInterfaceType.InterfaceIsIInspectable)]
	internal interface IAccountsSettingsPaneInterop
	{
		[STAThread]
		AccountsSettingsPane GetForWindow(IntPtr appWindow, Guid riid);
		IAsyncAction ShowManagedAccountsForWindowAsync(IntPtr appWindow, Guid riid);
		IAsyncAction ShowAddAccountForWindowAsync(IntPtr appWindow, Guid riid);
	}

	internal static class AccountsSettingsPaneInterop
	{
		private static readonly IAccountsSettingsPaneInterop accountsSettingsPaneInterop = WindowsRuntimeMarshal.GetActivationFactory(typeof(AccountsSettingsPane)) as IAccountsSettingsPaneInterop;

		[STAThread]
		public static AccountsSettingsPane GetForWindow(IntPtr appWindow)
		{
			return accountsSettingsPaneInterop.GetForWindow(appWindow, typeof(AccountsSettingsPane).GetInterface("IAccountsSettingsPane").GUID);
		}

		public static IAsyncAction ShowManagedAccountsForWindowAsync(IntPtr appWindow)
		{
			return accountsSettingsPaneInterop.ShowManagedAccountsForWindowAsync(appWindow, typeof(IAsyncAction).GUID);
		}

		public static IAsyncAction ShowAddAccountForWindowAsync(IntPtr appWindow)
		{
			return accountsSettingsPaneInterop.ShowAddAccountForWindowAsync(appWindow, typeof(IAsyncAction).GUID);
		}
	}
}

namespace Windows.Security.Authentication.Web.Core
{

	[Guid("F4B8E804-811E-4436-B69C-44CB67B72084")]
	[InterfaceType(ComInterfaceType.InterfaceIsIInspectable)]
	internal interface IWebAuthenticationCoreManagerInterop
	{
		IAsyncOperation<WebTokenRequestResult> RequestTokenForWindowAsync(IntPtr appWindow, WebTokenRequest request, Guid riid);
		IAsyncOperation<WebTokenRequestResult> RequestTokenWithWebAccountForWindowAsync(IntPtr appWindow, WebTokenRequest request, WebAccount webAccount, Guid riid);
	};

	public static class WebAuthenticationCoreManagerInterop
	{
		private static readonly IWebAuthenticationCoreManagerInterop webAuthenticationCoreManagerInterop = WindowsRuntimeMarshal.GetActivationFactory(typeof(WebAuthenticationCoreManager)) as IWebAuthenticationCoreManagerInterop;

		public static IAsyncOperation<WebTokenRequestResult> RequestTokenForWindowAsync(IntPtr appWindow, WebTokenRequest request)
		{
			return webAuthenticationCoreManagerInterop.RequestTokenForWindowAsync(appWindow, request, typeof(IAsyncOperation<WebTokenRequestResult>).GUID);
		}

		public static IAsyncOperation<WebTokenRequestResult> RequestTokenWithWebAccountForWindowAsync(IntPtr appWindow, WebTokenRequest request, WebAccount webAccount)
		{
			return webAuthenticationCoreManagerInterop.RequestTokenWithWebAccountForWindowAsync(appWindow, request, webAccount, typeof(IAsyncOperation<WebTokenRequestResult>).GUID);
		}
	}
}

class Window : NativeWindow
{
	public Window()
	{
		CreateHandle(new CreateParams()
		{
			Caption = "Microsoft account",
			Style = unchecked((int)0x10000000L) | unchecked((int)0x80000000L),
			X = Screen.PrimaryScreen.WorkingArea.Width / 2,
			Y = Screen.PrimaryScreen.WorkingArea.Height / 2
		});
	}
}

class Program
{
	[DllImport("User32")]
	[DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
	static extern int GetWindowThreadProcessId(IntPtr hWnd, out int lpdwProcessId);

	[DllImport("User32")]
	[DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
	static extern IntPtr GetShellWindow();

	[DllImport("User32")]
	[DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
	static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

	[DllImport("User32")]
	[DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
	static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

	[STAThread]
	static void Main()
	{
		new Thread(() =>
		{
			GetWindowThreadProcessId(GetShellWindow(), out int dwProcessId);
			try
			{
				using var process = Process.GetProcessById(dwProcessId);
				process.WaitForExit();
			}
			catch { }
			Environment.Exit(0);
		}).Start();

		IntPtr appWindow = new Window().Handle;
		using Icon icon = Icon.ExtractAssociatedIcon(Assembly.GetExecutingAssembly().Location);
		SendMessage(appWindow, 0x0080, new IntPtr(1), icon.Handle);
		SendMessage(appWindow, 0x0080, IntPtr.Zero, icon.Handle);

		AccountsSettingsPaneInterop.GetForWindow(appWindow).AccountCommandsRequested += (sender, e) =>
		{
			var provider = WebAuthenticationCoreManager.FindAccountProviderAsync("https://login.microsoft.com", "consumers").AsTask().Result;
			e.WebAccountProviderCommands.Add(new(provider,
											(command) => WebAuthenticationCoreManagerInterop.RequestTokenForWindowAsync(appWindow, new(provider, "wl.basic", "none")).AsTask().Wait()));
			e.GetDeferral().Complete();
		};
		AccountsSettingsPaneInterop.ShowAddAccountForWindowAsync(appWindow).AsTask().Wait();

		ShowWindow(appWindow, 0);
		Environment.Exit(0);
	}
}