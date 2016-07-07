using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(MemberCenter.Startup))]
namespace MemberCenter
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
