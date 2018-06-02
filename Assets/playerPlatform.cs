using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerPlatform : MonoBehaviour {

	void OnCollisionEnter2D(Collision2D col)
	{
		if (col.gameObject.tag == "platform")
			this.transform.parent = col.transform;
	}

	void OnCollisionExit2D(Collision2D col)
	{
		if (col.gameObject.tag == "platform")
			this.transform.parent = null;
	}
}
