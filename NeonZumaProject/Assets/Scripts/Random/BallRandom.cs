using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Generator
{
    public class BallRandom : MonoBehaviour
    {
        public BallTypesStorage storage;

        public int minSequence = 1;
        public int maxSequence = 9;

        List<BallInfo> selection;
        BallInfo lastSelected;

        void Awake()
        {
            selection = new List<BallInfo>();
            lastSelected = storage.ballTypes[0];
            for (int i = 1; i < storage.ballTypes.Count; i++) {
                selection.Add(storage.ballTypes[i]);
            }
        }

        public BallInfo GetSingleBall(BallType type)
        {
            return storage.ballTypes.Find(x => x.type == type);
        }

        public BallInfo GetSingleRandomBall()
        {
            int index = Random.Range(0, storage.ballTypes.Count);
            return storage.ballTypes[index];
        }

        public BallInfo GetRandomBallSequence(out int count)
        {
            count = Random.Range(minSequence, maxSequence + 1);
            int index = Random.Range(0, selection.Count);
            BallInfo selected = selection[index];
            selection.RemoveAt(index);
            selection.Add(lastSelected);
            lastSelected = selected;
            return selected;
        }
    }
}
