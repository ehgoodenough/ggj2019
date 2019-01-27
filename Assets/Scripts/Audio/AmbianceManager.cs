using UnityEngine;

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
        cityMusicInstance = FMODUnity.RuntimeManager.CreateInstance(CityMusicEvent);
        thunderAmbianceInstance = FMODUnity.RuntimeManager.CreateInstance(ThunderAmbianceEvent);

        homeMusicInstance = FMODUnity.RuntimeManager.CreateInstance(HomeMusicEvent);

        cityMusicInstance.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(transform.position));
        FMODUnity.RuntimeManager.AttachInstanceToGameObject(cityMusicInstance, gameObject.transform, rb);

        thunderAmbianceInstance.start();

        EventBus.Subscribe<EnterHomeEvent>(OnEnterHome);
        EventBus.Subscribe<EnterCityEvent>(OnEnterCity);
    }

    private void OnEnterHome(EnterHomeEvent e) {
        cityMusicInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        FMOD.Studio.ParameterInstance param;
        thunderAmbianceInstance.getParameter("Location", out param);
        param.setValue(100);
        homeMusicInstance.setTimelinePosition(0);
        homeMusicInstance.start();
    }

    private void OnEnterCity(EnterCityEvent e)
    {
        homeMusicInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        FMOD.Studio.ParameterInstance param;
        thunderAmbianceInstance.getParameter("Location", out param);
        param.setValue(0);
        cityMusicInstance.setTimelinePosition(0);
        cityMusicInstance.start();
    }
}
