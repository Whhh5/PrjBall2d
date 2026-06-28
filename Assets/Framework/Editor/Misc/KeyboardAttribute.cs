using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyboardAttribute : Attribute
{
    public KeyCode keyCode;
    public KeyboardAttribute(KeyCode keyCode)
    {
        this.keyCode = keyCode;
    }
}
