using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core
{
    public class Ball : MonoBehaviour
    {
        SpriteRenderer spriteRend;
        
        public BallType Type { get; private set; }

        public void Initialize(BallInfo info)
        {
            if(spriteRend == null) {
                spriteRend = GetComponent<SpriteRenderer>();
            }
            spriteRend.color = info.color;
            Type = info.type;
        }
    }
}
