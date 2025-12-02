using System;
using UnityEngine;

public class RemotePlayer : MonoBehaviour
{
    private bool IsSecondMethod;

    public void UpdateMethod(bool secondMethod) => IsSecondMethod = secondMethod;

    public bool GetSecondMethod() => IsSecondMethod;

    // For First Method......
    public float InterpolationTime = 0.08f;
    private Vector3 LastReceivedPos;
    private Vector3 PreviousPos;
    private float DesiredTime;

    //For Second Method.......
    public float MoveSmooth = 10f;
    private Vector3 TargetPos;

    private void Start()
    {
        if (!IsSecondMethod)
        {
            LastReceivedPos = transform.localPosition;
            PreviousPos = LastReceivedPos;
            DesiredTime = 1f;
        }
        else
        {
            TargetPos = transform.localPosition;
        }
    }

    public void UpdatePosition(byte[] bytes)
    {
        if (!IsSecondMethod)
        {
            PreviousPos = LastReceivedPos;
            LastReceivedPos = QuantizationUtils.DequantizeVector3FromBytes(bytes);
            DesiredTime = 0f;
            Debug.Log($"Received Position => {transform.localPosition}");
        }
        else
        {
            byte type = bytes[0];

            if (type == 0)
            {
                // === FULL VECTOR3 ===
                byte[] data = new byte[6];
                Array.Copy(bytes, 1, data, 0, 6);

                Vector3 pos = QuantizationUtils.DequantizeVector3FromBytes(data);
                TargetPos = pos;
            }
            else if (type == 1)
            {
                // === ONLY X UPDATED ===
                short x = BitConverter.ToInt16(bytes, 1);
                float dx = QuantizationUtils.DequantizeFloat(x);

                TargetPos = new Vector3(dx, TargetPos.y, TargetPos.z);
            }
            else if (type == 2)
            {
                // === ONLY Z UPDATED ===
                short z = BitConverter.ToInt16(bytes, 1);
                float dz = QuantizationUtils.DequantizeFloat(z);

                TargetPos = new Vector3(TargetPos.x, TargetPos.y, dz);
            }
        }
    }

    private void Update()
    {
        if (!IsSecondMethod)
        {
            if (DesiredTime < 1f)
            {
                DesiredTime += Time.deltaTime / InterpolationTime;
                transform.localPosition = Vector3.Lerp(PreviousPos, LastReceivedPos, DesiredTime);
            }
            else
            {
                transform.localPosition = LastReceivedPos;
            }
        }
        else
        {
            transform.localPosition = Vector3.Lerp(
            transform.localPosition,
            TargetPos,
            Time.deltaTime * MoveSmooth);
        }
    }
}