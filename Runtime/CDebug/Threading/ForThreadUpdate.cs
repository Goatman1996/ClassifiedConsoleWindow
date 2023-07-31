using System;
using UnityEngine;

namespace ClassifiedConsole
{
    internal class ForThreadUpdate : MonoBehaviour
    {
        public Action OnUpdate;
        private void Update()
        {
            this.OnUpdate?.Invoke();
        }
    }
}