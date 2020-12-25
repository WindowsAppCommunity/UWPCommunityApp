using Tizen.Applications;
using Uno.UI.Runtime.Skia;

namespace UWPCommunityApp.Skia.Tizen
{
	class Program
	{
		static void Main(string[] args)
		{
			var host = new TizenHost(() => new UWPCommunityApp.App(), args);
			host.Run();
		}
	}
}
