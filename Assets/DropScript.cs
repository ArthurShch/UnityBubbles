﻿using UnityEngine;
using System.Collections;
using System.Threading;
using UnityEngine.UI;
using System.Collections.Generic;
using Helpers;

public class DropScript : MonoBehaviour
{

    List<GameObject> sections;
    List<float> listOpacity;
    List<int> listRotate; 
    Dropdown Drop;  //выбор ранее созданной секции 
    Dropdown SideSection;  //выбор стороны поворота секции
    GameObject panelSection; // секция которая двигается 
    Toggle EnableClaster;
    GameObject respawnPrefab;
    GameObject Claster;
    GameObject AnimationPanel; // секция для анимации
    Vector3 centerCube;
    public float maxDist;
    Scrollbar ScrollbarOpacity;
    Toggle ModeView;
    //получить все точки для анимации

    List<GameObject> listOfSphere = new List<GameObject>();
    List<Vector3> listOfSphereVector3 = new List<Vector3>();
    Timer timer;
    int counterStop = 0;

    //проверка изменения цвета
    public static void Count(object obj, float param = 0f)
    {

        //Color ghjjuyg = ((GameObject)obj).gameObject.GetComponent<Renderer>().material.color;
        //((GameObject)obj).gameObject.GetComponent<Renderer>().material.color = new Color(1, ghjjuyg.g, ghjjuyg.b, param);

       
        //((GameObject)obj).gameObject.GetComponent<Renderer>().material.color = new Color(1, 0, 0, param);
    }
   
    

    //public  Drop;
    // Use this for initialization

    //Color getColor(Vector3 positionShare)
    //{


    //    float dist = maxDist - Vector3.Distance(centerCube, positionShare);

    //    dist = dist < 0 ? 0 : dist;

    //    float percentRED = dist / (maxDist / 100);
    //    float www = (100 - percentRED) / 100;

    //    Color result = new Color(1, www, www, 1);

    //    return result;
    //}

    void Start()
    {
        respawnPrefab = GameObject.FindWithTag("CenterAquo");
        SideSection = GameObject.FindWithTag("SideSection").GetComponent<Dropdown>();
        Drop = GameObject.FindWithTag("Drop").GetComponent<Dropdown>();
        EnableClaster = GameObject.FindWithTag("EnableClaster").GetComponent<Toggle>();
        panelSection = GameObject.FindWithTag("panelSection");
        Claster = GameObject.FindWithTag("Claster");
        ScrollbarOpacity = GameObject.FindWithTag("ScrollbarOpacity").GetComponent<Scrollbar>();
        AnimationPanel = GameObject.FindWithTag("AnimationPanel");
        ModeView = GameObject.FindWithTag("ModeView").GetComponent<Toggle>();

        sections = new List<GameObject>();
        listOpacity = new List<float>();
        listRotate = new List<int>();

        centerCube = respawnPrefab.GetComponent<Renderer>().bounds.center;

        Vector3 VMaxDist = new Vector3(
            respawnPrefab.GetComponent<Renderer>().bounds.center.x + respawnPrefab.GetComponent<Renderer>().transform.localScale.x * 0.5f,
            respawnPrefab.GetComponent<Renderer>().bounds.center.y + respawnPrefab.GetComponent<Renderer>().transform.localScale.y * 0.5f,
            respawnPrefab.GetComponent<Renderer>().bounds.center.z + respawnPrefab.GetComponent<Renderer>().transform.localScale.z * 0.5f
            );

        maxDist = Vector3.Distance(VMaxDist, centerCube);

        SideSection.onValueChanged.AddListener(delegate { SideSectionChange(); });
        Drop.onValueChanged.AddListener(delegate { myDropdownValueChangedHandler(); });
        EnableClaster.onValueChanged.AddListener(delegate { ToggleEnableClaster(); });
        ModeView.onValueChanged.AddListener(delegate { changeMode(); });
        ScrollbarOpacity.onValueChanged.AddListener(delegate { ChangeOpasitySelectedSection(); });


        /////ТЕСТ
        //AddNewOption();
        //ModeView.isOn = false;
        //changeMode();
        //Animation();
    }

  

