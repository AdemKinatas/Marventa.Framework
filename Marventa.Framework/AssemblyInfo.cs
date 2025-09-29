using System.Runtime.CompilerServices;

// Assembly forwarding to embedded modules - core extension classes only
[assembly: TypeForwardedTo(typeof(Marventa.Framework.Web.Extensions.MarventaExtensions))]
[assembly: TypeForwardedTo(typeof(Marventa.Framework.Core.Extensions.StringExtensions))]
[assembly: TypeForwardedTo(typeof(Marventa.Framework.Core.Extensions.DateTimeExtensions))]