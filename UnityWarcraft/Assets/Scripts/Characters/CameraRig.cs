using UnityEngine;

namespace Warcraft.Characters
{
    public class CameraRig : MonoBehaviour
    {
        [SerializeField] private Transform cameraTransform;
        [SerializeField] private float recoilRecoverySpeed = 5f;

        private Vector3 _currentRecoil;
        private Quaternion _originalRotation;

        private void Start()
        {
            if (cameraTransform == null)
            {
                cameraTransform = GetComponentInChildren<Camera>().transform;
            }

            _originalRotation = cameraTransform.localRotation;
        }

        private void Update()
        {
            // Recover from recoil
            _currentRecoil = Vector3.Lerp(_currentRecoil, Vector3.zero, Time.deltaTime * recoilRecoverySpeed);
            cameraTransform.localRotation = _originalRotation * Quaternion.Euler(_currentRecoil);
        }

        public void ApplyRecoil(float amount)
        {
            _currentRecoil += new Vector3(-amount, Random.Range(-amount * 0.5f, amount * 0.5f), 0f);
        }
    }
}
