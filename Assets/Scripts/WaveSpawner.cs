using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WaveSpawner : MonoBehaviour {
  public bool releaseWave = false;
    public bool spawning = false;
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
    public static float K = 0.1f;
    public static float MIN_PHASE = Mathf.PI / 4;
    public static float MIN_RADIUS = 4;
    public static float SPACING = 10;
    public static float PHASE = MIN_PHASE;
    public static float LEVEL_PUNISHMENT = 1;
    public static int ROSE_PETALS = 1;
    public static int CTCLOCKWISE = 1;

    // List of available patterns
    public enum EnumPattern : int
    {
        SPREADING_WAVE = 0,
        SPIRAL = 1,
        CIRCLE = 2,
        HEART = 3,
        ROSE = 4
    }
    public static int PATTERN_COUNT = 5;

    // Pattern getter
    public SpawnPattern getPattern(EnumPattern type)
    {
        switch (type)
        {
            // Spiral
            case EnumPattern.SPIRAL:
                return new SpawnPattern
                {
                    R = delegate (float time) { return MIN_RADIUS + K * time; },
                    A = delegate (float time) { return PHASE + CTCLOCKWISE * time; }
                };

            // Spreading Wave
            case EnumPattern.SPREADING_WAVE:
                return new SpawnPattern
                {
                    R = delegate (float time) { return MIN_RADIUS + K * time; },
                    A = delegate (float time) { return Mathf.Sin(PHASE + CTCLOCKWISE * time); }
                };

            // Heart
            case EnumPattern.HEART:
                return new SpawnPattern
                {
                    R = delegate (float time) { return MIN_RADIUS + K * Mathf.Sin(2 * time); },
                    A = delegate (float time) { return PHASE + CTCLOCKWISE * time; }
                };

            // Rose
            case EnumPattern.ROSE:
                return new SpawnPattern
                {
                    R = delegate (float time) { return MIN_RADIUS + K * Mathf.Sin(ROSE_PETALS * time); },
                    A = delegate (float time) { return PHASE + CTCLOCKWISE * time; }
                };

            // Parabola - Not Working
            //case EnumPattern.PARABOLA:
            //    return new SpawnPattern
            //    {
            //        A = delegate (float time) { return PHASE + CTCLOCKWISE * time; },
            //        R = delegate (float time) {
            //            float A = PHASE + CTCLOCKWISE * time;
            //            return Mathf.Cos(A) * Mathf.Tan(A);
            //        }
            //    };

            // Default: Circle
            default:
                return new SpawnPattern
                {
                    R = delegate (float time) { return MIN_RADIUS; },
                    A = delegate (float time) { return PHASE + CTCLOCKWISE * time; }
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
        spawning = false;
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

    private static string[] NAMES = { "SPREADING WAVE", "SPIRAL", "CIRCLE", "HEART", "ROSE"  };
  private void SpawnEnemies(int wave) {
        

        EnumPattern type = (EnumPattern)Random.Range(0, PATTERN_COUNT);
        //type = EnumPattern.CIRCLE;

        CTCLOCKWISE = 2 * Random.Range(0, 2) - 1;
        SpawnPattern pattern = getPattern(type);
        PHASE = Random.Range(0.0f, 2.0f) * Mathf.PI;

        int enemyCount = wave;
        //PatternType type = (PatternType) Random.Range(0, 3);


        Debug.Log(NAMES[(int)type]);

        spawning = true;
        StartCoroutine(SpawnDelayed(pattern, enemyCount, 0.4f));
        ROSE_PETALS = 1 + wave / 2;

        // Constant Radius needs cooldown for balancing
        //switch (type)
        //{
        //    case EnumPattern.CIRCLE:
        //    case EnumPattern.ROSE:
        //spawning = true;
        //StartCoroutine(SpawnDelayed(pattern, enemyCount, 0.4f));
        //break;

        //    default:
        //        for (int i = 0; i < enemyCount; i++)
        //        {
        //            SpawnEnemy(pattern, Mathf.Deg2Rad * i * 360 / enemyCount);
        //        }
        //        ROSE_PETALS = 1 + wave / 2;
        //        break;
        //}
    }

    public void SpawnEnemy (SpawnPattern pattern, float time)
    {
        BasicEnemy enemy = Instantiate(availableEnemies[0]);
        enemy.transform.position = Polar2Euclid(pattern, time);
        enemy.transform.rotation = Quaternion.LookRotation(GameManager.Instance().player.transform.position - enemy.transform.position, Vector3.back);
        GameManager.Instance().enemyList.Add(enemy);
    }

    IEnumerator SpawnDelayed(SpawnPattern pattern, int amount, float delay)
    {
        
        for (int i = 0; i < amount; i++)
        {
            SpawnEnemy(pattern, Mathf.Deg2Rad * i * 360 / amount);
            yield return new WaitForSeconds(delay);
        }
        spawning = false;
        ROSE_PETALS = 1 + amount / 2;
    }

}
