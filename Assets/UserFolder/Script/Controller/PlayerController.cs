using EnumType;
using Manager;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace main
{
    public class PlayerController : MonoBehaviour
    {
        private const float SLOWVALUE = 0.1f;
        private const float FIXEDTIME = 0.02f;
        private const float ROTATESPEED = 5;
        private const float ROTATETIME = 0.8f;
        
        private Rigidbody playerRigid;
        private Animator playerAnim;

        
        [SerializeField] private int normalSpeed = 10;
        [SerializeField] private int runSpeed = 60;
        private int currentSpeed;

        private bool flipOnce;
        private bool isChanging = false;
        private bool isJumping;

        #region KeyInput
        private float xMoveInput;
        private float zMoveInput;
        private float wheelInput;

        private bool isExitInput;
        private bool isJumpInput;
        private bool isRunInput;
        private bool isTimeControlInput;

        private int currentGravityKeyInput = 1;
        private readonly KeyCode[] gravityChangeInput =
        {
            KeyCode.Z,
            KeyCode.X,
            KeyCode.C
        };
        #endregion

        void Start()
        {
            playerRigid = GetComponent<Rigidbody>();
            playerAnim = GetComponentInChildren<Animator>();

            AIManager.PlayerTransfrom = transform;
            currentSpeed = normalSpeed;
        }

        void Update()
        {
            KeyInput();
            ProcessInput();
            Move();
            Rotate();

            if (xMoveInput != 0 || zMoveInput != 0) playerAnim.SetBool("isRun", true);
            else if (playerAnim.GetBool("isRun"))   playerAnim.SetBool("isRun", false);
        }

        private void KeyInput()
        {
            xMoveInput = Input.GetAxis("Horizontal");
            zMoveInput = Input.GetAxis("Vertical");
            wheelInput = Input.GetAxis("Mouse ScrollWheel");

            isRunInput = Input.GetKey(KeyCode.LeftShift);

            isExitInput = Input.GetKeyDown(KeyCode.Escape);
            isJumpInput = Input.GetKeyDown(KeyCode.Space);
            isTimeControlInput = Input.GetKeyDown(KeyCode.F);

            for (int i = 0; i < gravityChangeInput.Length; i++)
            {
                if (Input.GetKeyDown(gravityChangeInput[i]))
                {
                    currentGravityKeyInput = i;
                    break;
                }
            }
        }
        
        private void ProcessInput()
        {
            if (isExitInput) Application.Quit();
            if (isJumpInput) Jump();
            if (isRunInput) currentSpeed = runSpeed;
            else            currentSpeed = normalSpeed;
            
            if (wheelInput != 0)
            {
                flipOnce = true;
                isChanging = true;
                GravitiesManager.gravityDirection = (GravityDirection)currentGravityKeyInput;
                GravitiesManager.GravityChange(Mathf.FloorToInt(wheelInput * 10));
            }
            if (isTimeControlInput)
            {
                if (Time.timeScale != 1)
                {
                    Time.timeScale = 1;
                    Time.fixedDeltaTime = FIXEDTIME;
                }
                else
                {
                    Time.timeScale = SLOWVALUE;
                    Time.fixedDeltaTime = Time.timeScale * FIXEDTIME;
                }
            }
        }
        private void Jump()
        {
            isJumping = true;
            playerRigid.AddForce(transform.up * 5, ForceMode.Impulse);
            playerAnim.SetTrigger("Jump");
        }
        private void Move()
        {
            Vector3 moveHorizontal = transform.right * xMoveInput;
            Vector3 moveVertical = transform.forward * zMoveInput;
            Vector3 velocity = currentSpeed * Time.deltaTime * (moveHorizontal + moveVertical).normalized;

            playerRigid.MovePosition(transform.position + velocity);
        }

        [SerializeField]
        private float lookSensitivity;
        private float currentCameraRotationX;

        float _xRotation;
        float _cameraRotationX;
        int interpolationAngle;

        Vector3 newRotation;

        private void Rotate()
        {
            if (!isChanging || GravitiesManager.IsGravityDupleicated) //이부분을 없애야 함 (1)
                _xRotation = Input.GetAxisRaw("Mouse X");

            _cameraRotationX = _xRotation * lookSensitivity;

            if (flipOnce && (int)GravitiesManager.beforeGravityType % 2 != (int)GravitiesManager.currentGravityType % 2)
                currentCameraRotationX *= -1;

            currentCameraRotationX += (int)GravitiesManager.currentGravityType % 2 == 0 ? _cameraRotationX : -_cameraRotationX;

            switch (GravitiesManager.currentGravityType)
            {
                case GravitiesType.xUp:
                    interpolationAngle = (int)GravitiesManager.beforeGravityType >= 4 && !GravitiesManager.IsGravityDupleicated ? -90 : 0;

                    newRotation.x = currentCameraRotationX + interpolationAngle;

                    newRotation.y = 0;
                    newRotation.z = -90;
                    break;
                case GravitiesType.xDown:
                    interpolationAngle = (int)GravitiesManager.beforeGravityType >= 4 && !GravitiesManager.IsGravityDupleicated ? -90 : 0;

                    newRotation.x = currentCameraRotationX + interpolationAngle;

                    newRotation.y = 0;
                    newRotation.z = 90;
                    break;
                case GravitiesType.yUp:
                    interpolationAngle = 0;
                    newRotation.y = currentCameraRotationX + interpolationAngle;

                    newRotation.x = 0;
                    newRotation.z = 0;
                    break;
                case GravitiesType.yDown:
                    interpolationAngle = (int)GravitiesManager.beforeGravityType >= 3 && !GravitiesManager.IsGravityDupleicated ? -180 : 0;

                    newRotation.y = currentCameraRotationX + interpolationAngle;

                    newRotation.x = 0;
                    newRotation.z = 180;
                    break;
                case GravitiesType.zUp:
                    interpolationAngle = (int)GravitiesManager.beforeGravityType <= 1 ? -90 : 90;

                    newRotation.x = currentCameraRotationX + interpolationAngle;

                    newRotation.y = -90;
                    newRotation.z = -90;
                    break;
                case GravitiesType.zDown:
                    interpolationAngle = (int)GravitiesManager.beforeGravityType == 5 && !GravitiesManager.IsGravityDupleicated ? -180 : -90;

                    newRotation.x = currentCameraRotationX + interpolationAngle;

                    newRotation.y = -90;
                    newRotation.z = 90;
                    break;
            }
            #region 회전 1방식
            if (wheelInput == 0 || GravitiesManager.IsGravityDupleicated) transform.rotation = Quaternion.Euler(newRotation);
            else if (flipOnce) StartCoroutine(GravityRotate());
            #endregion
            #region 회전 2방식
            /*
            if (!isChanging) transform.rotation = Quaternion.Euler(newRotation);
            else transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(newRotation), ROTATESPEED * Time.deltaTime);
            
            float angle = Quaternion.Angle(transform.rotation, Quaternion.Euler(newRotation));
            if (angle <= 0) isChanging = false;
            */
            #endregion
            flipOnce = false;

            /*
             * 회전 1방식의 경우
             * isChaging으로 관리 세부적으로 해줘야함
             */
        }
        private IEnumerator GravityRotate()
        {
            Quaternion currentRotation = transform.rotation;
            float t = 0;
            while (t < 1)
            {
                t += Time.deltaTime / ROTATETIME;
                transform.rotation = Quaternion.Lerp(currentRotation, Quaternion.Euler(newRotation), t);
                yield return null;
            }
            isChanging = false;
            GravitiesManager.CompleteGravityChange();
        }
    }
}