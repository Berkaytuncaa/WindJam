using UnityEngine;
using UnityEngine.InputSystem;

namespace Fred
{
    public class FredController : MonoBehaviour, FredControls.IFredPlayingActions
    {
        private FredControls _controls;

        private void OnEnable()
        {
            if (_controls == null)
            {
                _controls = new FredControls();
                _controls.FredPlaying.SetCallbacks(this);
            }
        }

        // Start is called before the first frame update
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
        
        }

        public void OnMove(InputAction.CallbackContext context)
        {
        
        }
    }
}
