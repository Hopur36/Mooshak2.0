using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Mooshack_2.Startup))]
namespace Mooshack_2
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
