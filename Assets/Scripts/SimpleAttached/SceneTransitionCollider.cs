using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class SceneTransitionCollider : MonoBehaviour
{
    public string destination;

    private bool active;
    private RawImage solid; 
    // Start is called before the first frame update
    void Start()
    {
        solid = Managers.Menu.solid;
    }

    // Update is called once per frame
    void Update()
    {
        if(active)
        {
            Debug.Log("Changing scene");
            Managers.Player.Hold();
            if(solid.color.a < 1f)
            {
                Debug.Log("Fading");
                solid.color = new Color(0f, 0f, 0f, Mathf.MoveTowards(solid.color.a, 1f, 0.3f * Time.deltaTime));
            }
            else
            {
                Debug.Log("Switching");
                SceneManager.LoadScene(destination);
                Managers.Menu.sceneChange = false;
                Managers.Player.Release();
            }
            return;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            active = true;
            Managers.Menu.sceneChange = true;
        }
    }
    
}
