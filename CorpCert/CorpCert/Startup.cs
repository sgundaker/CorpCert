using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(L2Test.Startup))]
namespace L2Test
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
