using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class InteractabkeWater : MonoBehaviour
{
    [Header("Mesh Generation")] [Range(2, 500)]
    public int NumOfXVertices = 70;

    public float Width = 10f;
    public float Height = 4f;
}
