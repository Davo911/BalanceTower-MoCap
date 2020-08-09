using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class tablet_manager : MonoBehaviour
{
    int touch = 0;
    [SerializeField] public int TabletDrag;

    // Start is called before the first frame update
    void Start()
    {
        TabletDrag = 100;
    }

    // Update is called once per frame
    void Update()
    {
    }

    
}
