using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BlackCat_UI
{
    public abstract class DynamicUIPanel : MonoBehaviour,IBasePanel
    {
        public abstract void Init();

        public virtual void OpenPanel()
        {
           
        }

        public virtual void ClosePanel()
        {
            Destroy(gameObject);
        }
    }
}