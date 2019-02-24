﻿using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class AmbianceManager : MonoBehaviour
{
    [FMODUnity.EventRef]
    public string CityMusicEvent;
    
    [FMODUnity.EventRef]
    public string ThunderAmbianceEvent;

    [FMODUnity.EventRef]
    public string HomeMusicEvent;

    private FMOD.Studio.EventInstance cityMusicInstance;
    private FMOD.Studio.EventInstance homeMusicInstance;
    private FMOD.Studio.EventInstance thunderAmbianceInstance;

    private Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Start()
    {
        // Debug.Log("AmbienceManager.Start()");
        thunderAmbianceInstance = FMODUnity.RuntimeManager.CreateInstance(ThunderAmbianceEvent);
        homeMusicInstance = FMODUnity.RuntimeManager.CreateInstance(HomeMusicEvent);
        
        InitializeTitleScreenAmbience();

        EventBus.Subscribe<EnterHomeEvent>(OnEnterHome);
        EventBus.Subscribe<EnterCityEvent>(OnEnterCity);
        EventBus.Subscribe<EnterTitleScreenEvent>(OnEnterTitleScreen);
    }

    private void InitializeTitleScreenAmbience()
    {
        cityMusicInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        cityMusicInstance.release();

        homeMusicInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);

        thunderAmbianceInstance.setTimelinePosition(0);
        FMOD.Studio.ParameterInstance param;
        thunderAmbianceInstance.getParameter("Location", out param);
        param.setValue(0);
        thunderAmbianceInstance.start();
    }

    private void OnEnterTitleScreen(EnterTitleScreenEvent e)
    {
        InitializeTitleScreenAmbience();
    }

    private void OnEnterHome(EnterHomeEvent e)
    {
        cityMusicInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        cityMusicInstance.release();

        FMOD.Studio.ParameterInstance param;
        thunderAmbianceInstance.getParameter("Location", out param);
        param.setValue(100);

        homeMusicInstance.getParameter("Collection_Phase", out param);
        param.setValue(30 * GameProgress.NumObjectivesComplete);
        homeMusicInstance.setTimelinePosition(0);
        homeMusicInstance.start();
    }

    private void OnEnterCity(EnterCityEvent e)
    {
        homeMusicInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);

        FMOD.Studio.ParameterInstance param;
        thunderAmbianceInstance.getParameter("Location", out param);
        param.setValue(0);

        cityMusicInstance = FMODUnity.RuntimeManager.CreateInstance(CityMusicEvent);
        cityMusicInstance.start();
    }
}
