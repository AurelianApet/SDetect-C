using UnityEngine;
using UnityEngine.UI;
using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using AOT;

namespace AudienceNetwork
{
    public class AdHandler : MonoBehaviour
    {
        private readonly static Queue<Action> executeOnMainThreadQueue = new Queue<Action>();

        public void executeOnMainThread(Action action)
        {
            executeOnMainThreadQueue.Enqueue(action);
        }

        void Update()
        {
            // dispatch stuff on main thread
            while (executeOnMainThreadQueue.Count > 0) {
                executeOnMainThreadQueue.Dequeue().Invoke();
            }
        }

        public void removeFromParent()
        {
#if UNITY_EDITOR
            //UnityEngine.Object.DestroyImmediate (this);
#else
            UnityEngine.Object.Destroy(this);
#endif
        }
    }
}
