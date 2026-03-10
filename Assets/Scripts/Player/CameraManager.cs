
using UnityEngine;

namespace Assets.Scripts.Systems.Player
{

    public class CameraManager : MonoBehaviour
    {

        [SerializeField]
        private Camera _targetCamera;


        private void Start()
        {
            CenterOnOrigin();
        }

        public void CenterOnOrigin()
        {
            Vector2 center = new Vector2(0, 0);

            _targetCamera.transform.position = new Vector3(center.x, center.y, _targetCamera.transform.position.z);
        }
    }
}
