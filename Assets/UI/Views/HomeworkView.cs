using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HomeworkView : View
{
    [SerializeField] Homework homeworkObject;    
    [SerializeField] GameObject homeworkPrefab;    
    [SerializeField] GameObject content;    
    //[SerializeField] private Button RefreshButton;
    
    public override void Initialize()
    {
        //RefreshButton.onClick.AddListener(Initialize);
        List<Homework.Item> homework = homeworkObject.getHomework().items;
        

        foreach (Homework.Item HomeworkItem in homework)
        {
            var homeworkItem = Instantiate(homeworkPrefab, content.transform);

            string onderwerp = HomeworkItem.studiewijzerItem.onderwerp;
            
            if (onderwerp.Length == 0)
                onderwerp = HomeworkItem.studiewijzerItem.omschrijving;

            homeworkItem.GetComponent<HomeworkInfo>().SetHomeworkInfo(HomeworkItem.lesgroep.vak.naam, onderwerp, HomeworkItem.additionalObjects.swigemaaktVinkjes?.items[0].gemaakt ?? false);
        }
        
        base.Initialize();
    }
}
//lesgroep>vak>naam