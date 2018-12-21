using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraControl : MonoBehaviour {

    //kamera tez chce miec jednostki zaznaczone
    public float cameraSpeed, zoomSpeed, groundHeight;
    public Vector2 cameraHeighrMinMax;
    public Vector2 cameraRotationMinMax;

    Vector2 mousePos;
    Vector2 mousePosScreen;
    Vector2 keyboardInput;
    Vector2 mouseScroll;
    bool isCursorInScreen;

   [Range(0, 1)]
    public float zoomLerp = 0.1f;

    [Range(0, 0.2f)]
    public float cursorThreshold = 0;

    Rect selectionRect, boxRect;
    RectTransform selectionBox;
    new Camera camera;

    List<Unit> selectedUnits = new List<Unit>();

    private void Awake()
    {
        selectionBox = GetComponentInChildren<Image>(true).transform as RectTransform;
        camera = GetComponent<Camera>();
        selectionBox.gameObject.SetActive(false);
    }

    private void Update()
    {
        UpdateMovement();
        UpdateZoom();
        UpdateClicks();
    }



    void UpdateMovement()
    {
        keyboardInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        mousePos = Input.mousePosition;
        mousePosScreen = camera.ScreenToViewportPoint(mousePos);
        isCursorInScreen = mousePosScreen.x >= 0 && mousePosScreen.x <= 1 &&
             mousePosScreen.y >= 0 && mousePosScreen.y <= 1;


        Vector2 movementDirection = keyboardInput;

        if(isCursorInScreen)
        {
            if (mousePosScreen.x < cursorThreshold) movementDirection.x -= 1 - mousePosScreen.x / cursorThreshold;
            if (mousePosScreen.y < cursorThreshold) movementDirection.y -= 1 - mousePosScreen.y / cursorThreshold;
            if (mousePosScreen.x > 1 - cursorThreshold) movementDirection.x += 1 - (1 - mousePosScreen.x) /cursorThreshold;
            if (mousePosScreen.y > 1- cursorThreshold) movementDirection.y += 1 - (1 - mousePosScreen.y) / cursorThreshold;
        }

        var deltaPosition = new Vector3(movementDirection.x, 0, movementDirection.y);
        deltaPosition *= cameraSpeed * Time.deltaTime;
        transform.position += deltaPosition;
    }

    void UpdateZoom()
    {
        mouseScroll = Input.mouseScrollDelta;
        float zoomDelta = mouseScroll.y * zoomSpeed * Time.deltaTime;
        zoomLerp = Mathf.Clamp01(zoomLerp + zoomDelta);

        var position = transform.localPosition;
        //Interpolacja między kamera
        position.y = Mathf.Lerp(cameraHeighrMinMax.y, cameraHeighrMinMax.x, zoomLerp) + groundHeight;
        transform.localPosition = position;

        var rotation = transform.localEulerAngles;
        rotation.x = Mathf.Lerp(cameraRotationMinMax.y, cameraRotationMinMax.x, zoomLerp);
        transform.localEulerAngles = rotation;
    }

    void UpdateClicks()
    {
        if (Input.GetMouseButtonDown(0))
        {
            selectionBox.gameObject.SetActive(true);
            selectionRect.position = mousePos;
        }

        if (Input.GetMouseButtonUp(0))
        {
            selectionBox.gameObject.SetActive(false);
        }
        if (Input.GetMouseButton(0))
        {
            selectionRect.size = mousePos - selectionRect.position;
            boxRect = AbsRect(selectionRect);
            
            selectionBox.anchoredPosition = boxRect.position;
            selectionBox.sizeDelta = boxRect.size;
            UpdateSelecting();
        }

        if (Input.GetMouseButtonDown(1))
        {
            GiveCommands();
        }

    }


    //dzieki temu bedziemy wiedziec gdzie na ekranie znajduje sie nasza jednostka
    void UpdateSelecting()
    {
        selectedUnits.Clear();
        foreach (Unit unit in Unit.SelectableUnits)
        {
            Debug.Log(unit.ToString());
            if (!unit) continue;
            var pos = unit.transform.position;
            var posScreen = camera.WorldToScreenPoint(pos);// mozemy porownac z tym co siedzi w selection Rect
            bool inRect = IsPoinInRect(boxRect, posScreen);
            //rzutowanie, jezeli zolnierz nie mialby interfejsu -> nie znalazłby sie na liscie 
            (unit as ISelectable).SetSelected(inRect);
            if(inRect)
            {
                selectedUnits.Add(unit);
            }
        }
    }

    bool IsPoinInRect(Rect rect, Vector2 point)
    {
        return point.x >= rect.position.x && point.x <= (rect.position.x + rect.size.x) &&
            point.y >= rect.position.y && point.y <= (rect.position.y + rect.size.y);
    }


    Rect AbsRect(Rect rect)
    {
        if (rect.width < 0)
        {
            rect.x += rect.width;
            rect.width *= -1;
        }

        if (rect.height < 0)
        {
            rect.y += rect.height;
            rect.height *= -1;
        }

        return rect;
    }

    Ray ray;
    RaycastHit rayHit;
    [SerializeField]
    LayerMask commandLayerMask = -1;
    void GiveCommands()
    {
        ray = camera.ViewportPointToRay(mousePosScreen);
        if(Physics.Raycast(ray, out rayHit, 1000, commandLayerMask))
        {
            object commandData = null; // tutaj ten zdefiniowany object moze byc czymkolwiek
            // wyslemy go poprzez message, a nie bezposrednio do jakiejs funkcji
            if (rayHit.collider is TerrainCollider)
            {
                commandData = rayHit.point;
            }
            else
            {
                commandData = rayHit.collider.gameObject.GetComponent<Unit>();
            }
            GiveCommands(commandData);
        }
    }

    void GiveCommands(object dataCommand)
    {
        foreach(Unit unit in selectedUnits )
        {
            unit.SendMessage("Command", dataCommand, SendMessageOptions.DontRequireReceiver);
        }
    }

}
