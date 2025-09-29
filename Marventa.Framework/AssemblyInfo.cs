using System.Runtime.CompilerServices;

// Assembly forwarding to embedded modules - essential extension classes only
[assembly: TypeForwardedTo(typeof(Marventa.Framework.Web.Extensions.ApplicationBuilderExtensions))]
[assembly: TypeForwardedTo(typeof(Marventa.Framework.Web.Extensions.ServiceCollectionExtensions))]
[assembly: TypeForwardedTo(typeof(Marventa.Framework.Web.Extensions.MarventaExtensions))]
[assembly: TypeForwardedTo(typeof(Marventa.Framework.Web.Extensions.MessagingExtensions))]
[assembly: TypeForwardedTo(typeof(Marventa.Framework.Core.Extensions.StringExtensions))]
[assembly: TypeForwardedTo(typeof(Marventa.Framework.Core.Extensions.DateTimeExtensions))]