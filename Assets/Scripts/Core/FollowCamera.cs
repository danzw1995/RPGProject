﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Core
{
  public class FollowCamera : MonoBehaviour
  {
    [SerializeField] private Transform target = null;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void LateUpdate()
    {
      transform.position = target.position;
    }
  }

}
