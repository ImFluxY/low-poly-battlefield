using System;
using UnityEngine;

[Serializable]
[CreateAssetMenu(menuName = "Low Poly Battlefield/Photon/Game Map", fileName = "gameMap")]
public class GameMap : ScriptableObject
{
    [SerializeField] private string _name;
    [SerializeField] private int _buildIndex;

    public string Name
    {
        get { return _name; }
        private set { _name = value; }
    }

    public int BuilIndex
    {
        get { return _buildIndex; }
        private set { _buildIndex = value; }
    }
}
