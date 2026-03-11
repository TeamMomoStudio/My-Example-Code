using System;
using UnityEngine;

public class FishingMinigameQTE : MonoBehaviour
{
   
    public event Action<bool> OnMinigameEnded;

    [Header("QTE Settings")]
    public float cursorSpeed = 50f;
    [Tooltip("จุดศูนย์กลางของแถบที่กดแล้วได้ปลา (0 ถึง 100)")]
    public float sweetSpotCenter = 50f;
    [Tooltip("ความกว้างของเป้าหมาย")]
    public float sweetSpotTolerance = 15f; 

    private float cursorPosition = 0f;
    private int direction = 1; // 1 = วิ่งขวา, -1 = วิ่งซ้าย
    private bool isPlaying = false;

    // เรียกฟังก์ชันนี้จาก Event Manager เมื่อปลาฮุบเหยื่อ
    public void StartMinigame()
    {
        cursorPosition = 0f;
        isPlaying = true;
        gameObject.SetActive(true); // เปิด UI
    }

    private void Update()
    {
        if (!isPlaying) return;

        MoveCursor();
        HandleInput();
    }

    private void MoveCursor()
    {
        // คำนวณเคอร์เซอร์วิ่งไป-กลับ (0 ถึง 100)
        cursorPosition += direction * cursorSpeed * Time.deltaTime;

        if (cursorPosition >= 100f)
        {
            cursorPosition = 100f;
            direction = -1;
        }
        else if (cursorPosition <= 0f)
        {
            cursorPosition = 0f;
            direction = 1;
        }

        
    }

    private void HandleInput()
    {
        // รับ Input จากผู้เล่น (แตะหน้าจอ หรือ กด Spacebar)
        if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space))
        {
            CheckResult();
        }
    }

    private void CheckResult()
    {
        isPlaying = false;
        
        // เช็คว่าเคอร์เซอร์อยู่ในระยะ Sweet Spot หรือไม่
        float distance = Mathf.Abs(cursorPosition - sweetSpotCenter);
        bool isSuccess = distance <= sweetSpotTolerance;

        if (isSuccess)
        {
            Debug.Log("Catch Success! Perfect Timing.");
        }
        else
        {
            Debug.Log("Missed! Fish escaped.");
        }

        gameObject.SetActive(false); // ปิด UI
        
        //  Event บอกระบบอื่นให้เอาปลาเข้า Inventory หรือรีเซ็ตเบ็ด
        OnMinigameEnded?.Invoke(isSuccess);
    }
}