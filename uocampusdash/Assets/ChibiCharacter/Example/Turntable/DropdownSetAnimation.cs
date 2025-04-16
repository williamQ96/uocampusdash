using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChibiCharacter
{
    public class DropdownSetAnimation : MonoBehaviour
    {
        private Animator anim;
        // Start is called before the first frame update
        void Start()
        {
            anim = gameObject.GetComponent<Animator>();
        }


        public void changeAnimation(int value)
        {
            anim.SetInteger("animation", value);
        }
    }
}
