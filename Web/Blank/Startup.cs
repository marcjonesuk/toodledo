using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Blank.Startup))]
namespace Blank
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
