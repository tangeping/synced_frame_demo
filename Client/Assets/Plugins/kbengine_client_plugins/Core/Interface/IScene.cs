using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CBFrame.Core
{
    public interface IScene
    {
        void OnEnter();
        IEnumerator Loading();
        void OnExit();
    }
}