using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace FireElemental
{
    public class FireElementalController : MonoBehaviour, FireElementalControls.IGameplayActions
    {
        private FireElementalControls _controls;
        private Rigidbody2D _fireElRb;
<<<<<<< HEAD
        [SerializeField] private float jumpPower;
=======

>>>>>>> 62f76a519161ecd3b43e33dde2acc99bc4df6a6b
        private void OnEnable()
        {
            if (_controls == null)
            {
                _controls = new FireElementalControls();
                _controls.Gameplay.SetCallbacks(this);
            }
            _controls.Gameplay.Enable();
        }

<<<<<<< HEAD
        private void Awake()
        {
            _fireElRb = GetComponent<Rigidbody2D>();
        }

        // Start is called before the first frame update
        void Start()
        {
            
=======
        // Start is called before the first frame update
        void Start()
        {
        
>>>>>>> 62f76a519161ecd3b43e33dde2acc99bc4df6a6b
        }

        // Update is called once per frame
        void Update()
        {
            
        }

        public void OnMove(InputAction.CallbackContext context)
        {
            throw new NotImplementedException();
        }

        public void OnJump(InputAction.CallbackContext context)
        {
<<<<<<< HEAD
            _fireElRb.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
=======
            throw new NotImplementedException();
>>>>>>> 62f76a519161ecd3b43e33dde2acc99bc4df6a6b
        }
    }
}
