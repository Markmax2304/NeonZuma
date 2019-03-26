using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core
{
    public class BallOpener : MonoBehaviour
    {
        public BallHandler subscribeBall;
        public BallHandler unsubscribeBall;

        public void OnTriggerEnter2D(Collider2D coll)
        {
            if (coll.CompareTag("Chain") || coll.CompareTag("Edge")) {
                if (subscribeBall != null) {
                    //Debug.Log("enter " + coll.tag);
                    subscribeBall(coll.GetComponent<PathFollower>());
                }
            }
        }

        public void OnTriggerExit2D(Collider2D coll)
        {
            //Debug.Log("exit " + coll.tag);
            if (coll.CompareTag("Chain") || coll.CompareTag("Edge")) {
                if (unsubscribeBall != null) {
                    unsubscribeBall(coll.GetComponent<PathFollower>());
                }
            }
        }
    }
}
