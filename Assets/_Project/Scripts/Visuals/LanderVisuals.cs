using UnityEngine;

[RequireComponent(typeof(Lander))]
public class LanderVisuals : MonoBehaviour
{
    [SerializeField] private ParticleSystem _leftThrusterParticleSystem;
    [SerializeField] private ParticleSystem _middleThrusterParticleSystem;
    [SerializeField] private ParticleSystem _rightThrusterParticleSystem;
    [SerializeField] private GameObject _landerExplosionVFX;

    private Lander _lander;

    private void Awake()
    {
        _lander = GetComponent<Lander>();

        _lander.OnBeforeForce += Lander_OnBeforeForce;
        _lander.OnUpForce += Lander_OnUpForce;
        _lander.OnLeftForce += Lander_OnLeftForce;
        _lander.OnRightForce += Lander_OnRightForce;

        SetEnableThrusterParticleSystem(_leftThrusterParticleSystem, false);
        SetEnableThrusterParticleSystem(_middleThrusterParticleSystem, false);
        SetEnableThrusterParticleSystem(_rightThrusterParticleSystem, false);
    }

    private void Start()
    {
        _lander.OnLanded += Lander_OnLanded;
    }

    private void OnDestroy()
    {
        _lander.OnBeforeForce -= Lander_OnBeforeForce;
        _lander.OnUpForce -= Lander_OnUpForce;
        _lander.OnLeftForce -= Lander_OnLeftForce;
        _lander.OnRightForce -= Lander_OnRightForce;
        _lander.OnLanded -= Lander_OnLanded;
    }

    private void Lander_OnBeforeForce(object sender, System.EventArgs e)
    {
        SetEnableThrusterParticleSystem(_leftThrusterParticleSystem, false);
        SetEnableThrusterParticleSystem(_middleThrusterParticleSystem, false);
        SetEnableThrusterParticleSystem(_rightThrusterParticleSystem, false);
    }

    private void Lander_OnUpForce(object sender, System.EventArgs e)
    {
        SetEnableThrusterParticleSystem(_leftThrusterParticleSystem, true);
        SetEnableThrusterParticleSystem(_middleThrusterParticleSystem, true);
        SetEnableThrusterParticleSystem(_rightThrusterParticleSystem, true);
    }

    private void Lander_OnLeftForce(object sender, System.EventArgs e)
    {
        SetEnableThrusterParticleSystem(_rightThrusterParticleSystem, true);
    }

    private void Lander_OnRightForce(object sender, System.EventArgs e)
    {
        SetEnableThrusterParticleSystem(_leftThrusterParticleSystem, true);
    }

    private void Lander_OnLanded(object sender, Lander.OnLandedEventArgs e)
    {
        switch (e.landingType)
        {
            case Lander.LandingType.Success:
                break;
            case Lander.LandingType.WrongLandingArea:
            case Lander.LandingType.TooSteepAngle:
            case Lander.LandingType.TooFastLanding:
            case Lander.LandingType.CargoCrashed:
                Instantiate(_landerExplosionVFX, transform.position, Quaternion.identity);
                gameObject.SetActive(false);
                break;
            default:
                break;
        }
    }

    private void SetEnableThrusterParticleSystem(ParticleSystem particleSystem, bool enabled)
    {
        ParticleSystem.EmissionModule emissionModule = particleSystem.emission;
        emissionModule.enabled = enabled;
    }
}