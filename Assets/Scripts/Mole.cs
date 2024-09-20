using System.Collections;
using UnityEngine;

public class Mole : MonoBehaviour {
  [Header("Graphics")]
  [SerializeField] private Sprite mole;
  [SerializeField] private Sprite moleHit;
  [SerializeField] private Sprite bombSprite;
  [SerializeField] private Sprite knifeSprite;
  [SerializeField] private Sprite gunSprite;
  [SerializeField] private Sprite drugSprite;

  // [SerializeField] private Sprite moleHardHat;
  // [SerializeField] private Sprite moleHatBroken;
  // [SerializeField] private Sprite moleHatHit;

  [Header("GameManager")]
  [SerializeField] private GameManager gameManager;

  // The offset of the sprite to hide it.
  private Vector2 startPosition = new Vector2(0f, -2.56f);
  private Vector2 endPosition = Vector2.zero;
  // How long it takes to show a mole.
  private float showDuration = 0.5f;
  private float duration = 1.3f;

  private SpriteRenderer spriteRenderer;
  private BoxCollider2D boxCollider2D;
  private Vector2 boxOffset;
  private Vector2 boxSize;
  private Vector2 boxOffsetHidden;
  private Vector2 boxSizeHidden;

  // Mole Parameters 
  private bool hittable = true;
  // public enum MoleType { Standard, HardHat, Bomb };
  public enum MoleType { Standard, Bomb, Knife, Gun, Drug };
  private MoleType moleType;
  // private float hardRate = 0.25f;
  // private float bombRate = 0f;
  // private int lives;
  private float bombRate = 0.08f;
  private float knifeRate = 0.08f;
  private float gunRate = 0.08f;
  private float drugRate = 0.08f;
  private int moleIndex = 0;

  private IEnumerator ShowHide(Vector2 start, Vector2 end) 
  {
    // Make sure we start at the start.
    transform.localPosition = start;

    // Show the mole.
    float elapsed = 0f;
    while (elapsed < showDuration) {
      transform.localPosition = Vector2.Lerp(start, end, elapsed / showDuration);
      boxCollider2D.offset = Vector2.Lerp(boxOffsetHidden, boxOffset, elapsed / showDuration);
      boxCollider2D.size = Vector2.Lerp(boxSizeHidden, boxSize, elapsed / showDuration);
      // Update at max framerate.
      elapsed += Time.deltaTime;
      yield return null;
    }

    // Make sure we're exactly at the end.
    transform.localPosition = end;
    boxCollider2D.offset = boxOffset;
    boxCollider2D.size = boxSize;

    // Wait for duration to pass.
    yield return new WaitForSeconds(duration);

    // Hide the mole.
    elapsed = 0f;
    while (elapsed < showDuration) {
      transform.localPosition = Vector2.Lerp(end, start, elapsed / showDuration);
      boxCollider2D.offset = Vector2.Lerp(boxOffset, boxOffsetHidden, elapsed / showDuration);
      boxCollider2D.size = Vector2.Lerp(boxSize, boxSizeHidden, elapsed / showDuration);
      // Update at max framerate.
      elapsed += Time.deltaTime;
      yield return null;
    }
    // Make sure we're exactly back at the start position.
    transform.localPosition = start;
    boxCollider2D.offset = boxOffsetHidden;
    boxCollider2D.size = boxSizeHidden;

    // If we got to the end and it's still hittable then we missed it.
    if (hittable) {
      hittable = false;
      // We only give time penalty if it isn't a bomb.
      gameManager.Missed(moleIndex, moleType != MoleType.Bomb);
    }
  }

  public void Hide() {
    // Set the appropriate mole parameters to hide it.
    transform.localPosition = startPosition;
    boxCollider2D.offset = boxOffsetHidden;
    boxCollider2D.size = boxSizeHidden;
  }

  private IEnumerator QuickHide() {
    yield return new WaitForSeconds(0.25f);
    // Whilst we were waiting we may have spawned again here, so just
    // check that hasn't happened before hiding it. This will stop it
    // flickering in that case.
    if (!hittable) {
      Hide();
    }
  }

  private void OnMouseDown() 
    {   if (gameManager == null) {
        Debug.LogError("GameManager reference is null on Mole.");
        return;
        }
        if (hittable) {
            switch (moleType) {
                case MoleType.Standard:
                    spriteRenderer.sprite = moleHit;
                    gameManager.AddScore(moleIndex, 1);
                    break;
                case MoleType.Bomb:
                    spriteRenderer.sprite = bombSprite;
                    gameManager.AddScore(moleIndex, 0);
                    break;
                case MoleType.Knife:
                    spriteRenderer.sprite = knifeSprite;
                    gameManager.AddScore(moleIndex, 0);
                    break;
                case MoleType.Gun:
                    spriteRenderer.sprite = gunSprite;
                    gameManager.AddScore(moleIndex, 0);
                    break;
                case MoleType.Drug:
                    spriteRenderer.sprite = drugSprite;
                    gameManager.AddScore(moleIndex, 0);
                    break;
            }
            // Stop the animation
            StopAllCoroutines();
            StartCoroutine(QuickHide());
            // Turn off hittable so that we can't keep tapping for score.
            hittable = false;
        }
    }

  private void CreateNext() {
        float random = Random.Range(0f, 1f);
        if (random < bombRate) {
            moleType = MoleType.Bomb;
            spriteRenderer.sprite = bombSprite;
        } else if (random < bombRate + knifeRate) {
            moleType = MoleType.Knife;
            spriteRenderer.sprite = knifeSprite;
        } else if (random < bombRate + knifeRate + gunRate) {
            moleType = MoleType.Gun;
            spriteRenderer.sprite = gunSprite;
        } else if (random < bombRate + knifeRate + gunRate + drugRate) {
            moleType = MoleType.Drug;
            spriteRenderer.sprite = drugSprite;
        } else {
            moleType = MoleType.Standard;
            spriteRenderer.sprite = mole;
        }
        // Mark as hittable so we can register an onclick event.
        hittable = true;
    }

  // As the level progresses the game gets harder.    // As the level progresses, the game gets harder.
  private void SetLevel(int level) {
        // For young students, keep the game simpler and moles visible longer
        duration = 1.5f; // Moles stay visible for 1.5 seconds

        // Reduce the chance of harmful items appearing
        bombRate = 0.05f;
        knifeRate = 0.05f;
        gunRate = 0.05f;
        drugRate = 0.05f;
    }

  private void Awake() {
    // Get references to the components we'll need.
    spriteRenderer = GetComponent<SpriteRenderer>();
    boxCollider2D = GetComponent<BoxCollider2D>();
    if (gameManager == null)
    {gameManager = FindObjectOfType<GameManager>();}
    
    // Work out collider values.
    boxOffset = boxCollider2D.offset;
    boxSize = boxCollider2D.size;
    boxOffsetHidden = new Vector2(boxOffset.x, -startPosition.y / 2f);
    boxSizeHidden = new Vector2(boxSize.x, 0f);
  }

  public void Activate(int level) {
    SetLevel(level);
    CreateNext();
    StartCoroutine(ShowHide(startPosition, endPosition));
  }

  // Used by the game manager to uniquely identify moles. 
  public void SetIndex(int index) {
    moleIndex = index;
  }

  // Used to freeze the game on finish.
  public void StopGame() {
    hittable = false;
    StopAllCoroutines();
    Hide();
  }
}
