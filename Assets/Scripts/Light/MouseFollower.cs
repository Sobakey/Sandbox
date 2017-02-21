using UnityEngine;
using System.Collections;

namespace Light2D.Examples
{
    public class MouseFollower : MonoBehaviour
    {
        public bool RightClickRotation = false;
        private Vector2 _pressPos;

        private void LateUpdate()
        {
            if (Input.GetMouseButtonDown(0))
                _pressPos = GetMousePosInUnits();

            if (Input.GetMouseButton(0) && RightClickRotation)
            {
                var shift = GetMousePosInUnits() - _pressPos;
                if (shift.sqrMagnitude > 0.1f*0.1f)
                {
                    var angle = shift.AngleZ();
                    transform.rotation = Quaternion.Euler(0, 0, angle);
                }
            }
            else
            {
                Vector3 pos = GetMousePosInUnits();
                pos.z = transform.position.z;
                transform.position = pos;
            }
        }

        public static Vector2 GetMousePosInUnits()
        {
            var mouse = Input.mousePosition;
            var camera = Camera.main;
            var mouseWorld = camera.ScreenToWorldPoint(
                new Vector3(mouse.x, mouse.y, -camera.transform.position.z));
            return mouseWorld;
        }

    }
}
