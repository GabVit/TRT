using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    [SerializeField] float health;
    [SerializeField] Transform head;
    public CameraShake cameraShake;
    private AudioManagerEasy audioManagerEasy;
    public GameObject textoPrefab;

    private bool IsConnected;
    public string deviceName= "ESP32_BT";
    public string motor1 = "F";
    public string motor2 = "B";

    private void Start()
    {
        #if UNITY_2020_2_OR_NEWER
            #if UNITY_ANDROID
                if (!Permission.HasUserAuthorizedPermission(Permission.CoarseLocation)
                || !Permission.HasUserAuthorizedPermission(Permission.FineLocation)
                || !Permission.HasUserAuthorizedPermission("android.permission.BLUETOOTH_SCAN")
                || !Permission.HasUserAuthorizedPermission("android.permission.BLUETOOTH_ADVERTISE")
                || !Permission.HasUserAuthorizedPermission("android.permission.BLUETOOTH_CONNECT"))
                    Permission.RequestUserPermissions(new string[] {
                                Permission.CoarseLocation,
                                    Permission.FineLocation,
                                    "android.permission.BLUETOOTH_SCAN",
                                    "android.permission.BLUETOOTH_ADVERTISE",
                                    "android.permission.BLUETOOTH_CONNECT"
                            });
            #endif
        #endif

        IsConnected = false;
        
        BluetoothService.CreateBluetoothObject();

        if (!IsConnected)
        {
            IsConnected = BluetoothService.StartBluetoothConnection(deviceName);
            
        }
        if (!IsConnected)
        {
            IsConnected = BluetoothService.StartBluetoothConnection("ESP32_BT");
            
        }
        BluetoothService.WritetoBluetooth(motor1);
        BluetoothService.WritetoBluetooth("F");

    }
    private void Awake()
    {
        audioManagerEasy = FindObjectOfType<AudioManagerEasy>();
     
    }
    public void TakeDamage(float damage)
    {

       

        StartCoroutine(cameraShake.Shake());
        health -= damage;
        audioManagerEasy.SeleccionAudio(0, 3f);
       
        

        Debug.LogWarning(string.Format("Player health: {0}", health));
        //probabilidad 50% de activar 1 motor o el otro
        if( UnityEngine.Random.Range(0, 100) < 50)
        {
            BluetoothService.WritetoBluetooth(motor1);
            BluetoothService.WritetoBluetooth("F");
        }
        else
        {
            BluetoothService.WritetoBluetooth(motor2);
            BluetoothService.WritetoBluetooth("B");
        }

        
        // Verificar si la salud llega a cero o menos
        if (health <= 0)
        {

            // ir a la escena de Game Over pero a los 5 segundos
            Invoke("GameOver", 3f);
          
        }
    }
   
    public void SendBTMessage(string message){
        BluetoothService.WritetoBluetooth(message);
    }

    private void GameOver()
    {
        // Obtener el nombre de la escena actual
        string currentSceneName = SceneManager.GetActiveScene().name;

        // Ir a la escena de Game Over
        SceneManager.LoadScene("3 GameOver");
    }

    public Vector3 GetHeadPosition()
    {
        return head.position;
    }

    
}
