using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SpherePlotter : MonoBehaviour
{

	public SphereMover sphereMoverPrefab;

    // Start is called before the first frame update
    void Start()
    {
		GameObject.Instantiate(sphereMoverPrefab.gameObject, transform);
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
