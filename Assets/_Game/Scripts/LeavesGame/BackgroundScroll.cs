using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ibit.LeavesGame
{
    public class BackgroundScroll : MonoBehaviour
    {
        public float scrollSpeed = -1.5f;
        Vector2 startPos;

        void Start()
        {
            startPos = transform.position;
        }

        void Update()
        {
            float newPos = Mathf.Repeat(Time.time * scrollSpeed, 19);
            transform.position = startPos + Vector2.right * newPos;

        }
    }


}
