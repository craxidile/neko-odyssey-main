namespace NekoOdyssey.Scripts.Site.Core
{
    public class SiteCore
    {
        public Site.Site Site { get; } = new();

        public void Bind()
        {
            Site.Bind();
        }

        public void Start()
        {
            Site.Start();
        }

        public void Unbind()
        {
            Site.Unbind();
        }
    }
}