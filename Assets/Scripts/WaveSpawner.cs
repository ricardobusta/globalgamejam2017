using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WaveSpawner : MonoBehaviour {
  public bool releaseWave = false;
  public int currentWave = 0;

    public BasicEnemy[] availableEnemies;


    /* *********************************************
     *   High-order Polar Functions
     * ********************************************* */
    // Signature definition
    public delegate float Radius (float time);  // Describes radius through the time
    public delegate float Angle(float time);    // Describes angle through the time
    
    // Spawn Pattern definition
    public struct SpawnPattern
    {
        public Radius R;
        public Angle A;
    }

    // Pattern constant
    public static float PATTERN_K = 2;
    public static float MIN_DISTANCE = 2;
    public static float SPACING = 10;
    public static float PHASE = 45;
    public static float LEVEL_PUNISHMENT = 1;

    // List of available patterns
    public enum PatternType:int
    {
        SPREADING_WAVE = 0,
        SPIRAL = 1,
        PHOTON = 2
    }

    // Pattern getter
    public SpawnPattern getPattern (PatternType type)
    {
        switch (type)
        {
            // Spreading Wave
            case PatternType.PHOTON:
                return new SpawnPattern {
                    R = delegate (float time) { return MIN_DISTANCE + (PATTERN_K * time + Mathf.Sin(time)) * LEVEL_PUNISHMENT; },
                    A = delegate (float time) { return PHASE + PATTERN_K * time; }
                };
            
            // Circle
            case PatternType.SPIRAL:
                return new SpawnPattern
                {
                    R = delegate (float time) { return MIN_DISTANCE + (PATTERN_K * time) * LEVEL_PUNISHMENT; },
                    A = delegate (float time) { return PHASE + PATTERN_K * time; }
                };

            // Default: Spreading Wave
            default:
                return new SpawnPattern
                {
                    R = delegate (float time) { return MIN_DISTANCE + PATTERN_K * time * LEVEL_PUNISHMENT; },
                    A = delegate (float time) { return PHASE + Mathf.Sin(time); }
                };
        }
    }

    public Vector3 Polar2Euclid(SpawnPattern pattern, float time)
    {
        float r = pattern.R(time);
        return new Vector3(r * Mathf.Cos(pattern.A(time)), r * Mathf.Sin(pattern.A(time)), 0);
    }

    // ---------------------------------------------


    // Use this for initialization
    void Start() {
    GameManager.Instance().waveName.gameObject.SetActive(false);
  }

  public void Handle() {
    if (releaseWave) {
      currentWave++;
      releaseWave = false;

      SpawnEnemies(currentWave);

      StartCoroutine(GameManager.Instance().ShowWaveName(currentWave));
    }
  }

  private void SpawnEnemies(int wave) {
    int enemyCount = 30;
    //PatternType type = (PatternType) Random.Range(0, 3);

        PatternType type = PatternType.PHOTON;

        SpawnPattern pattern = getPattern(type);
    PHASE = Random.Range(0, 359);

    for (int i = 0; i < enemyCount; i++)
    {
        SpawnEnemy(pattern, Mathf.Deg2Rad * i * SPACING);
    }

    LEVEL_PUNISHMENT *= 0.99f;
  }

    public void SpawnEnemy (SpawnPattern pattern, float time)
    {
        BasicEnemy enemy = Instantiate(availableEnemies[0]);
        enemy.transform.position = Polar2Euclid(pattern, time);
        enemy.transform.rotation = Quaternion.LookRotation(GameManager.Instance().player.transform.position - enemy.transform.position, Vector3.back);
        GameManager.Instance().enemyList.Add(enemy);
    }

    

}
