using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core
{
    public class BallSpawner : MonoBehaviour
    {
        public CommonHandler spawnBall;

        public void OnTriggerExit2D(Collider2D coll)            //протестить, если шары будут закатываться
        {
            //Debug.Log("exit " + coll.tag);
            if (!coll.CompareTag("Chain") && !coll.CompareTag("Edge"))
                return;

            if(spawnBall != null) {
                spawnBall();
            }
        }
    }
}
