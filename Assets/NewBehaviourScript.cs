﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System;
using Helpers;
public class NewBehaviourScript : MonoBehaviour
{
    GameObject panelSection;
    public GameObject respawnPrefab;
    Slider myS;
    Button NewPanelButton;
    GameObject Claster;
    Vector3 centerCube;


    List<GameObject> sharePoint;
    float startPoint;
    float maxDist;


    //устранить повторение кода 
    //1 генерация точек 



    //1 обёмные фигуры 
    //2 показывать какая выбрана
  
    //4 движение панели с кружочками в 3х плоскостях
    
    //!!!!!!!!!!!!новое
    //5 анимированное прохождение по срезам, с регулировакой скорости прохождения 
    

    //6 при перемещении panelSection соблюдать 
    //законы отрисовки относительно вписанной фигуры
    //т.е. за краем фигуры точки белые 

    [SerializeField]
    private Transform target; //невидимая цель для камеры

    public float zoomSpeed = 5.0f; //скорость приближения камеры
    private Vector3 _offset; //смещение камеры относительно объекта
    public float mouse_sens = 1f;
    public Camera cam_holder;
    float x_axis, y_axis, z_axis, _rotY, _rotX; //мышь по x, y, зум, координаты для обзора


    // Use this for initialization
    public void ValueChangeCheck()
    {
        Debug.Log(myS.value);

        panelSection.transform.position = new Vector3(
            panelSection.transform.position.x,
            panelSection.transform.position.y,
            startPoint - myS.value
            );


        //здесь можно использовать функуию из хелпера
        foreach (GameObject item in sharePoint)
        {
            float dist = maxDist - Vector3.Distance(centerCube, item.GetComponent<Renderer>().bounds.center);

            dist = dist < 0 ? 0 : dist;

            float percentRED = dist / (maxDist / 100);


            float www = (100 - percentRED) / 100;

            item.GetComponent<Renderer>().material.shader = Shader.Find("Transparent/Diffuse");
            item.GetComponent<Renderer>().material.color = new Color(1, www, www, 1);

        }

    }


    

    void Start()
    {

        cam_holder = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();
        
        
        respawnPrefab = GameObject.FindWithTag("CenterAquo");
        respawnPrefab.GetComponent<Renderer>().material.color = new Color(1, 0, 0, 0);
        // Vector3[] wwww =  respawnPrefab.GetComponent<MeshFilter>().mesh.vertices;
        // respawnPrefab.GetComponent<MeshRenderer>().bounds.
        
        centerCube = respawnPrefab.GetComponent<Renderer>().bounds.center;


        //Vector3 centerScale = respawnPrefab.GetComponent<Renderer>().transform.localScale;

        myS = GameObject.FindWithTag("SliderOfNewSection").GetComponent<Slider>();
        myS.onValueChanged.AddListener(delegate { ValueChangeCheck(); });
        myS.minValue = 0;
        myS.maxValue = respawnPrefab.GetComponent<Renderer>().transform.localScale.z;

        startPoint = respawnPrefab.GetComponent<Renderer>().bounds.center.z
            + respawnPrefab.GetComponent<Renderer>().transform.localScale.z * 0.5f;

        Vector3 VMaxDist = new Vector3(
            respawnPrefab.GetComponent<Renderer>().bounds.center.x + respawnPrefab.GetComponent<Renderer>().transform.localScale.x * 0.5f,
            respawnPrefab.GetComponent<Renderer>().bounds.center.y + respawnPrefab.GetComponent<Renderer>().transform.localScale.y * 0.5f,
            respawnPrefab.GetComponent<Renderer>().bounds.center.z + respawnPrefab.GetComponent<Renderer>().transform.localScale.z * 0.5f
            );

        maxDist = Vector3.Distance(VMaxDist, centerCube);

        NewPanelButton = GameObject.FindWithTag("NewPanelButton").GetComponent<Button>();


        Claster = GameObject.FindWithTag("Claster");
        Claster.GetComponent<Renderer>().material.color = new Color(1, 0, 0, 0);


        //panelSection.gameObject.transform.localScale += Vector3.zero;
        //panelSection.GetComponent<Renderer>().transform.localScale = Vector3.zero;
            //respawnPrefab.GetComponent<Renderer>().transform.localScale;

        panelSection = GameObject.FindWithTag("panelSection");

        panelSection.GetComponent<Renderer>().material.color = new Color(1, 0, 0, 0);
        panelSection.GetComponent<Renderer>().material.shader = Shader.Find("Transparent/Diffuse");
        Vector3 centerPanelSection = panelSection.GetComponent<Renderer>().bounds.center;
        //float cla = panelSection.GetComponent<Renderer>().transform.localScale.x;

        Helper.putBools(ref panelSection, true, 1f, panelSection.transform.position, Claster);

        sharePoint = new List<GameObject>();

        for (int i = 0; i < panelSection.transform.childCount; i++)
        {
            sharePoint.Add(panelSection.transform.GetChild(i).gameObject);
        }

        target = respawnPrefab.transform;

        //камера
        _rotY = transform.eulerAngles.y;
        _rotX = transform.eulerAngles.x;
        _offset = target.position - transform.position; //получает начальное смещение

        Debug.Log("Start");
    }

    // Update is called once per frame
    void Update()
    {
        
        
    }

    //public void Awake()
    //{
    //    Debug.Log("Awake");
    //}
    void LookAtTarget()
    {
        Quaternion rotation = Quaternion.Euler(_rotY, _rotX, 0); //задает вращение камеры 
        transform.position = target.position - (rotation * _offset);
        transform.LookAt(target);
    }
    void LateUpdate()
    {
        float input = Input.GetAxis("Mouse ScrollWheel");
        if (input != 0) //если крутится колесико мыши
        {
            cam_holder.fieldOfView *= 1 - input;// *zoomSpeed; //зум           
        }

        if (Input.GetMouseButton(1)) //левая кнопка мыши
        { //вращение вокруг объекта
            _rotX += Input.GetAxis("Mouse X") * mouse_sens; //поворот камеры вокруг объекта и сохранение координат
            _rotY -= Input.GetAxis("Mouse Y") * mouse_sens;

            LookAtTarget();
        }

        //if (Input.GetMouseButton(0)) //правая кнопка
        //{
        //    //обзор вокруг объекта
        //    //смещение камеры по осям X и Y
        //    x_axis = Input.GetAxis("Mouse X") * mouse_sens;
        //    y_axis = Input.GetAxis("Mouse Y") * mouse_sens;

        //    target.position = new Vector3(target.position.x + x_axis, target.position.y + y_axis, target.position.z);

        //    LookAtTarget();
        //}
        if (Input.GetMouseButton(2)) //колесико
        {
            //обзор вокруг камеры
            x_axis = Input.GetAxis("Mouse X") * mouse_sens;
            y_axis = Input.GetAxis("Mouse Y") * mouse_sens;
            //z_axis = Input.GetAxis("Mouse ScrollWheel") * wheel_sens;

            cam_holder.transform.Rotate(Vector3.up, x_axis, Space.World);
            cam_holder.transform.Rotate(Vector3.right, y_axis, Space.Self);
            //cam_holder.transform.localPosition = cam_holder.transform.localPosition * (1 - z_axis);
        }
    }

}