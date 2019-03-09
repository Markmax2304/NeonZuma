using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core
{
    public class BallBorderCatcher : MonoBehaviour
    {
        public void OnTriggerExit2D(Collider2D coll)
        {
            if (coll.CompareTag("Projectile")) {
                coll.GetComponent<Projectile>().Die();
            }
        }
    }
}
