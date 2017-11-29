using UnityEngine;
using UnityEngine.EventSystems;

/**
 *  Sets the position of this game object by click
 */
public class SetDestinationByClick : MonoBehaviour
{
    void Update()
    {
        if (Input.GetMouseButton(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit[] hits = Physics.RaycastAll(ray);

            if (hits.Length > 0)
            {
                transform.position = hits[0].point;
            }
        }
    }
}
