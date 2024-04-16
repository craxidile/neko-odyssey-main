namespace NekoOdyssey.Scripts.Site.Core
{
    public class SiteCore
    {
        public Site.Site Site { get; } = new();
        public Transition.Transition Transition { get; } = new();

        public void Bind()
        {
            Site.Bind();
            Transition.Bind();
        }

        public void Start()
        {
            Site.Start();
            Transition.Start();
        }

        public void Unbind()
        {
            Site.Unbind();
            Transition.Unbind();
        }
    }
}