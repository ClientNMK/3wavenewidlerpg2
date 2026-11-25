using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoRotate : MonoBehaviour
{
    public enum RotateDirection
    {
        Left,
        Right
    }

    #region Fields
    [SerializeField] private RotateDirection rotateDir;
    public RotateDirection RotateDIR { get { return rotateDir; } }

    private Transform transfrom;

    public float _speed;
    #endregion

    #region Unity Methods
    private void Start()
    {
        transfrom = transform;
    }

    private void Update()
    {
        if (rotateDir == RotateDirection.Left)
        {
            transfrom.Rotate(Vector3.forward, _speed * Time.deltaTime);
        }
        else if (rotateDir == RotateDirection.Right)
        {
            transfrom.Rotate(Vector3.back, _speed * Time.deltaTime);
        }
    }
    #endregion
}