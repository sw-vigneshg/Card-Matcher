using System;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private bool SecondMethod;
    [SerializeField] private RemotePlayer RemotePlayer;

    private float MoveSpeed = 5f;
    private Vector3 CurrentDirection = Vector3.zero;
    private Vector3 PreviosDirection = Vector3.zero;
    private float AxisValue; // Whether should be X or Z
    private byte AxisType; // 1 for X, 2 for Z

    private void OnValidate()
    {
        if (RemotePlayer != null && SecondMethod != RemotePlayer.GetSecondMethod())
        {
            RemotePlayer.UpdateMethod(SecondMethod);
        }
    }

    private void Update()
    {
        OnInput();
    }

    private void OnInput()
    {
        if (Input.GetKey(KeyCode.W))
        {
            CurrentDirection = Vector3.forward;
            MovePlayer();
        }
        else if (Input.GetKey(KeyCode.S))
        {
            CurrentDirection = Vector3.back;
            MovePlayer();
        }
        else if (Input.GetKey(KeyCode.A))
        {
            CurrentDirection = Vector3.left;
            MovePlayer();
        }
        else if (Input.GetKey(KeyCode.D))
        {
            CurrentDirection = Vector3.right;
            MovePlayer();
        }
    }

    private void MovePlayer()
    {
        if (SecondMethod)
        {
            Vector3 moveDirection = CurrentDirection.normalized;
            transform.localPosition += moveDirection * MoveSpeed * Time.deltaTime;

            if (RemotePlayer == null)
                return;

            // CASE 1 — Direction changed → send FULL Vector3
            if (CurrentDirection != PreviosDirection)
            {
                PreviosDirection = CurrentDirection;

                byte[] full = QuantizationUtils.QuantizeVector3ToBytes(transform.localPosition);

                // Packet: [0][x,y,z]
                byte[] packet = new byte[1 + full.Length];
                packet[0] = 0; // packet type
                Array.Copy(full, 0, packet, 1, full.Length);

                RemotePlayer.UpdatePosition(packet);

                Debug.Log($"[FULL] {packet.Length} bytes");
                return;
            }

            // CASE 2 — Same direction → send only one float

            if (CurrentDirection == Vector3.right || CurrentDirection == Vector3.left)
            {
                AxisType = 1;
                AxisValue = transform.localPosition.x;
            }
            else
            {
                AxisType = 2;
                AxisValue = transform.localPosition.z;
            }

            byte[] comp = QuantizationUtils.QuantizeFloatToBytes(AxisValue);

            // Packet: [1 or 2][axis value]
            byte[] axisPacket = new byte[1 + 2];
            axisPacket[0] = AxisType; // packet type
            axisPacket[1] = comp[0];
            axisPacket[2] = comp[1];
            RemotePlayer.UpdatePosition(axisPacket);
            Debug.Log($"[AXIS {AxisType}] {axisPacket.Length} bytes");
        }
        else
        {
            Vector3 moveDirection = CurrentDirection.normalized;
            transform.localPosition += (moveDirection * MoveSpeed * Time.deltaTime);

            if (RemotePlayer != null)
            {
                byte[] compressed = QuantizationUtils.QuantizeVector3ToBytes(transform.localPosition);
                RemotePlayer.UpdatePosition(compressed);
                Debug.Log($"Sent Size => {compressed.Length} \n Position => {transform.localPosition}");
            }
        }
    }
}

public static class QuantizationUtils
{
    private const float PositionMin = -100f;
    private const float PositionMax = 100f;

    public static short QuantizeFloat(float value)
    {
        float t = Mathf.InverseLerp(PositionMin, PositionMax, value);
        float scaled = (t * 2f - 1f) * short.MaxValue;
        return (short)Mathf.RoundToInt(scaled);
    }

    public static float DequantizeFloat(short value)
    {
        float normalized = (float)value / short.MaxValue;
        float t = (normalized + 1f) * 0.5f;
        return Mathf.Lerp(PositionMin, PositionMax, t);
    }

    public static byte[] QuantizeFloatToBytes(float value)
    {
        short q = QuantizeFloat(value);
        byte[] bytes = new byte[2];
        Buffer.BlockCopy(new short[] { q }, 0, bytes, 0, 2);
        return bytes;
    }

    public static short[] QuantizeVector3(Vector3 pos)
    {
        return new short[3]
        {
            QuantizeFloat(pos.x),
            QuantizeFloat(pos.y),
            QuantizeFloat(pos.z)
        };
    }

    public static Vector3 DequantizeVector3(short[] data)
    {
        return new Vector3(
            DequantizeFloat(data[0]),
            DequantizeFloat(data[1]),
            DequantizeFloat(data[2])
        );
    }

    public static byte[] ShortsToBytes(short[] shorts)
    {
        byte[] buffer = new byte[6];

        Buffer.BlockCopy(shorts, 0, buffer, 0, 6);

        return buffer;
    }

    public static short[] BytesToShorts(byte[] bytes)
    {
        short[] shorts = new short[3];

        Buffer.BlockCopy(bytes, 0, shorts, 0, 6);

        return shorts;
    }

    public static byte[] QuantizeVector3ToBytes(Vector3 pos)
    {
        return ShortsToBytes(QuantizeVector3(pos));
    }

    public static Vector3 DequantizeVector3FromBytes(byte[] bytes)
    {
        return DequantizeVector3(BytesToShorts(bytes));
    }
}
