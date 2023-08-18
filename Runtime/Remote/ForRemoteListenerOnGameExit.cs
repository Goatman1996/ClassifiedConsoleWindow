using System;
using UnityEngine;

namespace ClassifiedConsole.Runtime
{
    internal class ForRemoteListenerOnGameExit : MonoBehaviour
    {
        public Action OnDestroyEvt;
        private void OnDestroy()
        {
            this.OnDestroyEvt?.Invoke();
        }
    }
}