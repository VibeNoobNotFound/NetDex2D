using Godot;

namespace NetDex.UI.Common;

public partial class MobileScrollContainer : ScrollContainer
{
    [Export]
    public float DragThresholdPixels { get; set; } = 10.0f;

    [Export]
    public bool EnableMouseDragFallback { get; set; } = true;

    private bool _pointerDown;
    private bool _isDragging;
    private int _touchIndex = -1;
    private Vector2 _startPosition;
    private Vector2 _lastPosition;

    public override void _Input(InputEvent @event)
    {
        if (!IsVisibleInTree())
        {
            return;
        }

        switch (@event)
        {
            case InputEventScreenTouch touch:
                HandleScreenTouch(touch);
                break;
            case InputEventScreenDrag drag:
                HandleScreenDrag(drag);
                break;
            case InputEventMouseButton button when EnableMouseDragFallback && button.ButtonIndex == MouseButton.Left:
                HandleMouseButton(button);
                break;
            case InputEventMouseMotion motion when EnableMouseDragFallback:
                HandleMouseMotion(motion);
                break;
        }
    }

    private void HandleScreenTouch(InputEventScreenTouch touch)
    {
        if (touch.Pressed)
        {
            if (!GetGlobalRect().HasPoint(touch.Position))
            {
                return;
            }

            BeginPointer(touch.Index, touch.Position);
            return;
        }

        if (touch.Index == _touchIndex)
        {
            EndPointer();
        }
    }

    private void HandleScreenDrag(InputEventScreenDrag drag)
    {
        if (!_pointerDown || drag.Index != _touchIndex)
        {
            return;
        }

        UpdateDrag(drag.Position);
    }

    private void HandleMouseButton(InputEventMouseButton button)
    {
        if (button.Pressed)
        {
            if (!GetGlobalRect().HasPoint(button.Position))
            {
                return;
            }

            BeginPointer(-1, button.Position);
            return;
        }

        if (_isDragging)
        {
            GetViewport().SetInputAsHandled();
        }

        EndPointer();
    }

    private void HandleMouseMotion(InputEventMouseMotion motion)
    {
        if (!_pointerDown || _touchIndex != -1)
        {
            return;
        }

        UpdateDrag(motion.Position);
    }

    private void BeginPointer(int pointerIndex, Vector2 position)
    {
        _pointerDown = true;
        _isDragging = false;
        _touchIndex = pointerIndex;
        _startPosition = position;
        _lastPosition = position;
    }

    private void EndPointer()
    {
        _pointerDown = false;
        _isDragging = false;
        _touchIndex = -1;
    }

    private void UpdateDrag(Vector2 currentPosition)
    {
        var moved = currentPosition - _startPosition;
        if (!_isDragging && Mathf.Abs(moved.Y) >= DragThresholdPixels)
        {
            _isDragging = true;
        }

        if (!_isDragging)
        {
            _lastPosition = currentPosition;
            return;
        }

        var deltaY = currentPosition.Y - _lastPosition.Y;
        if (Mathf.Abs(deltaY) > Mathf.Epsilon)
        {
            var scrollBar = GetVScrollBar();
            var max = scrollBar != null ? Mathf.RoundToInt(scrollBar.MaxValue) : int.MaxValue;
            ScrollVertical = Mathf.Clamp(ScrollVertical - Mathf.RoundToInt(deltaY), 0, max);
        }

        _lastPosition = currentPosition;
        GetViewport().SetInputAsHandled();
    }
}
