using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BlackCat_UI
{
    public abstract class StaticUIPanel : MonoBehaviour,IBasePanel
    {
        public abstract void Init( );
        
        public virtual void OpenPanel()
        {
            gameObject.SetActive(true);
        }

        public virtual void ClosePanel()
        {
            gameObject.SetActive(false);
        }
    }
}