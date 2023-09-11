using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(appinno2.Startup))]
namespace appinno2
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
