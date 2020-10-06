using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pincushion.LD47
{
    [System.Serializable]
    public class ConnectedGear
    {
        [SerializeField]
        public GearController gear;

        [SerializeField]
        public bool connectedRight;
    }
}