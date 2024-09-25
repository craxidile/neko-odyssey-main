using System;
using System.Linq;
using NekoOdyssey.Scripts;
using UnityEngine;
using Random = UnityEngine.Random;

namespace MiniGame05_Fishing.Scripts
{
    public class MiniGame05RewardManager : MonoBehaviour
    {
        public static MiniGame05RewardManager Instance { get; private set; }

        private static readonly string[][] Rewards = 
        {
            IdsToItemCodes(new[] { 1, 2, 3, 4, 5 }),
            IdsToItemCodes(new[] { 6, 7, 8 }),
            IdsToItemCodes(new[] { 9, 10 }),
            IdsToItemCodes(new[] { 11 }),
        };
        
        // Start is called before the first frame update
        private void Start()
        {
            Instance = this;
            var connector = MiniGameRunner.Instance.Connector;
            var manager = MiniGame05.Instance;
            Debug.Log($">>difficulty<< {connector.inputValue}");
            manager.difficult = connector.inputValue;
        }

        private static string[] IdsToItemCodes(int[] itemIds) =>
            itemIds.Select(iid => $"CatFoodFish{($"{iid}".PadLeft(3, '0'))}").ToArray();

        public void RandomReward(int pull)
        {
            var randomValue = Random.Range(0f, 1f);
            Debug.Log($">>pull<< compare pull:{pull} random:{Math.Min(Rewards.Length - 1, pull)} length:{Rewards.Length}");
            var pullToRandom = randomValue < 0.8f
                ? Rewards[pull - 1] : Rewards[Math.Min(Rewards.Length - 1, pull)];
            SendRewards(pullToRandom[Random.Range(0, pullToRandom.Length)]);
        }
        
        private static void SendRewards(string code)
        {
            var connector = MiniGameRunner.Instance.Connector;
            connector.ReceiveItem(new[] {(code, 1)}.ToList());
        }
    }
}