    /// <summary>
    /// Изменение режима просмотра
    /// </summary>
    void changeMode()
    {
        if (ModeView.isOn)
        {
            for (int i = 0; i < panelSection.transform.childCount; i++)
            {
                Color oldCol = panelSection.transform.GetChild(i).gameObject.GetComponent<Renderer>().material.color;

                panelSection.transform.GetChild(i).gameObject.GetComponent<Renderer>().material.color
                    = new Color(oldCol.r, oldCol.g, oldCol.b, 1);
            }

            Color calCol = sections[Drop.value].GetComponent<Renderer>().material.color;
            sections[Drop.value].GetComponent<Renderer>().material.color = new Color(calCol.r, calCol.g, calCol.b, 1);
        }
        else
        {
            for (int i = 0; i < panelSection.transform.childCount; i++)
            {
                Color oldCol = panelSection.transform.GetChild(i).gameObject.GetComponent<Renderer>().material.color;

                panelSection.transform.GetChild(i).gameObject.GetComponent<Renderer>().material.color
                    = new Color(oldCol.r, oldCol.g, oldCol.b, 0);
            }

            foreach (GameObject item in sections)
            {
                Color oldCol = item.GetComponent<Renderer>().material.color;
                item.GetComponent<Renderer>().material.color = new Color(oldCol.r, oldCol.g, oldCol.b, 0);
            }
            //panelSection
        }
    }
    /// <summary>
    /// Изменение прозрачности выделенной секции
    /// </summary>
    void ChangeOpasitySelectedSection()
    {
        if (sections.Count != 0)
        {
            int selected = Drop.value;
            float opVal = ScrollbarOpacity.value;
            int count = sections[selected].transform.childCount;
            for (int i = 0; i < count; i++)
            {
                Color oldColor = sections[selected].transform.GetChild(i).gameObject.GetComponent<Renderer>().material.color;
                sections[selected].transform.GetChild(i).gameObject.GetComponent<Renderer>().material.color
                    = new Color(oldColor.r, oldColor.g, oldColor.b, opVal);
            }
        }
    }
    /// <summary>
    /// Прересоздать секции с учётом поподания точек в кластер
    /// </summary>
    private void ToggleEnableClaster()
    {
        List<Vector3> positionSections = new List<Vector3>();

        foreach (GameObject item in sections)
        {
            positionSections.Add(item.transform.position);
            Destroy(item);
        }

        sections.Clear();

        for (int i = 0; i < positionSections.Count; i++)
        {
            sections.Add(Helper.createNewBoolsPanel
            (
                respawnPrefab,
                Claster,
                EnableClaster.isOn,
                listOpacity[i],
                maxDist,
                positionSections[i], 
                listRotate[i]
            ));
        }

    }

    private void SideSectionChange() 
    {

    }
    

    /// <summary>
    /// Изменение цвета секции которая выделена с учётом режима просмотра
    /// </summary>
    private void myDropdownValueChangedHandler()
    {
        //полная прозрачность для не выделенных
        foreach (var item in sections)
        {
            item.GetComponent<Renderer>().material.color = new Color(1, 0, 0, 0);
        }

        //режима просмотра
        if (ModeView.isOn)
        {
            sections[Drop.value].GetComponent<Renderer>().material.color = new Color(0, 1, 0, 0.8f);
        }
        // sections.RemoveAt(Drop.value);


        // Debug.Log("selected: " + target.value);
    }

    //public GameObject kreate(Vector3 cenet, float opVal)
    //{
    //    GameObject cyb = GameObject.CreatePrimitive(PrimitiveType.Cube);

