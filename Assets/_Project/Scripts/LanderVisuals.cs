using UnityEngine;

[RequireComponent(typeof(Lander))]
public class LanderVisuals : MonoBehaviour
{
    [SerializeField] private ParticleSystem _leftThrusterParticleSystem;
    [SerializeField] private ParticleSystem _middleThrusterParticleSystem;
    [SerializeField] private ParticleSystem _rightThrusterParticleSystem;

    private Lander lander;

    private void Awake()
    {
        lander = GetComponent<Lander>();

        lander.OnBeforeForce += Lander_OnBeforeForce;
        lander.OnUpForce += Lander_OnUpForce;
        lander.OnLeftForce += Lander_OnLeftForce;
        lander.OnRightForce += Lander_OnRightForce;

        SetEnableThrusterParticleSystem(_leftThrusterParticleSystem, false);
        SetEnableThrusterParticleSystem(_middleThrusterParticleSystem, false);
        SetEnableThrusterParticleSystem(_rightThrusterParticleSystem, false);
    }

    private void OnDisable()
    {
        lander.OnBeforeForce -= Lander_OnBeforeForce;
        lander.OnUpForce -= Lander_OnUpForce;
        lander.OnLeftForce -= Lander_OnLeftForce;
        lander.OnRightForce -= Lander_OnRightForce;
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

    private void SetEnableThrusterParticleSystem(ParticleSystem particleSystem, bool enabled)
    {
        ParticleSystem.EmissionModule emissionModule = particleSystem.emission;
        emissionModule.enabled = enabled;
    }
}