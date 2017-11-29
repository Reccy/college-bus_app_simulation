using UnityEngine;
using UnityEngine.EventSystems;

/**
 *  Sets the position of this game object by click
 */
public class SetBusDestinationByClick : MonoBehaviour
{
    [SerializeField]
    private BusDriver busDriver;
    public BusDriver BusDriver { set; get; }

    private void Update()
    {
        if (busDriver.CurrentDriverMode == BusDriver.BusDriverMode.Debug)
        {
            SetDestinationOnClick();
        }
    }

    private void SetDestinationOnClick()
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