    //    //Vector3 centerPanelSection = panelSection.GetComponent<Renderer>().bounds.center;
    //    Vector3 centerPanelSection = cenet;
    //    cyb.GetComponent<Renderer>().transform.position = centerPanelSection;
    //    // cyb.GetComponent<Renderer>().transform.localScale = panelSection.GetComponent<Renderer>().transform.lossyScale;

    //    cyb.GetComponent<Renderer>().material.color = new Color(255, 0, 0, 0);
    //    cyb.GetComponent<Renderer>().material.shader = Shader.Find("Transparent/Diffuse");
    //    cyb.transform.localScale = new Vector3(50, 50, 2);

    //    for (int x = 0; x < 50; x++)
    //    {
    //        for (int y = 0; y < 50; y++)
    //        {
    //            Vector3 Dot = new Vector3(
    //                x + centerPanelSection.x - (50 * 0.5f),
    //                y + centerPanelSection.y - (50 * 0.5f),
    //                centerPanelSection.z);

    //            // здесь использование тугле
    //            //(!EnableClaster.isOn) &&

    //            if (!EnableClaster.isOn)
    //            {
    //                if (!Claster.GetComponent<Renderer>().bounds.Contains(Dot))
    //                    continue;
    //            }


    //            GameObject cylinder = GameObject.CreatePrimitive(PrimitiveType.Sphere);
    //            cylinder.transform.position = Dot;

    //            //(cylinder.GetComponent<Collider>() as SphereCollider).radius = 100f;
    //            cylinder.GetComponent<Renderer>().material.color = new Color(1, 0, 0, opVal);
    //            cylinder.GetComponent<Renderer>().material.shader = Shader.Find("Transparent/Diffuse");


    //            // cylinder.transform.localScale = new Vector3(cylinder.transform.localScale.x, cylinder.transform.localScale.y, 0.1f);
    //            cylinder.transform.parent = cyb.transform;
    //            //cylinder.transform.parent = panelSection.transform;
    //            //cylinder.transform.localScale = new Vector3(1, 1, 1);

    //        }
    //    }


    //    for (int i = 0; i < cyb.transform.childCount; i++)
    //    {
    //        // Color wqww = panelSection.transform.GetChild(i).gameObject.GetComponent<Renderer>().material.color;

    //        // cyb.transform.GetChild(i).gameObject.GetComponent<Renderer>().material.color = new Color(wqww.r, wqww.g, wqww.b, wqww.a);
    //        cyb.transform.GetChild(i).gameObject.GetComponent<Renderer>().material.color
    //            = getColor(cyb.transform.GetChild(i).gameObject.transform.position);
    //    }

    //    return cyb;

    //}


    /// <summary>
    /// Добавление новой секции
    /// </summary>
    public void AddNewOption()
    {
        //ScrollbarOpacity.value;

        sections.Add(Helper.createNewBoolsPanel
            (
                respawnPrefab,
                Claster,
                EnableClaster.isOn,
                ScrollbarOpacity.value,
                maxDist,
                panelSection.GetComponent<Renderer>().bounds.center, 
                SideSection.value
            ));
      

        //sections.Add(kreate(panelSection.GetComponent<Renderer>().bounds.center, ScrollbarOpacity.value));
        listOpacity.Add(ScrollbarOpacity.value);
        listRotate.Add(SideSection.value);
        //        sections.Add(Instantiate(panelSection));
        Drop.options.Add(new Dropdown.OptionData("new text"));

        // Drop.transform.GetChild

        //получить координаты панели
        //добавить в список контролла и свой список

       // Debug.Log("new text");
    }
    /// <summary>
    /// Удаление секции 
    /// </summary>
    public void DeleteOption()
    {
        Drop.options.RemoveAt(Drop.value);

        Destroy(sections[Drop.value]);
        sections.RemoveAt(Drop.value);
        listOpacity.RemoveAt(Drop.value);
        listRotate.RemoveAt(Drop.value);
    }

