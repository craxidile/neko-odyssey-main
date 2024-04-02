namespace NekoOdyssey.Scripts.Models
{
    public class CatProfile
    {
        public string CatCode { get; set; }
        public string AnimatorControllerName { get; set; }
        
        public float NearestPlayerDistance { get; set; }
        public float MoveSpeed { get; set; }
        public float JumpSpeed { get; set; }
        public float EatingDuration { get; set; }
        
        public bool HasCallToFeedBehaviour;
        public bool HasFollowPlayerBehaviour;
    }
}