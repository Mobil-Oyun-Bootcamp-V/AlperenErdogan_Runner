using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SocialPlatforms;

namespace Player
{
    public class PlayerMovementController : MonoBehaviour
    {
        [SerializeField] private float forwardSpeed;
        [SerializeField] private float jumpForce;
        //Actions
        public UnityAction<Direction> ONSlide;
        //Component references
        private Rigidbody _rigidbody;
        [SerializeField] private Animator animator;
        [SerializeField] private LevelInitializer levelInitializer;
        [SerializeField] private GameObject boxCollider;
        //States
        private Vector3 _defaultBoxColliderLocalScale;
        public Vector3 targetPosition;
        private bool _canJump = true;
        private bool _canSlide = true;
        private bool _gameStarted = false;
        private Vector3 _playerStartPos;
        private void Start()
        {
            _playerStartPos = transform.position;
            _defaultBoxColliderLocalScale = boxCollider.transform.localScale;
            _rigidbody = GetComponent<Rigidbody>();
            targetPosition = transform.position;
            ONSlide += OnSlide;
        }
        private void FixedUpdate()
        {
            if (!_gameStarted)
            {
                return;
            }
            MoveForward();
            SetPositionToTargetPosition();
        }

        public void StartGame()
        {
            if (levelInitializer._level==3)
            {
                return;
            }
            _gameStarted = true;
        }
        private void SetPositionToTargetPosition()
        {
            var pos=Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * 10);
            pos.y = transform.position.y;
            transform.position = pos;
        }
        
        void OnSlide(Direction direction)
        {
            switch (direction)
            {
                case Direction.Left:
                    Debug.Log("Left");
                    targetPosition=transform.position+ Vector3.left;
                    break;
                case Direction.Right:
                    Debug.Log("Right");
                    targetPosition += Vector3.right;
                    break;
                case Direction.Up:                    
                    Debug.Log("Up");
                    Jump();
                    break;
                case Direction.Down:
                    if(!_canSlide || !_canJump)
                        return;
                    StartCoroutine(SlideCoroutine());
                    // SlideCoroutine();
                    Debug.Log("Down");
                    break;
            }
        }
        private void MoveForward()
        {
            targetPosition += Vector3.forward * (Time.fixedDeltaTime * forwardSpeed);
        }
        private IEnumerator SlideCoroutine()
        {
            _canSlide = false;
            animator.SetTrigger("Slide");
            Vector3 localScale=boxCollider.transform.localScale;
            localScale.y = _defaultBoxColliderLocalScale.y / 2;
            boxCollider.transform.localScale = localScale;
            yield return new WaitUntil(()=>animator.GetCurrentAnimatorStateInfo(0).IsName("Slide"));
            //Go back old position
            const float time = .25f; // Time that how long it will take to old scale
            Vector3 originalScale = transform.localScale;
            float currentTime = 0.0f;
            do
            {
                var scale = boxCollider.transform.localScale;
                scale.y=   Vector3.Lerp(originalScale, _defaultBoxColliderLocalScale, currentTime / time).y;
                boxCollider.transform.localScale = scale;
                currentTime += Time.deltaTime;
                yield return null;
            } while (currentTime <= time);
            //set _canSlide true at animation end
            _canSlide = true;
        }
        // private void SetScale()
        // {
        //     if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Slide"))
        //     {
        //         Vector3 originalScale = boxCollider.transform.localScale;
        //
        //         if (Mathf.Abs(boxCollider.transform.localScale.y - _defaultBoxColliderLocalScale.y)>.1f)
        //         {
        //             var scale = boxCollider.transform.localScale;
        //             scale.y=   Vector3.Lerp(originalScale, _defaultBoxColliderLocalScale, Time.fixedDeltaTime*10).y;
        //             boxCollider.transform.localScale = scale;
        //         }
        //         else
        //         {
        //             _canSlide = true;
        //         }
        //     }
        // }
        public void Restart()
        {
            _gameStarted = false;
            PlayerInputController._currentLine = Line.Center;
            transform.position = _playerStartPos;
            targetPosition = transform.position;
        }
        private void Jump()
        { 
            if(!_canSlide || !_canJump)
                return;
            _rigidbody.AddForce(Vector3.up*jumpForce,ForceMode.Impulse);
            animator.SetTrigger("Jump");
            _canJump = false;
        }
        private void OnCollisionEnter(Collision other)
        {
            if (other.gameObject.CompareTag("Ground"))
            {
                _canJump = true;
            }

            if (other.gameObject.CompareTag("Trap"))
            {
                Restart();
                levelInitializer.Trapped();
            }
            
            if (other.gameObject.CompareTag("Finish"))
            {
                Restart();
                levelInitializer.NextLevel();
            }
        }
    }
}
