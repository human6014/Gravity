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
        private const float ROTATESPEED = 5;
        private const float ROTATETIME = 0.8f;
        private const float FIXEDTIME = 0.02f;

        Rigidbody playerRigid;
        Animator playerAnim;
        public CameraController cameraController;

        public int playerSpeed = 50;

        float xMove;
        float zMove;
        float wheelInput;

        bool flipOnce;
        bool isChanging;
        bool isJumping;
        void Start()
        {
            playerRigid = GetComponent<Rigidbody>();
            playerAnim = GetComponentInChildren<Animator>();
        }

        void Update()
        {
            KeyInput();
            Move();
            Rotate();
            if (xMove != 0 || zMove != 0) playerAnim.SetBool("isRun", true);
            else playerAnim.SetBool("isRun", false);
        }
        private void KeyInput()
        {
            xMove = Input.GetAxis("Horizontal");
            zMove = Input.GetAxis("Vertical");

            if      (Input.GetKeyDown(KeyCode.Space)) Jump();
            if      (Input.GetKeyDown(KeyCode.Z)) GravitiesManager.gravityDirection = GravityDirection.X;
            else if (Input.GetKeyDown(KeyCode.X)) GravitiesManager.gravityDirection = GravityDirection.Y;
            else if (Input.GetKeyDown(KeyCode.C)) GravitiesManager.gravityDirection = GravityDirection.Z;

            wheelInput = Input.GetAxis("Mouse ScrollWheel");
            if (wheelInput != 0)
            {
                flipOnce = true;
                isChanging = true;
                if (wheelInput > 0) GravitiesManager.GravityChange(1);
                else GravitiesManager.GravityChange(-1);
            }

            if (Input.GetKeyDown(KeyCode.F))
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
            playerRigid.AddForce(transform.up * 10, ForceMode.Impulse);
            playerAnim.SetTrigger("Jump");
        }
        private void Move()
        {
            Vector3 moveHorizontal = transform.right * xMove;
            Vector3 moveVertical = transform.forward * zMove;
            Vector3 velocity = playerSpeed * Time.deltaTime * (moveHorizontal + moveVertical).normalized;

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
            
            if (flipOnce && (int)GravitiesManager.beforeType % 2 != (int)GravitiesManager.type % 2)
                currentCameraRotationX *= -1; 

            currentCameraRotationX += (int)GravitiesManager.type % 2 == 0 ? _cameraRotationX : -_cameraRotationX;

            switch (GravitiesManager.type)
            {
                case GravitiesType.xUp:
                    interpolationAngle = (int)GravitiesManager.beforeType >= 4 && !GravitiesManager.IsGravityDupleicated ? -90 : 0;

                    newRotation.x = currentCameraRotationX + interpolationAngle;

                    newRotation.y = 0;
                    newRotation.z = -90;
                    break;
                case GravitiesType.xDown:
                    interpolationAngle = (int)GravitiesManager.beforeType >= 4 && !GravitiesManager.IsGravityDupleicated ? -90 : 0;

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
                    interpolationAngle = (int)GravitiesManager.beforeType >= 3 && !GravitiesManager.IsGravityDupleicated ? -180 : 0;

                    newRotation.y = currentCameraRotationX + interpolationAngle;

                    newRotation.x = 0;
                    newRotation.z = 180;
                    break;
                case GravitiesType.zUp:
                    interpolationAngle = (int)GravitiesManager.beforeType <= 1 ? -90 : 90;

                    newRotation.x = currentCameraRotationX + interpolationAngle;

                    newRotation.y = -90;
                    newRotation.z = -90;
                    break;
                case GravitiesType.zDown:
                    interpolationAngle = (int)GravitiesManager.beforeType == 5 && !GravitiesManager.IsGravityDupleicated ? -180 : -90;

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