    public void Animation() 
    {
       // //Time.deltaTime

       //// Update();
       // получить все точки
        listOfSphere = new List<GameObject>();

        foreach (GameObject item in sections)
        {
            for (int i = 0; i < item.transform.childCount; i++)
            {
                //panelSection.transform.GetChild(i).gameObject.GetComponent<Renderer>().material.color

                listOfSphere.Add(item.transform.GetChild(i).gameObject);
                listOfSphereVector3.Add(item.transform.GetChild(i).gameObject.transform.localPosition);
            }
        }



       // //по ка что выключим так как оно и так в отсортированном виде listOfSphere[j] > listOfSphere[j +1]

       // ////сортировка по Х 
       // //for (int i = 0; i < listOfSphere.Count - 1; i++)
       // //{
       // //    for (int j = 0; j < listOfSphere.Count - 1; j++)
       // //    {
       // //        if (listOfSphere[j].transform.localPosition.x > listOfSphere[j + 1].transform.localPosition.x)
       // //        {
       // //            GameObject buf = listOfSphere[j];
       // //            listOfSphere[j] = listOfSphere[j + 1];
       // //            listOfSphere[j + 1] = buf;
       // //        }
       // //    }
       // //}


       //int state = 2;

       // foreach (GameObject item in listOfSphere)
       // {
       //     Count(item);
       // }



       // TimerCallback tm = new TimerCallback(CheckStatus);
       // timer = new Timer(tm, state, 0, 1000);



       // foreach (GameObject item in listOfSphere)
       // {


       //     //Count(item);


       //     // item.gameObject.GetComponent<Renderer>().material.color = new Color(0, 1, 0, 1);
       //     //GetComponent<Renderer>().material.color = new Color(1, 0, 0, 0);
       // }




    }

    public void CheckStatus(object stateInfo)
    {


        if (counterStop < 10)
        {
            float oneStep = 0.5f - counterStop * 0.1f;//0.5f; //5 / 10;

            //oneStep -= counterStop * 0.1f; 
            List<GameObject> RangeList = new List<GameObject>();

            //foreach (GameObject item in listOfSphere)
            for (int i = 0; i < listOfSphereVector3.Count; i++)            
            {
                if (listOfSphereVector3[i].x <= oneStep + 0.1f && listOfSphereVector3[i].x >= oneStep)
                {
                    RangeList.Add(listOfSphere[i]);
                }
            }

            foreach (GameObject item in RangeList)
            {
                Count(item, 1f);
            }

            counterStop++;
          
            Debug.Log(counterStop);
        }
    }
    // Update is called once per frame

    //public float titi = 0.25f;
    public float tyty = 0;

    


    void Update()
    {
        //if (titi > 0)
        //{
        //    titi -= Time.deltaTime;
        //}
        //if (titi <= 0)
        //{
        //    titi -= Time.deltaTime;
        //}


        //if (tyty > 0)
        //{
        //    tyty -= Time.deltaTime;
        //}
        //if (tyty <= 0)
        //{

        //    if (counterStop < 10)
        //    {
        //        float oneStep = 0.5f - counterStop * 0.1f;//0.5f; //5 / 10;

        //        //oneStep -= counterStop * 0.1f; 
        //        List<GameObject> RangeList = new List<GameObject>();

        //        foreach (GameObject item in listOfSphere)
        //        {
        //            if (item.transform.localPosition.x <= oneStep + 0.1f && item.transform.localPosition.x >= oneStep)
        //            {
        //                RangeList.Add(item);
        //            }
        //        }

        //        foreach (GameObject item in RangeList)
        //        {
        //            Count(item, 1f);
        //        }

        //        counterStop++;
        //        tyty = 0.0001f;
        //        Debug.Log(counterStop);
        //    }
            
        //}


          //  Debug.Log(Time.deltaTime);


        




        //counterStop++;
        //if (counterStop >= 10)
        //{
        //    timer.Change(System.Threading.Timeout.Infinite, 0);
        //    counterStop = 0;
        //}
    }
}
