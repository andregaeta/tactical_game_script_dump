using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    //public float cameraSpeed;
    //public float panBorder;
    public bool dragging;
    Vector3 initialPos;

    void Update()
    {
       
        /*
        if (Input.mousePosition.x < panBorder)
        {
            position.x -= cameraSpeed * Time.deltaTime;
        }
        if (Input.mousePosition.x > Screen.width - panBorder)
        {
            position.x += cameraSpeed * Time.deltaTime;
        }
        if (Input.mousePosition.y < panBorder)
        {
            position.y -= cameraSpeed * Time.deltaTime;
        }
        if (Input.mousePosition.y > Screen.height - panBorder)
        {
            position.y += cameraSpeed * Time.deltaTime;
        }
        */

        if (Input.GetMouseButtonDown(1) && dragging == false)
        {
            if (!GameState.Instance.TryStartMinorState(GameState.minorStates.MovingCamera))
                return;
            dragging = true;
            initialPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }
        if (Input.GetMouseButtonUp(1) && dragging)
        {
            if (!GameState.Instance.TryStopMinorState(GameState.minorStates.MovingCamera))
                return;
            dragging = false;
        }
        if (dragging)
        {
            Vector3 position = transform.position;
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3 offset = initialPos - mousePos;
            position += new Vector3(offset.x, offset.y, 0);
            position.x = Mathf.Clamp(position.x, -10, 10);
            position.y = Mathf.Clamp(position.y, -15, -3);

            transform.position = position;
        }

    }

    private void LateUpdate()
    {
        /*
        if (GameState.Instance.MajorState != GameState.majorStates.EnemyTurn) return;
        
        if (GameState.Instance.ActiveEnemy == null) return;
        if (transform.position.x == GameState.Instance.ActiveEnemy.transform.position.x && transform.position.y == GameState.Instance.ActiveEnemy.transform.position.y)
            return;
        Vector3 position = transform.position;
        position.x = GameState.Instance.ActiveEnemy.transform.position.x;
        position.y = GameState.Instance.ActiveEnemy.transform.position.y;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, position, Time.deltaTime * 10f);
        transform.position = smoothedPosition;
            */
            
        if (GameState.Instance.MinorState != GameState.minorStates.Animating && GameState.Instance.MajorState != GameState.majorStates.EnemyTurn)
            return;
        if (GameState.Instance.CameraTarget == null) return;
        GameObject target = GameState.Instance.CameraTarget;
        if (transform.position.x == target.transform.position.x && transform.position.y == target.transform.position.y)
            return;

        Vector3 position = transform.position;
        position.x = target.transform.position.x;
        position.y = target.transform.position.y;
        position.x = Mathf.Clamp(position.x, -10, 10);
        position.y = Mathf.Clamp(position.y, -15, -3);
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, position, Time.deltaTime * 10f);
        transform.position = smoothedPosition;
        
    }
}
