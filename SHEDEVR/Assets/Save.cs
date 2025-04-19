using System;
using System.Collections;
using System.IO;
using UnityEngine;

[System.Serializable]
public class PlayerData
{
    public float posX;
    public float posY;
    public float posZ;

    public float rotX;
    public float rotY;
    public float rotZ;
    public float rotW;
}

public class Save : MonoBehaviour
{
    FirstPersonController fps;

    private string savePath;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        fps = GameObject.FindWithTag("Player").GetComponent<FirstPersonController>();
    }

    private void Awake()
    {
        // �������� ���� ��� ���������� �� Android
        savePath = Path.Combine(Application.persistentDataPath, "playerSave.txt");
        Debug.Log(savePath);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public IEnumerator AddSave()
    {
        yield return new WaitForSeconds(0.5f);

        PlayerData data = new PlayerData();

        // ��������� �������
        data.posX = fps.transform.position.x;
        data.posY = fps.transform.position.y;
        data.posZ = fps.transform.position.z;

        // ��������� �������
        data.rotX = fps.transform.rotation.x;
        data.rotY = fps.transform.rotation.y;
        data.rotZ = fps.transform.rotation.z;
        data.rotW = fps.transform.rotation.w;

        // ������������ � JSON
        string jsonData = JsonUtility.ToJson(data);

        try
        {
            // ���������� � ���� (��������� �������������, ���� �� ����������)
            File.WriteAllText(savePath, jsonData);
        }
        catch (Exception e)
        {
        }
    }
    public bool LoadPlayerData(Transform playerTransform)
    {
        if (File.Exists(savePath))
        {
            try
            {
                string jsonData = File.ReadAllText(savePath);
                PlayerData data = JsonUtility.FromJson<PlayerData>(jsonData);

                playerTransform.position = new Vector3(data.posX, data.posY, data.posZ);

                playerTransform.rotation = new Quaternion(data.rotX, data.rotY, data.rotZ, data.rotW);


                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }
        else
        {

            return false;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StartCoroutine(AddSave());
        }
    }
}

