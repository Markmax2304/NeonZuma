using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core
{
    public class BallRandom : MonoBehaviour
    {
        [SerializeField] BallTypesStorage storage;
        [SerializeField] int minSequence = 1;
        [SerializeField] int maxSequence = 9;

        List<BallInfo> selection;
        BallInfo lastSelected;

        CountBallRecords ballRecords;

        void Awake()
        {
            selection = new List<BallInfo>();
            lastSelected = storage.ballTypes[0];
            for (int i = 1; i < storage.ballTypes.Count; i++) {
                selection.Add(storage.ballTypes[i]);
            }
        }

        public void Init(CountBallRecords records)
        {
            ballRecords = records;
        }


        public BallType GetNextBallType()
        {
            int index = Random.Range(0, ballRecords.balls.Count);
            return ballRecords.balls[index].type;
        }

        public BallInfo GetSingleBall()
        {
            if(ballRecords.balls.Count == 0) {
                return GetSingleRandomBall();
            }
            else {
                int index = Random.Range(0, ballRecords.balls.Count);
                BallType type = ballRecords.balls[index].type;
                return storage.ballTypes.Find(x => x.type == type);
            }
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
