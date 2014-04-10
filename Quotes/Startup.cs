using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Quotes.Startup))]
namespace Quotes
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
