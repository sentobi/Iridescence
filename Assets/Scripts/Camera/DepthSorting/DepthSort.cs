﻿using UnityEngine;
using System.Collections;

public class DepthSort : MonoBehaviour {

	// Update is called once per frame
	void LateUpdate () {
        DoDepthSort();
    }

    public void DoDepthSort()
    {
        if (transform.hasChanged)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y,
                DepthSortManager.instance.CalculateZCoord(transform.position.y));

            transform.hasChanged = false;
        }
    }
}
