using UnityEngine;
using UnityEngine.InputSystem;

public class Weapon : MonoBehaviour
{
    public void OnAim(InputValue value)
    {
        // Raycast the mouse position onto the middle plane
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layerMask: LayerMask.GetMask("Middle Plane"));

        // Rotate to face this position
        transform.rotation = Quaternion.LookRotation(hit.point - transform.position, Vector3.up);
    }
}
