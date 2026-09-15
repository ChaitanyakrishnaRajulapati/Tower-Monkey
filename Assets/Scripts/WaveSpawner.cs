using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WaveSpawner : MonoBehaviour
{
    [Header("Enemy Prefabs")]
    public GameObject foxEnemy;     // fast
    public GameObject boarEnemy;    // normal
    public GameObject rhinoEnemy;   // heavy
    public GameObject bugFlyEnemy;  // beetle / air

    [Header("Spawn Points")]
    public Transform spawnPoint;
    public Transform goalPoint;

    [Header("Y Offsets")]
    public float groundYOffset = 1.2f;
    public float rhinoYOffset = 1.5f;
    public float flyYOffset = 2.5f;

    [Header("Timing")]
    public float startDelay = 2f;
    public float betweenWavesDelay = 3f;

    [Header("Pacing Control")]
    public float gapMultiplierWave1_2 = 1.0f;
    public float gapMultiplierWave3Plus = 0.7f;

    [Header("UI (TextMeshPro)")]
    public TextMeshProUGUI waveText;
    public TextMeshProUGUI nextWaveText;
    public TextMeshProUGUI enemiesLeftText;

    private List<Wave> waves = new List<Wave>();
    private int waveIndex = -1;
    private int aliveInWave = 0;
    private bool spawningWave = false;
    private bool startedNext = false;

    [System.Serializable]
    public class Step
    {
        public GameObject prefab;
        public int count;
        public float gap;
        public float yOffset;

        public Step(GameObject p, int c, float g, float y)
        {
            prefab = p;
            count = c;
            gap = g;
            yOffset = y;
        }
    }

    public class Wave
    {
        public List<Step> steps = new List<Step>();
        public Wave(params Step[] s) { steps.AddRange(s); }
    }

    void Start()
    {
        Build10Waves();
        StartCoroutine(Run());
    }

    IEnumerator Run()
    {
        if (spawnPoint == null)
        {
            Debug.LogError("WaveSpawner: spawnPoint missing!");
            yield break;
        }

        yield return new WaitForSeconds(startDelay);
        yield return NextWave();
    }

    public void NotifyEnemyDied()
    {
        aliveInWave = Mathf.Max(0, aliveInWave - 1);
        

        if (!spawningWave && aliveInWave == 0 && !startedNext)
        {
            startedNext = true;
            StartCoroutine(NextWave());
        }
    }

    IEnumerator NextWave()
    {
        if (waveIndex >= waves.Count - 1)
        {
            SetNextWaveText("ALL WAVES COMPLETE");
            yield break;
        }

        int nextWaveNumber = waveIndex + 2;
        SetNextWaveText("Next Wave: " + nextWaveNumber);

        yield return new WaitForSeconds(betweenWavesDelay);

        waveIndex++;
        startedNext = false;

        SetWaveText("Wave: " + (waveIndex + 1) + " / " + waves.Count);
        SetNextWaveText("");

        yield return StartCoroutine(SpawnWave(waves[waveIndex]));
    }

    IEnumerator SpawnWave(Wave wave)
    {
        spawningWave = true;

        float mult = (waveIndex <= 1) ? gapMultiplierWave1_2 : gapMultiplierWave3Plus;

        for (int i = 0; i < wave.steps.Count; i++)
        {
            Step step = wave.steps[i];
            if (step.prefab == null || step.count <= 0) continue;

            for (int k = 0; k < step.count; k++)
            {
                SpawnOne(step.prefab, step.yOffset);

                float wait = step.gap * mult;
                if (wait > 0f)
                    yield return new WaitForSeconds(wait);
            }
        }

        spawningWave = false;

        if (aliveInWave == 0 && !startedNext)
        {
            startedNext = true;
            StartCoroutine(NextWave());
        }
    }

    void SpawnOne(GameObject prefab, float yOffset)
    {
        if (prefab == null) return;

        Vector3 pos = spawnPoint.position + new Vector3(0f, yOffset, 0f);
        GameObject e = Instantiate(prefab, pos, Quaternion.identity);

        EnemyNavMovement mv = e.GetComponent<EnemyNavMovement>();
        if (mv != null && goalPoint != null)
            mv.goal = goalPoint;

        aliveInWave++;
        

        WaveEnemyTracker t = e.GetComponent<WaveEnemyTracker>();
        if (t == null) t = e.AddComponent<WaveEnemyTracker>();
        t.Init(this);
    }

    void SetWaveText(string s)
    {
        if (waveText != null) waveText.text = s;
    }

    void SetNextWaveText(string s)
    {
        if (nextWaveText != null) nextWaveText.text = s;
    }

    

 void Build10Waves()
{
    waves.Clear();

    // Base gaps (WaveSpawner already multiplies by mult: wave 1-2 slower, 3+ faster)
    float foxGap   = 0.85f;
    float boarGap  = 0.75f;
    float flyGap   = 0.85f;
    float rhinoGap = 1.25f;

    // -------------------------
    // WAVE 1 – very easy
    // -------------------------
    waves.Add(new Wave(
        new Step(boarEnemy, 4, boarGap, groundYOffset)
    ));

    // -------------------------
    // WAVE 2 – introduce fox pressure
    // -------------------------
    waves.Add(new Wave(
        new Step(foxEnemy, 4, foxGap,  groundYOffset),
        new Step(boarEnemy, 3, boarGap, groundYOffset)
    ));

    // -------------------------
    // WAVE 3 – first beetle + first rhino (first spike)
    // -------------------------
    waves.Add(new Wave(
        new Step(foxEnemy, 4, 0.78f, groundYOffset),
        new Step(boarEnemy, 3, 0.72f, groundYOffset),
        new Step(bugFlyEnemy, 2, flyGap, flyYOffset),
        new Step(rhinoEnemy, 1, rhinoGap, rhinoYOffset)
    ));

    // -------------------------
    // WAVE 4 – mixed pressure (forces multiple towers)
    // -------------------------
    waves.Add(new Wave(
        new Step(foxEnemy, 6, 0.70f, groundYOffset),
        new Step(boarEnemy, 4, 0.70f, groundYOffset),
        new Step(bugFlyEnemy, 2, 0.80f, flyYOffset)
    ));

    // -------------------------
    // WAVE 5 – rhino mid-wave + swarm after
    // -------------------------
    waves.Add(new Wave(
        new Step(boarEnemy, 5, 0.68f, groundYOffset),
        new Step(rhinoEnemy, 1, rhinoGap, rhinoYOffset),
        new Step(foxEnemy, 6, 0.62f, groundYOffset),
        new Step(bugFlyEnemy, 2, 0.78f, flyYOffset)
    ));

    // -------------------------
    // WAVE 6 – double beetle + double rhino (big step up)
    // -------------------------
    waves.Add(new Wave(
        new Step(foxEnemy, 7, 0.60f, groundYOffset),
        new Step(bugFlyEnemy, 4, 0.70f, flyYOffset),
        new Step(rhinoEnemy, 2, rhinoGap, rhinoYOffset)
    ));

    // -------------------------
    // WAVE 7 – pressure wave (lots of bodies)
    // -------------------------
    waves.Add(new Wave(
        new Step(boarEnemy, 7, 0.60f, groundYOffset),
        new Step(foxEnemy, 9, 0.52f, groundYOffset),
        new Step(bugFlyEnemy, 3, 0.68f, flyYOffset)
    ));

    // -------------------------
    // WAVE 8 – triple rhino (tank check)
    // -------------------------
    waves.Add(new Wave(
        new Step(bugFlyEnemy, 4, 0.65f, flyYOffset),
        new Step(rhinoEnemy, 3, rhinoGap, rhinoYOffset),
        new Step(foxEnemy, 8, 0.50f, groundYOffset)
    ));

    // -------------------------
    // WAVE 9 – chaos controlled (everything + 2 rhinos)
    // -------------------------
    waves.Add(new Wave(
        new Step(boarEnemy, 8, 0.55f, groundYOffset),
        new Step(foxEnemy, 10, 0.48f, groundYOffset),
        new Step(bugFlyEnemy, 5, 0.62f, flyYOffset),
        new Step(rhinoEnemy, 2, rhinoGap, rhinoYOffset)
    ));

    // -------------------------
    // WAVE 10 – final (boss feel): 4 rhinos + heavy mixed
    // -------------------------
    waves.Add(new Wave(
        new Step(foxEnemy, 12, 0.45f, groundYOffset),
        new Step(boarEnemy, 10, 0.50f, groundYOffset),
        new Step(bugFlyEnemy, 6, 0.58f, flyYOffset),
        new Step(rhinoEnemy, 4, rhinoGap, rhinoYOffset)
    ));
}

}