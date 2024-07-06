using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knight_animation : MonoBehaviour
{
    private Animator anim;

    public string[] idleDirections = { "Idle_N", "Idle_NW", "Idle_W", "Idle_SW", "Idle_S", "Idle_SE", "Idle_E", "Idle_NE" };
    public string[] walkDirections = { "Walk_N", "Walk_NW", "Walk_W", "Walk_SW", "Walk_S", "Walk_SE", "Walk_E", "Walk_NE" };

    int lastDirection;

    private void Awake()
    {
        anim = GetComponent<Animator>();

    }

    public void SetDirection(Vector2 _direction)
    {
        string[] directionArray = null;

        if (_direction.magnitude < 0.01)
        {
            directionArray = idleDirections;
        }
        else
        {
            directionArray = walkDirections;

            lastDirection = DirectionToIndex(_direction);
        }
        anim.Play(directionArray[lastDirection]);

    }

    private int DirectionToIndex(Vector2 _direction)
    {
        Vector2 norDir = _direction.normalized;

        float step = 360 / 8;
        float offset = step / 2;

        float angle = Vector2.SignedAngle(Vector2.up, norDir);

        angle += offset;

        if (angle < 0)
        {
            angle += 360;
        }

        float stepCount = angle / step;

        return Mathf.FloorToInt(stepCount);
    }
}
