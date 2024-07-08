using System;
using System.Collections.Generic;
using UnityEngine;

public class GameState
{
    [Serializable]
    public struct slidesState
    {
        public List<Boolean> slidesA;
    }

    [SerializeField]
    private slidesState slideSave;
}
