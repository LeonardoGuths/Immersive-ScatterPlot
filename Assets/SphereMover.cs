using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphereMover : MonoBehaviour
{

	public TextMesh myText;
	public float multiplier = 1f;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
		Vector3 pos = this.transform.localPosition;
		pos.y = Mathf.Sin(Time.timeSinceLevelLoad) * multiplier; 
		this.transform.localPosition = pos;

		myText.text = pos.y.ToString();
   
    }
}
