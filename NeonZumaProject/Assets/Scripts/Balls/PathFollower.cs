using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;

namespace Core
{
    public class PathFollower : MonoBehaviour
    {
        public EndOfPathInstruction endOfPathInstruction;
        public float distanceTravelled;
        public int id;
        static int nextId = 0;

        PathCreator pathCreator;
        Transform _trasform;

        public void Initialize(PathCreator path, float distance = 0)
        {
            id = nextId++;
            _trasform = transform;
            pathCreator = path;
            distanceTravelled = distance;

            if (pathCreator != null) {
                // Subscribed to the pathUpdated event so that we're notified if the path changes during the game           //maybe excess
                pathCreator.pathUpdated += OnPathChanged;
            }
        }

        //change it if need to optimize
        public float Distance {
            get { return distanceTravelled; }       
            set { distanceTravelled = value; }
        }

        public void MoveUpdate(float delta)
        {
            if (pathCreator != null) {
                distanceTravelled += delta;
                _trasform.position = pathCreator.path.GetPointAtDistance(distanceTravelled, endOfPathInstruction);
            }
        }

        // If the path changes during the game, update the distance travelled so that the follower's position on the new path
        // is as close as possible to its position on the old path
        void OnPathChanged()
        {
            distanceTravelled = pathCreator.path.GetClosestDistanceAlongPath(_trasform.position);
        }
    }
}
