using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitializeGame : MonoBehaviour
{

    [SerializeField]
    public NewTown Elvenhold;

    [SerializeField]
    public Camera mainCamera;
    // Start is called before the first frame update
    void Start()
    {
        //GameConstants.mainCamera = this.mainCamera;
        //GameObject elfGroup = GameObject.Find("ElfStartHome");

        //GameConstants.elves = new List<Elf>(elfGroup.GetComponentsInChildren<Elf>());

        //GridManager gm = Elvenhold.GetComponent<GridManager>();
        //foreach(Elf elf in GameConstants.elves)
        //{
        //    gm.AddElement(elf.gameObject);
        //}
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
