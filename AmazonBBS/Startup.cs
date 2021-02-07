using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(AmazonBBS.Startup))]
namespace AmazonBBS
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        } 
    }
}
