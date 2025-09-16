using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    public List<GameObject> NotePrefabs;

    [SerializeField]
    public List<Transform> NoteBoardLines;

    [SerializeField]
    public MusicVisualizer musicVisualizer;
}
