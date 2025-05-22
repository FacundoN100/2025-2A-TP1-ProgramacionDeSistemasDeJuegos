using UnityEngine;
using UnityEngine.InputSystem;

// PlayerController actualizado:
// - Se introdujo el patron Strategy para gestionar el salto mediante la interfaz IJumpStrategy.
// - Se creo BasicJumpStrategy que encapsula el salto simple y permite futuras extensiones (doble/triple salto, nado, escalada).
// - Se añadio un Decorator (LoggingJumpStrategyDecorator) para ilustrar como envolver y extender la estrategia sin modificarla.
// - La logica de input permanece intacta; el controller delega salto y reseteo de estado a la estrategia inyectada.

// -Deje los TO-DO por si acaso .
namespace Gameplay
{
    [RequireComponent(typeof(Character))]
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private InputActionReference moveInput;
        [SerializeField] private InputActionReference jumpInput;
        [SerializeField] private float airborneSpeedMultiplier = .5f;

        //TODO: This booleans are not flexible enough. If we want to have a third jump or other things, it will become a hassle.
        //private bool _isJumping;
        //private bool _isDoubleJumping;

        // Strategy
        private IJumpStrategy _jumpStrategy;
        private Character _character;
        private Coroutine _jumpCoroutine;

        // (Strategy + Decorator)
        private void Awake()
        {
            _character = GetComponent<Character>();
            
            IJumpStrategy basic = new BasicJumpStrategy(_character);
            _jumpStrategy = new LoggingJumpStrategyDecorator(basic);
        }

        private void OnEnable()
        {
            if (moveInput?.action != null)
            {
                moveInput.action.started += HandleMoveInput;
                moveInput.action.performed += HandleMoveInput;
                moveInput.action.canceled += HandleMoveInput;
            }
            if (jumpInput?.action != null)
                jumpInput.action.performed += HandleJumpInput;
        }

        private void OnDisable()
        {
            if (moveInput?.action != null)
            {
                moveInput.action.started -= HandleMoveInput;
                moveInput.action.performed -= HandleMoveInput;
                moveInput.action.canceled -= HandleMoveInput;
            }
            if (jumpInput?.action != null)
                jumpInput.action.performed -= HandleJumpInput;
        }

        //Strategy 
        private void HandleMoveInput(InputAction.CallbackContext ctx)
        {
            var direction = ctx.ReadValue<Vector2>().ToHorizontalPlane();
           
            if (_jumpStrategy.IsJumping)
                direction *= airborneSpeedMultiplier;
            _character?.SetDirection(direction);
        }

        private void HandleJumpInput(InputAction.CallbackContext ctx)
        {
            // Strategy para el  IJumpStrategy
            _jumpStrategy.TryJump(ref _jumpCoroutine);
        }
        //TO-DO: This function is barely readable. We need to refactor how we control the jumping
        private void OnCollisionEnter(Collision other)
        {
            foreach (var contact in other.contacts)
            {
                if (Vector3.Angle(contact.normal, Vector3.up) < 5)
                {
                    
                    // delega al strategy 
                    _jumpStrategy.OnLand();
                }
            }
        }

        public interface IJumpStrategy
        {
            bool IsJumping { get; }
            void TryJump(ref Coroutine jumpCoroutine);
            void OnLand();
        }

        public class BasicJumpStrategy : IJumpStrategy
        {
            private readonly Character _character;
            private bool _hasJumped;

            public bool IsJumping => _hasJumped;

            public BasicJumpStrategy(Character character)
            {
                _character = character;
                _hasJumped = false;
            }

            public void TryJump(ref Coroutine jumpCoroutine)
            {
                if (_hasJumped) return;
                if (jumpCoroutine != null)
                    _character.StopCoroutine(jumpCoroutine);
                jumpCoroutine = _character.StartCoroutine(_character.Jump());
                _hasJumped = true;
            }

            public void OnLand()
            {
                _hasJumped = false;
            }
        }

        // Decorator  para IJumpStrategy
        public abstract class JumpStrategyDecorator : IJumpStrategy
        {
            protected IJumpStrategy _inner;
            protected JumpStrategyDecorator(IJumpStrategy inner)
            {
                _inner = inner;
            }

            public virtual bool IsJumping => _inner.IsJumping;
            public virtual void TryJump(ref Coroutine jumpCoroutine)
            {
                _inner.TryJump(ref jumpCoroutine);
            }
            public virtual void OnLand()
            {
                _inner.OnLand();
            }
        }

        // Decorator registra cada salto
        public class LoggingJumpStrategyDecorator : JumpStrategyDecorator
        {
            public LoggingJumpStrategyDecorator(IJumpStrategy inner) : base(inner) { }

            public override void TryJump(ref Coroutine jumpCoroutine)
            {
                Debug.Log("[Jump] Intentando salto");
                base.TryJump(ref jumpCoroutine);
                Debug.Log("[Jump] Estado IsJumping: " + _inner.IsJumping);
            }

            public override void OnLand()
            {
                base.OnLand();
                Debug.Log("[Jump] Tocó suelo, reset salto");
            }
        }




        
    }
}
