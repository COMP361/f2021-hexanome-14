using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoginUIManager : MonoBehaviour
{

    [SerializeField] InputField usernameInput;
    [SerializeField] InputField passwordInput;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnLoginClicked()
    {
        string username = usernameInput.text;
        string password = passwordInput.text;

        Debug.Log($"Logging in with username: {username} and password: {password}");
    }
}
