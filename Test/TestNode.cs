using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TestNode", menuName = "UnitySupport/TestNode", order = 0)]
public class TestNode : ScriptableObject
{
    public int value;

    public List<GameObject> gameObjects;
}
