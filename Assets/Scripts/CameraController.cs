using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class CameraController : MonoBehaviour
{

    public float cameraSpeed, zoomSpeed, groundHight, zoomAngleMin = 4, zoomAngleMax = 60;
    public Vector2 cameraHeightMinMax;
    public Vector2 cameraRotationMinMax;

    [Range(0,60)]
    public float zoomLerp = 0.1f;
    [Range(0, 0.2f)]
    public float cursorTreshord;

    RectTransform selectionBox;
    new Camera camera;

    private Vector2 mousePos, mousePosScreen, keyboardInput, mouseScroll;
    private bool inCursorInGameScreen;
    Rect selectionRect, boxRect;

    List<Unit> selectedUnits = new();

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
        //keyboardInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        keyboardInput = new Vector2(0,0);
        mousePos = Input.mousePosition;
        mousePosScreen = camera.ScreenToViewportPoint(mousePos);
        inCursorInGameScreen = mousePosScreen.x >= 0 && mousePosScreen.x <= 1 &&
            mousePosScreen.y >= 0 && mousePosScreen.y <= 1;

        Vector2 movementDirection = keyboardInput;

        if (inCursorInGameScreen)
        {
            if(mousePosScreen.x < cursorTreshord)
            {
                movementDirection.x -= 1 - mousePosScreen.x / cursorTreshord;
            }
            if (mousePosScreen.x > 1 - cursorTreshord)
            {
                movementDirection.x += 1 - (1 - mousePosScreen.x) / cursorTreshord;
            }

            if (mousePosScreen.y < cursorTreshord)
            {
                movementDirection.y -= 1 - mousePosScreen.y / cursorTreshord;
            }
            if (mousePosScreen.y > 1 - cursorTreshord)
            {
                movementDirection.y += 1 - (1 - mousePosScreen.y) / cursorTreshord;
            }
        }

        var deltaPosition = new Vector3(movementDirection.x, 0, movementDirection.y);
        deltaPosition *= cameraSpeed * Time.deltaTime;
        transform.localPosition += deltaPosition;
    }

    private void UpdateZoom()
    {
        mouseScroll = Input.mouseScrollDelta;
        float zoomDelta = mouseScroll.y * zoomSpeed;
        zoomLerp = Mathf.Clamp(zoomLerp + zoomDelta, zoomAngleMin, 60);

        var position = transform.localPosition;
        position.y = Mathf.Clamp(cameraHeightMinMax.y, cameraHeightMinMax.x, zoomLerp) + groundHight;
        transform.localPosition = position;

        var rotation = transform.localEulerAngles;
        rotation.x = Mathf.Clamp(cameraRotationMinMax.y, cameraRotationMinMax.x, zoomLerp);
        transform.localEulerAngles = rotation;
    }

    void UpdateClicks()
    {
        if(Input.GetMouseButtonDown(0))
        {
            selectionBox.gameObject.SetActive(true);
            selectionRect.position = mousePos;   
        }
        else if(Input.GetMouseButtonUp(0))
        {
            selectionBox.gameObject.SetActive(false);
        }
        if(Input.GetMouseButton(0))
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

    Rect AbsRect(Rect rect)
    {
        if(rect.width < 0)
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

    bool IsPointInRect(Rect rect, Vector2 point)
    {
        return point.x >= rect.position.x && point.x <= (rect.position.x + rect.size.x) &&
            point.y >= rect.position.y && point.y <= (rect.position.y + rect.size.y);
    }

    void UpdateSelecting()
    {  
        selectedUnits.Clear();
        foreach(Unit unit in Unit.SelectableUnits)
        {
            if(!unit) continue;
            var pos = unit.transform.position;
            var posScreen = camera.WorldToScreenPoint(pos);
            bool inRect = IsPointInRect(boxRect, posScreen);
            (unit as ISelectable).SetSelected(inRect);
            if(inRect)
            {
                selectedUnits.Add(unit);
            }
        }
    }

    Ray ray;
    RaycastHit hit;
    [SerializeField]
    LayerMask commandLayerMask = -1;

    void GiveCommands()
    {
        ray = camera.ViewportPointToRay(mousePosScreen);
        if(Physics.Raycast(ray, out hit, 1000, commandLayerMask))
        {
            object commandData = null;
            if(hit.collider is TerrainCollider)
            {
                commandData = hit.point;
            }
            else
            {
                commandData = hit.collider.gameObject.GetComponent<Unit>();
            }
            GiveCommands(commandData);
        }
        
    }

    void GiveCommands(object dataCoomand)
    {
        foreach (Unit unit in selectedUnits)
            unit.SendMessage("Command", dataCoomand, SendMessageOptions.DontRequireReceiver);
    }
}
