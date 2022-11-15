using System;
using UnityEngine;

namespace ClassifiedConsole
{
    internal class ForThreadOnGameExit : MonoBehaviour
    {
        public Action OnDestroyEvt;
        private void OnDestroy()
        {
            this.OnDestroyEvt?.Invoke();
        }
    }
}