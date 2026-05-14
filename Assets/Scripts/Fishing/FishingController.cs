// ──────────────────────────────────────────────
// TheSprouty | Fishing/FishingController.cs
// Manages the core fishing state machine.
// ──────────────────────────────────────────────
// TODO: replace Input.GetMouseButtonDown(1) with InputSystem when stable.
// ──────────────────────────────────────────────
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;

public class FishingController : MonoBehaviour
{
    // ----------------------------------------------------------
    // State
    // ----------------------------------------------------------
    private enum FishingState { Inactive, Casting, WaitingForBite, NibbleWindow, Reeling }

    // ----------------------------------------------------------
    // Serialized fields
    // ----------------------------------------------------------
    [Header("References")]
    [SerializeField] private PlayerAnimator playerAnimator;
    [SerializeField] private GameInput gameInput;
    [SerializeField] private PlayerIndicator playerIndicator;
    [SerializeField] private Tilemap waterTilemap;

    [Header("Fish")]
    [SerializeField] private FishSO testFish;

    [Header("Timing (seconds)")]
    [SerializeField] private float biteDelay = 3f;
    [SerializeField] private float nibbleWindowDuration = 1.5f;

    // ----------------------------------------------------------
    // Private state
    // ----------------------------------------------------------
    private Player _player;
    private FishingState _state = FishingState.Inactive;
    private Coroutine _activeRoutine;
    private bool _isOnWater;

    // ----------------------------------------------------------
    // Properties
    // ----------------------------------------------------------
    public bool IsFishing => _state != FishingState.Inactive;

    // ----------------------------------------------------------
    // Unity lifecycle
    // ----------------------------------------------------------
    private void Awake()
    {
        _player = GetComponent<Player>();
    }

    private void Start()
    {
        gameInput.OnUseToolAction += OnLMBPressed;
    }

    private void OnDestroy()
    {
        gameInput.OnUseToolAction -= OnLMBPressed;
    }

    private void Update()
    {
        if ((_state == FishingState.WaitingForBite || _state == FishingState.NibbleWindow)
            && Input.GetMouseButtonDown(1))
        {
            StopActiveRoutine();
            ExitFishing();
        }
    }

    // ----------------------------------------------------------
    // Public API  (called by PlayerAnimator animation events)
    // ----------------------------------------------------------

    /// <summary>Called by PlayerAnimator when FishingRod tool is used — cast animation starts.</summary>
    public void OnCastStart()
    {
        _isOnWater = IsIndicatorOnWater();
        playerAnimator.SetIsFishingOnWater(_isOnWater);
        _state = FishingState.Casting;
    }

    /// <summary>Called by AnimationEvent_CastComplete when cast animation finishes.</summary>
    public void OnCastComplete()
    {
        if (_state != FishingState.Casting) return;

        if (!_isOnWater)
        {
            _state = FishingState.Inactive;
            _player.UnlockAction();
            return;
        }

        _state = FishingState.WaitingForBite;
        Debug.Log("[Fishing] Cast complete — waiting for bite.");
        _activeRoutine = StartCoroutine(BiteRoutine());
    }

    /// <summary>Called by AnimationEvent_FishingComplete on last frame of Fishing_Happy.</summary>
    public void OnFishingComplete()
    {
        _state = FishingState.Inactive;
        playerIndicator.gameObject.SetActive(true);
        _player.UnlockAction();
    }

    // ----------------------------------------------------------
    // Private methods
    // ----------------------------------------------------------
    private void OnLMBPressed(object sender, EventArgs e)
    {
        if (_state != FishingState.NibbleWindow) return;

        StopActiveRoutine();
        CatchFish();
    }

    private IEnumerator BiteRoutine()
    {
        yield return new WaitForSeconds(biteDelay);

        _state = FishingState.NibbleWindow;
        Debug.Log("[Fishing] Fish is biting! Press LMB!");

        _activeRoutine = StartCoroutine(NibbleWindowRoutine());
    }

    private IEnumerator NibbleWindowRoutine()
    {
        yield return new WaitForSeconds(nibbleWindowDuration);

        Debug.Log("[Fishing] Fish got away.");
        _state = FishingState.WaitingForBite;
    }

    private void CatchFish()
    {
        _state = FishingState.Reeling;
        playerIndicator.gameObject.SetActive(false);
        playerAnimator.TriggerFishingReel();

        if (testFish != null && InventoryManager.Instance != null)
        {
            bool added = InventoryManager.Instance.AddItem(testFish, 1);
            Debug.Log(added
                ? $"[Fishing] Caught {testFish.itemName}!"
                : $"[Fishing] Caught {testFish.itemName} but inventory is full!");
        }
    }

    private void ExitFishing()
    {
        playerAnimator.TriggerFishingExit();
        _player.UnlockAction();
        StartCoroutine(SetInactiveNextFrameRoutine());
    }

    private IEnumerator SetInactiveNextFrameRoutine()
    {
        yield return null;
        _state = FishingState.Inactive;
    }

    private void StopActiveRoutine()
    {
        if (_activeRoutine == null) return;
        StopCoroutine(_activeRoutine);
        _activeRoutine = null;
    }

    private bool IsIndicatorOnWater()
    {
        if (waterTilemap == null || playerIndicator == null) return false;
        Vector3Int cell = waterTilemap.WorldToCell(playerIndicator.transform.position);
        return waterTilemap.GetTile(cell) != null;
    }
}
