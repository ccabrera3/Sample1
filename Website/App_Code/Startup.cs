using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Sample1.Startup))]
namespace Sample1
{
    public partial class Startup {
        public void Configuration(IAppBuilder app) {
            ConfigureAuth(app);
        }
    }
}
