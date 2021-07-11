using UnityEngine;
using UnityEngine.Events;

namespace Player
{
    public enum Direction
    {
        Nothing,Left,Right,Up,Down
    }
    public enum Line
    {
        Left,Center,Right
    }
    public class PlayerInputController : MonoBehaviour
    {
        //Serialized Fields
        [SerializeField] private float swipeSensitivity;
        //
        public static Line _currentLine = Line.Center;
        private bool _canTouchAction = true;
        private Vector2 _touchStartPos;
        private UnityAction<Direction> _onSwipe;
        //References
        [SerializeField ]private PlayerMovementController playerMovementController;
        void Start()
        {
            _onSwipe += OnSwipe;
        }
        void Update()
        {
            ReadInput();
        }
        
        private void ReadInput()
        {
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);
                switch (touch.phase)
                {
                    case TouchPhase.Began:
                        _touchStartPos = touch.position;
                        break;
                    case TouchPhase.Moved:
                        if(!_canTouchAction)
                            return;
                        if ((Mathf.Abs(touch.position.x - _touchStartPos.x) > swipeSensitivity))
                        {
                            _onSwipe?.Invoke(touch.position.x - _touchStartPos.x >0 ? Direction.Right : Direction.Left);
                        }
                        else if ((Mathf.Abs(touch.position.y - _touchStartPos.y) > swipeSensitivity))
                        {
                            _onSwipe?.Invoke(touch.position.y - _touchStartPos.y >0 ? Direction.Up : Direction.Down);
                        }
                        break;
                    case TouchPhase.Ended:
                        _canTouchAction = true;
                        break;
                }
            }
        }
        
        void OnSwipe(Direction direction)
        {
            _canTouchAction = false;
            Direction targetDirection = Direction.Nothing;
            switch (direction)
            {
                case Direction.Left:
                    if (_currentLine != _currentLine.Previous())
                    {
                        _currentLine = _currentLine.Previous();
                        // _playerMovementController.targetPosition =transform.position + Vector3.left;
                        targetDirection = Direction.Left;
                    }
                    break;
                case Direction.Right:
                    if (_currentLine != _currentLine.Next())
                    {
                        _currentLine = _currentLine.Next();
                        // _playerMovementController.targetPosition =transform.position + Vector3.right;
                        targetDirection = Direction.Right;
                    }
                    break;
                case Direction.Up:
                    targetDirection = Direction.Up;
                    break;
                case Direction.Down:
                    targetDirection = Direction.Down;
                    break;
            }
            playerMovementController.ONSlide(targetDirection);
            
        }
    }
}
