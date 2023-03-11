using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Contoller.Camera
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField]
        private float walkSpeed;

        [SerializeField]
        private float lookSensitivity;

        [SerializeField]
        private float cameraXRotationLimit;
        [SerializeField]
        private float cameraYRotationLimit;

        //private float currentCameraRotationX;
        private float currentCameraRotationY;

        [SerializeField]
        private UnityEngine.Camera mainCamera;
        private Rigidbody myRigid;

        void Start()
        {
            myRigid = GetComponent<Rigidbody>();
            currentCameraRotationY = 0;

            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }

        void Update()
        {
            //Move();                 // 1 키보드 입력에 따라 이동
            CameraRotation();       // 2️ 마우스를 위아래(Y) 움직임에 따라 카메라 X 축 회전 
            //CharacterRotation();    // 3 마우스 좌우(X) 움직임에 따라 캐릭터 Y 축 회전 
        }

        private void Move()
        {
            float _moveDirX = Input.GetAxisRaw("Horizontal");
            float _moveDirZ = Input.GetAxisRaw("Vertical");
            Vector3 _moveHorizontal = transform.right * _moveDirX;
            Vector3 _moveVertical = transform.forward * _moveDirZ;
        }

        public void SetRotation(Quaternion newRotation)
        {
            transform.rotation = newRotation;
        }

        private void CameraRotation()
        {
            float _yRotation = Input.GetAxisRaw("Mouse Y");
            float _cameraRotationY = _yRotation * lookSensitivity;

            currentCameraRotationY -= _cameraRotationY;
            currentCameraRotationY = Mathf.Clamp(currentCameraRotationY, -cameraXRotationLimit, cameraXRotationLimit);

            mainCamera.transform.localEulerAngles = new Vector3(currentCameraRotationY, 0, 0f);
        }

        private void CharacterRotation()  // 좌우 캐릭터 회전
        {
            float _yRotation = Input.GetAxisRaw("Mouse X");
            Vector3 _characterRotationY = new Vector3(0f, _yRotation, 0f) * lookSensitivity;
            myRigid.MoveRotation(myRigid.rotation * Quaternion.Euler(_characterRotationY)); // 쿼터니언 * 쿼터니언
        }
    }
}
