using System;
using System.Collections;
using System.Collections.Generic;
using Player;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class LevelInitializer : MonoBehaviour
{
    [SerializeField] private GameObject ground;
    [SerializeField] private List<GameObject> traps;
    [SerializeField] private float roadLength;
    [SerializeField] private float spacing;
    [SerializeField] private GameObject trapParent;
    [SerializeField] private GameObject finishLine;
    [SerializeField] private GameObject[] tutorials;
    [SerializeField] private GameObject failed;
    [SerializeField] private GameObject nextLevel;
    [SerializeField] private Text levelText;

    public int _level = 0;
    void Start()
    {
        var localScale = ground.transform.localScale;
        localScale.z = localScale.z * roadLength;
        ground.transform.localScale = localScale;
        tutorials[0].SetActive(true);
    }

    public void ReCreateLevel()
    {
        if (_level == 3)
        {
            tutorials[2].SetActive(true);
            return;
        }
        if (_level == 0)
        {
            CreateLevel();
        }
        else
        {
            foreach (Transform child in trapParent.transform) {
                Destroy(child.gameObject);
            }
            CreateLevel();
        }
        
        tutorials[_level].SetActive(false);
        
        _level++;
        levelText.text = "Level: " + (_level);
      
    }
    
    private void CreateLevel()
    {
        int trapCount = Convert.ToInt32(roadLength / spacing)-1;
        Line lastSelected;
        var values = Enum.GetValues(typeof(Line));
        Line randomLine=(Line) values.GetValue(Random.Range(0, values.Length));
        for (int i = 0; i < trapCount; i++)
        {
            var randomIndex = Random.Range(0, traps.Count);
            var pos = new Vector3(0, .5f, (i + 1) * spacing);
            if(traps[randomIndex].name=="UpTrap")
            {
                pos.y = 1.1f;
            }else if (traps[randomIndex].name == "Butcher")
            {
                lastSelected = randomLine;
                while (randomLine==lastSelected)
                {
                    randomLine= (Line) values.GetValue(Random.Range(0, values.Length));
                }
                
                switch (randomLine)
                {
                    case Line.Left:
                        pos.x = -1;
                        break;
                    case Line.Center:
                        break;
                    case Line.Right:
                        pos.x = 1;
                        break;
                }
            }
            Instantiate(traps[randomIndex], pos,Quaternion.Euler(0,180,0),trapParent.transform);
        }
        var finishPos = new Vector3(0, 0, roadLength);
        Instantiate(finishLine, finishPos,Quaternion.identity,trapParent.transform);
    }

    public void Trapped()
    {
        failed.SetActive(!failed.activeSelf);
    }
    
    public void NextLevel()
    {
        nextLevel.SetActive(!nextLevel.activeSelf);
    }
}
