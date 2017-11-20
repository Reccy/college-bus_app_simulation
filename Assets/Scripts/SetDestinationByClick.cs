using UnityEngine;

/**
 *  Sets the position of this game object by click
 */
public class SetDestinationByClick : MonoBehaviour
{
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
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
