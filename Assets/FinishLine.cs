using UnityEngine;

public class FinishLine : MonoBehaviour
{
    [SerializeField] private ParticleSystem[] PSs;

    public void FinalParticle()
    {
        for (int i = 0; i < PSs.Length; i++)
        {
            PSs[i].Play();
        }
    }
}
