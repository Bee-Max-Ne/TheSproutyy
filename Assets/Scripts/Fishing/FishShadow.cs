// ──────────────────────────────────────────────
// TheSprouty | Fishing/FishShadow.cs
// Controls fish shadow behaviour:
// Idle (animation plays) → Approaching → Nibbling → fires OnReachedBobber.
// Shadow size determines the fish pool. Rarity determines drop chance within pool.
// ──────────────────────────────────────────────
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishShadow : MonoBehaviour
{
    // ----------------------------------------------------------
    // State
    // ----------------------------------------------------------
    private enum FishShadowState { Idle, Approaching, Nibbling }

    // ----------------------------------------------------------
    // Events
    // ----------------------------------------------------------

    /// <summary>Fired when bite delay completes. FishingManager relays to FishingController.</summary>
    public event Action<FishShadow> OnReachedBobber;

    // ----------------------------------------------------------
    // Serialized fields
    // ----------------------------------------------------------
    [Header("References")]
    [SerializeField] private SpriteRenderer visualRenderer;

    [Header("Fish Pool")]
    [Tooltip("Fish that can drop from this shadow size. Rarity determines weight.")]
    [SerializeField] private FishSO[] possibleFish;

    [Header("Shadow Behaviour")]
    [Min(0.1f)] public float swimSpeed = 2f;

    [Header("Nibble Settings")]
    [SerializeField] private float nibbleAmplitude = 0.15f;
    [SerializeField] private float nibbleFrequency = 2f;

    // ----------------------------------------------------------
    // Private state
    // ----------------------------------------------------------
    private FishShadowState _state = FishShadowState.Idle;
    private Transform _bobber;
    private Coroutine _activeRoutine;

    // ----------------------------------------------------------
    // Properties
    // ----------------------------------------------------------
    public bool IsActive => _state != FishShadowState.Idle;

    // ----------------------------------------------------------
    // Unity lifecycle
    // ----------------------------------------------------------
    private void Start()
    {
        Debug.Log($"[FishShadow] {gameObject.name} spawned at {transform.position}");
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_state != FishShadowState.Idle) return;
        if (!other.CompareTag("Bobber")) return;

        _bobber = other.transform;
        EnterState(FishShadowState.Approaching);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Bobber")) return;

        // Bobber rời khỏi vùng detection khi đang Idle → không làm gì
        // Nếu đang Approaching/Nibbling → tiếp tục vì đang trong quá trình rồi
    }

    // ----------------------------------------------------------
    // Public API
    // ----------------------------------------------------------

    /// <summary>Called by FishingController when bobber spawns on water.</summary>
    public void SetBobber(Transform bobber)
    {
        _bobber = bobber;
    }

    /// <summary>Called when fishing ends — shadow returns to Idle.</summary>
    public void ClearBobber()
    {
        _bobber = null;
        if (_state != FishShadowState.Idle)
            EnterState(FishShadowState.Idle);
    }

    /// <summary>Returns a random fish from the pool weighted by rarity.</summary>
    public FishSO GetRandomFish()
    {
        if (possibleFish == null || possibleFish.Length == 0) return null;

        // Build weighted list: Legendary=1, Rare=3, Uncommon=6, Common=10
        List<FishSO> weightedPool = new();
        foreach (FishSO fish in possibleFish)
        {
            if (fish == null) continue;
            int weight = fish.rarity switch
            {
                FishRarity.Legendary => 1,
                FishRarity.Rare      => 3,
                FishRarity.Uncommon  => 6,
                FishRarity.Common    => 10,
                _                    => 10
            };
            for (int i = 0; i < weight; i++)
                weightedPool.Add(fish);
        }

        return weightedPool.Count > 0
            ? weightedPool[UnityEngine.Random.Range(0, weightedPool.Count)]
            : null;
    }

    // ----------------------------------------------------------
    // Private — state routines
    // ----------------------------------------------------------
    private void EnterState(FishShadowState newState)
    {
        _state = newState;
        StopActiveRoutine();

        _activeRoutine = newState switch
        {
            FishShadowState.Approaching => StartCoroutine(ApproachRoutine()),
            FishShadowState.Nibbling    => StartCoroutine(NibbleRoutine()),
            _                           => null
        };
    }

    private IEnumerator ApproachRoutine()
    {
        while (true)
        {
            if (_bobber == null) { EnterState(FishShadowState.Idle); yield break; }

            transform.position = Vector2.MoveTowards(
                transform.position, _bobber.position, swimSpeed * Time.deltaTime);
            FlipToward(_bobber.position);

            if (Vector2.Distance(transform.position, _bobber.position) < 0.15f)
            {
                EnterState(FishShadowState.Nibbling);
                yield break;
            }

            yield return null;
        }
    }

    private IEnumerator NibbleRoutine()
    {
        if (_bobber == null) { EnterState(FishShadowState.Idle); yield break; }

        // Pick a fish and use its bite delay
        FishSO fish    = GetRandomFish();
        float biteDelay = fish != null ? fish.GetRandomBiteDelay() : 2f;
        Vector2 basePos = _bobber.position;
        float timer     = 0f;

        while (timer < biteDelay)
        {
            if (_bobber == null) { EnterState(FishShadowState.Idle); yield break; }

            float yOffset = Mathf.Sin(timer * nibbleFrequency * Mathf.PI * 2f) * nibbleAmplitude;
            transform.position = basePos + new Vector2(0f, yOffset);

            timer += Time.deltaTime;
            yield return null;
        }

        OnReachedBobber?.Invoke(this);
    }

    // ----------------------------------------------------------
    // Private helpers
    // ----------------------------------------------------------
    private void FlipToward(Vector2 target)
    {
        if (visualRenderer == null) return;
        float dx = target.x - transform.position.x;
        if (Mathf.Abs(dx) > 0.01f)
            visualRenderer.flipX = dx < 0;
    }

    private void StopActiveRoutine()
    {
        if (_activeRoutine == null) return;
        StopCoroutine(_activeRoutine);
        _activeRoutine = null;
    }
}
