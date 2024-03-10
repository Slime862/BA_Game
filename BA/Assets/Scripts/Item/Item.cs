using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ba
{
    public interface IItem
    {
        public bool Picked(GameObject obj);
        public void Used(GameObject obj);
    }
}
