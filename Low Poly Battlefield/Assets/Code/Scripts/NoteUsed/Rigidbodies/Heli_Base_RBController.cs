using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Helicopter
{
    [RequireComponent(typeof(Rigidbody))]
    public class Heli_Base_RBController : MonoBehaviour
    {
        #region Variables
        [Header("Base Properties")]
        public float weightInKg = 10f;
        public Transform cog;

        protected float weight;
        protected Rigidbody rb;
        #endregion

        #region Builtin Methods
        public virtual void Start()
        {
            weight = weightInKg;
            rb = GetComponent<Rigidbody>();
            if(rb)
            {
                rb.mass = weight;
            }
        }

        private void FixedUpdate()
        {
            if (rb)
            {
                HandlePhysics();
            }
        }
        #endregion

        #region Custom Methods
        protected virtual void HandlePhysics() { }
        #endregion
    }

}