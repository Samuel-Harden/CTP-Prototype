﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ROTATE : MonoBehaviour
{
    // Update is called once per frame
    void Update ()
    {
        transform.Rotate(0, 5 * Time.deltaTime, 0);
	}
}
