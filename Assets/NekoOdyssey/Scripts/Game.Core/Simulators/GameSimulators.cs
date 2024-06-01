using NekoOdyssey.Scripts.Game.Core.Simulators.SocialNetwork;

namespace NekoOdyssey.Scripts.Game.Core.Simulators
{
    public class GameSimulators
    {
        public readonly SocialNetworkSimulator SocialNetworkSimulator = new();
        
        public void Bind()
        {
            SocialNetworkSimulator.Bind();
        }

        public void Start()
        {
            SocialNetworkSimulator.Start();
        }

        public void Unbind()
        {
            SocialNetworkSimulator.Unbind();
        }
        
    }
}