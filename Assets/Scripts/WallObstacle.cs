using UnityEngine;

public class WallObstacle : MonoBehaviour
{
    public GameObject[] walls;
    public ParticleSystem[] particles;

    public void ExplodeAll()
    {
        for (int i = 0; i < 3; i++)
        {
            if (walls[i].activeInHierarchy)
            {
                walls[i].SetActive(false);
                particles[i].Play();
            }
        }
    }
}
