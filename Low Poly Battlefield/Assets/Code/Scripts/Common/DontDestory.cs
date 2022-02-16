using UnityEngine;

namespace KnoxGameStudios
{
    public class DontDestory : MonoBehaviour
    {
        private void Awake()
        {
          
        }

        private void Start()
        {
            GameObject[] objs = GameObject.FindGameObjectsWithTag("Team Manager");

            Debug.Log(objs);

            if (objs.Length > 1)
            {
                Destroy(gameObject);
            }
            else
            {
                gameObject.transform.parent = null;
                DontDestroyOnLoad(gameObject);
            }
        }
    }
}