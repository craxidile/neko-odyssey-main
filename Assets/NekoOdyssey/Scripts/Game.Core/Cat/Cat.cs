using UniRx;

namespace NekoOdyssey.Scripts.Game.Core.Cat
{
    public class Cat
    {
        public string Code { get; }
        public Subject<CatEmotion> OnChangeEmotion { get; } = new();
        public CatEmotion Emotion { get; set; } = CatEmotion.None;

        public Cat(string code)
        {
            Code = code;
        }

        public void Bind()
        {
        }

        public void Unbind()
        {
        }

        public void Start()
        {
        }

        public void SetEmotion(CatEmotion emotion)
        {
            Emotion = emotion;
            OnChangeEmotion.OnNext(emotion);
        }
        
    }
}