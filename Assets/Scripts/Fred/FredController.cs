using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Fred
{
    public class FredController : MonoBehaviour, FredControls.IFredPlayingActions
    {
        private FredControls _controls;
        private Rigidbody2D _fredRb;
        [SerializeField] private float moveSpeed;
        
        private void OnEnable()
        {
            if (_controls == null)
            {
                _controls = new FredControls();
                _controls.FredPlaying.SetCallbacks(this);
            }
            _controls.FredPlaying.Enable();
        }

        // Start is called before the first frame update
        private void Awake()
        {
            _fredRb = GetComponent<Rigidbody2D>();
        }

        // Update is called once per frame
        void Update()
        {
            
        }

        public void OnMove(InputAction.CallbackContext context)
        {
            _fredRb.velocity = new Vector2(context.ReadValue<Vector2>().normalized.x * moveSpeed, 0);
        }

        public void OnFire(InputAction.CallbackContext context)
        {
            _fredRb.AddForceAtPosition(Vector2.one, transform.TransformPoint(Vector2.right *.2f), ForceMode2D.Force);
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawWireSphere(transform.TransformPoint(Vector2.right * .2f), .05f);
        }
    }
}
